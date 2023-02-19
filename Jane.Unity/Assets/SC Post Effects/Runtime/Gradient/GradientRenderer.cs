using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class GradientRenderer : PostProcessEffectRenderer<Gradient>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Gradient);
            //settings.gradient.value = new Gradient();
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            //This should be editor inspector only, but that's not possible currently
            //Texture2D gradientTexture = settings.GenerateGradient(settings.gradient.value);
            //if(settings.gradient.value.colorKeys.Length > 0) settings.gradientTex.value = settings.GenerateGradient(settings.gradient);

            if (settings.gradientTex.value) sheet.properties.SetTexture("_Gradient", settings.gradientTex);
            sheet.properties.SetColor("_Color1", settings.color1);
            sheet.properties.SetColor("_Color2", settings.color2);
            sheet.properties.SetFloat("_Rotation", settings.rotation * 360f);
            sheet.properties.SetFloat("_Intensity", settings.intensity);
            sheet.properties.SetFloat("_BlendMode", (int)settings.mode.value);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)settings.input.value);
        }
    }
}