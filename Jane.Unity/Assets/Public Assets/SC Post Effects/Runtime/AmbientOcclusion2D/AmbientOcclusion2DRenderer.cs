using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class AmbientOcclusion2DRenderer : ScriptableRendererFeature
    {
        class AmbientOcclusion2DRenderPass : PostEffectRenderer<AmbientOcclusion2D>
        {
            private int aoTexID = Shader.PropertyToID("_AO");
            // get two smaller RTs
            int blurredID = Shader.PropertyToID("_Temp1");
            int blurredID2 = Shader.PropertyToID("_Temp2");
            
            public AmbientOcclusion2DRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.AO2D;
                ProfilerTag = this.ToString();
            }
            
            enum Pass
            {
                LuminanceDiff,
                Blur,
                Blend,
                Debug
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<AmbientOcclusion2D>();
                
                if(volumeSettings && volumeSettings.IsActive()) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                base.ConfigurePass(cmd, cameraTextureDescriptor);

                cmd.GetTemporaryRT(aoTexID, cameraTextureDescriptor.width, cameraTextureDescriptor.height, 0, FilterMode.Point, RenderTextureFormat.R8);

                RenderTextureDescriptor rtDsc = cameraTextureDescriptor;
                rtDsc.width /= volumeSettings.downscaling.value;
                rtDsc.height /= volumeSettings.downscaling.value;
                
                cmd.GetTemporaryRT(blurredID, rtDsc.width, rtDsc.height, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
                cmd.GetTemporaryRT(blurredID2, rtDsc.width, rtDsc.height, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (ShouldRender(renderingData) == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd, renderingData);
                
                cmd.SetGlobalFloat("_SampleDistance", volumeSettings.distance.value);
                float luminanceThreshold = QualitySettings.activeColorSpace == ColorSpace.Gamma ? Mathf.GammaToLinearSpace(volumeSettings.luminanceThreshold.value) : volumeSettings.luminanceThreshold.value;

                cmd.SetGlobalFloat("_Threshold", luminanceThreshold);
                cmd.SetGlobalFloat("_Blur", volumeSettings.blurAmount.value);
                cmd.SetGlobalFloat("_Intensity", volumeSettings.intensity.value);
                
                Blit(cmd, cameraColorTarget, aoTexID, base.Material, (int)Pass.LuminanceDiff);

                //Pass AO into blur target texture
                Blit(cmd,aoTexID, blurredID);
                
                for (int i = 0; i < volumeSettings.iterations.value; i++)
                {
                    // horizontal blur
                    cmd.SetGlobalVector("_BlurOffsets", new Vector4((volumeSettings.blurAmount.value) / renderingData.cameraData.camera.scaledPixelWidth, 0, 0, 0));
                    Blit(this, cmd, blurredID, blurredID2, Material, (int)Pass.Blur);

                    // vertical blur
                    cmd.SetGlobalVector("_BlurOffsets", new Vector4(0, (volumeSettings.blurAmount.value) / renderingData.cameraData.camera.scaledPixelHeight, 0, 0));
                    Blit(this, cmd, blurredID2, blurredID, Material, (int)Pass.Blur);
                }
                
                cmd.SetGlobalTexture(aoTexID, blurredID);

                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, (volumeSettings.aoOnly.value) ? (int)Pass.Debug : (int)Pass.Blend);
            }

            public override void Cleanup(CommandBuffer cmd)
            {
                base.Cleanup(cmd);
                
                cmd.ReleaseTemporaryRT(blurredID);
                cmd.ReleaseTemporaryRT(blurredID2);
                cmd.ReleaseTemporaryRT(aoTexID);
            }
        }

        AmbientOcclusion2DRenderPass m_ScriptablePass;
        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings(true);
        
        public override void Create()
        {
            #if URP_12_0_OR_NEWER || SCPE_DEV
            m_ScriptablePass = new AmbientOcclusion2DRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
            #endif
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            //Render features aren't supported for 2D in older versions
            #if URP_12_0_OR_NEWER || SCPE_DEV
            m_ScriptablePass.Setup(renderer);
            #endif
        }
    }
}