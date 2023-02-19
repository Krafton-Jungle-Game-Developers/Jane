using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;

namespace SCPE
{
    [PostProcess(typeof(PosterizeRenderer), PostProcessEvent.BeforeStack, "SC Post Effects/Retro/Posterize", true)]
    [Serializable]
    public sealed class Posterize : PostProcessEffectSettings
    {
        public BoolParameter hsvMode = new BoolParameter { value = false };

        [Range(0, 256)]
        public IntParameter levels = new IntParameter { value = 256 };

        [Header("Levels")]
        [Range(0, 256)]
        public IntParameter hue = new IntParameter { value = 256 };
        [Range(0, 256)]
        public IntParameter saturation = new IntParameter { value = 256 };
        [Range(0, 256)]
        public IntParameter value = new IntParameter { value = 256 };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (!hsvMode && levels == 256) { return false; }
                return true;
            }

            return false;
        }
    }
}