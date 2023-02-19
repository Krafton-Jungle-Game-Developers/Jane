using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Stylized/Mosaic")]
    public sealed class Mosaic : VolumeComponent, IPostProcessComponent
    {
        public enum MosaicMode
        {
            Triangles = 0,
            Hexagons = 1,
            Circles
        }

        [Serializable]
        public sealed class MosaicModeParam : VolumeParameter<MosaicMode> { }

        //[DisplayName("Direction"), Tooltip("")]
        public MosaicModeParam mode = new MosaicModeParam { value = MosaicMode.Hexagons };

        [Range(0f, 1f), Tooltip("Size")]
        public ClampedFloatParameter size = new ClampedFloatParameter(0f, 0f, 1f);

        public bool IsActive() => size.value > 0f && this.active;

        public bool IsTileCompatible() => false;
    }
}