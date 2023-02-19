using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Stylized/Sketch")]
    public sealed class Sketch : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("The Red channel is used for darker shades, whereas the Green channel is for lighter.")]
        public TextureParameter strokeTex = new TextureParameter(null);

        public enum SketchProjectionMode
        {
            WorldSpace,
            ScreenSpace
        }
        [Serializable]
        public sealed class SketchProjectionParameter : VolumeParameter<SketchProjectionMode> { }

        [Space]
        [Tooltip("Choose the type of UV space being used")]
        public SketchProjectionParameter projectionMode = new SketchProjectionParameter { value = SketchProjectionMode.WorldSpace };

        public enum SketchMode
        {
            EffectOnly,
            Multiply,
            Add
        }
        [Serializable]
        public sealed class SketchModeParameter : VolumeParameter<SketchMode> { }

        [Tooltip("Choose one of the different modes")]
        public SketchModeParameter blendMode = new SketchModeParameter { value = SketchMode.EffectOnly };    

        [Range(0f, 1f), Tooltip("Fades the effect in or out")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        public Vector2Parameter brightness = new Vector2Parameter(new Vector2(0f, 1f));

        public ClampedFloatParameter tiling = new ClampedFloatParameter(8f, 1f, 32f);

        public bool IsActive() { return active && intensity.value > 0f; }

        public bool IsTileCompatible() => false;
        
        #if UNITY_EDITOR
        private void Reset()
        {
            //Auto assign default texture
            if (strokeTex.value == null) strokeTex.value = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(UnityEditor.AssetDatabase.GUIDToAssetPath("8bd158bf28fbf3d4cb9151f3c3ae5516"));
        }
        #endif
    }
}