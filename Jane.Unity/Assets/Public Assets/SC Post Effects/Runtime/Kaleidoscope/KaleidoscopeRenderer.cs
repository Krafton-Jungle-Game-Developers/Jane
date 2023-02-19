using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class KaleidoscopeRenderer : PostProcessEffectRenderer<Kaleidoscope>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Kaleidoscope);
        }
        
        private static readonly int _KaleidoscopeSplits = Shader.PropertyToID("_KaleidoscopeSplits");

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);
            
            sheet.properties.SetVector(ShaderParameters.Params, new Vector4(Mathf.PI * 2 / Mathf.Max(1, settings.radialSplits.value), settings.maintainAspectRatio.value ? 1 : 0, settings.center.value.x, settings.center.value.y));
            sheet.properties.SetVector(_KaleidoscopeSplits, new Vector4(settings.horizontalSplits.value, settings.verticalSplits.value, 0, 0));
            
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}