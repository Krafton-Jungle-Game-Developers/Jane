using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Rendering/Lens Flares")]
    public sealed class LensFlares : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter debug = new BoolParameter(false);

        [Space]

        [Range(0f, 1f)]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        [Range(0.01f, 5f), Tooltip("Luminance threshold, pixels above this threshold will contribute to the effect")]
        public ClampedFloatParameter luminanceThreshold = new ClampedFloatParameter(1f, 0.01f, 5f);


        [Header("Flares")]
        [Range(1, 4)]
        public ClampedIntParameter iterations = new ClampedIntParameter(2,1,4 );

        [Range(1, 2), Tooltip("Offsets the Flares towards the edge of the screen")]
        public ClampedFloatParameter distance = new ClampedFloatParameter( 1.5f, 1f, 2f);

        [Range(1, 10), Tooltip("Fades out the Flares towards the edge of the screen")]
        public ClampedFloatParameter falloff = new ClampedFloatParameter(10f, 1f, 10f);

        [Header("Halo"), Tooltip("Creates a halo at the center of the screen when looking directly at a bright spot")]
        [Range(0, 1)]
        public ClampedFloatParameter haloSize = new ClampedFloatParameter(0.2f, 0f, 1f);

        [Range(0f, 100f)]
        public ClampedFloatParameter haloWidth = new ClampedFloatParameter(70f, 0f, 100f);

        [Header("Colors and masking")]
        [Tooltip("Use a texture to mask out the effect")]
        public TextureParameter maskTex = new TextureParameter(null);

        [Range(0f, 20f), Tooltip("Refracts the color channels")]
        public ClampedFloatParameter chromaticAbberation = new ClampedFloatParameter(10f, 0f, 20f);

        [Tooltip("Color the flares from the center of the screen to the outer edges")]
        public TextureParameter colorTex = new TextureParameter(null);

        [Header("Blur")]
        [Range(1, 8), Tooltip("The amount of blurring that must be performed")]
        public ClampedFloatParameter blur = new ClampedFloatParameter(2f, 1f, 8f);

        [Range(1, 12), Tooltip("The number of times the effect is blurred. More iterations provide a smoother effect but induce more drawcalls.")]
        public ClampedIntParameter passes = new ClampedIntParameter(3, 1, 12);
        public bool IsActive() => intensity.value > 0f && this.active;

        public bool IsTileCompatible() => false;

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