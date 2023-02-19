using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class LightStreaksRenderer : PostProcessEffectRenderer<LightStreaks>
    {
        Shader shader;
        private int emissionTex;
        RenderTexture aoRT;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.LightStreaks);
            emissionTex = Shader.PropertyToID("_BloomTex");
        }

        public override void Release()
        {
            base.Release();
        }

        enum Pass
        {
            LuminanceDiff,
            BlurFast,
            Blur,
            Blend,
            Debug
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);
            CommandBuffer cmd = context.command;

            int blurMode = (settings.quality.value == LightStreaks.Quality.Performance) ? (int)Pass.BlurFast : (int)Pass.Blur;

            float luminanceThreshold = Mathf.GammaToLinearSpace(settings.luminanceThreshold.value);

            sheet.properties.SetVector(ShaderParameters.Params, new Vector4(luminanceThreshold, settings.intensity.value, 0f, 0f));

            context.command.GetTemporaryRT(emissionTex, context.width, context.height, 0, FilterMode.Bilinear, context.sourceFormat);

            //Luminance difference check on RT
            context.command.BlitFullscreenTriangle(context.source, emissionTex, sheet, (int)Pass.LuminanceDiff);

            int downSamples = settings.downscaling + 1;
            // get two smaller RTs
            int blurredID = Shader.PropertyToID("_Temp1");
            int blurredID2 = Shader.PropertyToID("_Temp2");
            cmd.GetTemporaryRT(blurredID, context.width / downSamples, context.height / downSamples, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(blurredID2, context.width / downSamples, context.height / downSamples, 0, FilterMode.Bilinear);

            //Pass into blur target texture
            cmd.Blit(emissionTex, blurredID);

            float ratio = Mathf.Clamp(settings.direction.value, -1, 1);
            float rw = ratio < 0 ? -ratio * 16 : 0f;
            float rh = ratio > 0 ? ratio * 8 : 0f;

            int iterations = (settings.quality.value == LightStreaks.Quality.Performance) ? settings.iterations.value * 3 : settings.iterations.value;

            for (int i = 0; i < iterations; i++)
            {
                // vertical blur 1
                cmd.SetGlobalVector("_BlurOffsets", new Vector4(rw * settings.blur / context.screenWidth, rh / context.screenHeight, 0, 0));
                context.command.BlitFullscreenTriangle(blurredID, blurredID2, sheet, blurMode);

                // vertical blur 2
                cmd.SetGlobalVector("_BlurOffsets", new Vector4((rw * settings.blur) * 2f / context.screenWidth, rh * 2f / context.screenHeight, 0, 0));
                context.command.BlitFullscreenTriangle(blurredID2, blurredID, sheet, blurMode);
            }

            context.command.SetGlobalTexture("_BloomTex", blurredID);

            //Blend AO tex with image
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (settings.debug) ? (int)Pass.Debug : (int)Pass.Blend);

            // release
            context.command.ReleaseTemporaryRT(blurredID);
            context.command.ReleaseTemporaryRT(blurredID2);
            context.command.ReleaseTemporaryRT(emissionTex);
        }
    }
}