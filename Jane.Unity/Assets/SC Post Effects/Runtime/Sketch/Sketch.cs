using System;
using System.Collections;
using System.Collections.Generic;
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
    [PostProcess(typeof(SketchRenderer), PostProcessEvent.BeforeStack, "SC Post Effects/Stylized/Sketch", true)]
    [Serializable]
    public sealed class Sketch : PostProcessEffectSettings
    {
        [Tooltip("The Red channel is used for darker shades, whereas the Green channel is for lighter.")]
        public TextureParameter strokeTex = new TextureParameter { value = null };

        public enum SketchProjectionMode
        {
            WorldSpace,
            ScreenSpace
        }
        [Serializable]
        public sealed class SketchProjectioParameter : ParameterOverride<SketchProjectionMode> { }

        [Space]
        [Tooltip("Choose the type of UV space being used")]
        public SketchProjectioParameter projectionMode = new SketchProjectioParameter { value = SketchProjectionMode.WorldSpace };

        public enum SketchMode
        {
            EffectOnly,
            Multiply,
            Add
        }

        [Serializable]
        public sealed class SketchModeParameter : ParameterOverride<SketchMode> { }

        [Tooltip("Choose one of the different modes")]
        public SketchModeParameter blendMode = new SketchModeParameter { value = SketchMode.EffectOnly };

        [Space]

        [Range(0f, 1f)]
        public FloatParameter intensity = new FloatParameter { value = 0f };

        public Vector2Parameter brightness = new Vector2Parameter { value = new Vector2(0f, 1f) };

        [Range(1f, 32f)]
        public FloatParameter tiling = new FloatParameter { value = 8f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (intensity == 0 || strokeTex.value == null) return false;
                return true;
            }

            return false;
        }
        
        #if UNITY_EDITOR
        private void Reset()
        {
            //Auto assign default texture
            if (strokeTex.value == null) strokeTex.value = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(UnityEditor.AssetDatabase.GUIDToAssetPath("8bd158bf28fbf3d4cb9151f3c3ae5516"));
        }
        #endif
    }
}