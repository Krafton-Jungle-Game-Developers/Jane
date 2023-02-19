using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Screen/Ripples")]
    public sealed class Ripples : VolumeComponent, IPostProcessComponent
    {
        public enum RipplesMode
        {
            Radial = 0,
            OmniDirectional = 1,
        }

        [Serializable]
        public sealed class RipplesModeParam : VolumeParameter<RipplesMode> { }

        //[DisplayName("Method")]
        public RipplesModeParam mode = new RipplesModeParam { value = RipplesMode.Radial };

        //[Range(0f, 10), DisplayName("Intensity")]
        public ClampedFloatParameter strength = new ClampedFloatParameter (0f, 0f, 10f);

        [Range(1f, 10), Tooltip("The frequency of the waves")]
        public ClampedFloatParameter distance = new ClampedFloatParameter(5f, 1f, 10f);

        [Range(0f, 10), Tooltip("Speed")]
        public ClampedFloatParameter speed = new ClampedFloatParameter(3f, 1f, 10f);

        [Range(0f, 5), Tooltip("Width")]
        public ClampedFloatParameter width = new ClampedFloatParameter(1.5f, 0f, 5f);

        [Range(0f, 5), Tooltip("Height")]
        public ClampedFloatParameter height = new ClampedFloatParameter (1f, 0f, 5f);

        public bool IsActive() => strength.value > 0f && this.active;

        public bool IsTileCompatible() => false;
    }
}