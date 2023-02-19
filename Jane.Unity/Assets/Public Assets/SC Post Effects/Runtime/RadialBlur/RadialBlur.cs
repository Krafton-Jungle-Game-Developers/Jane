using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Blurring/Radial Blur")]
    public sealed class RadialBlur : VolumeComponent, IPostProcessComponent
    {
        [Range(0f, 1f)]
        public ClampedFloatParameter amount = new ClampedFloatParameter(0f, 0f, 1f);

        [Space]
        
        [Tooltip("Sets the blur center point (screen center is [0.5, 0.5]).")]
        public Vector2Parameter center = new Vector2Parameter( new Vector2(0.5f, 0.5f) );
        
        [Range(-180f, 180f)]
        public ClampedFloatParameter angle = new ClampedFloatParameter(0f, -180f, 180f);
        
        [Space]
        
        [Range(3, 12)]
        public ClampedIntParameter iterations = new ClampedIntParameter(6, 3,12);

        public bool IsActive() => amount.value > 0f && this.active;

        public bool IsTileCompatible() => false;
    }
}