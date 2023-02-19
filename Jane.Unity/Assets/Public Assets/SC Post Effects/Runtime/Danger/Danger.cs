using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Screen/Danger")]
    public sealed class Danger : VolumeComponent, IPostProcessComponent
    {
        public TextureParameter overlayTex = new TextureParameter(null);
        public ColorParameter color = new ColorParameter(new Color(0.66f, 0f, 0f));

        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter size = new ClampedFloatParameter(0f, 0f, 1f);

        public bool IsActive() => size.value > 0f || intensity.value > 0 && this.active;

        public bool IsTileCompatible() => false;
    }
}