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
    [PostProcess(typeof(DoubleVisionRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Blurring/Double Vision", true)]
    [Serializable]
    public sealed class DoubleVision : PostProcessEffectSettings
    {
        public enum Mode
        {
            FullScreen = 0,
            Edges = 1,
        }

        [Serializable]
        public sealed class DoubleVisionMode : ParameterOverride<Mode> { }

        [DisplayName("Method"), Tooltip("Choose to apply the effect over the entire screen or just the edges")]
        public DoubleVisionMode mode = new DoubleVisionMode { value = Mode.FullScreen };

        [Range(0f, 1f), Tooltip("Intensity")]
        public FloatParameter intensity = new FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (intensity == 0) { return false; }
                return true;
            }

            return false;
        }
    }
}