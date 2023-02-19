using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class BlackBarsRenderer : PostProcessEffectRenderer<BlackBars>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.BlackBars);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetVector("_Size", new Vector2(settings.size / 10f, settings.maxSize * 5));

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)settings.mode.value);
        }

    }
}