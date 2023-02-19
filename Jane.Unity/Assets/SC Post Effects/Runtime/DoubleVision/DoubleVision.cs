using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Blurring/Double Vision")]
    public sealed class DoubleVision : VolumeComponent, IPostProcessComponent
    {
        public enum Mode
        {
            FullScreen = 0,
            Edges = 1,
        }

        [Serializable]
        public sealed class DoubleVisionMode : VolumeParameter<Mode> { }

        [Tooltip("Choose to apply the effect over the entire screen or just the edges")]
        public DoubleVisionMode mode = new DoubleVisionMode { value = Mode.FullScreen };

        [Range(0f, 1f), Tooltip("Intensity")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f,0f,1f);

        public bool IsActive() => intensity.value > 0f && this.active;

        public bool IsTileCompatible() => false;
    }
}