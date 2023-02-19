using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Misc/Kaleidoscope")]
    public sealed class Kaleidoscope : VolumeComponent, IPostProcessComponent
    {
        [FormerlySerializedAs("splits")]
        [Range(0, 10), Tooltip("The number of times the screen is split up")]
        public ClampedIntParameter radialSplits = new ClampedIntParameter(0, 0, 10);

        [Range(1, 6)]
        public ClampedIntParameter horizontalSplits = new ClampedIntParameter(1, 1, 6);
        
        [Range(1, 6)]
        public ClampedIntParameter verticalSplits = new ClampedIntParameter(1, 1 ,6);
        
        [Tooltip("Sets the pivot point (screen center is [0.5, 0.5]).")]
        public Vector2Parameter center = new Vector2Parameter( new Vector2(0.5f, 0.5f) );
        
        [Space]
        
        public BoolParameter maintainAspectRatio = new BoolParameter (true);
        
        public bool IsActive() => radialSplits.value > 0 && this.active;

        public bool IsTileCompatible() => false;
    }
}