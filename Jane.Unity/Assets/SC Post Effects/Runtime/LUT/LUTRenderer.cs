using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class LUTRenderer : PostProcessEffectRenderer<LUT>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.LUT);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            if (LUT.Bypass) return;
            
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetVector("_LUT_Params", new Vector4(settings.intensity.value, settings.invert.value));
            
            if (settings.lutNear.value)
            {
                sheet.properties.SetTexture("_LUT_Near", settings.lutNear);
            }

            if ((int)settings.mode.value == 1)
            {
                context.command.SetGlobalVector("_FadeParams", new Vector4(settings.startFadeDistance.value, settings.endFadeDistance.value, 0, 0));

                if (settings.lutFar.value) sheet.properties.SetTexture("_LUT_Far", settings.lutFar);
            }
            
            sheet.properties.SetVector(ShaderParameters.Params, new Vector4(settings.vibranceRGBBalance.value.r, settings.vibranceRGBBalance.value.g, settings.vibranceRGBBalance.value.b, settings.vibrance.value));
            

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)settings.mode.value);
        }
    }
}
