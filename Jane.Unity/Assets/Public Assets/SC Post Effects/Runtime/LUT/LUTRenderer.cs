using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class LUTRenderer : ScriptableRendererFeature
    {
        class LUTRenderPass : PostEffectRenderer<LUT>
        {

            public LUTRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.LUT;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                this.cameraDepthTarget = GetCameraDepthTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<LUT>();
                
                if(volumeSettings && volumeSettings.IsActive()) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                requiresDepth = volumeSettings.mode == LUT.Mode.DistanceBased;

                base.ConfigurePass(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (LUT.Bypass) return;
                
                if (ShouldRender(renderingData) == false) return;
                
                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd, renderingData);

                Material.SetVector("_LUT_Params", new Vector4(volumeSettings.lutNear.value ? volumeSettings.intensity.value : 0f, volumeSettings.invert.value));
                
                if (volumeSettings.lutNear.value)
                {
                    Material.SetTexture("_LUT_Near", volumeSettings.lutNear.value);
                }

                if ((int)volumeSettings.mode.value == 1)
                {
                    Material.SetVector("_FadeParams", new Vector4(volumeSettings.startFadeDistance.value, volumeSettings.endFadeDistance.value, 0, 0));

                    if (volumeSettings.lutFar.value) Material.SetTexture("_LUT_Far", volumeSettings.lutFar.value);
                }
                
                Material.SetVector(ShaderParameters.Params, new Vector4(volumeSettings.vibranceRGBBalance.value.r, volumeSettings.vibranceRGBBalance.value.g, volumeSettings.vibranceRGBBalance.value.b, volumeSettings.vibrance.value));
                
                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, (int)volumeSettings.mode.value);
            }
        }

        LUTRenderPass m_ScriptablePass;

        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings();

        public override void Create()
        {
            m_ScriptablePass = new LUTRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}
