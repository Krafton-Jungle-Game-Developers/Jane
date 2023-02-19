using UnityEngine.Rendering.Universal;
using System;
using UnityEngine.Rendering;
using UnityEngine;

namespace SCPE
{
    #if URP_11_0_OR_NEWER
    [DisallowMultipleRendererFeature]
    #endif
    public class FogRenderer : ScriptableRendererFeature
    {
        public static Material FogMaterial;
        public static bool enableSkyboxCapture;

        class FogRenderPass : PostEffectRenderer<Fog>
        {
            public FogRenderPass(EffectBaseSettings settings)
            {
                this.settings = settings;
                shaderName = ShaderNames.Fog;
                requiresDepth = true;
                ProfilerTag = this.ToString();
            }
            enum Pass
            {
                Prefilter,
                Downsample,
                Upsample,
                Blend,
                BlendScattering
            }
            
            public void Setup(ScriptableRenderer renderer)
            {
                FogMaterial = this.Material;
                this.cameraColorTarget = GetCameraTarget(renderer);
                volumeSettings = VolumeManager.instance.stack.GetComponent<Fog>();

                if (volumeSettings && volumeSettings.IsActive())
                {
                    renderer.EnqueuePass(this);
                }
                else
                {
                    enableSkyboxCapture = false;
                }
            }

            public override void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                if (!volumeSettings)
                {
                    enableSkyboxCapture = false;
                    return;
                }

                base.ConfigurePass(cmd, cameraTextureDescriptor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (ShouldRender(renderingData) == false) return;

                base.Execute(context, ref renderingData);

                var cmd = CommandBufferPool.Get(ProfilerTag);
                
                CopyTargets(cmd, renderingData);
                
                enableSkyboxCapture = volumeSettings.colorSource == Fog.FogColorSource.SkyboxColor;

                #region Property value composition
     float FdotC = renderingData.cameraData.camera.transform.position.y - volumeSettings.height.value;
                float paramK = (FdotC <= 0.0f ? 1.0f : 0.0f);
                //Always exclude skybox for skybox color mode
                //Always include when using light scattering to avoid depth discontinuity
                float skyboxInfluence = (volumeSettings.lightScattering.value) ? 1.0f : volumeSettings.skyboxInfluence.value;
                float distanceFog = (volumeSettings.distanceFog.value) ? 1.0f : 0.0f;
                float heightFog = (volumeSettings.heightFog.value) ? 1.0f : 0.0f;

                int colorSource = (volumeSettings.useSceneSettings.value) ? 0 : (int)volumeSettings.colorSource.value;
                var sceneMode = (volumeSettings.useSceneSettings.value) ? RenderSettings.fogMode : volumeSettings.fogMode.value;
                var sceneDensity = (volumeSettings.useSceneSettings.value) ? RenderSettings.fogDensity : volumeSettings.globalDensity.value / 100;
                var sceneStart = (volumeSettings.useSceneSettings.value) ? RenderSettings.fogStartDistance : volumeSettings.fogStartDistance.value;
                var sceneEnd = (volumeSettings.useSceneSettings.value) ? RenderSettings.fogEndDistance : volumeSettings.fogEndDistance.value;

                bool linear = (sceneMode == FogMode.Linear);
                float diff = linear ? sceneEnd - sceneStart : 0.0f;
                float invDiff = Mathf.Abs(diff) > 0.0001f ? 1.0f / diff : 0.0f;

                Vector4 sceneParams;
                sceneParams.x = sceneDensity * 1.2011224087f; // density / sqrt(ln(2)), used by Exp2 fog mode
                sceneParams.y = sceneDensity * 1.4426950408f; // density / ln(2), used by Exp fog mode
                sceneParams.z = linear ? -invDiff : 0.0f;
                sceneParams.w = linear ? sceneEnd * invDiff : 0.0f;

                float gradientDistance = (volumeSettings.gradientUseFarClipPlane.value) ? volumeSettings.gradientDistance.value : renderingData.cameraData.camera.farClipPlane;
                #endregion

                #region Property assignment
                if (volumeSettings.heightNoiseTex.value) cmd.SetGlobalTexture("_NoiseTex", volumeSettings.heightNoiseTex.value);
                if (volumeSettings.fogColorGradient.value) cmd.SetGlobalTexture("_ColorGradient", volumeSettings.fogColorGradient.value);
                cmd.SetGlobalFloat("_FarClippingPlane", gradientDistance);
                cmd.SetGlobalVector("_SceneFogParams", sceneParams);
                cmd.SetGlobalVector("_SceneFogMode", new Vector4((int)sceneMode, volumeSettings.useRadialDistance.value ? 1 : 0, colorSource, volumeSettings.heightFogNoise.value ? 1 : 0));
                cmd.SetGlobalVector("_NoiseParams", new Vector4(volumeSettings.heightNoiseSize.value * 0.01f, volumeSettings.heightNoiseSpeed.value * 0.01f, volumeSettings.heightNoiseStrength.value, 0));
                cmd.SetGlobalVector("_DensityParams", new Vector4(volumeSettings.distanceDensity.value, volumeSettings.heightNoiseStrength.value, volumeSettings.skyboxMipLevel.value, 0));
                cmd.SetGlobalVector("_HeightParams", new Vector4(volumeSettings.height.value, FdotC, paramK, volumeSettings.heightDensity.value * 0.5f));
                cmd.SetGlobalVector("_DistanceParams", new Vector4(-sceneStart, 0f, distanceFog, heightFog));
                cmd.SetGlobalColor("_FogColor", (volumeSettings.useSceneSettings.value) ? RenderSettings.fogColor : volumeSettings.fogColor.value);
                cmd.SetGlobalVector("_SkyboxParams", new Vector4(skyboxInfluence, volumeSettings.skyboxMipLevel.value, 0, 0));

                Vector3 sunDir = (volumeSettings.useLightDirection.value && RenderSettings.sun) ? -RenderSettings.sun.transform.forward : volumeSettings.lightDirection.value.normalized;
                float sunIntensity = (volumeSettings.useLightIntensity.value && RenderSettings.sun) ? RenderSettings.sun.intensity : volumeSettings.lightIntensity.value;
                sunIntensity = (volumeSettings.enableDirectionalLight.value) ? sunIntensity : 0f;
                cmd.SetGlobalVector("_DirLightParams", new Vector4(sunDir.x, sunDir.y, sunDir.z, sunIntensity));

                Color sunColor = (volumeSettings.useLightColor.value && RenderSettings.sun) ? RenderSettings.sun.color.linear : volumeSettings.lightColor.value;
                cmd.SetGlobalVector("_DirLightColor", new Vector4(sunColor.r, sunColor.g, sunColor.b, 0));
                #endregion

                bool enableScattering = false;

                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, enableScattering ? (int)Pass.BlendScattering : (int)Pass.Blend);
            }
        }
        
