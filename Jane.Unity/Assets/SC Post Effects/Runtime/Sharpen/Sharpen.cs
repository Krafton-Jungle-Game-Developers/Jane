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
    [PostProcess(typeof(SharpenRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Image/Sharpen", true)]
    [Serializable]
    public sealed class Sharpen : PostProcessEffectSettings
    {
        public enum Method
        {
            [InspectorName("Luminance Enhancement (4 samples)")]
            LuminanceEnhancement = 0,
            [InspectorName("Contrast Adaptive (9 samples)")]
            ContrastAdaptive = 1,
        }

        [Serializable]
        public sealed class MethodParam : ParameterOverride<Method> { }

        public MethodParam mode = new MethodParam { value = Method.LuminanceEnhancement };
        
        [Range(0f, 1f)]
        public FloatParameter amount = new FloatParameter { value = 0f };
        [Range(0.1f,2f)]
        public FloatParameter radius = new FloatParameter { value = 1f };
        [Range(0f,1f)]
        public FloatParameter contrast = new FloatParameter { value = 1f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (amount == 0) { return false; }
                return true;
            }

            return false;
        }
    }
}