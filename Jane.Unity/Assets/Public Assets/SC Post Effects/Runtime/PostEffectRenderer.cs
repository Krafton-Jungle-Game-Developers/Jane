// SC Post Effects
// Staggart Creations http://staggart.xyz
// Copyright protected under the Unity Asset Store EULA

using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCPE
{
    /// <summary>
    /// Base class for screen-space post processing through a ScriptableRenderPass
    /// </summary>
    /// <typeparam name="T">Related settings class</typeparam>
    public class PostEffectRenderer<T> : ScriptableRenderPass
    {
        /// <summary>
        /// VolumeComponent settings instance
        /// </summary>
        public T volumeSettings;
        public EffectBaseSettings settings;

        #if URP_12_0_OR_NEWER //No longer required to work with a copy (unless using the 2D renderer)
        public static bool is2D;
        //2D renderer still requires a buffer copy, otherwise effects fail to render
        private bool RequireBufferCopy => is2D;
        #else
        private bool RequireBufferCopy => true;
        #endif
        
        public bool requiresDepth = false;
        public bool requiresDepthNormals = false;

        public string shaderName;
        [SerializeField]
        private Shader shader;
        public string ProfilerTag;
        public Material Material;

        public RenderTargetIdentifier cameraColorTarget;
        public RenderTargetIdentifier cameraDepthTarget;
        public RenderTextureDescriptor mainTexDesc;
        public RenderTargetHandle mainTexHandle;
        private int mainTexID = Shader.PropertyToID(TextureNames.Main);

        public bool reconstructDepthNormals;
        public RenderTextureDescriptor depthNormalDsc;
        private int depthNormalsID = Shader.PropertyToID(TextureNames.DepthNormals);

        #if URP_12_0_OR_NEWER
        private static bool hasDetermendRendererType;
        #endif

        //Execute this only once per domain reload. Super unlikely anyone switches from 3D to 2D rendering.
        //No way to check if the 2D renderer is being used at runtime
        private void DetermineRenderer()
        {
            #if URP_12_0_OR_NEWER
            if (hasDetermendRendererType) return;
            
            ScriptableRendererData[] rendererDataList = (ScriptableRendererData[])typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(UniversalRenderPipeline.asset);
            int defaultRendererIndex = (int)typeof(UniversalRenderPipelineAsset).GetField("m_DefaultRendererIndex", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(UniversalRenderPipeline.asset);
            ScriptableRendererData forwardRenderer = rendererDataList[defaultRendererIndex];

            is2D = forwardRenderer.GetType() == typeof(Renderer2DData);

            #if SCPE_DEV
            Debug.Log("DetermineRenderer: is 2D renderer? " + is2D);
            #endif
            #endif
        }
        
        public RenderTargetIdentifier GetCameraTarget(ScriptableRenderer renderer)
        {
            //Calling this here, since it's one of the first things that happens
            DetermineRenderer();
            
#if URP_10_0_OR_NEWER
            //Fetched in CopyTargets function, no longer allowed from a ScriptableRenderFeature setup function (target may be not be created yet, or was disposed)
            return cameraColorTarget;
#else
            return renderer.cameraColorTarget;
#endif
        }
        
        public RenderTargetIdentifier GetCameraDepthTarget(ScriptableRenderer renderer)
        {
#if URP_10_0_OR_NEWER
            //Fetched in CopyTargets function, no longer allowed from a ScriptableRenderFeature setup function (target may be not be created yet, or was disposed)
            return cameraDepthTarget;
#else
            return renderer.cameraDepth;
#endif
        }

        /// <summary>
        /// Checks if post-processing pass should execute, based on current settings
        /// </summary>
        /// <param name="renderingData"></param>
        /// <returns></returns>
        public bool ShouldRender(RenderingData renderingData)
        {
            if (renderingData.postProcessingEnabled == false && !settings.alwaysEnable) return false;
            
            #if UNITY_EDITOR
            if (renderingData.cameraData.camera.cameraType == CameraType.SceneView)
            {
                if (settings.cameraTypes.HasFlag(EffectBaseSettings.CameraTypeFlags.SceneView) == false) return false;
                if (SceneView.lastActiveSceneView && SceneView.lastActiveSceneView.sceneViewState.showImageEffects == false) return false;
            }
			
			#if URP_12_0_OR_NEWER
            if (Shader.IsKeywordEnabled(ShaderKeywordStrings.DEBUG_DISPLAY)) return false;
            #endif
            #endif
            
            if (renderingData.cameraData.camera.cameraType == CameraType.Game)
            {
                if(renderingData.cameraData.camera.hideFlags != HideFlags.None && settings.cameraTypes.HasFlag(EffectBaseSettings.CameraTypeFlags.Hidden) == false) return false;
                
                if(renderingData.cameraData.renderType == CameraRenderType.Base && settings.cameraTypes.HasFlag(EffectBaseSettings.CameraTypeFlags.GameBase) == false) return false;
                if(renderingData.cameraData.renderType == CameraRenderType.Overlay && settings.cameraTypes.HasFlag(EffectBaseSettings.CameraTypeFlags.GameOverlay) == false) return false;
            } 
            
            if(renderingData.cameraData.camera.cameraType == CameraType.Reflection && settings.cameraTypes.HasFlag(EffectBaseSettings.CameraTypeFlags.Reflection) == false) return false;
            if(renderingData.cameraData.camera.cameraType == CameraType.Preview && settings.cameraTypes.HasFlag(EffectBaseSettings.CameraTypeFlags.Preview) == false) return false;

            return true;
        }
        
        private void CreateMaterialIfNull(ref Material material, ref Shader m_shader, string m_shaderName)
        {
            if (material) return;

             if(!m_shader) m_shader = Shader.Find(m_shaderName);
             
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!m_shader)
            {
                Debug.LogError("[SC Post Effects] Shader with the name <i>" + m_shaderName + "</i> could not be found, it should be in a \"Resources\" folder or in the Always Include list");
                return;
            }
#endif
            
            material = CoreUtils.CreateEngineMaterial(m_shader);
            //Material cannot be serialized anyway
            material.hideFlags = HideFlags.DontSave;
            material.name = m_shaderName;
        }

        public void Setup(ScriptableRenderer renderer, bool depthInput, bool depthNormalsInput)
        {

        }
        
        /// <summary>
        /// Sets up MainTex RT and depth normals if needed. Check if settings are valid before calling this base implementation
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="cameraTextureDescriptor"></param>
#if URP_9_0_OR_NEWER
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
#else
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
#endif
        {
#if URP_9_0_OR_NEWER
            RenderTextureDescriptor cameraTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
#endif
            ConfigurePass(cmd, cameraTextureDescriptor);
        }

        public virtual void ConfigurePass(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            CreateMaterialIfNull(ref Material, ref shader, shaderName);

            mainTexDesc = cameraTextureDescriptor;
            mainTexDesc.msaaSamples = 1;
            //Buffer needs to be 16 bit to support HDR
            cameraTextureDescriptor.colorFormat =
                SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.DefaultHDR) &&
                UniversalRenderPipeline.asset.supportsHDR
                    ? RenderTextureFormat.DefaultHDR
                    : RenderTextureFormat.Default;
            
            mainTexHandle = new RenderTargetHandle();
            //v2.1.8, now cached
            mainTexHandle.id = mainTexID;
            if (RequireBufferCopy)
            {
                cmd.GetTemporaryRT(mainTexHandle.id, mainTexDesc);
            }

            #if URP_10_0_OR_NEWER
            if(requiresDepth) ConfigureInput(ScriptableRenderPassInput.Depth);
            if(!reconstructDepthNormals) ConfigureInput(ScriptableRenderPassInput.Normal);
            CoreUtils.SetKeyword(Material, ShaderKeywords.ReconstructedDepthNormals, reconstructDepthNormals);
            #endif

            if (requiresDepthNormals)
            {
                #if URP_10_0_OR_NEWER
                if(reconstructDepthNormals)
                #endif
                {
                    //https://github.com/Unity-Technologies/Graphics/blob/c6eb37bbad8d85f5c6f9aa53648d2f4a49c33b59/com.unity.render-pipelines.universal/Runtime/Passes/DepthNormalOnlyPass.cs#L40
                    depthNormalDsc = cameraTextureDescriptor;
                    depthNormalDsc.depthBufferBits = 0;
                    depthNormalDsc.colorFormat = RenderTextureFormat.RGHalf;
                    depthNormalDsc.msaaSamples = 1;
                    
                    cmd.GetTemporaryRT(depthNormalsID, depthNormalDsc);
                    cmd.SetGlobalTexture(depthNormalsID, depthNormalsID);
                }
            }
        }

        /// <summary>
        /// Compose and execute command buffer. No need to call base implementation
        /// </summary>
        /// <param name="context"></param>
        /// <param name="renderingData"></param>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            
        }

        /// <summary>
        /// Do not override!
        /// </summary>
        /// <param name="cmd"></param>
