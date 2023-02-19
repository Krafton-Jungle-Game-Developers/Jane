using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Screen/Transition")]
    public sealed class Transition : VolumeComponent, IPostProcessComponent
    {
        public TextureParameter gradientTex = new TextureParameter(null);

        public ClampedFloatParameter progress = new ClampedFloatParameter(0f, 0f, 1f);

        public bool IsActive() => progress.value > 0f && this.active;

        public bool IsTileCompatible() => false;
    }
}