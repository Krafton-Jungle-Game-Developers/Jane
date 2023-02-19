using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class MosaicRenderer : PostProcessEffectRenderer<Mosaic>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Mosaic);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            float size = settings.size.value;

            switch ((Mosaic.MosaicMode)settings.mode)
            {
                case Mosaic.MosaicMode.Triangles:
                    size = 10f / settings.size.value;
                    break;
                case Mosaic.MosaicMode.Hexagons:
                    size = settings.size.value / 10f;
                    break;
                case Mosaic.MosaicMode.Circles:
                    size = (1-settings.size.value) * 100f;
                    break;
            }

            Vector4 parameters = new Vector4(size, ((context.screenWidth * 2 / context.screenHeight) * size / Mathf.Sqrt(3f)), 0f, 0f);

            sheet.properties.SetVector(ShaderParameters.Params, parameters);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)settings.mode.value);
        }
    }
}