using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class TransitionRenderer : ScriptableRendererFeature
    {
        class TransitionRenderPass : PostEffectRenderer<Transition>
        {
            public TransitionRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.Transition;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Transition>();
                
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

                Material.SetFloat("_Progress", volumeSettings.progress.value);
                var overlayTexture = volumeSettings.gradientTex.value == null ? Texture2D.whiteTexture : volumeSettings.gradientTex.value;
                Material.SetTexture("_Gradient", overlayTexture);

                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, 0);
            }
        }

        TransitionRenderPass m_ScriptablePass;

        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings();

        public override void Create()
        {
            m_ScriptablePass = new TransitionRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}