using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class PosterizeRenderer : ScriptableRendererFeature
    {
        class PosterizeRenderPass : PostEffectRenderer<Posterize>
        {
            public PosterizeRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.Posterize;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Posterize>();
                
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

                Material.SetVector("_Params", new Vector4(volumeSettings.hue.value, volumeSettings.saturation.value, volumeSettings.value.value, volumeSettings.levels.value));

                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, volumeSettings.hsvMode.value ? 1 : 0);
            }
        }

        PosterizeRenderPass m_ScriptablePass;

        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings();

        public override void Create()
        {
            m_ScriptablePass = new PosterizeRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}