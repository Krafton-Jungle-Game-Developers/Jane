using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Screen/Gradient")]
    public sealed class Gradient : VolumeComponent, IPostProcessComponent
    {

        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f,0f,1f);

        public enum Mode
        {
            ColorFields,
            Texture
        }

        [Serializable]
        public sealed class GradientModeParameter : VolumeParameter<Mode> { }

        //[DisplayName("Direction"), Tooltip("")]
        public GradientModeParameter input = new GradientModeParameter();

        [Tooltip("The color's alpha channel controls its opacity")]
        public ColorParameter color1 = new ColorParameter(new Color(0f, 0.8f, 0.56f, 0.5f));
        [Tooltip("The color's alpha channel controls its opacity")]
        public ColorParameter color2 = new ColorParameter(new Color(0.81f, 0.37f, 1f, 0.5f));

        [Range(0f, 1f), Tooltip("Size")]
        public ClampedFloatParameter rotation = new ClampedFloatParameter(0f, 0f, 1f);

        //[DisplayName("Gradient"), Tooltip("")]
        public TextureParameter gradientTex = new TextureParameter(null);

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
        public BlendModeParameter mode = new BlendModeParameter();

        public bool IsActive() => (intensity.value > 0f || (input.value == Mode.Texture && gradientTex.value == null)) && this.active;

        public bool IsTileCompatible() => false;
    }
}