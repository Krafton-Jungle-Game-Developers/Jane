using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class FogRenderer : PostProcessEffectRenderer<Fog>
    {
        Shader shader;

        struct MipLevel
        {
            internal int down;
            internal int up;
        }

        MipLevel[] m_Pyramid;
        const int k_MaxPyramidSize = 16;

        enum Pass
        {
            Prefilter,
            Downsample,
            Upsample,
            Blend,
            BlendScattering
        }

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Fog);

            m_Pyramid = new MipLevel[k_MaxPyramidSize];

            for (int i = 0; i < k_MaxPyramidSize; i++)
            {
                m_Pyramid[i] = new MipLevel
                {
                    down = Shader.PropertyToID("_BloomMipDown" + i),
                    up = Shader.PropertyToID("_BloomMipUp" + i)
                };
            }
        }

        public override void Release()
        {
            base.Release();
        }

        public static Dictionary<Camera, RenderScreenSpaceSkybox> skyboxCams = new Dictionary<Camera, RenderScreenSpaceSkybox>();

        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(shader);
            CommandBuffer cmd = context.command;

            Camera cam = context.camera;

            #region Skybox sampling
            //Add the skybox rendering component to any camera rendering Fog
            if (settings.colorSource.value == Fog.FogColorSource.SkyboxColor)
            {
                //Ignore hidden camera's, except scene-view cam
                if (cam.hideFlags != HideFlags.None && cam.name != "SceneCamera") return;

                if (!skyboxCams.ContainsKey(cam))
                {
                    skyboxCams[cam] = cam.gameObject.GetComponent<RenderScreenSpaceSkybox>();

                    if (!skyboxCams[cam])
                    {
                        skyboxCams[cam] = cam.gameObject.AddComponent<RenderScreenSpaceSkybox>();
                    }
                    skyboxCams[cam].manuallyAdded = false; //Don't show warning on component
                }
            }
            #endregion

            //Clip-space to world-space camera matrix conversion
            #region Property value composition

            float FdotC = cam.transform.position.y - settings.height;
            float paramK = (FdotC <= 0.0f ? 1.0f : 0.0f);
            //Always exclude skybox for skybox color mode
            //Always include when using light scattering to avoid depth discontinuity
            float skyboxInfluence = (settings.lightScattering) ? 1.0f : settings.skyboxInfluence;
            float distanceFog = (settings.distanceFog) ? 1.0f : 0.0f;
            float heightFog = (settings.heightFog) ? 1.0f : 0.0f;

            int colorSource = (settings.useSceneSettings) ? 0 : (int)settings.colorSource.value;
            var sceneMode = (settings.useSceneSettings) ? RenderSettings.fogMode : settings.fogMode;
            var sceneDensity = (settings.useSceneSettings) ? RenderSettings.fogDensity : settings.globalDensity / 100;
            var sceneStart = (settings.useSceneSettings) ? RenderSettings.fogStartDistance : settings.fogStartDistance;
            var sceneEnd = (settings.useSceneSettings) ? RenderSettings.fogEndDistance : settings.fogEndDistance;

            bool linear = (sceneMode == FogMode.Linear);
            float diff = linear ? sceneEnd - sceneStart : 0.0f;
            float invDiff = Mathf.Abs(diff) > 0.0001f ? 1.0f / diff : 0.0f;

            Vector4 sceneParams;
            sceneParams.x = sceneDensity * 1.2011224087f; // density / sqrt(ln(2)), used by Exp2 fog mode
            sceneParams.y = sceneDensity * 1.4426950408f; // density / ln(2), used by Exp fog mode
            sceneParams.z = linear ? -invDiff : 0.0f;
            sceneParams.w = linear ? sceneEnd * invDiff : 0.0f;

            float gradientDistance = (settings.gradientUseFarClipPlane.value) ? settings.gradientDistance : context.camera.farClipPlane;
            #endregion

            #region Property assignment
            if (settings.heightNoiseTex.value) sheet.properties.SetTexture("_NoiseTex", settings.heightNoiseTex);
            if (settings.fogColorGradient.value) sheet.properties.SetTexture("_ColorGradient", settings.fogColorGradient);
            cmd.SetGlobalFloat("_FarClippingPlane", gradientDistance);
            cmd.SetGlobalVector("_SceneFogParams", sceneParams);
            cmd.SetGlobalVector("_SceneFogMode", new Vector4((int)sceneMode, settings.useRadialDistance ? 1 : 0, colorSource, settings.heightFogNoise ? 1 : 0));
            cmd.SetGlobalVector("_NoiseParams", new Vector4(settings.heightNoiseSize * 0.01f, settings.heightNoiseSpeed * 0.01f, settings.heightNoiseStrength, 0));
            cmd.SetGlobalVector("_DensityParams", new Vector4(settings.distanceDensity, settings.heightNoiseStrength, settings.skyboxMipLevel, 0));
            cmd.SetGlobalVector("_HeightParams", new Vector4(settings.height, FdotC, paramK, settings.heightDensity * 0.5f));
            cmd.SetGlobalVector("_DistanceParams", new Vector4(-sceneStart, 0f, distanceFog, heightFog));
            cmd.SetGlobalColor("_FogColor", (settings.useSceneSettings) ? RenderSettings.fogColor : settings.fogColor);
            cmd.SetGlobalVector("_SkyboxParams", new Vector4(skyboxInfluence, settings.skyboxMipLevel, 0, 0));

            Vector3 sunDir = (settings.useLightDirection && RenderSettings.sun) ? -RenderSettings.sun.transform.forward : settings.lightDirection.value.normalized;
            float sunIntensity = (settings.useLightIntensity && RenderSettings.sun) ? RenderSettings.sun.intensity : settings.lightIntensity.value;
            sunIntensity = (settings.enableDirectionalLight) ? sunIntensity : 0f;
            cmd.SetGlobalVector("_DirLightParams", new Vector4(sunDir.x, sunDir.y, sunDir.z, sunIntensity));

            Color sunColor = (settings.useLightColor && RenderSettings.sun) ? RenderSettings.sun.color : settings.lightColor.value;
            cmd.SetGlobalVector("_DirLightColor", new Vector4(sunColor.r, sunColor.g, sunColor.b, 0));

            #endregion

            #region Light scattering
            //Repurpose parts of the bloom effect
            bool enableScattering = (settings.lightScattering) ? true : false;

            if (enableScattering)
            {
                int tw = Mathf.FloorToInt(context.screenWidth / (2f));
                int th = Mathf.FloorToInt(context.screenHeight / (2f));
                bool singlePassDoubleWide = (context.stereoActive && (context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass) && (context.camera.stereoTargetEye == StereoTargetEyeMask.Both));
                int tw_stereo = singlePassDoubleWide ? tw * 2 : tw;

                // Determine the iteration count
                int s = Mathf.Max(tw, th);
                float logs = Mathf.Log(s, 2f) + Mathf.Min(settings.scatterDiffusion.value, 10f) - 10f;
                int logs_i = Mathf.FloorToInt(logs);
                int iterations = Mathf.Clamp(logs_i, 1, k_MaxPyramidSize);
                float sampleScale = 0.5f + logs - logs_i;
                sheet.properties.SetFloat("_SampleScale", sampleScale);

                // Prefiltering parameters
                float lthresh = Mathf.GammaToLinearSpace(settings.scatterThreshold.value);
                float knee = lthresh * settings.scatterSoftKnee.value + 1e-5f;
                var threshold = new Vector4(lthresh, lthresh - knee, knee * 2f, 0.25f / knee);
                cmd.SetGlobalVector("_Threshold", threshold);

                // Downsample
                var lastDown = context.source;
                for (int i = 0; i < iterations; i++)
                {
                    int mipDown = m_Pyramid[i].down;
                    int mipUp = m_Pyramid[i].up;
                    int pass = i == 0 ? (int)Pass.Prefilter : (int)Pass.Downsample;

                    context.GetScreenSpaceTemporaryRT(cmd, mipDown, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, tw_stereo, th);
                    context.GetScreenSpaceTemporaryRT(cmd, mipUp, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, tw_stereo, th);
                    cmd.BlitFullscreenTriangle(lastDown, mipDown, sheet, pass);

                    lastDown = mipDown;
                    tw_stereo = (singlePassDoubleWide && ((tw_stereo / 2) % 2 > 0)) ? 1 + tw_stereo / 2 : tw_stereo / 2;
                    tw_stereo = Mathf.Max(tw_stereo, 1);
                    th = Mathf.Max(th / 2, 1);
                }

                // Upsample
                int lastUp = m_Pyramid[iterations - 1].down;
                for (int i = iterations - 2; i >= 0; i--)
                {
                    int mipDown = m_Pyramid[i].down;
                    int mipUp = m_Pyramid[i].up;
                    cmd.SetGlobalTexture("_BloomTex", mipDown);
                    cmd.BlitFullscreenTriangle(lastUp, mipUp, sheet, (int)Pass.Upsample);
                    lastUp = mipUp;
                }

                float intensity = RuntimeUtilities.Exp2(settings.scatterIntensity.value / 10f) - 1f;
                var shaderSettings = new Vector4(sampleScale, intensity, 0, iterations);
                cmd.SetGlobalVector("_ScatteringParams", shaderSettings);

                cmd.SetGlobalTexture("_BloomTex", lastUp);

                // Cleanup
                for (int i = 0; i < iterations; i++)
                {
                    if (m_Pyramid[i].down != lastUp)
                        cmd.ReleaseTemporaryRT(m_Pyramid[i].down);
                    if (m_Pyramid[i].up != lastUp)
                        cmd.ReleaseTemporaryRT(m_Pyramid[i].up);
                }
            }

            #endregion

            #region shader passes
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, enableScattering ? (int)Pass.BlendScattering : (int)Pass.Blend);
            #endregion

        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth;
        }
    }
}