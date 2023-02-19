using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class LightStreaksRenderer : ScriptableRendererFeature
    {
        class LightStreaksRenderPass : PostEffectRenderer<LightStreaks>
        {
            private int emissionTex;
            int blurredID;
            int blurredID2;

            enum Pass
            {
                LuminanceDiff,
                BlurFast,
                Blur,
                Blend,
                Debug
            }

            public LightStreaksRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.LightStreaks;
                ProfilerTag = this.ToString();
                
                emissionTex = Shader.PropertyToID("_BloomTex");
                blurredID = Shader.PropertyToID("_Temp1");
                blurredID2 = Shader.PropertyToID("_Temp2");
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<LightStreaks>();
                
                if(volumeSettings && volumeSettings.IsActive()) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                base.ConfigurePass(cmd, cameraTextureDescriptor);

                cmd.GetTemporaryRT(emissionTex, cameraTextureDescriptor);

                RenderTextureDescriptor opaqueDesc = cameraTextureDescriptor;
                opaqueDesc.width /= volumeSettings.downscaling.value;
                opaqueDesc.height /= volumeSettings.downscaling.value;

                cmd.GetTemporaryRT(blurredID, opaqueDesc);
                cmd.GetTemporaryRT(blurredID2, opaqueDesc);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (ShouldRender(renderingData) == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                int blurMode = (volumeSettings.quality.value == LightStreaks.Quality.Performance) ? (int)Pass.BlurFast : (int)Pass.Blur;

                float luminanceThreshold = Mathf.GammaToLinearSpace(volumeSettings.luminanceThreshold.value);

                Material.SetVector("_Params", new Vector4(luminanceThreshold, volumeSettings.intensity.value, 0f, 0f));

                CopyTargets(cmd, renderingData);
                
                Blit(this, cmd, cameraColorTarget, emissionTex, Material, (int)Pass.LuminanceDiff);
                Blit(cmd, emissionTex, blurredID);

                float ratio = Mathf.Clamp(volumeSettings.direction.value, -1, 1);
                float rw = ratio < 0 ? -ratio * 1f : 0f;
                float rh = ratio > 0 ? ratio * 4f : 0f;

                int iterations = (volumeSettings.quality.value == LightStreaks.Quality.Performance) ? volumeSettings.iterations.value * 3 : volumeSettings.iterations.value;

                for (int i = 0; i < iterations; i++)
                {
                    // horizontal blur
                    cmd.SetGlobalVector("_BlurOffsets", new Vector4(rw * volumeSettings.blur.value / renderingData.cameraData.camera.scaledPixelWidth, rh / renderingData.cameraData.camera.scaledPixelHeight, 0, 0));
                    Blit(this, cmd, blurredID, blurredID2, Material, blurMode);

                    // vertical blur
                    cmd.SetGlobalVector("_BlurOffsets", new Vector4((rw * volumeSettings.blur.value) * 2f / renderingData.cameraData.camera.scaledPixelWidth, rh * 2f / renderingData.cameraData.camera.scaledPixelHeight, 0, 0));
                    Blit(this, cmd, blurredID2, blurredID, Material, blurMode);
                }

                cmd.SetGlobalTexture("_BloomTex", blurredID);

                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, (volumeSettings.debug.value) ? (int)Pass.Debug : (int)Pass.Blend);
            }

            public override void Cleanup(CommandBuffer cmd)
            {
                base.Cleanup(cmd);

                cmd.ReleaseTemporaryRT(emissionTex);
                cmd.ReleaseTemporaryRT(blurredID);
                cmd.ReleaseTemporaryRT(blurredID2);
            }
        }

        LightStreaksRenderPass m_ScriptablePass;

        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings();

        public override void Create()
        {
            m_ScriptablePass = new LightStreaksRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}