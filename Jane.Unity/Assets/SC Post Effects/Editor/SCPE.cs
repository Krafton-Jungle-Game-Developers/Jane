// SC Post Effects
// Staggart Creations
// http://staggart.xyz

#if PPS
using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;
#endif

#if URP
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;
using UnityEditor.Rendering;
using UnityEngine.Rendering;
using System.Reflection;
#endif

using System.Collections.Generic;
using UnityEditor.PackageManager;

using UnityEngine;
using UnityEditor;
using System.Net;

namespace SCPE
{
    public class SCPE : Editor
    {
        //Asset specifics
        public const string ASSET_NAME = "SC Post Effects";
        public const string PUBLISHER_NAME = "Staggart Creations";
        public const string ASSET_ID = "108753";
        public const string ASSET_ABRV = "SCPE";

        public const string INSTALLED_VERSION = "2.2.0";

        public const string MIN_UNITY_VERSION = "2019.4.0";

        public const string VERSION_FETCH_URL = "http://www.staggart.xyz/backend/versions/scpe.php";
        public const string DOC_URL = "http://staggart.xyz/unity/sc-post-effects/scpe-docs/";
        public const string FORUM_URL = "https://forum.unity.com/threads/513191";

        public const string PP_LAYER_NAME = "PostProcessing";

        public static string PACKAGE_ROOT_FOLDER
        {
            get { return SessionState.GetString(SCPE.ASSET_ABRV + "_BASE_FOLDER", string.Empty); }
            set { SessionState.SetString(SCPE.ASSET_ABRV + "_BASE_FOLDER", value); }
        }
        public static string PACKAGE_PARENT_FOLDER
        {
            get { return SessionState.GetString(SCPE.ASSET_ABRV + "_PARENT_FOLDER", string.Empty); }
            set { SessionState.SetString(SCPE.ASSET_ABRV + "_PARENT_FOLDER", value); }
        }

        public enum RenderPipeline
        {
            Legacy,
            Lightweight,
            Universal,
            HighDefinition
        };
        public static RenderPipeline pipeline = RenderPipeline.Legacy;

#if SCPE_DEV
        [MenuItem("SCPE/TEST/GetRenderPipeline")]
#endif
        public static RenderPipeline GetRenderPipeline()
        {
            UnityEngine.Rendering.RenderPipelineAsset renderPipelineAsset = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;

            if (renderPipelineAsset)
            {
                if (renderPipelineAsset.GetType().ToString() == "UnityEngine.Rendering.LWRP.LightweightRenderPipelineAsset") { pipeline = RenderPipeline.Lightweight; }
                if (renderPipelineAsset.GetType().ToString() == "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset") { pipeline = RenderPipeline.Universal; }
                if (renderPipelineAsset.GetType().ToString() == "UnityEngine.Rendering.HighDefinition.HDRenderPipelineAsset") { pipeline = RenderPipeline.HighDefinition; }
            }
            else { pipeline = RenderPipeline.Legacy; }

#if SCPE_DEV
            Debug.Log("<b>" + SCPE.ASSET_NAME + "</b> Pipeline active: " + pipeline.ToString());
#endif

            return pipeline;
        }

        public static bool IsCompatiblePlatform
        {
            get
            {
                return EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS || 
                       EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android || 
                       EditorUserBuildSettings.activeBuildTarget != BuildTarget.Switch || 
                       EditorUserBuildSettings.activeBuildTarget != BuildTarget.tvOS
                    ;
            }
        }

        //Check for correct settings, in order to avoid artifacts
        public static bool IsValidGradient(TextureImporter importer)
        {
            return importer.mipmapEnabled == false && importer.wrapMode == TextureWrapMode.Clamp && importer.filterMode == FilterMode.Bilinear;
        }

        public static void SetGradientImportSettings(TextureImporter importer)
        {
            importer.textureType = TextureImporterType.Default;
            importer.filterMode = FilterMode.Bilinear;
            importer.anisoLevel = 0;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;

            importer.SaveAndReimport();
        }

        #if PPS
        //Fallback overload for <2.1.8 to 2.1.9+
        public static void CheckGradientImportSettings(UnityEditor.Rendering.PostProcessing.SerializedParameterOverride tex) { }
        #endif
        
        #if URP
        //Fallback overload for <2.1.8 to 2.1.9+
        public static void CheckGradientImportSettings(SerializedDataParameter tex) { }
        #endif
        
        public static void CheckGradientImportSettings(Object tex)
        {
            if (tex == null) return;

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex));

