using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;

namespace SCPE
{
    [PostProcess(typeof(DangerRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Screen/Danger", true)]
    [Serializable]
    public sealed class Danger : PostProcessEffectSettings
    {
        public TextureParameter overlayTex = new TextureParameter { value = null, defaultState = TextureParameterDefault.None };
        public ColorParameter color = new ColorParameter { value = new Color(0.66f, 0f, 0f) };

        [Range(0f, 1f), DisplayName("Opacity")]
        public FloatParameter intensity = new FloatParameter { value = 1f };

        [Range(0f, 1f), Tooltip("Size")]
        public FloatParameter size = new FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (size == 0 || intensity == 0) { return false; }
                return true;
            }

            return false;
        }
    }
}