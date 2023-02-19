using UnityEngine.Rendering.Universal;

using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class SketchRenderer : ScriptableRendererFeature
    {
        class SketchRenderPass : PostEffectRenderer<Sketch>
        {
            public SketchRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.Sketch;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                this.cameraDepthTarget = GetCameraDepthTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Sketch>();
                
                if(volumeSettings && volumeSettings.IsActive()) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                requiresDepth = volumeSettings.projectionMode == Sketch.SketchProjectionMode.WorldSpace;

                base.ConfigurePass(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (ShouldRender(renderingData) == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);
                
                CopyTargets(cmd, renderingData);

                if (volumeSettings.strokeTex.value) Material.SetTexture("_Strokes", volumeSettings.strokeTex.value);

                Material.SetVector("_Params", new Vector4(0, (int)volumeSettings.blendMode.value, volumeSettings.intensity.value, ((int)volumeSettings.projectionMode.value == 1) ? volumeSettings.tiling.value * 0.1f : volumeSettings.tiling.value));
                Material.SetVector("_Brightness", volumeSettings.brightness.value);

                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, (int)volumeSettings.projectionMode.value);
            }
        }

        SketchRenderPass m_ScriptablePass;

        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings();

        public override void Create()
        {
            m_ScriptablePass = new SketchRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}
