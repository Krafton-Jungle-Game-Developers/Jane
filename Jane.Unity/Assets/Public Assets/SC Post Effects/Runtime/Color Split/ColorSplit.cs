using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Retro/Color Split")]
    public sealed class ColorSplit : VolumeComponent, IPostProcessComponent
    {
        public enum SplitMode
        {
            Single = 0,
            SingleBoxFiltered = 1,
            Double = 2,
            DoubleBoxFiltered = 3
        }

        [Serializable]
        public sealed class SplitModeParam : VolumeParameter<SplitMode> { }

        [Tooltip("Box filtered methods provide a subtle blur effect and are less efficient")]
        public SplitModeParam mode = new SplitModeParam { value = SplitMode.Single };

        [Range(0f, 1f), Tooltip("The amount by which the color channels offset")]
        public FloatParameter offset = new FloatParameter(0f);

        public bool IsActive() => offset.value > 0f && this.active;

        public bool IsTileCompatible() => false;
    }
}