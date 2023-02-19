using UnityEngine.Rendering.PostProcessing;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    public sealed class ScanlinesRenderer : PostProcessEffectRenderer<Scanlines>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Scanlines);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetVector(ShaderParameters.Params, new Vector4(settings.amount, settings.intensity / 1000, settings.speed * 8f, 0f));

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}