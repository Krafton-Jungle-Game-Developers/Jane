using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Image/Colorize")]
    public sealed class Colorize : VolumeComponent, IPostProcessComponent
    {
        public enum BlendMode
        {
            Linear,
            Additive,
            Multiply,
            Screen
        }

        [Serializable]
        public sealed class BlendModeParameter : VolumeParameter<BlendMode> { }

        [Tooltip("Blends the gradient through various Photoshop-like blending modes")]
        public BlendModeParameter mode = new BlendModeParameter { value = BlendMode.Linear };

        [Range(0f, 1f), Tooltip("Fades the effect in or out")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        [Tooltip("Supply a gradient texture.\n\nLuminance values are colorized from left to right")]
        public TextureParameter colorRamp = new TextureParameter(null);

        public bool IsActive() => intensity.value > 0f && this.active;

        public bool IsTileCompatible() => false;
    }
}