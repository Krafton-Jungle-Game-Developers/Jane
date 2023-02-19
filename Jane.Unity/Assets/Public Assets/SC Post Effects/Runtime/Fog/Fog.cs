using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System;
using UnityEngine;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Environment/Fog")]
    public sealed class Fog : VolumeComponent, IPostProcessComponent
    {
        [Range(0f, 1f), Tooltip("Use the settings of the current active scene found under the Lighting tab\n\nThis is also advisable for third-party scripts that modify fog settings\n\nThis will force the effect to use the scene's fog color")]
        public BoolParameter useSceneSettings = new BoolParameter(false);

        [Serializable]
        public sealed class FogModeParameter : VolumeParameter<FogMode> { }
        [Tooltip("Sets how the fog distance is calculated")]
        public FogModeParameter fogMode = new FogModeParameter { value = FogMode.Exponential };

        [Range(0f, 1f)]
        public ClampedFloatParameter globalDensity = new ClampedFloatParameter(0f,0f,1f);
        //[DisplayName("Start")]
        public FloatParameter fogStartDistance = new FloatParameter(0f);
        //[DisplayName("End")]
        public FloatParameter fogEndDistance = new FloatParameter(600f);

        public enum FogColorSource
        {
            UniformColor,
            GradientTexture,
            SkyboxColor
        }

        [Serializable]
        public sealed class FogColorSourceParameter : VolumeParameter<FogColorSource> { }

        [Space]

        [Tooltip("Color: use a uniform color for the fog\n\nGradient: sample a gradient texture to control the fog color over distance, the alpha channel controls the density\n\nSkybox: Sample the skybox's color for the fog, only works well with low detail skies")]
        public FogColorSourceParameter colorSource = new FogColorSourceParameter { value = FogColorSource.UniformColor };

        [ Tooltip("Set the mipmap level for the skybox texture"), Range(0f, 8f)]
        public FloatParameter skyboxMipLevel = new FloatParameter(0f);

#if UNITY_2018_1_OR_NEWER
        [ColorUsage(true, true)]
#else
        [ColorUsage(true, true, 1f, 10f, 0.125f, 3f)]
#endif
        public ColorParameter fogColor = new ColorParameter(new Color(0.76f, 0.94f, 1f, 1f), true, false, true, false);

        //[DisplayName("Texture")]
        public TextureParameter fogColorGradient = new TextureParameter(null);
        [Tooltip("Automatic mode uses the current camera's far clipping plane to set the max distance\n\nOtherwise, a fixed value may be used instead")]
        public FloatParameter gradientDistance = new FloatParameter(1000f);
        public BoolParameter gradientUseFarClipPlane = new BoolParameter(true);

        [Header("Distance")]
        //[DisplayName("Enable")]
        public BoolParameter distanceFog = new BoolParameter(true);
        [Range(0.001f, 1.0f)]
        //[DisplayName("Density")]
        public ClampedFloatParameter distanceDensity = new ClampedFloatParameter(1f, 0.001f, 1f);
        [Tooltip("Distance based on radial distance from viewer, rather than parrallel")]
        public BoolParameter useRadialDistance = new BoolParameter(true);

        [Header("Skybox")]
        [Range(0f, 1f), Tooltip("Determines how much the fog influences the skybox")]
        public ClampedFloatParameter skyboxInfluence = new ClampedFloatParameter(0f,0f,1f);

        [Header("Directional Light")]
        [Tooltip("Translates the a Directional Light's direction and color into the fog. Creates a faux-atmospheric scattering effect.")]
        public BoolParameter enableDirectionalLight = new BoolParameter(false);
        [Tooltip("Use the intensity of the Directional Light that's set as the caster")]
        public BoolParameter useLightDirection = new BoolParameter(true);
        [Tooltip("Use the color of the Directional Light that's set as the caster")]
        public BoolParameter useLightColor = new BoolParameter(true);
        [Tooltip("Use the intensity of the Directional Light that's set as the caster")]
        public BoolParameter useLightIntensity = new BoolParameter(true);

        public static Vector3 LightDirection = Vector3.zero;
#if UNITY_2018_1_OR_NEWER
        [ColorUsage(true, true)]
#else
        [ColorUsage(true, true, 1f, 10f, 0.125f, 3f)]
#endif
        public ColorParameter lightColor = new ColorParameter(new Color(1f, 0.89f, 0.55f, 1f));
        //[Max(1f)]
        public Vector3Parameter lightDirection = new Vector3Parameter(new Vector3(0, 0.5f, -1f));
        public FloatParameter lightIntensity = new FloatParameter(1f);

        [Header("Height")]
        [Tooltip("Enable vertical height fog")]
        public BoolParameter heightFog = new BoolParameter(true);

        [Tooltip("Height relative to 0 world height position")]
        public FloatParameter height = new FloatParameter(10f);

        [Range(0.001f, 1.0f)]
        //[DisplayName("Density")]
        public FloatParameter heightDensity = new FloatParameter(0.75f);

        [Header("Height noise (2D)")]
        [Tooltip("Enables height fog density variation through the use of a texture")]
        public BoolParameter heightFogNoise = new BoolParameter(true);
        [Tooltip("The density is read from this texture's red color channel")]
        public TextureParameter heightNoiseTex = new TextureParameter(null);
        [Range(0f, 1f)]
        //[DisplayName("Size")]
        public ClampedFloatParameter heightNoiseSize = new ClampedFloatParameter (0.25f,0f,1f);
        [Range(0f, 1f)]
        //[DisplayName("Strength")]
        public ClampedFloatParameter heightNoiseStrength = new ClampedFloatParameter(1f,0f,1f);
        [Range(0f, 10f)]
        //[DisplayName("Speed")]
        public ClampedFloatParameter heightNoiseSpeed = new ClampedFloatParameter(2f,0f,10f);

        [Header("Light scattering")]
        [Tooltip("Execute a bloom pass to diffuse light in dense fog")]
        public BoolParameter lightScattering = new BoolParameter(false);

        [Space]

        [Min(0f)]
        public FloatParameter scatterIntensity = new FloatParameter(10f);

        [Min(0f), Tooltip("Filters out pixels under this level of brightness. Value is in gamma-space.")]
        public FloatParameter scatterThreshold = new FloatParameter(1f);

        [Range(1f, 10f)]
        public ClampedFloatParameter scatterDiffusion = new ClampedFloatParameter(10f,1f,10f);

        [Range(0f, 1f), Tooltip("Makes transitions between under/over-threshold gradual. 0 for a hard threshold, 1 for a soft threshold).")]
        public ClampedFloatParameter scatterSoftKnee = new ClampedFloatParameter(0.5f, 0f,1f);

        public bool IsActive() => active;

        public bool IsTileCompatible() => false;
        
        #if UNITY_EDITOR
        private void Reset()
        {
                //Auto assign default texture
                if (heightNoiseTex.value == null) heightNoiseTex.value = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(UnityEditor.AssetDatabase.GUIDToAssetPath("82a8d5f38e2ccf44c86362214b2d3e5e"));
        }
        #endif
    }
}