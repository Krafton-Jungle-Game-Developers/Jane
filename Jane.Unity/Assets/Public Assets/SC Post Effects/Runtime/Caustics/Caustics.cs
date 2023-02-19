using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Environment/Caustics")]
    public sealed class Caustics : VolumeComponent, IPostProcessComponent
    {
        public TextureParameter causticsTexture = new TextureParameter(null);
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 5f);
        public ClampedFloatParameter luminanceThreshold = new ClampedFloatParameter(0f, 0f, 2f);
        public BoolParameter projectFromSun = new BoolParameter(false);
        
        public FloatParameter minHeight = new FloatParameter(-5f);
        public ClampedFloatParameter minHeightFalloff = new ClampedFloatParameter(10f, 0.01f, 64f);
        
        public FloatParameter maxHeight = new FloatParameter(0f);
        public ClampedFloatParameter maxHeightFalloff = new ClampedFloatParameter(10f, 0.01f, 64f);
        
        public ClampedFloatParameter size = new ClampedFloatParameter(0.5f, 0.1f, 1f);
        public ClampedFloatParameter speed = new ClampedFloatParameter(0.2f, 0f, 1f);
        
        public BoolParameter distanceFade = new BoolParameter(false);
        public FloatParameter startFadeDistance = new FloatParameter(0f);
        public FloatParameter endFadeDistance = new FloatParameter(200f);
        
        public bool IsActive() => intensity.value > 0f && this.active;

        public bool IsTileCompatible() => false;
        
        #if UNITY_EDITOR
        private void Reset()
        {
            //Auto assign default texture
            if (causticsTexture.value == null)causticsTexture.value = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(UnityEditor.AssetDatabase.GUIDToAssetPath("f76f6be48fafde34b818e658b93e7850"));
        }
        #endif
    }
}