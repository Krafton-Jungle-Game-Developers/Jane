using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class BlurRenderer : ScriptableRendererFeature
    {
        class BlurRenderPass : PostEffectRenderer<Blur>
        {
            int blurredID;
            int blurredID2;

            enum Pass
            {
                Blend,
                BlendDepthFade,
                Gaussian,
                Box
            }
            public BlurRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.Blur;
                ProfilerTag = this.ToString();

                blurredID = Shader.PropertyToID("_Temp1");
                blurredID2 = Shader.PropertyToID("_Temp2");
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Blur>();
                
                if(volumeSettings && volumeSettings.IsActive()) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;
                
                base.ConfigurePass(cmd, cameraTextureDescriptor);

                cameraTextureDescriptor.width /= volumeSettings.downscaling.value;
                cameraTextureDescriptor.height /= volumeSettings.downscaling.value;

                cmd.GetTemporaryRT(blurredID, cameraTextureDescriptor.width, cameraTextureDescriptor.height, 0, FilterMode.Bilinear, cameraTextureDescriptor.graphicsFormat);
                cmd.GetTemporaryRT(blurredID2, cameraTextureDescriptor.width, cameraTextureDescriptor.height, 0, FilterMode.Bilinear, cameraTextureDescriptor.graphicsFormat);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (ShouldRender(renderingData) == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd, renderingData);
                Blit(cmd, cameraColorTarget, blurredID);

                int blurPass = (volumeSettings.mode == Blur.BlurMethod.Gaussian) ? (int)Pass.Gaussian : (int)Pass.Box;

                for (int i = 0; i < volumeSettings.iterations.value; i++)
                {
                    //Safeguard for exploding GPUs
                    if (volumeSettings.iterations.value > 12) return;

                    // horizontal blur
                    cmd.SetGlobalVector("_BlurOffsets", new Vector4(volumeSettings.amount.value / renderingData.cameraData.camera.scaledPixelWidth, 0, 0, 0));
                    Blit(this, cmd, blurredID, blurredID2, Material, blurPass);

                    // vertical blur
                    cmd.SetGlobalVector("_BlurOffsets", new Vector4(0, volumeSettings.amount.value / renderingData.cameraData.camera.scaledPixelHeight, 0, 0));
                    Blit(this, cmd, blurredID2, blurredID, Material, blurPass);

                    //Double blur
                    if (volumeSettings.highQuality.value)
                    {
                        // horizontal blur
                        cmd.SetGlobalVector("_BlurOffsets", new Vector4(volumeSettings.amount.value / renderingData.cameraData.camera.scaledPixelWidth, 0, 0, 0));
                        Blit(this, cmd, blurredID, blurredID2, Material, blurPass);

                        // vertical blur
                        cmd.SetGlobalVector("_BlurOffsets", new Vector4(0, volumeSettings.amount.value / renderingData.cameraData.camera.scaledPixelHeight, 0, 0));
                        Blit(this, cmd, blurredID2, blurredID, Material, blurPass);
                    }
                }
                
                cmd.SetGlobalTexture("_BlurredTex", blurredID);
                
                if(volumeSettings.distanceFade.value) cmd.SetGlobalVector("_FadeParams", new Vector4(volumeSettings.startFadeDistance.value, volumeSettings.endFadeDistance.value, 0, volumeSettings.distanceFade.value ? 1 : 0));

                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, volumeSettings.distanceFade.value ? (int)Pass.BlendDepthFade : (int)Pass.Blend);
            }

            public override void Cleanup(CommandBuffer cmd)
            {
                base.Cleanup(cmd);
                
                cmd.ReleaseTemporaryRT(blurredID);
                cmd.ReleaseTemporaryRT(blurredID2);
            }
        }

        BlurRenderPass m_ScriptablePass;
        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings(false);
        
        public override void Create()
        {
            m_ScriptablePass = new BlurRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}