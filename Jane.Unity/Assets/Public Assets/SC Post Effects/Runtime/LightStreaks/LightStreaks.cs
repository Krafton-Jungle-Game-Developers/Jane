using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using MinAttribute = UnityEngine.Rendering.PostProcessing.MinAttribute;

namespace SCPE
{
    [PostProcess(typeof(LightStreaksRenderer), PostProcessEvent.BeforeStack, "SC Post Effects/Rendering/Light Streaks", true)]
    [Serializable]
    public sealed class LightStreaks : PostProcessEffectSettings
    {
        public enum Quality
        {
            Performance,
            Appearance
        }

        [Serializable]
        public sealed class BlurMethodParameter : ParameterOverride<Quality> { }

        [DisplayName("Quality"), Tooltip("Choose between Box and Gaussian blurring methods.\n\nBox blurring is more efficient but has a limited blur range")]
        public BlurMethodParameter quality = new BlurMethodParameter { value = Quality.Appearance };

        [Range(0f, 1f), DisplayName("Streaks Only"), Tooltip("Shows only the effect, to allow for finetuning")]
        public BoolParameter debug = new BoolParameter { value = false };

        [Header("Anamorphic Lensfares")]
        [Range(0f, 1f), Tooltip("Intensity")]
        public FloatParameter intensity = new FloatParameter { value = 0f };

        [Range(0.01f, 5f), Tooltip("Luminance threshold, pixels above this threshold (material's emission value) will contribute to the effect")]
        public FloatParameter luminanceThreshold = new FloatParameter { value = 1f };

        [Range(-1f, 1f), Tooltip("Negative values become horizontal whereas postive values are vertical")]
        public FloatParameter direction = new FloatParameter { value = -1f };

        [Header("Blur")]
        [Range(0f, 10f), DisplayName("Amount"), Tooltip("The amount of blurring that must be performed")]
        public FloatParameter blur = new FloatParameter { value = 1f };

        [Range(1, 8), Tooltip("The number of times the effect is blurred. More iterations provide a smoother effect but induce more drawcalls.")]
        public IntParameter iterations = new IntParameter { value = 2 };

        [Range(1, 4), Tooltip("Every step halfs the resolution of the blur effect. Lower resolution provides a smoother blur but may induce flickering")]
        public IntParameter downscaling = new IntParameter { value = 2 };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (blur == 0 || intensity == 0 || direction == 0) { return false; }
                return true;
            }

            return false;
        }
    }
}