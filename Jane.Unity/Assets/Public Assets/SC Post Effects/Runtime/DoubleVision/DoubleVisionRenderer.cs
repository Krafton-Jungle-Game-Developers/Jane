using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class DoubleVisionRenderer : PostProcessEffectRenderer<DoubleVision>
    {
        Shader DoubleVisionShader;

        public override void Init()
        {
            DoubleVisionShader = Shader.Find(ShaderNames.DoubleVision);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {

            var sheet = context.propertySheets.Get(DoubleVisionShader);

            sheet.properties.SetFloat("_Amount", settings.intensity.value / 10);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)settings.mode.value);
        }
    }
}