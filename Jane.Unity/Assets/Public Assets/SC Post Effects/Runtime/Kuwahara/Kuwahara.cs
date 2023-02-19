using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Stylized/Kuwahara")]
    public sealed class Kuwahara : VolumeComponent, IPostProcessComponent
    {
        public enum KuwaharaMode
        {
            FullScreen = 0,
            DepthFade = 1
        }

        [Serializable]
        public sealed class KuwaharaModeParam : VolumeParameter<KuwaharaMode> { }

        [Tooltip("Choose to apply the effect to the entire screen, or fade in/out over a distance")]
        public KuwaharaModeParam mode = new KuwaharaModeParam { value = KuwaharaMode.FullScreen };

        //[Range(0, 8), DisplayName("Radius")]
        public ClampedIntParameter radius = new ClampedIntParameter(0, 0, 8);

        public FloatParameter startFadeDistance = new FloatParameter(100f);
        public FloatParameter endFadeDistance = new FloatParameter(500f);

        public bool IsActive() => radius.value > 0 && this.active;

        public bool IsTileCompatible() => false;
    }
}