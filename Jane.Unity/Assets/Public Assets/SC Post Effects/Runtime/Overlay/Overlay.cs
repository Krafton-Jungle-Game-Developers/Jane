using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Screen/Overlay")]
    public sealed class Overlay : VolumeComponent, IPostProcessComponent
    {
        public TextureParameter overlayTex = new TextureParameter(null);

        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        [Tooltip("The screen's luminance values control the opacity of the image")]
        public ClampedFloatParameter luminanceThreshold = new ClampedFloatParameter(0f, 0f, 1f);

        [Tooltip("Maintains the image aspect ratio, regardless of the screen width")]
        public BoolParameter autoAspect = new BoolParameter(false);


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
        public BlendModeParameter blendMode = new BlendModeParameter();

        [Range(0f, 1f)]
        public ClampedFloatParameter tiling = new ClampedFloatParameter(0f, 0f, 1f);

        public bool IsActive() => intensity.value > 0f && overlayTex.value != null && this.active;

        public bool IsTileCompatible() => false;
    }
}