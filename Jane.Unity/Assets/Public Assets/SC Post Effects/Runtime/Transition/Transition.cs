using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;

namespace SCPE
{
    [PostProcess(typeof(TransitionRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Screen/Transition", true)]
    [Serializable]
    public sealed class Transition : PostProcessEffectSettings
    {
        public TextureParameter gradientTex = new TextureParameter { value = null, defaultState = TextureParameterDefault.None };

        [Range(0f, 1f), Tooltip("Progress")]
        public FloatParameter progress = new FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (progress == 0) { return false; }
                return true;
            }

            return false;
        }
    }
}