using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class PosterizeRenderer : PostProcessEffectRenderer<Posterize>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Posterize);
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetVector(ShaderParameters.Params, new Vector4(settings.hue.value, settings.saturation.value, settings.value.value, settings.levels.value));

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, settings.hsvMode.value ? 1 : 0);
        }
    }
}