        public class SkyboxTextureRenderPass : ScriptableRenderPass
        {
            private int skyboxTexID = Shader.PropertyToID("_SkyboxTex");

            public string ProfilerTag = "Skybox to texture";

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                //Half resolution
                cameraTextureDescriptor.width /= 2;
                cameraTextureDescriptor.height /= 2;
 
                cmd.GetTemporaryRT(skyboxTexID, cameraTextureDescriptor);

                ConfigureTarget(skyboxTexID);
                ConfigureClear(ClearFlag.All, Color.black);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var cmd = CommandBufferPool.Get(ProfilerTag);

                context.DrawSkybox(renderingData.cameraData.camera);
                
                cmd.SetGlobalTexture(skyboxTexID, skyboxTexID);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
            
#if URP_9_0_OR_NEWER
            public override void OnCameraCleanup(CommandBuffer cmd)
#else
            public override void FrameCleanup(CommandBuffer cmd)
#endif
            {
                cmd.ReleaseTemporaryRT(skyboxTexID);
            }
        }

        FogRenderPass fogRenderPass;
        SkyboxTextureRenderPass skyboxRenderPass;

        [Serializable]
        public class FogSettings : EffectBaseSettings
        {
            [Header("Effect specific")]
            [Tooltip("Executes the effect before transparent materials are rendered.")]
            public bool skipTransparents;
        }

        [SerializeField]
        public FogSettings settings = new FogSettings();

        public override void Create()
        {
            fogRenderPass = new FogRenderPass(settings);
            skyboxRenderPass = new SkyboxTextureRenderPass();
            
            fogRenderPass.renderPassEvent = settings.skipTransparents ? RenderPassEvent.BeforeRenderingTransparents : settings.injectionPoint;
            skyboxRenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            fogRenderPass.Setup(renderer);

            if (enableSkyboxCapture) renderer.EnqueuePass(skyboxRenderPass);
        }
    }
}