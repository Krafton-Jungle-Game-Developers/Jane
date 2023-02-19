using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using MinAttribute = UnityEngine.Rendering.PostProcessing.MinAttribute;

namespace SCPE
{
    [PostProcess(typeof(BlurRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Blurring/Blur")]
    [Serializable]
    public sealed class Blur : PostProcessEffectSettings
    {
        public enum BlurMethod
        {
            Gaussian,
            Box
        }

        [Serializable]
        public sealed class BlurMethodParameter : ParameterOverride<BlurMethod> { }

        [DisplayName("Method"), Tooltip("Box blurring uses fewer texture samples but has a limited blur range")]
        public BlurMethodParameter mode = new BlurMethodParameter { value = BlurMethod.Gaussian };

        [Tooltip("When enabled, the amount of blur passes is doubled")]
        public BoolParameter highQuality = new BoolParameter { value = false };
        public BoolParameter distanceFade = new BoolParameter { value = false };
        public FloatParameter startFadeDistance = new FloatParameter { value = 0f };
        public FloatParameter endFadeDistance = new FloatParameter { value = 500f };

        [Space]

        [Range(0f, 5f), Tooltip("The amount of blurring that must be performed")]
        public FloatParameter amount = new FloatParameter { value = 0f };

        [Range(1, 12), Tooltip("The number of times the effect is blurred. More iterations provide a smoother effect but induce more drawcalls.")]
        public IntParameter iterations = new IntParameter { value = 6 };

        [Range(1, 4), Tooltip("Every step halfs the resolution of the blur effect. Lower resolution provides a smoother blur but may induce flickering.")]
        public IntParameter downscaling = new IntParameter { value = 2 };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value && amount > 0) return true;

            return false;
        }
    }
}