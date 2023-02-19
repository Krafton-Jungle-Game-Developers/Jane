using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    public sealed class KuwaharaRenderer : PostProcessEffectRenderer<Kuwahara>
    {
        Shader shader;
        private int mode;
        
        public override void Init()
        {
            shader = Shader.Find(ShaderNames.Kuwahara);
        }

        public override void Release()
        {
            base.Release();
        }

        public override void Render(PostProcessRenderContext context)
        {
            mode = (int)settings.mode.value;
            if (context.camera.orthographic) mode = (int)Kuwahara.KuwaharaMode.FullScreen;

            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetFloat("_Radius", (float)settings.radius);

            if(mode == (int)Kuwahara.KuwaharaMode.DepthFade) context.command.SetGlobalVector("_FadeParams", new Vector4(settings.startFadeDistance.value, settings.endFadeDistance.value, 0, 0));
            
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, mode);
        }

        public override DepthTextureMode GetCameraFlags()
        {
            if ((int)settings.mode.value == 1)
            {
                return DepthTextureMode.Depth;
            }
            else
            {
                return DepthTextureMode.None;
            }
        }
    }
}
