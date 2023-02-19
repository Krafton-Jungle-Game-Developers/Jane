using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class RipplesRenderer : PostProcessEffectRenderer<Ripples>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Ripples);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetFloat("_Strength", (settings.strength * 0.01f));
            sheet.properties.SetFloat("_Distance", (settings.distance * 0.01f));
            sheet.properties.SetFloat("_Speed", settings.speed);
            sheet.properties.SetVector("_Size", new Vector2(settings.width, settings.height));

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)settings.mode.value);
        }
    }
}