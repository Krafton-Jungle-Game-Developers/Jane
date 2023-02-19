using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class LensFlaresRenderer : ScriptableRendererFeature
    {
        class LensFlaresRenderPass : PostEffectRenderer<LensFlares>
        {
            private int emissionTex;
            private int flaresTex;
            private int blurredID;
            private int blurredID2;
            
            public LensFlaresRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.LensFlares;
                ProfilerTag = this.ToString();
                
                emissionTex = Shader.PropertyToID("_BloomTex");
                flaresTex = Shader.PropertyToID("_FlaresTex");
                blurredID = Shader.PropertyToID("_Temp1");
                blurredID2 = Shader.PropertyToID("_Temp2");
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<LensFlares>();
                
                if(volumeSettings && volumeSettings.IsActive()) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                base.ConfigurePass(cmd, cameraTextureDescriptor);
                
                RenderTextureDescriptor opaqueDesc = cameraTextureDescriptor;
                opaqueDesc.colorFormat = RenderTextureFormat.DefaultHDR;
                
                cmd.GetTemporaryRT(emissionTex, opaqueDesc);
                cmd.SetGlobalTexture("_BloomTex", emissionTex);

                cmd.GetTemporaryRT(flaresTex, opaqueDesc);
                cmd.SetGlobalTexture("_FlaresTex", flaresTex);
                
                opaqueDesc.width /= 2;
                opaqueDesc.height /= 2;
                cmd.GetTemporaryRT(blurredID, opaqueDesc);
                cmd.GetTemporaryRT(blurredID2, opaqueDesc);
            }
            
            enum Pass
            {
                LuminanceDiff,
                Ghosting,
                Blur,
                Blend,
                Debug
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (ShouldRender(renderingData) == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd, renderingData);

                Material.SetFloat("_Intensity", volumeSettings.intensity.value);
                float luminanceThreshold = Mathf.GammaToLinearSpace(volumeSettings.luminanceThreshold.value);
                Material.SetFloat("_Threshold", luminanceThreshold);
                Material.SetFloat("_Distance", volumeSettings.distance.value);
                Material.SetFloat("_Falloff", volumeSettings.falloff.value);
                Material.SetFloat("_Ghosts", volumeSettings.iterations.value);
                Material.SetFloat("_HaloSize", volumeSettings.haloSize.value);
                Material.SetFloat("_HaloWidth", volumeSettings.haloWidth.value);
                Material.SetFloat("_ChromaticAbberation", volumeSettings.chromaticAbberation.value);

                Material.SetTexture("_ColorTex", volumeSettings.colorTex.value ? volumeSettings.colorTex.value : Texture2D.whiteTexture as Texture);
                Material.SetTexture("_MaskTex", volumeSettings.maskTex.value ? volumeSettings.maskTex.value : Texture2D.whiteTexture as Texture);
                
                Blit(this, cmd, cameraColorTarget, emissionTex, Material, (int)Pass.LuminanceDiff);
                Blit(this, cmd, emissionTex, flaresTex, Material, (int)Pass.Ghosting );
                
                // downsample screen copy into smaller RT, release screen RT
                Blit(cmd,flaresTex, blurredID);
                for (int i = 0; i < volumeSettings.passes.value; i++)
                {
                    // horizontal blur
                    cmd.SetGlobalVector("_BlurOffsets", new Vector4(volumeSettings.blur.value / renderingData.cameraData.camera.scaledPixelWidth, 0, 0, 0));
                    Blit(this, cmd, blurredID, blurredID2, Material, (int)Pass.Blur );

                    // vertical blur
                    cmd.SetGlobalVector("_BlurOffsets", new Vector4(0, volumeSettings.blur.value / renderingData.cameraData.camera.scaledPixelHeight, 0, 0));
                    Blit(this, cmd, blurredID2, blurredID, Material, (int)Pass.Blur );

                }

                cmd.SetGlobalTexture("_FlaresTex", blurredID);
                
                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, (volumeSettings.debug.value) ? (int)Pass.Debug : (int)Pass.Blend);
            }

            public override void Cleanup(CommandBuffer cmd)
            {
                base.Cleanup(cmd);
                
                cmd.ReleaseTemporaryRT(emissionTex);
                cmd.ReleaseTemporaryRT(flaresTex);
                cmd.ReleaseTemporaryRT(blurredID);
                cmd.ReleaseTemporaryRT(blurredID2);
            }
        }

        LensFlaresRenderPass m_ScriptablePass;

        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings();

        public override void Create()
        {
            m_ScriptablePass = new LensFlaresRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}