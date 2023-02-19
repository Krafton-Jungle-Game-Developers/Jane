using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Rendering/Light Streaks")]
    public sealed class LightStreaks : VolumeComponent, IPostProcessComponent
    {
        public enum Quality
        {
            Performance,
            Appearance
        }

        [Serializable]
        public sealed class BlurMethodParameter : VolumeParameter<Quality> { }

        [Tooltip("Choose between Box and Gaussian blurring methods.\n\nBox blurring is more efficient but has a limited blur range")]
        public BlurMethodParameter quality = new BlurMethodParameter { value = Quality.Appearance };

        [Range(0f, 1f), Tooltip("Shows only the effect, to allow for finetuning")]
        public BoolParameter debug = new BoolParameter(false);

        [Header("Anamorphic Lensfares")]
        [Range(0f, 1f), Tooltip("Intensity")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        [Range(0.01f, 5f), Tooltip("Luminance threshold, pixels above this threshold (material's emission value) will contribute to the effect")]
        public ClampedFloatParameter luminanceThreshold = new ClampedFloatParameter(1f, 0.01f, 5f);

        [Range(-1f, 1f), Tooltip("Negative values become horizontal whereas postive values are vertical")]
        public ClampedFloatParameter direction = new ClampedFloatParameter(-1f, -1f, 1f);

        [Header("Blur")]
        [Range(0f, 10f), Tooltip("The amount of blurring that must be performed")]
        public ClampedFloatParameter blur = new ClampedFloatParameter(1f, 0f, 10f);

        [Range(1, 8), Tooltip("The number of times the effect is blurred. More iterations provide a smoother effect but induce more drawcalls.")]
        public ClampedIntParameter iterations = new ClampedIntParameter (2,1,8);

        [Range(1, 4), Tooltip("Every step halfs the resolution of the blur effect. Lower resolution provides a smoother blur but may induce flickering")]
        public ClampedIntParameter downscaling = new ClampedIntParameter(2, 1, 4);

        public bool IsActive() => intensity.value > 0f && this.active;

        public bool IsTileCompatible() => false;
    }
}