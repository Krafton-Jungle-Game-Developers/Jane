using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using Vector3Parameter = UnityEngine.Rendering.PostProcessing.Vector3Parameter;
using MinAttribute = UnityEngine.Rendering.PostProcessing.MinAttribute;
using MaxAttribute = UnityEngine.Rendering.PostProcessing.MaxAttribute;

namespace SCPE
{
    [PostProcess(typeof(FogRenderer), PostProcessEvent.BeforeStack, "SC Post Effects/Environment/Screen-Space Fog")]
    [Serializable]
    public sealed class Fog : PostProcessEffectSettings
    {
        [DisplayName("Use scene's settings"), Tooltip("Use the settings of the current active scene found under the Lighting tab\n\nThis is also advisable for third-party scripts that modify fog settings\n\nThis will force the effect to use the scene's fog color")]
        public BoolParameter useSceneSettings = new BoolParameter { value = false };
        [Serializable]
        public sealed class FogModeParameter : ParameterOverride<FogMode> { }
        [DisplayName("Mode"), Tooltip("Sets how the fog distance is calculated")]
        public FogModeParameter fogMode = new FogModeParameter { value = FogMode.Exponential };

        [Range(0f, 1f)]
        public FloatParameter globalDensity = new FloatParameter { value = 0f };
        [DisplayName("Start")]
        public FloatParameter fogStartDistance = new FloatParameter { value = 0f };
        [DisplayName("End")]
        public FloatParameter fogEndDistance = new FloatParameter { value = 600f };

        public enum FogColorSource
        {
            UniformColor,
            GradientTexture,
            SkyboxColor
        }

        [Serializable]

        public sealed class FogColorSourceParameter : ParameterOverride<FogColorSource> { }
        [Space]

        [Tooltip("Color: use a uniform color for the fog\n\nGradient: sample a gradient texture to control the fog color over distance, the alpha channel controls the density\n\nSkybox: Sample the skybox's color for the fog, only works well with low detail skies")]
        public FogColorSourceParameter colorSource = new FogColorSourceParameter { value = FogColorSource.UniformColor };

        [DisplayName("Mipmap"), Tooltip("Set the mipmap level for the skybox texture"), Range(0f, 8f)]
        public FloatParameter skyboxMipLevel = new FloatParameter { value = 0f };

#if UNITY_2018_1_OR_NEWER
        [DisplayName("Color"), ColorUsage(true, true)]
#else
        [DisplayName("Color"), ColorUsage(true, true, 1f, 10f, 0.125f, 3f)]
#endif
        public ColorParameter fogColor = new ColorParameter { value = new Color(0.76f, 0.94f, 1f, 1f) };

        [DisplayName("Texture")]
        public TextureParameter fogColorGradient = new TextureParameter { value = null };
        [Tooltip("Automatic mode uses the current camera's far clipping plane to set the max distance\n\nOtherwise, a fixed value may be used instead")]
        public FloatParameter gradientDistance = new FloatParameter { value = 1000f };
        public BoolParameter gradientUseFarClipPlane = new BoolParameter { value = true };

        [Header("Distance")]
        [DisplayName("Enable")]
        public BoolParameter distanceFog = new BoolParameter { value = true };
        [Range(0.001f, 1.0f)]
        [DisplayName("Density")]
        public FloatParameter distanceDensity = new FloatParameter { value = 1f };
        [Tooltip("Distance based on radial distance from viewer, rather than parrallel")]
        public BoolParameter useRadialDistance = new BoolParameter { value = true };

        [Header("Skybox")]
        [Range(0f, 1f), Tooltip("Determines how much the fog influences the skybox")]
        public FloatParameter skyboxInfluence = new FloatParameter { value = 1f };

        [Header("Directional Light")]
        [Tooltip("Translates the a Directional Light's direction and color into the fog. Creates a faux-atmospheric scattering effect.")]
        public BoolParameter enableDirectionalLight = new BoolParameter { value = false };
        [Tooltip("Use the intensity of the Directional Light that's set as the caster")]
        public BoolParameter useLightDirection = new BoolParameter { value = true };
        [Tooltip("Use the color of the Directional Light that's set as the caster")]
        public BoolParameter useLightColor = new BoolParameter { value = true };
        [Tooltip("Use the intensity of the Directional Light that's set as the caster")]
        public BoolParameter useLightIntensity = new BoolParameter { value = true };

        //Obsolete, to be removed
        public static Vector3 LightDirection = Vector3.zero;
        
#if UNITY_2018_1_OR_NEWER
        [DisplayName("Color"), ColorUsage(true, true)]
#else
        [DisplayName("Color"), ColorUsage(true, true, 1f, 10f, 0.125f, 3f)]
#endif
        public ColorParameter lightColor = new ColorParameter { value = new Color(1f, 0.89f, 0.55f, 1f) };
        [Max(1f)]
        public Vector3Parameter lightDirection = new Vector3Parameter { value = new Vector3(0, 0.5f, -1f) };
        public FloatParameter lightIntensity = new FloatParameter { value = 1f };

        [Header("Height")]
        [DisplayName("Enable"), Tooltip("Enable vertical height fog")]
        public BoolParameter heightFog = new BoolParameter { value = true };

        [Tooltip("Height relative to 0 world height position")]
        public FloatParameter height = new FloatParameter { value = 10f };

        [Range(0.001f, 1.0f)]
        [DisplayName("Density")]
        public FloatParameter heightDensity = new FloatParameter { value = 0.75f };

        [Header("Height noise (2D)")]
        [DisplayName("Enable"), Tooltip("Enables height fog density variation through the use of a texture")]
        public BoolParameter heightFogNoise = new BoolParameter { value = false };
        [DisplayName("Texture (R)"), Tooltip("The density is read from this texture's red color channel")]
        public TextureParameter heightNoiseTex = new TextureParameter { value = null };
        [Range(0f, 1f)]
        [DisplayName("Size")]
        public FloatParameter heightNoiseSize = new FloatParameter { value = 0.25f };
        [Range(0f, 1f)]
        [DisplayName("Strength")]
        public FloatParameter heightNoiseStrength = new FloatParameter { value = 1f };
        [Range(0f, 10f)]
        [DisplayName("Speed")]
        public FloatParameter heightNoiseSpeed = new FloatParameter { value = 2f };

        [Header("Light scattering")]
        [DisplayName("Enable"), Tooltip("Execute a bloom pass to diffuse light in dense fog")]
        public BoolParameter lightScattering = new BoolParameter { value = false };

        [Space]

        [Min(0f), DisplayName("Intensity")]
        public FloatParameter scatterIntensity = new FloatParameter { value = 10f };

        [Min(0f), DisplayName("Threshold"), Tooltip("Filters out pixels under this level of brightness. Value is in gamma-space.")]
        public FloatParameter scatterThreshold = new FloatParameter { value = 1f };

        [Range(1f, 10f), DisplayName("Diffusion")]
        public FloatParameter scatterDiffusion = new FloatParameter { value = 10f };

        [Range(0f, 1f), DisplayName("Smoothness"), Tooltip("Makes transitions between under/over-threshold gradual. 0 for a hard threshold, 1 for a soft threshold).")]
        public FloatParameter scatterSoftKnee = new FloatParameter { value = 0.5f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value;
        }
        
        #if UNITY_EDITOR
        private void Reset()
        {
            //Auto assign default texture
            if (heightNoiseTex.value == null) heightNoiseTex.value = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(UnityEditor.AssetDatabase.GUIDToAssetPath("82a8d5f38e2ccf44c86362214b2d3e5e"));
        }
        #endif
    }
}