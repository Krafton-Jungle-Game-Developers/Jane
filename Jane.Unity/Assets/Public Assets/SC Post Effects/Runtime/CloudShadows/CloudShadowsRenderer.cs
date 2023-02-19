using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class CloudShadowsRenderer : ScriptableRendererFeature
    {
        class CloudShadowsRenderPass : PostEffectRenderer<CloudShadows>
        {
            public CloudShadowsRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                this.shaderName = ShaderNames.CloudShadows;
                ProfilerTag = this.ToString();
            }
            
            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<CloudShadows>();
                
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
   
                CloudShadows.isOrtho = renderingData.cameraData.camera.orthographic;

                var noiseTexture = volumeSettings.texture.value == null ? Texture2D.whiteTexture : volumeSettings.texture.value;
                Material.SetTexture("_NoiseTex", noiseTexture);

                float cloudsSpeed = volumeSettings.speed.value * 0.1f;
                Material.SetVector("_CloudParams", new Vector4(volumeSettings.size.value * 0.01f, volumeSettings.direction.value.x * cloudsSpeed, volumeSettings.direction.value.y * cloudsSpeed, volumeSettings.density.value));             

                if (volumeSettings.projectFromSun.value) SetMainLightProjection(cmd, renderingData);
                Material.SetFloat("_ProjectionEnabled", volumeSettings.projectFromSun.value ? 1 : 0);

                cmd.SetGlobalVector("_FadeParams", new Vector4(volumeSettings.startFadeDistance.value, volumeSettings.endFadeDistance.value, 0, 0));

                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, 0);
            }

            public override void Cleanup(CommandBuffer cmd)
            {
                base.Cleanup(cmd);
            }
        }

        CloudShadowsRenderPass m_ScriptablePass;
        [SerializeField]
        public EffectBaseSettings settings = new EffectBaseSettings();
        
        public override void Create()
        {
            m_ScriptablePass = new CloudShadowsRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}