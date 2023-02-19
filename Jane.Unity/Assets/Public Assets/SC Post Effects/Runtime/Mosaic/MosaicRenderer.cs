using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class MosaicRenderer : ScriptableRendererFeature
    {
        class MosaicRenderPass : PostEffectRenderer<Mosaic>
        {
            public MosaicRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.Mosaic;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Mosaic>();
                
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

                float size = volumeSettings.size.value;

                switch ((Mosaic.MosaicMode)volumeSettings.mode)
                {
                    case Mosaic.MosaicMode.Triangles:
                        size = 10f / volumeSettings.size.value;
                        break;
                    case Mosaic.MosaicMode.Hexagons:
                        size = volumeSettings.size.value / 10f;
                        break;
                    case Mosaic.MosaicMode.Circles:
                        size = (1 - volumeSettings.size.value) * 100f;
                        break;
                }

                Vector4 parameters = new Vector4(size, ((renderingData.cameraData.camera.scaledPixelWidth * 2 / renderingData.cameraData.camera.scaledPixelHeight) * size / Mathf.Sqrt(3f)), 0f, 0f);

                Material.SetVector("_Params", parameters);

                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, (int)volumeSettings.mode.value);
            }
        }

        MosaicRenderPass m_ScriptablePass;

        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings();

        public override void Create()
        {
            m_ScriptablePass = new MosaicRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}