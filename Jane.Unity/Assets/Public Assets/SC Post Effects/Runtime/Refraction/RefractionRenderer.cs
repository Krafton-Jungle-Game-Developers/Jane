using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class RefractionRenderer : PostProcessEffectRenderer<Refraction>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Refraction);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetFloat("_Amount", settings.amount);
            if (settings.refractionTex.value) sheet.properties.SetTexture("_RefractionTex", settings.refractionTex);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (settings.convertNormalMap) ? 1 : 0);
        }
    }
}