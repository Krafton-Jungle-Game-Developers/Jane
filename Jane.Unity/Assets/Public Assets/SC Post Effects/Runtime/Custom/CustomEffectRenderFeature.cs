using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SCPE
{
    public class CustomEffectRenderFeature : ScriptableRendererFeature
    {
        public class CustomEffectPass : PostEffectRenderer<VolumeComponent>
        {
            public CustomEffectPass(PostEffectSettings settings)
            {
                this.settings = settings;
                ProfilerTag = this.ToString();
                
                //Assign the custom material
                Material = settings.material;
            }
            
            public void Setup(ScriptableRenderer renderer)
            {
                this.cameraColorTarget = GetCameraTarget(renderer);

                requiresDepth = true;
                //Don't execute without material, otherwise the attempt will be made to create it from a null shader
                if(Material) renderer.EnqueuePass(this);
            }
            
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (ShouldRender(renderingData) == false) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);
                
                CopyTargets(cmd, renderingData);
                
                FinalBlit(this, context, cmd, renderingData, mainTexHandle.id, cameraColorTarget, Material, 0);
            }
        }

        [Serializable]
        public class PostEffectSettings : EffectBaseSettings
        {
            [Space]
            public Material material;
            [Tooltip("Executes the effect before transparent materials are rendered.")]
            public bool skipTransparents;

        }

        CustomEffectPass m_ScriptablePass;
        [SerializeField]
        public PostEffectSettings settings = new PostEffectSettings();
        
        public override void Create()
        {
            m_ScriptablePass = new CustomEffectPass(settings);
            m_ScriptablePass.renderPassEvent = settings.skipTransparents ? RenderPassEvent.BeforeRenderingTransparents : settings.injectionPoint;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.Setup(renderer);
        }
    }
}