#if URP_9_0_OR_NEWER
        public override void OnCameraCleanup(CommandBuffer cmd)
#else
        public override void FrameCleanup(CommandBuffer cmd)
#endif
        {
            Cleanup(cmd);
        }

        /// <summary>
        /// Releases the basic resources used by any effect. Cleanup be effect specific resources before calling the base implementation!
        /// Wrapper function, called by different functions between URP 8- and 9+
        /// </summary>
        public virtual void Cleanup(CommandBuffer cmd)
        {
#if SCPE_DEV
            //Debug.Log(ProfilerTag + " cleaned up");
#endif
            
            if (RequireBufferCopy) cmd.ReleaseTemporaryRT(mainTexHandle.id);
            cmd.ReleaseTemporaryRT(depthNormalsID);
        }

        /// <summary>
        /// Copies the color, depth and depth normals if required
        /// </summary>
        /// <param name="cmd"></param>
        protected void CopyTargets(CommandBuffer cmd, RenderingData renderingData)
        {
            #if URP_10_0_OR_NEWER
            //Color target can now only be fetched inside a ScriptableRenderPass
            this.cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;
            #endif

            if (RequireBufferCopy)
            {
                Blit(cmd, cameraColorTarget, mainTexHandle.id);
            }

            GenerateDepthNormals(this, cmd);
        }

        private int unity_WorldToLight = Shader.PropertyToID("unity_WorldToLight");
        
        public void SetMainLightProjection(CommandBuffer cmd, RenderingData renderingData)
        {
            if (renderingData.lightData.mainLightIndex > -1)
            {
                VisibleLight mainLight = renderingData.lightData.visibleLights[renderingData.lightData.mainLightIndex];
    
                if (mainLight.lightType == LightType.Directional)
                {
                    cmd.SetGlobalMatrix(unity_WorldToLight, mainLight.light.transform.worldToLocalMatrix);
                }
            }
        }
        
        [SerializeField]
        private Material DepthNormalsMat;
        private static Shader DepthNormalsShader;
        
        /// <summary>
        /// Reconstructs view-space normals from depth texture
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="cmd"></param>
        /// <param name="dest"></param>
        private void GenerateDepthNormals(ScriptableRenderPass pass, CommandBuffer cmd)
        {
            if (!requiresDepthNormals) return;
            
            #if URP_10_0_OR_NEWER
            //Using depth-normals pre-pass
            if(reconstructDepthNormals == false) return;
            #endif
            
            CreateMaterialIfNull(ref DepthNormalsMat, ref DepthNormalsShader, ShaderNames.DepthNormals);
            
            Blit(pass, cmd, pass.depthAttachment /* not actually used */, depthNormalsID, DepthNormalsMat, 0);
        }

        /// <summary>
        /// Wrapper for ScriptableRenderPass.Blit but allows shaders to keep using _MainTex across render pipelines
        /// </summary>
        /// <param name="cmd">Command buffer to record command for execution.</param>
        /// <param name="source">Source texture or target identifier to blit from.</param>
        /// <param name="destination">Destination texture or target identifier to blit into. This becomes the renderer active render target.</param>
        /// <param name="material">Material to use.</param>
        /// <param name="passIndex">Shader pass to use. Default is 0.</param>
        protected static void Blit(ScriptableRenderPass pass, CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat, int passIndex)
        {
            //TODO: Simply use a shader macro?
            cmd.SetGlobalTexture(TextureNames.Main, source);
            pass.Blit(cmd, source, dest, mat, passIndex);
        }

        /// <summary>
        /// Blits and executes the command buffer
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="context"></param>
        /// <param name="cmd"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="mat"></param>
        /// <param name="passIndex"></param>
        protected void FinalBlit(ScriptableRenderPass pass, ScriptableRenderContext context, CommandBuffer cmd, RenderingData renderingData, RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat, int passIndex)
        {
            if (RequireBufferCopy)
            {
                Blit(pass, cmd, source, dest, mat, passIndex);
            }
            else
            {
                #if URP_12_0_OR_NEWER //In older versions RequireBufferCopy will be true anyway, so always a valid code path
                cmd.SetGlobalTexture(mainTexHandle.id, cameraColorTarget);
                pass.Blit(cmd, ref renderingData, Material, passIndex);
                #endif
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}