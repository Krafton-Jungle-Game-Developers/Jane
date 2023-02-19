using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class SpeedLinesRenderer : ScriptableRendererFeature
    {
        class SpeedLinesRenderPass : PostEffectRenderer<SpeedLines>
        {
            public SpeedLinesRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.SpeedLines;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<SpeedLines>();
                
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

                float falloff = 2f + (volumeSettings.falloff.value - 0.0f) * (16.0f - 2f) / (1.0f - 0.0f);
                Material.SetVector("_Params", new Vector4(volumeSettings.intensity.value, falloff, volumeSettings.size.value * 2, 0));
                if (volumeSettings.noiseTex.value) Material.SetTexture("_NoiseTex", volumeSettings.noiseTex.value);

                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, 0);
            }
        }

        SpeedLinesRenderPass m_ScriptablePass;

        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings();

        public override void Create()
        {
            m_ScriptablePass = new SpeedLinesRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}