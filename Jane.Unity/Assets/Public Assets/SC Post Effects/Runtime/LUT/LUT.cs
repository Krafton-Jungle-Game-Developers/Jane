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
    [PostProcess(typeof(LUTRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Image/Color Grading LUT", true)]
    [Serializable]
    public sealed class LUT : PostProcessEffectSettings
    {
        public enum Mode
        {
            Single = 0,
            DistanceBased = 1,
        }

        [Serializable]
        public sealed class ModeParam : ParameterOverride<Mode> { }

        [DisplayName("Mode"), Tooltip("Distance-based mode blends two LUTs over a distance")]
        public ModeParam mode = new ModeParam { value = Mode.Single };
        
        public FloatParameter startFadeDistance = new FloatParameter { value = 0 };
        public FloatParameter endFadeDistance = new FloatParameter { value = 1000f };
        
        [Range(0f, 1f), Tooltip("Fades the effect in or out")]
        public FloatParameter intensity = new FloatParameter { value = 0f };

        [Tooltip("Supply a LUT strip texture.")]
        public TextureParameter lutNear = new TextureParameter { value = null };
        [DisplayName("Far")]
        public TextureParameter lutFar = new TextureParameter { value = null };

        [Range(0f, 1f)]
        public FloatParameter invert = new FloatParameter { value = 0f };
        
        [Range(0f, 1f), Tooltip("Increases the saturation of muted colors")]
        public FloatParameter vibrance = new FloatParameter { value = 0f };
        
        [ColorUsage(false, false), Tooltip("Controls the effect of vibrancy for each color channel (RGB)")]
        public ColorParameter vibranceRGBBalance = new ColorParameter { value = Color.white };
        
        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value;
        }
        
        //Used to temporarily disable the effect while capturing a screenshot for LUT extraction
        public static bool Bypass = false;
    }
}