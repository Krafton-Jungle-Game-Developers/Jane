using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;

namespace SCPE
{
    public sealed class TransitionRenderer : PostProcessEffectRenderer<Transition>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Transition);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetFloat("_Progress", settings.progress.value);
            var overlayTexture = settings.gradientTex.value == null ? RuntimeUtilities.whiteTexture : settings.gradientTex.value;
            sheet.properties.SetTexture("_Gradient", overlayTexture);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}