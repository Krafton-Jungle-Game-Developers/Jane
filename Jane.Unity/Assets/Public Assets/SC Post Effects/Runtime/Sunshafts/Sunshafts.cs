using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System;
using UnityEngine;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Environment/Sun Shafts")]
    public sealed class Sunshafts : VolumeComponent, IPostProcessComponent
    {
        public enum BlendMode
        {
            Additive,
            Screen
        }

        public enum SunShaftsResolution
        {
            High = 1,
            Normal = 2,
            Low = 3,
        }
        
        [Tooltip("Use the color of the Directional Light that's set as the caster")]
        public BoolParameter useCasterColor = new BoolParameter(true, false);
        [Tooltip("Use the intensity of the Directional Light that's set as the caster")]
        public BoolParameter useCasterIntensity = new BoolParameter(false, false);

        [Serializable]
        public sealed class SunShaftsSourceParameter : VolumeParameter<BlendMode> { }
        [Tooltip("Additive mode adds the sunshaft color to the image, while Screen mode perserves color values")]
        public SunShaftsSourceParameter blendMode = new SunShaftsSourceParameter { value = BlendMode.Additive };

        [Serializable]
        public sealed class SunShaftsResolutionParameter : VolumeParameter<SunShaftsResolution> { }
        [InspectorName("Resolution"), Tooltip("Low, quater resolution\n\nNormal, half resolution\n\nHigh, full resolution\n\nLower resolutions may induce jittering")]
        public SunShaftsResolutionParameter resolution = new SunShaftsResolutionParameter { value = SunShaftsResolution.Normal };

        [Tooltip("Enabling this option doubles the amount of blurring performed. Resulting in smoother sunshafts at a higher performance cost.")]
        public BoolParameter highQuality = new BoolParameter(false, false);

        [Tooltip("Any color values over this threshold will contribute to the sunshafts effect")]
        [InspectorName("Sky color threshold")]
        public ColorParameter sunThreshold = new ColorParameter(Color.black, false);

        [InspectorName("Color")]
        public ColorParameter sunColor = new ColorParameter(Color.white, true, false, false, false);
        [InspectorName("Intensity")]
        public FloatParameter sunShaftIntensity = new FloatParameter(0f, false);
        [Range(0.1f, 1f)]
        [Tooltip("The degree to which the shafts’ brightness diminishes with distance from the caster")]
        public ClampedFloatParameter falloff = new ClampedFloatParameter(0.5f, 0.1f, 1f);

        [Tooltip("The length of the sunrays from the caster's position to the camera")]
        [Min(0f)]
        public FloatParameter length = new FloatParameter(5f, false);

        [Range(0f, 1f)]
        public FloatParameter noiseStrength = new FloatParameter(0f, false);

        public static Vector3 sunPosition = Vector3.zero;

        public bool IsActive() => active && sunShaftIntensity.value > 0f;

        public bool IsTileCompatible() => false;
    }
}