using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    public class HueShift3DRenderer : ScriptableRendererFeature
    {
        class HueShift3DRenderPass : PostEffectRenderer<HueShift3D>
        {
            public HueShift3DRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.HueShift3D;
                ProfilerTag = this.ToString();
            }

            public void Setup(ScriptableRenderer renderer, bool reconstructDepthNormals)
            {
                this.reconstructDepthNormals = reconstructDepthNormals;
                this.cameraColorTarget = GetCameraTarget(renderer);
                this.cameraDepthTarget = GetCameraDepthTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<HueShift3D>();
                
                if(volumeSettings && volumeSettings.IsActive()) renderer.EnqueuePass(this);
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings) return;

                requiresDepthNormals = volumeSettings.IsActive() && volumeSettings.geoInfluence.value > 0f;

                base.ConfigurePass(cmd, cameraTextureDescriptor);
            }
            
            enum Pass
            {
                ColorSpectrum,
                GradientTexture
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (ShouldRender(renderingData) == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

                CopyTargets(cmd, renderingData);

                HueShift3D.isOrtho = renderingData.cameraData.camera.orthographic;

                Material.SetVector("_Params", new Vector4(volumeSettings.speed.value, volumeSettings.size.value, volumeSettings.geoInfluence.value, volumeSettings.intensity.value));
                if(volumeSettings.gradientTex.value) Material.SetTexture("_GradientTex", volumeSettings.gradientTex.value);
                
                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, volumeSettings.colorSource.value == (int)HueShift3D.ColorSource.RGBSpectrum ? (int)Pass.ColorSpectrum : (int)Pass.GradientTexture);
            }
        }

        HueShift3DRenderPass m_ScriptablePass;

        [System.Serializable]
        public class HueShift3DSettings : EffectBaseSettings
        {
            [Header("Effect specific")]
            [Tooltip("Reconstruct the scene geometry's normals from the depth texture." +
                     "\n\nIn Unity 2020.3+, disabling this will have the effect use the Depth-Normals prepass, which is more accurate. This will have all object re-render, if the scene isn't already optimized for draw calls, this will negatively affect performance")]
            public bool reconstructDepthNormals = false;
        }

        [SerializeField]
        public HueShift3DSettings settings = new HueShift3DSettings();

        public override void Create()
        {
            m_ScriptablePass = new HueShift3DRenderPass(settings);
            m_ScriptablePass.renderPassEvent = settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer, settings.reconstructDepthNormals);
        }
    }
}