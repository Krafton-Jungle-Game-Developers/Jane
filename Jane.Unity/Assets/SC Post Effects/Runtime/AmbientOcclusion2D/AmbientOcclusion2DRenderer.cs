using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class AmbientOcclusion2DRenderer : PostProcessEffectRenderer<AmbientOcclusion2D>
    {
        Shader shader;
        private int aoTexID;
        private int screenCopyID;
        RenderTexture aoRT;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.AO2D);
            aoTexID = Shader.PropertyToID("_AO");
        }

        public override void Release()
        {
            base.Release();
        }

        enum Pass
        {
            LuminanceDiff,
            Blur,
            Blend,
            Debug
        }


        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);
            CommandBuffer cmd = context.command;

            sheet.properties.SetFloat("_SampleDistance", settings.distance);
            float luminanceThreshold = QualitySettings.activeColorSpace == ColorSpace.Gamma ? Mathf.GammaToLinearSpace(settings.luminanceThreshold.value) : settings.luminanceThreshold.value;

            sheet.properties.SetFloat("_Threshold", luminanceThreshold);
            sheet.properties.SetFloat("_Blur", settings.blurAmount);
            sheet.properties.SetFloat("_Intensity", settings.intensity);

            // Create RT for storing edge detection in
            context.command.GetTemporaryRT(aoTexID, context.width, context.height, 0, FilterMode.Bilinear, context.sourceFormat);

            //Luminance difference check on RT
            context.command.BlitFullscreenTriangle(context.source, aoTexID, sheet, (int)Pass.LuminanceDiff);

            // get two smaller RTs
            int blurredID = Shader.PropertyToID("_Temp1");
            int blurredID2 = Shader.PropertyToID("_Temp2");
            cmd.GetTemporaryRT(blurredID, context.screenWidth / settings.downscaling, context.screenHeight / settings.downscaling, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(blurredID2, context.screenWidth / settings.downscaling, context.screenHeight / settings.downscaling, 0, FilterMode.Bilinear);

            //Pass AO into blur target texture
            cmd.Blit(aoTexID, blurredID);

            for (int i = 0; i < settings.iterations; i++)
            {
                // horizontal blur
                cmd.SetGlobalVector("_BlurOffsets", new Vector4((settings.blurAmount) / context.screenWidth, 0, 0, 0));
                context.command.BlitFullscreenTriangle(blurredID, blurredID2, sheet, (int)Pass.Blur);

                // vertical blur
                cmd.SetGlobalVector("_BlurOffsets", new Vector4(0, (settings.blurAmount) / context.screenHeight, 0, 0));
                context.command.BlitFullscreenTriangle(blurredID2, blurredID, sheet, (int)Pass.Blur);
            }

            context.command.SetGlobalTexture("_AO", blurredID);

            //Blend AO tex with image
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (settings.aoOnly) ? (int)Pass.Debug : (int)Pass.Blend);

            // release
            context.command.ReleaseTemporaryRT(blurredID);
            context.command.ReleaseTemporaryRT(blurredID2);
            context.command.ReleaseTemporaryRT(aoTexID);
        }
    }
}