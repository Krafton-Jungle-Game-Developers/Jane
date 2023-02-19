using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class CausticsRenderer : PostProcessEffectRenderer<Caustics>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Caustics);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(shader);
            var cmd = context.command;
    
            if(settings.causticsTexture.value) sheet.properties.SetTexture("_CausticsTex", settings.causticsTexture.value);
            sheet.properties.SetFloat("_LuminanceThreshold", Mathf.GammaToLinearSpace(settings.luminanceThreshold.value));
            sheet.properties.SetVector("_CausticsParams", new Vector4(settings.size, settings.speed, settings.projectFromSun.value ? 1 : 0, settings.intensity));
            sheet.properties.SetVector("_HeightParams", new Vector4(settings.minHeight.value, settings.minHeightFalloff.value, settings.maxHeight.value, settings.maxHeightFalloff.value));
            
            if(RenderSettings.sun) cmd.SetGlobalMatrix("unity_WorldToLight", RenderSettings.sun.transform.localToWorldMatrix);
            cmd.SetGlobalVector("_FadeParams", new Vector4(settings.startFadeDistance.value, settings.endFadeDistance.value, 0, settings.distanceFade.value ? 1 : 0));

            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth;
        }
    }
}