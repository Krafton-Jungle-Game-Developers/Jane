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
    [PostProcess(typeof(RipplesRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Screen/Ripples", true)]
    [Serializable]
    public sealed class Ripples : PostProcessEffectSettings
    {
        public enum RipplesMode
        {
            Radial = 0,
            OmniDirectional = 1,
        }

        [Serializable]
        public sealed class RipplesModeParam : ParameterOverride<RipplesMode> { }

        [DisplayName("Method")]
        public RipplesModeParam mode = new RipplesModeParam { value = RipplesMode.Radial };

        [Range(0f, 10), DisplayName("Intensity")]
        public FloatParameter strength = new FloatParameter { value = 0f };

        [Range(1f, 10), Tooltip("The frequency of the waves")]
        public FloatParameter distance = new FloatParameter { value = 5f };

        [Range(0f, 10), Tooltip("Speed")]
        public FloatParameter speed = new FloatParameter { value = 3f };

        [Range(0f, 5), Tooltip("Width")]
        public FloatParameter width = new FloatParameter { value = 1.5f };

        [Range(0f, 5), Tooltip("Height")]
        public FloatParameter height = new FloatParameter { value = 1f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (strength == 0) { return false; }
                return true;
            }

            return false;
        }
    }
}