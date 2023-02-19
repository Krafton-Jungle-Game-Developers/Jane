using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;

namespace SCPE
{
    [PostProcess(typeof(RefractionRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Screen/Refraction", true)]
    [Serializable]
    public sealed class Refraction : PostProcessEffectSettings
    {
        [Tooltip("Takes a DUDV map (normal map without a blue channel) to perturb the image")]
        public TextureParameter refractionTex = new TextureParameter { value = null };

        [DisplayName("Using normal map"), Tooltip("In the absense of a DUDV map, the supplied normal map can be converted in the shader")]
        public BoolParameter convertNormalMap = new BoolParameter { value = false };

        [Range(0f, 1f), Tooltip("Amount")]
        public FloatParameter amount = new FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (amount == 0 || refractionTex.value == null) { return false; }
                return true;
            }

            return false;
        }
    }
}