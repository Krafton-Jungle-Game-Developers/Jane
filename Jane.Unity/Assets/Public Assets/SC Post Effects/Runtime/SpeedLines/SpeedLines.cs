using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Screen/Speed Lines")]
    public sealed class SpeedLines : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Assign any grayscale texture with a vertically repeating pattern and a falloff from left to right")]
        public TextureParameter noiseTex = new TextureParameter(null);

        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        [Range(0f, 1f), Tooltip("Determines the radial tiling of the noise texture")]
        public ClampedFloatParameter size = new ClampedFloatParameter(0.5f, 0f, 1f);

        [Range(0f, 1f)]
        public ClampedFloatParameter falloff = new ClampedFloatParameter(0.25f, 0, 1f);

        public bool IsActive() => intensity.value > 0f && noiseTex.value && this.active;

        public bool IsTileCompatible() => false;
        
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