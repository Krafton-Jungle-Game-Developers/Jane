using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Screen/Tube Distortion")]
    public sealed class TubeDistortion : VolumeComponent, IPostProcessComponent
    {
        public enum DistortionMode
        {
            Buldged = 0,
            Pinched = 1,
            Beveled = 2
        }

        [Serializable]
        public sealed class DistortionModeParam : VolumeParameter<DistortionMode> { }

        public DistortionModeParam mode = new DistortionModeParam { value = DistortionMode.Buldged };

        [Range(0f, 1f)]
        public ClampedFloatParameter amount = new ClampedFloatParameter(0f, 0f, 1f);

        public bool IsActive() => amount.value > 0f && this.active;

        public bool IsTileCompatible() => false;
    }
}