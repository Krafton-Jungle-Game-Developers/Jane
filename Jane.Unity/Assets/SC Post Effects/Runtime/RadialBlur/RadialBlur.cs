using System;
using UnityEngine;
using UnityEngine.Rendering;

using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using Vector2Parameter = UnityEngine.Rendering.PostProcessing.Vector2Parameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using MinAttribute = UnityEngine.Rendering.PostProcessing.MinAttribute;

namespace SCPE
{
    [PostProcess(typeof(RadialBlurRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Blurring/Radial Blur", true)]
    [Serializable]
    public sealed class RadialBlur : PostProcessEffectSettings
    {
        [Range(0f, 1f)]
        public FloatParameter amount = new FloatParameter { value = 0f };
        
        [Space]
        
        [Tooltip("Sets the blur center point (screen center is [0.5, 0.5]).")]
        public Vector2Parameter center = new Vector2Parameter { value = new Vector2(0.5f, 0.5f) };
        
        [Range(-180f, 180f)]
        public FloatParameter angle = new FloatParameter { value = 0f };
        
        [Space]

        [Range(3, 12)]
        public IntParameter iterations = new IntParameter { value = 6 };
        
        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (amount == 0) { return false; }
                return true;
            }

            return false;
        }
    }
}