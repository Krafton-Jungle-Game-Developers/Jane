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
    [PostProcess(typeof(SpeedLinesRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Screen/Speed Lines", true)]
    [Serializable]
    public sealed class SpeedLines : PostProcessEffectSettings
    {
        [Tooltip("Assign any grayscale texture with a vertically repeating pattern and a falloff from left to right")]
        public TextureParameter noiseTex = new TextureParameter { value = null };

        [Range(0f, 1f)]
        public FloatParameter intensity = new FloatParameter { value = 0f };
        [Range(0f, 1f)]

        [Tooltip("Determines the radial tiling of the noise texture")]
        public FloatParameter size = new FloatParameter { value = 0.5f };

        [Range(0f, 1f)]
        public FloatParameter falloff = new FloatParameter { value = 0.25f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (intensity == 0 || noiseTex.value == null) { return false; }
                return true;
            }

            return false;
        }
        
        #if UNITY_EDITOR
        private void Reset()
        {
            if (noiseTex.value == null)
            {
                //Auto assign default texture
                noiseTex.value = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(UnityEditor.AssetDatabase.GUIDToAssetPath("fe06d9035cfd4064497518ed947395ca"));
            }
        }
        #endif
    }
}