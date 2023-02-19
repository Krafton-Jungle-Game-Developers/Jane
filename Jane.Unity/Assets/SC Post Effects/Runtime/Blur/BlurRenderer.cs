using UnityEngine.Rendering;
using UnityEngine;

using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using MinAttribute = UnityEngine.Rendering.PostProcessing.MinAttribute;

namespace SCPE 
{
    public sealed class BlurRenderer : PostProcessEffectRenderer<Blur>
    {
        Shader shader;
        int screenCopyID;
        int blurredID = Shader.PropertyToID("_Temp1");
        int blurredID2 = Shader.PropertyToID("_Temp2");

        enum Pass
        {
            Blend,
            BlendDepthFade,
            Gaussian,
            Box
        }

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Blur);
            screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");
        }

        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(shader);
            CommandBuffer cmd = context.command;

            cmd.GetTemporaryRT(screenCopyID, context.width, context.height, 0, FilterMode.Bilinear, context.sourceFormat);
            cmd.Blit(context.source, screenCopyID);

            // get two smaller RTs
            cmd.GetTemporaryRT(blurredID, context.screenWidth / settings.downscaling, context.screenHeight / settings.downscaling, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(blurredID2, context.screenWidth / settings.downscaling, context.screenHeight / settings.downscaling, 0, FilterMode.Bilinear);

            // downsample screen copy into smaller RT, release screen RT
            cmd.Blit(screenCopyID, blurredID);

            int blurPass = (settings.mode == Blur.BlurMethod.Gaussian) ? (int)Pass.Gaussian : (int)Pass.Box;

            for (int i = 0; i < settings.iterations; i++)
            {
                //Safeguard for exploding GPUs
                if (settings.iterations > 12) return;

                // horizontal blur
                cmd.SetGlobalVector("_BlurOffsets", new Vector4(settings.amount / context.screenWidth, 0, 0, 0));
                context.command.BlitFullscreenTriangle(blurredID, blurredID2, sheet, blurPass);

                // vertical blur
                cmd.SetGlobalVector("_BlurOffsets", new Vector4(0, settings.amount / context.screenHeight, 0, 0));
                context.command.BlitFullscreenTriangle(blurredID2, blurredID, sheet, blurPass);

                //Double blur
                if (settings.highQuality)
                {
                    // horizontal blur
                    cmd.SetGlobalVector("_BlurOffsets", new Vector4(settings.amount / context.screenWidth, 0, 0, 0));
                    context.command.BlitFullscreenTriangle(blurredID, blurredID2, sheet, blurPass);

                    // vertical blur
                    cmd.SetGlobalVector("_BlurOffsets", new Vector4(0, settings.amount / context.screenHeight, 0, 0));
                    context.command.BlitFullscreenTriangle(blurredID2, blurredID, sheet, blurPass);
                }
            }

            cmd.SetGlobalTexture("_BlurredTex", blurredID);
            
            if( settings.distanceFade.value) cmd.SetGlobalVector("_FadeParams", new Vector4(settings.startFadeDistance.value, settings.endFadeDistance.value, 0, 0));
            
            // Render blurred texture in blend pass
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, settings.distanceFade.value ? (int)Pass.BlendDepthFade : (int)Pass.Blend);

            // release
            cmd.ReleaseTemporaryRT(screenCopyID);
            cmd.ReleaseTemporaryRT(blurredID);
            cmd.ReleaseTemporaryRT(blurredID2);
        }
    }
}