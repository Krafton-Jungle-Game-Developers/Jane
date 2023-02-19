using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using Vector2Parameter = UnityEngine.Rendering.PostProcessing.Vector2Parameter;
using MinAttribute = UnityEngine.Rendering.PostProcessing.MinAttribute;

namespace SCPE
{
    [PostProcess(typeof(CloudShadowsRenderer), PostProcessEvent.BeforeStack, "SC Post Effects/Environment/Cloud Shadows")]
    [Serializable]
    public sealed class CloudShadows : PostProcessEffectSettings
    {
        [DisplayName("Texture (R)"), Tooltip("The red channel of this texture is used to sample the clouds")]
        public TextureParameter texture = new TextureParameter { value = null };
        [Range(0f, 1f)]
        [DisplayName("Density")]
        public FloatParameter density = new FloatParameter { value = 0f };

        [Space]

        [Range(0f, 1f)]
        [DisplayName("Size")]
        public FloatParameter size = new FloatParameter { value = 0.5f };
        [Range(0f, 1f)]
        [DisplayName("Speed")]
        public FloatParameter speed = new FloatParameter { value = 0.5f };

        [DisplayName("Direction"), Tooltip("Set the X and Z world-space direction the clouds should move in")]
        public Vector2Parameter direction = new Vector2Parameter { value = new Vector2(0f, 1f) };
        public BoolParameter projectFromSun = new BoolParameter { value = false};
        
        public FloatParameter startFadeDistance = new FloatParameter { value = 0f };
        public FloatParameter endFadeDistance = new FloatParameter { value = 200f };
        
        #if UNITY_EDITOR
        private void Reset()
        {
            //Auto assign default texture
            if (texture.value == null)texture.value = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(UnityEditor.AssetDatabase.GUIDToAssetPath("32ef32965c344ed4c9af86f0dc15661a"));
        }
        #endif

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (density == 0 || texture.value == null) return false;
                return true;
            }

            return false;
        }
    }
}