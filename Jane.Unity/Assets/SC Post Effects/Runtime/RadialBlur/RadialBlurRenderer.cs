using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class RadialBlurRenderer : PostProcessEffectRenderer<RadialBlur>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.RadialBlur);
        }

        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(shader);

            //Rotation angle is divided by the reciprocal of 720 degrees
            sheet.properties.SetVector(ShaderParameters.Params, new Vector4(settings.amount.value * 0.25f, settings.center.value.x, settings.center.value.y, settings.angle.value));
            sheet.properties.SetFloat("_Iterations", settings.iterations.value);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}