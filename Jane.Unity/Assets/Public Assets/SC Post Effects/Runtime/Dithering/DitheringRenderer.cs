using System;
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
    public sealed class DitheringRenderer : PostProcessEffectRenderer<Dithering>
    {
        Shader shader;

        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Dithering);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(shader);

            var lutTexture = settings.lut.value == null ? RuntimeUtilities.blackTexture : settings.lut.value;
            sheet.properties.SetTexture("_LUT", lutTexture);
            float luminanceThreshold = QualitySettings.activeColorSpace == ColorSpace.Gamma ? Mathf.LinearToGammaSpace(settings.luminanceThreshold.value) : settings.luminanceThreshold.value;

            Vector4 ditherParams = new Vector4(0f, settings.tiling, luminanceThreshold, settings.intensity);
            sheet.properties.SetVector("_Dithering_Coords", ditherParams);

            #if DITHERING_WORLD_PROJECTION
            if (settings.worldProjected.value)
            {
                var p = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, false);
                p[2, 3] = p[3, 2] = 0.0f;
                p[3, 3] = 1.0f;
                var clipToWorld = Matrix4x4.Inverse(p * context.camera.worldToCameraMatrix) * Matrix4x4.TRS(new Vector3(0, 0, -p[2, 2]), Quaternion.identity, Vector3.one);
                sheet.properties.SetMatrix("clipToWorld", clipToWorld);
                sheet.properties.SetMatrix("cameraToWorld", context.camera.cameraToWorldMatrix);
            }

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, settings.worldProjected.value ? 1 : 0);
            #else
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            #endif
        }
        
#if DITHERING_WORLD_PROJECTION
        public override DepthTextureMode GetCameraFlags()
        {
            return settings.worldProjected.value ? DepthTextureMode.DepthNormals : DepthTextureMode.None;
        }
#endif
    }
}