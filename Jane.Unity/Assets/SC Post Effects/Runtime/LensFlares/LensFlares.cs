using System;
using UnityEngine;
using UnityEngine.Rendering;

using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using MinAttribute = UnityEngine.Rendering.PostProcessing.MinAttribute;

namespace SCPE
{
    [PostProcess(typeof(LensFlaresRenderer), PostProcessEvent.BeforeStack, "SC Post Effects/Rendering/Lens Flares", true)]
    [Serializable]
    public sealed class LensFlares : PostProcessEffectSettings
    {
        public BoolParameter debug = new BoolParameter { value = false };

        [Space]

        [Range(0f, 1f), DisplayName("Intensity")]
        public FloatParameter intensity = new FloatParameter { value = 0f };

        [Range(0.01f, 5f), DisplayName("Threshold"), Tooltip("Luminance threshold, pixels above this threshold will contribute to the effect")]
        public FloatParameter luminanceThreshold = new FloatParameter { value = 1f };

        [Header("Flares")]
        [Range(1, 4), DisplayName("Number")]
        public IntParameter iterations = new IntParameter { value = 2 };

        [Range(1, 2), DisplayName("Distance"), Tooltip("Offsets the Flares towards the edge of the screen")]
        public FloatParameter distance = new FloatParameter { value = 1.5f };

        [Range(1, 10), DisplayName("Falloff"), Tooltip("Fades out the Flares towards the edge of the screen")]
        public FloatParameter falloff = new FloatParameter { value = 10f };

        [Header("Halo"), Tooltip("Creates a halo at the center of the screen when looking directly at a bright spot")]
        [Range(0, 1), DisplayName("Size")]
        public FloatParameter haloSize = new FloatParameter { value = 0.2f };

        [Range(0f, 100f), DisplayName("Width")]
        public FloatParameter haloWidth = new FloatParameter { value = 70f };

        [Header("Colors and masking")]
        [DisplayName("Mask"), Tooltip("Use a texture to mask out the effect")]
        public TextureParameter maskTex = new TextureParameter { value = null };

        [Range(0f, 20f), DisplayName("Chromatic Abberation"), Tooltip("Refracts the color channels")]
        public FloatParameter chromaticAbberation = new FloatParameter { value = 10f };

        [DisplayName("Gradient"), Tooltip("Color the flares from the center of the screen to the outer edges")]
        public TextureParameter colorTex = new TextureParameter { value = null };

        [Header("Blur")]
        [Range(1, 8), DisplayName("Blur"), Tooltip("The amount of blurring that must be performed")]
        public FloatParameter blur = new FloatParameter { value = 2f };

        [Range(1, 12), DisplayName("Iterations"), Tooltip("The number of times the effect is blurred. More iterations provide a smoother effect but induce more drawcalls.")]
        public IntParameter passes = new IntParameter { value = 3 };
        
        #if UNITY_EDITOR
        private void Reset()
        {
            //Auto assign default texture
            if (colorTex.value == null) colorTex.value = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(UnityEditor.AssetDatabase.GUIDToAssetPath("af4273a5206164b4fa126b91bc0f5e78"));
            if (maskTex.value == null) maskTex.value = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(UnityEditor.AssetDatabase.GUIDToAssetPath("b0860ff85f5d4e040b7596fafe2d7c8f"));
            
        }
        #endif
    }
}