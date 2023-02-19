using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class OverlayRenderer : PostProcessEffectRenderer<Overlay>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Overlay);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            if (settings.overlayTex.value) sheet.properties.SetTexture("_OverlayTex", settings.overlayTex);
            sheet.properties.SetVector(ShaderParameters.Params, new Vector4(settings.intensity, Mathf.Pow(settings.tiling + 1, 2), settings.autoAspect ? 1f : 0f, (int)settings.blendMode.value));
            float luminanceThreshold = QualitySettings.activeColorSpace == ColorSpace.Gamma ? Mathf.LinearToGammaSpace(settings.luminanceThreshold.value) : settings.luminanceThreshold.value;
            sheet.properties.SetFloat("_LuminanceThreshold", luminanceThreshold);
            
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}