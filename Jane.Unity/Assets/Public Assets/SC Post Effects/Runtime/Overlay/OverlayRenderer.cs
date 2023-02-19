using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class OverlayRenderer : ScriptableRendererFeature
    {
        class OverlayRenderPass : PostEffectRenderer<Overlay>
        {
            public OverlayRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.Overlay;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Overlay>();
                
                if(volumeSettings && volumeSettings.IsActive()) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                base.ConfigurePass(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (ShouldRender(renderingData) == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd, renderingData);

                if (volumeSettings.overlayTex.value) Material.SetTexture("_OverlayTex", volumeSettings.overlayTex.value);
                Material.SetVector("_Params", new Vector4(volumeSettings.intensity.value, Mathf.Pow(volumeSettings.tiling.value + 1, 2), volumeSettings.autoAspect.value ? 1f : 0f, (int)volumeSettings.blendMode.value));

                float luminanceThreshold = QualitySettings.activeColorSpace == ColorSpace.Gamma ? Mathf.LinearToGammaSpace(volumeSettings.luminanceThreshold.value) : volumeSettings.luminanceThreshold.value;
                Material.SetFloat("_LuminanceThreshold", luminanceThreshold);
                
                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, 0);
            }
        }

        OverlayRenderPass m_ScriptablePass;

        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings();

        public override void Create()
        {
            m_ScriptablePass = new OverlayRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}