using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class SpeedLinesRenderer : PostProcessEffectRenderer<SpeedLines>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.SpeedLines);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            float falloff = 2f + (settings.falloff.value - 0.0f) * (16.0f - 2f) / (1.0f - 0.0f);
            sheet.properties.SetVector(ShaderParameters.Params, new Vector4(settings.intensity.value, falloff, settings.size.value * 2, 0));
            if (settings.noiseTex.value) sheet.properties.SetTexture("_NoiseTex", settings.noiseTex.value);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}