using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;

namespace SCPE
{
    [PostProcess(typeof(OverlayRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Screen/Overlay", true)]
    [Serializable]
    public sealed class Overlay : PostProcessEffectSettings
    {
        [Tooltip("The texture's alpha channel controls its opacity")]
        public TextureParameter overlayTex = new TextureParameter { value = null };

        [Range(0f, 1f)]
        public FloatParameter intensity = new FloatParameter { value = 0f };

        [Range(0f, 1f), Tooltip("The screen's luminance values control the opacity of the image")]
        public FloatParameter luminanceThreshold = new FloatParameter { value = 0f };
        
        [Tooltip("Maintains the image aspect ratio, regardless of the screen width")]
        public BoolParameter autoAspect = new BoolParameter { value = false };

        public enum BlendMode
        {
            Linear,
            Additive,
            Multiply,
            Screen
        }

        [Serializable]
        public sealed class BlendModeParameter : ParameterOverride<BlendMode> { }

        [Tooltip("Blends the gradient through various Photoshop-like blending modes")]
        public BlendModeParameter blendMode = new BlendModeParameter { value = BlendMode.Linear };


        [Range(0f, 1f)]
        public FloatParameter tiling = new FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (overlayTex.value == null || intensity == 0) { return false; }
                return true;
            }

            return false;
        }
    }
}