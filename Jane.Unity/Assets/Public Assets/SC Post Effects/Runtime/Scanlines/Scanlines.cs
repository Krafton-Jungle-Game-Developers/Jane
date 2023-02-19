using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Retro/Scanlines")]
    public sealed class Scanlines : VolumeComponent, IPostProcessComponent
    {
        [Range(0f, 1f), Tooltip("Intensity")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        [Range(0f, 1f), Tooltip("Lines")]
        public ClampedFloatParameter amount = new ClampedFloatParameter(700f, 0f, 2048f);

        [Range(0f, 1f), Tooltip("Animation speed")]
        public ClampedFloatParameter speed = new ClampedFloatParameter(0f, 0f, 1f);

        public bool IsActive() => intensity.value > 0f && this.active;

        public bool IsTileCompatible() => false;

    }
}