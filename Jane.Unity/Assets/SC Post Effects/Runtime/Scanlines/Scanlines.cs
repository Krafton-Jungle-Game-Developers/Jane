using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;

using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [PostProcess(typeof(ScanlinesRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Retro/Scanlines", true)]
    [Serializable]
    public sealed class Scanlines : PostProcessEffectSettings
    {
        [Range(0f, 1f), Tooltip("Intensity")]
        public FloatParameter intensity = new FloatParameter { value = 0f };

        [Range(0f, 2048f), DisplayName("Lines")]
        public FloatParameter amount = new FloatParameter { value = 700 };

        [Range(0f, 1f), Tooltip("Animation speed")]
        public FloatParameter speed = new FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (intensity.value == 0) return false;
                return true;
            }

            return false;
        }
    }
}