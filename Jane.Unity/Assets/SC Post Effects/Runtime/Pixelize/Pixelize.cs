using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcess(typeof(PixelizeRenderer), PostProcessEvent.BeforeStack, "SC Post Effects/Retro/Pixelize", true)]
    [Serializable]
    public sealed class Pixelize : PostProcessEffectSettings
    {
        [Range(0f, 1f), Tooltip("Amount")]
        public UnityEngine.Rendering.PostProcessing.FloatParameter amount = new UnityEngine.Rendering.PostProcessing.FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (amount == 0f) { return false; }
                return true;
            }

            return false;
        }
    }
}