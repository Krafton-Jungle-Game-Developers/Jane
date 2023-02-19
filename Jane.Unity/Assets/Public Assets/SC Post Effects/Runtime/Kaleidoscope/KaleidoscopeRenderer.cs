using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class KaleidoscopeRenderer : ScriptableRendererFeature
    {
        class KaleidoscopeRenderPass : PostEffectRenderer<Kaleidoscope>
        {
            public KaleidoscopeRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.Kaleidoscope;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Kaleidoscope>();
                
                if(volumeSettings && volumeSettings.IsActive()) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                base.ConfigurePass(cmd, cameraTextureDescriptor);
            }

            private static readonly int _KaleidoscopeSplits = Shader.PropertyToID("_KaleidoscopeSplits");

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (ShouldRender(renderingData) == false) return;
                
                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd, renderingData);

                Material.SetVector(ShaderParameters.Params, new Vector4(Mathf.PI * 2 / Mathf.Max(1, volumeSettings.radialSplits.value), volumeSettings.maintainAspectRatio.value ? 1 : 0, volumeSettings.center.value.x, volumeSettings.center.value.y));
                Material.SetVector(_KaleidoscopeSplits, new Vector4(volumeSettings.horizontalSplits.value, volumeSettings.verticalSplits.value, 0, 0));

                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, 0);
            }
        }

        KaleidoscopeRenderPass m_ScriptablePass;

        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings(false);

        public override void Create()
        {
            m_ScriptablePass = new KaleidoscopeRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}