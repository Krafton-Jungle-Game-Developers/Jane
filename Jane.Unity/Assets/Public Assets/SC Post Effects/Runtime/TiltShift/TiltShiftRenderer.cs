using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class TiltShiftRenderer : PostProcessEffectRenderer<TiltShift>
    {
        Shader shader;
        int screenCopyID;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.TiltShift);
            screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");
        }

        enum Pass
        {
            FragHorizontal,
            FragHorizontalHQ,
            FragRadial,
            FragRadialHQ,
            FragBlend,
            FragDebug
        }

        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(shader);
            CommandBuffer cmd = context.command;

            sheet.properties.SetVector(ShaderParameters.Params, new Vector4(settings.areaSize.value, settings.areaFalloff.value, settings.amount.value, (int)settings.mode.value));
            sheet.properties.SetFloat("_Offset", settings.offset.value);
            sheet.properties.SetFloat("_Angle", settings.angle.value);

            //Copy screen contents
            context.command.GetTemporaryRT(screenCopyID, context.width, context.height, 0, FilterMode.Bilinear, context.sourceFormat);
            int pass = (int)settings.mode.value + (int)settings.quality.value;

            switch ((int)settings.mode.value)
            {
                case 0:
                    pass = 0 + (int)settings.quality.value;
                    break;
                case 1:
                    pass = 2 + (int)settings.quality.value;
                    break;
            }
            cmd.BlitFullscreenTriangle(context.source, screenCopyID, sheet, pass);
            cmd.SetGlobalTexture("_BlurredTex", screenCopyID);

            // Render blurred texture in blend pass
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, TiltShift.debug ? (int)Pass.FragDebug : (int)Pass.FragBlend);

            cmd.ReleaseTemporaryRT(screenCopyID);
        }
    }
}