            if (importer != null) // Fails when using a non-persistent texture
            {
                bool valid = IsValidGradient(importer);

                if (!valid)
                {
                    EditorGUILayout.HelpBox("\"" + tex.name + "\" has invalid import settings.", MessageType.Warning);

                    GUILayout.Space(-32);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Fix", GUILayout.Width(60)))
                        {
                            if (EditorUtility.IsPersistent(tex) == false)
                            {
                                Debug.LogError("Cannot set import settings for textures not saved on disk");
                                return;
                            }

                            SetGradientImportSettings(importer);
                            AssetDatabase.Refresh();
                        }
                        GUILayout.Space(8);
                    }
                    GUILayout.Space(11);
                }
            }

        }
        
        public static void UpdateRootFolder()
        {
            //Get script path
            string[] scriptGUID = AssetDatabase.FindAssets("SCPE t:script");
            string scriptFilePath = AssetDatabase.GUIDToAssetPath(scriptGUID[0]);

            //Truncate to get relative path
            PACKAGE_ROOT_FOLDER = scriptFilePath.Replace("/Editor/SCPE.cs", string.Empty);
            PACKAGE_PARENT_FOLDER = scriptFilePath.Replace(SCPE.ASSET_NAME + "/Editor/SCPE.cs", string.Empty);

#if SCPE_DEV
            Debug.Log("<b>Package parent</b>: " + PACKAGE_PARENT_FOLDER);
            Debug.Log("<b>Package root</b> " + PACKAGE_ROOT_FOLDER);
#endif
        }

        public static void OpenAssetInPackageManager()
        {
            Application.OpenURL("com.unity3d.kharma:content/" + ASSET_ID);
        }
        
        public static void OpenStorePage()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/slug/" + ASSET_ID + "#reviews");
        }

        public static int GetLayerID()
        {
            return LayerMask.NameToLayer(PP_LAYER_NAME);
        }
    }

    public static class AutoSetup
    {
        [MenuItem("CONTEXT/Camera/Add Post Processing Layer")]
        public static void SetupCamera()
        {
            Camera cam = (Camera.main) ? Camera.main : GameObject.FindObjectOfType<Camera>();
            GameObject mainCamera = cam.gameObject;

            if (!mainCamera)
            {
                Debug.LogError("<b>SC Post Effects</b> No camera found in scene to configure");
                return;
            }

#if URP
            UniversalAdditionalCameraData data = mainCamera.GetComponent<UniversalAdditionalCameraData>();
            if (data)
            {
                data.renderPostProcessing = true;
                data.volumeTrigger = mainCamera.transform;

                EditorUtility.SetDirty(data);
            }
#endif

#if PPS //Avoid missing PostProcessing scripts
            //Add PostProcessLayer component if not already present
            if (mainCamera.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>() == false)
            {
                UnityEngine.Rendering.PostProcessing.PostProcessLayer ppLayer = mainCamera.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>();
                ppLayer.volumeLayer = LayerMask.GetMask(LayerMask.LayerToName(SCPE.GetLayerID()));
                ppLayer.fog.enabled = false;
                
                Debug.Log("<b>PostProcessLayer</b> component was added to <b>" + mainCamera.name + "</b>");
                
                Selection.objects = new[] { mainCamera };
                EditorUtility.SetDirty(mainCamera);
            }
#endif
        }

        //Create a global post processing volume and assign the correct layer and default profile
        public static void SetupGlobalVolume()
        {
            GameObject volumeObject = new GameObject("Global Post-process Volume");

#if PPS && URP
            //Can't determine which system to use!
#else
    #if PPS
            UnityEngine.Rendering.PostProcessing.PostProcessVolume volume = volumeObject.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>();
            volumeObject.layer = SCPE.GetLayerID();
            volume.isGlobal = true;
    #endif

    #if URP
            Volume volume = volumeObject.AddComponent<Volume>();
            volume.isGlobal = true;
    #endif

            string type = "PostProcessProfile";
    #if URP
            type = "VolumeProfile";
    #endif
            //Find default profile
            string[] assets = AssetDatabase.FindAssets("SC Default Profile t: " + type);

            if (assets.Length > 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
    #if PPS
                UnityEngine.Rendering.PostProcessing.PostProcessProfile defaultProfile = (UnityEngine.Rendering.PostProcessing.PostProcessProfile)AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Rendering.PostProcessing.PostProcessProfile));
                volume.sharedProfile = defaultProfile;
    #endif
    #if URP
                UnityEngine.Rendering.VolumeProfile defaultProfile = (UnityEngine.Rendering.VolumeProfile)AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Rendering.VolumeProfile));
                volume.sharedProfile = defaultProfile;
    #endif
            }
            else
            {
                Debug.Log("The default \"SC Post Effects\" profile could not be found. Add a new profile to the volume to get started.");
            }

            Selection.objects = new[] { volumeObject };
            EditorUtility.SetDirty(volumeObject);
