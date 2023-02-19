using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using MinAttribute = UnityEngine.Rendering.PostProcessing.MinAttribute;

namespace SCPE
{
    [PostProcess(typeof(GradientRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Screen/Gradient", true)]
    [Serializable]
    public sealed class Gradient : PostProcessEffectSettings
    {
        [Range(0f, 1f)]
        [DisplayName("Opacity")]
        public FloatParameter intensity = new FloatParameter { value = 0f };

        public enum Mode
        {
            ColorFields,
            Texture
        }

        [Serializable]
        public sealed class GradientModeParameter : ParameterOverride<Mode> { }
        [Space]
        [Tooltip("Set the color either through 2 color fields, or a gradient texture")]
        public GradientModeParameter input = new GradientModeParameter { value = Mode.ColorFields };

        [Tooltip("The color's alpha channel controls its opacity")]
        public ColorParameter color1 = new ColorParameter { value = new Color(0f, 0.8f, 0.56f, 0.5f) };
        [Tooltip("The color's alpha channel controls its opacity")]
        public ColorParameter color2 = new ColorParameter { value = new Color(0.81f, 0.37f, 1f, 0.5f) };

        [Range(0f,1f)]
        [Space]
        [Tooltip("Controls the rotation of the gradient")]
        public FloatParameter rotation = new FloatParameter { value = 0f };

        [DisplayName("Gradient"), Tooltip("")]
        public TextureParameter gradientTex = new TextureParameter { value = null };

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
        public BlendModeParameter mode = new BlendModeParameter { value = BlendMode.Linear };

        private const int RESOLUTION = 64;

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (intensity == 0 || (input.value == Mode.Texture && gradientTex.value == null)) return false;
                return true;
            }

            return false;
        }

        //Converting a gradient to a texture currently breaks volume blending

        /*
        public Texture2D m_gradientTex;

        public Texture2D GenerateGradient(Gradient gradient)
        {
            if (this.gradient.overrideState == false) return null;
            Debug.Log("Converting gradient to texture");

            //Create texture first time
            if (!m_gradientTex)
            {
                m_gradientTex = new Texture2D(RESOLUTION, 1, TextureFormat.ARGB32, false)
                {
                    //Smooth interpolation
                    filterMode = FilterMode.Bilinear
                };
            }

            Color gradientPixel;

            for (int x = 0; x < RESOLUTION; x++)
            {
                gradientPixel = gradient.Evaluate(x / (float)RESOLUTION);
                m_gradientTex.SetPixel(x, 1, gradientPixel);
            }

            m_gradientTex.Apply();

            return m_gradientTex;
        }
        */
    }
}