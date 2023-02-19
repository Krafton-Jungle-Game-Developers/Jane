using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class KuwaharaRenderer : ScriptableRendererFeature
    {
        class KuwaharaRenderPass : PostEffectRenderer<Kuwahara>
        {
            private int mode;
            
            public KuwaharaRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.Kuwahara;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer, EffectBaseSettings settings)
            {
                this.settings = settings;
                this.cameraColorTarget = GetCameraTarget(renderer);
                this.cameraDepthTarget = GetCameraDepthTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Kuwahara>();
                
                if(volumeSettings && volumeSettings.IsActive()) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                requiresDepth = volumeSettings.mode == Kuwahara.KuwaharaMode.DepthFade;

                base.ConfigurePass(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (ShouldRender(renderingData) == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);
                
                mode = (int)volumeSettings.mode.value;
                if (renderingData.cameraData.camera.orthographic) mode = (int)Kuwahara.KuwaharaMode.FullScreen;

                CopyTargets(cmd, renderingData);

                Material.SetFloat("_Radius", (float)volumeSettings.radius);
                if(mode == (int)Kuwahara.KuwaharaMode.DepthFade) Material.SetVector("_FadeParams", new Vector4(volumeSettings.startFadeDistance.value, volumeSettings.endFadeDistance.value, 0, 0));

                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, mode);
            }
        }

        KuwaharaRenderPass m_ScriptablePass;

        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings();

        public override void Create()
        {
            m_ScriptablePass = new KuwaharaRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer, settings);
        }
    }
}