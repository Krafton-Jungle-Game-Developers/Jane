using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class CausticsRenderer : ScriptableRendererFeature
    {
        class CausticsRenderPass : PostEffectRenderer<Caustics>
        {
            public CausticsRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.Caustics;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Caustics>();
                
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

                if(volumeSettings.causticsTexture.value) Material.SetTexture("_CausticsTex", volumeSettings.causticsTexture.value);
                Material.SetFloat("_LuminanceThreshold", Mathf.GammaToLinearSpace(volumeSettings.luminanceThreshold.value));

                if (volumeSettings.projectFromSun.value) SetMainLightProjection(cmd, renderingData);

                Material.SetVector("_CausticsParams", new Vector4(volumeSettings.size.value, volumeSettings.speed.value, volumeSettings.projectFromSun.value ? 1 : 0, volumeSettings.intensity.value));
                Material.SetVector("_HeightParams", new Vector4(volumeSettings.minHeight.value, volumeSettings.minHeightFalloff.value, volumeSettings.maxHeight.value, volumeSettings.maxHeightFalloff.value));
            
                cmd.SetGlobalVector("_FadeParams", new Vector4(volumeSettings.startFadeDistance.value, volumeSettings.endFadeDistance.value, 0, volumeSettings.distanceFade.value ? 1 : 0));
                
                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, 0);
            }
        }

        CausticsRenderPass m_ScriptablePass;
        [System.Serializable]
        public class Causticsettings : EffectBaseSettings
        {
            [Header("Effect specific")]
            [Tooltip("Executes the effect before transparent materials are rendered.")]
            public bool skipTransparents;
        }

        [SerializeField]
        public Causticsettings settings = new Causticsettings();
        
        public override void Create()
        {
            m_ScriptablePass = new CausticsRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.skipTransparents ? RenderPassEvent.BeforeRenderingTransparents : settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}