using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Screen/Black Bars")]
    public sealed class BlackBars : VolumeComponent, IPostProcessComponent
    {
        public enum Direction
        {
            Horizontal = 0,
            Vertical = 1,
        }

        [Serializable]
        public sealed class DirectionParam : VolumeParameter<Direction> { }

        //[DisplayName("Direction"), Tooltip("")]
        public DirectionParam mode = new DirectionParam { value = Direction.Horizontal };

        [Range(0f, 1f), Tooltip("Size")]
        public ClampedFloatParameter size = new ClampedFloatParameter(0f, 0f, 1f);

        [Range(0f, 1f), Tooltip("Max Size")]
        public ClampedFloatParameter maxSize = new ClampedFloatParameter(0.33f, 0f, 1f);

        public bool IsActive() => size.value > 0f && this.active;

        public bool IsTileCompatible() => false;
    }
}