#endif
        }

        public static bool ValidEffectSetup<T>()
        {
            bool state = false;
#if URP
            if(UniversalRenderPipeline.asset == null) return false;

            ScriptableRendererData[] rendererDataList = (ScriptableRendererData[])typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(UniversalRenderPipeline.asset);
            ScriptableRendererData forwardRenderer = rendererDataList[0];

            foreach (ScriptableRendererFeature item in forwardRenderer.rendererFeatures)
            {
                if (item == null) continue;

                if (item.GetType() == typeof(T))
                {
                    state = true;
                }
            }
#endif

            return state;
        }

        public static void SetupEffect<T>()
        {
#if URP
            if(UniversalRenderPipeline.asset == null) return;

            ScriptableRendererData[] rendererDataList = (ScriptableRendererData[])typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(UniversalRenderPipeline.asset);
            int defaultRendererIndex = (int)typeof(UniversalRenderPipelineAsset).GetField("m_DefaultRendererIndex", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(UniversalRenderPipeline.asset);
            ScriptableRendererData forwardRenderer = rendererDataList[defaultRendererIndex];
            
            #if !URP_12_0_OR_NEWER
            if (forwardRenderer.GetType() == typeof(Renderer2DData))
            {
                Debug.LogError("2D renderer is not supported. Requires Unity 2021.2.0 (URP v12)");
                return;
            }
            #endif
            
            ScriptableRendererFeature feature = (ScriptableRendererFeature)ScriptableRendererFeature.CreateInstance(typeof(T).ToString());
            feature.name = "[SCPE] " + typeof(T).ToString().Replace("SCPE.", string.Empty);

            //Add component https://github.com/Unity-Technologies/Graphics/blob/d0473769091ff202422ad13b7b764c7b6a7ef0be/com.unity.render-pipelines.universal/Editor/ScriptableRendererDataEditor.cs#L180
            AssetDatabase.AddObjectToAsset(feature, forwardRenderer);
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(feature, out var guid, out long localId);

            //Get feature list
            FieldInfo renderFeaturesInfo = typeof(ScriptableRendererData).GetField("m_RendererFeatures", BindingFlags.Instance | BindingFlags.NonPublic);
            List<ScriptableRendererFeature> m_RendererFeatures = (List<ScriptableRendererFeature>)renderFeaturesInfo.GetValue(forwardRenderer);

            //Modify and set list
            m_RendererFeatures.Add(feature);
            renderFeaturesInfo.SetValue(forwardRenderer, m_RendererFeatures);

            //Onvalidate will call ValidateRendererFeatures and update m_RendererPassMap
            MethodInfo validateInfo = typeof(ScriptableRendererData).GetMethod("OnValidate", BindingFlags.Instance | BindingFlags.NonPublic);
            validateInfo.Invoke(forwardRenderer, null);

#if UNITY_EDITOR
            EditorUtility.SetDirty(forwardRenderer);
            AssetDatabase.SaveAssets();
#endif
            Debug.Log("<b>" + feature.name + "</b> was added to the <i>" + forwardRenderer.name + "</i> renderer", forwardRenderer);
#endif
        }
    }//Auto setup class

#if UNITY_EDITOR
    public class AssetVersionCheck : Editor
    {
        public static bool IS_UPDATED = true;

        public static string fetchedVersionString;
        public static int fetchedVersion;

        public enum QueryStatus
        {
            Fetching,
            Completed,
            Failed
        }
        public static QueryStatus queryStatus = QueryStatus.Completed;

#if SCPE_DEV
        [MenuItem("SCPE/TEST/Check for update")]
#endif
        public static void GetLatestVersionPopup()
        {
            CheckForUpdate();

            while (queryStatus == QueryStatus.Fetching)
            {
                //
            }

            if (!IS_UPDATED)
            {
                if (EditorUtility.DisplayDialog(SCPE.ASSET_NAME + ", version " + SCPE.INSTALLED_VERSION, "A new version is available: " + fetchedVersionString, "Open store page", "Close"))
                {
                    SCPE.OpenAssetInPackageManager();
                }
            }
            else
            {
                if (EditorUtility.DisplayDialog(SCPE.ASSET_NAME + ", version " + SCPE.INSTALLED_VERSION, "Installed version is up-to-date!", "Close")) { }
            }
        }

        public static int VersionStringToInt(string input)
        {
            //Remove all non-alphanumeric characters from version 
            input = input.Replace(".", string.Empty);
            input = input.Replace(" BETA", string.Empty);
            return int.Parse(input, System.Globalization.NumberStyles.Any);
        }

        public static void CheckForUpdate()
        {
            queryStatus = QueryStatus.Fetching;

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(OnRetreivedServerVersion);
                webClient.DownloadStringAsync(new System.Uri(SCPE.VERSION_FETCH_URL), fetchedVersionString);
            }

        }

        private static void OnRetreivedServerVersion(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && !e.Cancelled)
            {
                queryStatus = QueryStatus.Completed;

                fetchedVersionString = e.Result;
                fetchedVersion = VersionStringToInt(fetchedVersionString);
                int installedVersion = VersionStringToInt(SCPE.INSTALLED_VERSION);

                //Success
                IS_UPDATED = (installedVersion >= fetchedVersion) ? true : false;

#if SCPE_DEV
                Debug.Log("<b>PackageVersionCheck</b> Up-to-date = " + IS_UPDATED + " (Installed:" + SCPE.INSTALLED_VERSION + ") (Remote:" + fetchedVersionString + ")");
#endif
            }
            else
            {
                Debug.LogWarning("[" + SCPE.ASSET_NAME + "] Contacting update server failed: " + e.Error.Message);
                queryStatus = QueryStatus.Failed;

                //When failed, assume installation is up-to-date
                IS_UPDATED = true;
            }
        }

    }
#endif
}//namespace