using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class DitheringRenderer : ScriptableRendererFeature
    {
        class DitheringRenderPass : PostEffectRenderer<Dithering>
        {
            public DitheringRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.Dithering;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Dithering>();
                
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

                var lutTexture = volumeSettings.lut.value == null ? Texture2D.blackTexture : volumeSettings.lut.value;
                Material.SetTexture("_LUT", lutTexture);
                float luminanceThreshold = QualitySettings.activeColorSpace == ColorSpace.Gamma ? Mathf.LinearToGammaSpace(volumeSettings.luminanceThreshold.value) : volumeSettings.luminanceThreshold.value;

                Vector4 ditherParams = new Vector4(0f, volumeSettings.tiling.value, luminanceThreshold, volumeSettings.intensity.value);
                Material.SetVector("_Dithering_Coords", ditherParams);

                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, 0);
            }
        }

        DitheringRenderPass m_ScriptablePass;
        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings();
        
        public override void Create()
        {
            m_ScriptablePass = new DitheringRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}