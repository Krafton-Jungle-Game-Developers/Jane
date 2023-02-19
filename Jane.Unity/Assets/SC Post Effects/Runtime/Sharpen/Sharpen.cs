using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Image/Sharpen")]
    public sealed class Sharpen : VolumeComponent, IPostProcessComponent
    {
        public enum Method
        {
            [InspectorName("Luminance Enhancement (4 samples)")]
            LuminanceEnhancement = 0,
            [InspectorName("Contrast Adaptive (9 samples)")]
            ContrastAdaptive = 1,
        }

        [Serializable]
        public sealed class MethodParam : VolumeParameter<Method> { }

        public MethodParam mode = new MethodParam { value = Method.LuminanceEnhancement };
        
        public ClampedFloatParameter amount = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter radius = new ClampedFloatParameter(1f, 0.1f, 2f);
        public ClampedFloatParameter contrast = new ClampedFloatParameter(1f, 0f, 1f);

        public bool IsActive() { return active && amount.value > 0f; }

        public bool IsTileCompatible() => false;
    }
}