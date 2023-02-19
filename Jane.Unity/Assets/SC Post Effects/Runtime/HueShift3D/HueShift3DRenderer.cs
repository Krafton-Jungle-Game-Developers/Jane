using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class HueShift3DRenderer : PostProcessEffectRenderer<HueShift3D>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.HueShift3D);
        }

        public override void Release()
        {
            base.Release();
        }

        enum Pass
        {
            ColorSpectrum,
            GradientTexture
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            HueShift3D.isOrtho = context.camera.orthographic;

            sheet.properties.SetVector(ShaderParameters.Params, new Vector4(settings.speed.value, settings.size.value, settings.geoInfluence.value, settings.intensity.value));
            if(settings.gradientTex.value) sheet.properties.SetTexture("_GradientTex", settings.gradientTex.value);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, settings.colorSource.value == (int)HueShift3D.ColorSource.RGBSpectrum ? (int)Pass.ColorSpectrum : (int)Pass.GradientTexture);
        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.DepthNormals;
        }
    }
}