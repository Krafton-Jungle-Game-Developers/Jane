using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    [Serializable, VolumeComponentMenu("SC Post Effects/Screen/Refraction")]
    public sealed class Refraction : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Takes a DUDV map (normal map without a blue channel) to perturb the image")]
        public TextureParameter refractionTex = new TextureParameter(null);

        [Tooltip("In the absense of a DUDV map, the supplied normal map can be converted in the shader")]
        public BoolParameter convertNormalMap = new BoolParameter(false);

        [Range(0f, 1f), Tooltip("Amount")]
        public ClampedFloatParameter amount = new ClampedFloatParameter(0f,0f,1f);

        public bool IsActive() => amount.value > 0f && refractionTex.value != null && this.active;

        public bool IsTileCompatible() => false;
    }
}