using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;

namespace SCPE
{
    [PostProcess(typeof(KaleidoscopeRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Misc/Kaleidoscope", false)]
    [Serializable]
    public sealed class Kaleidoscope : PostProcessEffectSettings
    {
        [FormerlySerializedAs("splits")]
        [Range(0, 10), Tooltip("The number of times the screen is split up")]
        public IntParameter radialSplits = new IntParameter { value = 0 };
        
        [Range(1, 6)]
        public IntParameter horizontalSplits = new IntParameter { value = 1 };
        
        [Range(1, 6)]
        public IntParameter verticalSplits = new IntParameter { value = 1 };

        [Tooltip("Sets the pivot point (screen center is [0.5, 0.5]).")]
        public Vector2Parameter center = new Vector2Parameter { value = new Vector2(0.5f, 0.5f) };
        
        [Space]
        
        public BoolParameter maintainAspectRatio = new BoolParameter { value = true };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value && radialSplits > 0;
        }
    }
}