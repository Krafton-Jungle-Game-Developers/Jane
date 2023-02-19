using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class EdgeDetectionRenderer : PostProcessEffectRenderer<EdgeDetection>
    {
        Shader shader;
        public override void Init()
        {
            shader = Shader.Find(ShaderNames.EdgeDetection);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);
            CommandBuffer cmd = context.command;
            
            Vector2 sensitivity = new Vector2(settings.sensitivityDepth, settings.sensitivityNormals);
            sheet.properties.SetVector("_Sensitivity", sensitivity);
            sheet.properties.SetFloat("_BackgroundFade", (settings.debug) ? 1f : 0f);
            sheet.properties.SetFloat("_EdgeSize", settings.edgeSize);
            sheet.properties.SetFloat("_Exponent", settings.edgeExp);
            sheet.properties.SetFloat("_Threshold", settings.lumThreshold);
            sheet.properties.SetColor("_EdgeColor", settings.edgeColor);
            sheet.properties.SetFloat("_EdgeOpacity", settings.edgeOpacity);

            sheet.properties.SetVector("_FadeParams", new Vector4(settings.startFadeDistance.value, settings.endFadeDistance.value, (settings.invertFadeDistance.value) ? 1 : 0, settings.distanceFade.value ? 1 : 0));

            sheet.properties.SetVector("_SobelParams", new Vector4((settings.sobelThin) ? 1 : 0, 0, 0, 0));

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)settings.mode.value);
        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.DepthNormals;
        }
    }
}