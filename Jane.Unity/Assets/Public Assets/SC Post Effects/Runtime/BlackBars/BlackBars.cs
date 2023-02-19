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
    [PostProcess(typeof(BlackBarsRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Screen/Black Bars", true)]
    [Serializable]
    public sealed class BlackBars : PostProcessEffectSettings
    {
        public enum Direction
        {
            Horizontal = 0,
            Vertical = 1,
        }

        [Serializable]
        public sealed class DirectionParam : ParameterOverride<Direction> { }

        [DisplayName("Direction"), Tooltip("")]
        public DirectionParam mode = new DirectionParam { value = Direction.Horizontal };

        [Range(0f, 1f), Tooltip("Size")]
        public FloatParameter size = new FloatParameter { value = 0f };

        [Range(0f, 1f), Tooltip("Max Size")]
        public FloatParameter maxSize = new FloatParameter { value = 0.33f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (size == 0 || maxSize == 0) { return false; }
                return true;
            }

            return false;
        }
    }
}