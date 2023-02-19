using System;
using UnityEngine;
using UnityEngine.Rendering;

using UnityEngine.Rendering.PostProcessing;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using ColorParameter = UnityEngine.Rendering.PostProcessing.ColorParameter;
using Vector2Parameter = UnityEngine.Rendering.PostProcessing.Vector2Parameter;
using MinAttribute = UnityEngine.Rendering.PostProcessing.MinAttribute;

namespace SCPE
{
    public sealed class CloudShadowsRenderer : PostProcessEffectRenderer<CloudShadows>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.CloudShadows);
        }

        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(shader);
            CommandBuffer cmd = context.command;

            Camera cam = context.camera;

            var noiseTexture = settings.texture.value == null ? RuntimeUtilities.whiteTexture : settings.texture.value;
            sheet.properties.SetTexture("_NoiseTex", noiseTexture);

            float cloudsSpeed = settings.speed * 0.1f;
            sheet.properties.SetVector("_CloudParams", new Vector4(settings.size * 0.01f, settings.direction.value.x * cloudsSpeed, settings.direction.value.y * cloudsSpeed, settings.density));
            sheet.properties.SetFloat("_ProjectionEnabled", settings.projectFromSun.value ? 1 : 0);
            
            if(RenderSettings.sun) cmd.SetGlobalMatrix("unity_WorldToLight", RenderSettings.sun.transform.localToWorldMatrix);

            cmd.SetGlobalVector("_FadeParams", new Vector4(settings.startFadeDistance.value, settings.endFadeDistance.value, 0, 0));

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth;
        }
    }
}