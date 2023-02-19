using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcess(typeof(MosaicRenderer), PostProcessEvent.AfterStack, "SC Post Effects/Stylized/Mosaic", true)]
    [Serializable]
    public sealed class Mosaic : PostProcessEffectSettings
    {
        public enum MosaicMode
        {
            Triangles = 0,
            Hexagons = 1,
            Circles
        }

        [Serializable]
        public sealed class MosaicModeParam : ParameterOverride<MosaicMode> { }

        [DisplayName("Method"), Tooltip("")]
        public MosaicModeParam mode = new MosaicModeParam { value = MosaicMode.Hexagons };

        [Range(0f, 1f), Tooltip("Size")]
        public UnityEngine.Rendering.PostProcessing.FloatParameter size = new UnityEngine.Rendering.PostProcessing.FloatParameter { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            if (enabled.value)
            {
                if (size == 0) { return false; }
                return true;
            }

            return false;
        }
    }
}