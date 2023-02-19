using System;
using System.Collections;
using System.Collections.Generic;
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
    [PostProcess(typeof(TiltShiftRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Blurring/Tilt Shift")]
    [Serializable]
    public class TiltShift : PostProcessEffectSettings
    {
        public enum TiltShiftMethod
        {
            Horizontal,
            Radial,
        }

        [Serializable]
        public sealed class TiltShifMethodParameter : ParameterOverride<TiltShiftMethod> { }

        [Range(0f, 1f), Tooltip("The amount of blurring that must be performed")]
        public FloatParameter amount = new FloatParameter { value = 0f };
        
        [DisplayName("Method")]
        public TiltShifMethodParameter mode = new TiltShifMethodParameter { value = TiltShiftMethod.Horizontal };

        public enum Quality
        {
            Performance,
            Appearance
        }

        [Serializable]
        public sealed class TiltShiftQualityParameter : ParameterOverride<Quality> { }

        [DisplayName("Quality"), Tooltip("Choose to use more texture samples, for a smoother blur when using a high blur amout")]
        public TiltShiftQualityParameter quality = new TiltShiftQualityParameter { value = Quality.Appearance };

        [Range(0f, 1f)]
        public FloatParameter areaSize = new FloatParameter { value = 1f };
        [Range(0f, 1f)]
        public FloatParameter areaFalloff = new FloatParameter { value = 1f };
        [Range(-1f, 1f)]
        public FloatParameter offset = new FloatParameter { value = 0f };
        [Range(0f, 360f)]
        public FloatParameter angle = new FloatParameter { value = 0f };

        public static bool debug;

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if ((areaSize == 0f && areaFalloff == 0f) || amount == 0f) { return false; }
                return true;
            }

            return false;
        }
    }
}