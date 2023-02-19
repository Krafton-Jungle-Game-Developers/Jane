using UnityEngine.Rendering.PostProcessing;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    public sealed class SunshaftsRenderer : PostProcessEffectRenderer<Sunshafts>
    {
        Shader shader;
        private int skyboxBufferID;

        enum Pass
        {
            SkySource,
            RadialBlur,
            Blend
        }

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Sunshafts);
            skyboxBufferID = Shader.PropertyToID("_SkyboxBuffer");
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(shader);
            CommandBuffer cmd = context.command;

    #region Parameters
            float sunIntensity = (settings.useCasterIntensity && RenderSettings.sun) ? RenderSettings.sun.intensity : settings.sunShaftIntensity.value;

            //Screen-space sun position
            Vector3 v = Vector3.one * 0.5f;
            if(RenderSettings.sun) v = context.camera.WorldToViewportPoint(-RenderSettings.sun.transform.forward * 1E10f);
            sheet.properties.SetVector("_SunPosition", new Vector4(v.x, v.y, sunIntensity, settings.falloff));

            Color sunColor = (settings.useCasterColor && RenderSettings.sun) ? RenderSettings.sun.color : settings.sunColor.value;
            sheet.properties.SetFloat("_BlendMode", (int)settings.blendMode.value);
            sheet.properties.SetColor("_SunColor", (v.z >= 0.0f) ? sunColor : new Color(0, 0, 0, 0));
            sheet.properties.SetColor("_SunThreshold", settings.sunThreshold);
    #endregion

            int res = (int)settings.resolution.value;

            //Create skybox mask
            context.command.GetTemporaryRT(skyboxBufferID, context.width / 2, context.height / 2, 0, FilterMode.Bilinear, context.sourceFormat);
            context.command.BlitFullscreenTriangle(context.source, skyboxBufferID, sheet, (int)Pass.SkySource);
            cmd.SetGlobalTexture("_SunshaftBuffer", skyboxBufferID);

            //Blur buffer
    #region Blur
            cmd.BeginSample("Sunshafts blur");
            int blurredID = Shader.PropertyToID("_Temp1");
            int blurredID2 = Shader.PropertyToID("_Temp2");
            cmd.GetTemporaryRT(blurredID, context.width / res, context.height / res, 0, FilterMode.Bilinear);
            cmd.GetTemporaryRT(blurredID2, context.width / res, context.height / res, 0, FilterMode.Bilinear);

            cmd.Blit(skyboxBufferID, blurredID);

            float offset = settings.length * (1.0f / 768.0f);

            int iterations = (settings.highQuality) ? 2 : 1;
            float blurAmount = (settings.highQuality) ? settings.length / 2.5f : settings.length;

            for (int i = 0; i < iterations; i++)
            {
                context.command.BlitFullscreenTriangle(blurredID, blurredID2, sheet, (int)Pass.RadialBlur);
                offset = blurAmount * (((i * 2.0f + 1.0f) * 6.0f)) / context.screenWidth;
                sheet.properties.SetFloat("_BlurRadius", offset);

                context.command.BlitFullscreenTriangle(blurredID2, blurredID, sheet, (int)Pass.RadialBlur);
                offset = blurAmount * (((i * 2.0f + 2.0f) * 6.0f)) / context.screenWidth;
                sheet.properties.SetFloat("_BlurRadius", offset);

            }
            cmd.EndSample("Sunshafts blur");

            cmd.SetGlobalTexture("_SunshaftBuffer", blurredID);
    #endregion

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)Pass.Blend);

            cmd.ReleaseTemporaryRT(blurredID);
            cmd.ReleaseTemporaryRT(blurredID2);
            cmd.ReleaseTemporaryRT(skyboxBufferID);
        }
    }
}
