// SC Post Effects
// Staggart Creations
// http://staggart.xyz

#if PPS
using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;
#endif

#if URP
using UnityEngine.Rendering;
using UnityEditor.Rendering;

using UnityEngine.Rendering.Universal;
using UnityEditor.Rendering.Universal;

using UnityEngine.Experimental.Rendering.Universal;
#endif

#if XR
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;
#endif

#if OCULUS
using Unity.XR.Oculus;
#endif

using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace SCPE
{
    public class SCPE_GUI : Editor
    {
        public static void DisplayDocumentationButton(string section)
        {
            GUILayout.Space(-18f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(new GUIContent("Doc", HelpIcon, "Open documentation web page\n\nHover over a parameter\nto read its description."), DocButton))
                {
                    Application.OpenURL(SCPE.DOC_URL + "?section=" + section);
                }
            }

            GUILayout.Space(2f);
        }

#if URP
        public static void ShowDepthTextureWarning(bool required = true)
        {
#if !URP_10_0_OR_NEWER //Supports the ConfigureInput function, which prompts the RP to render the depth pre-pass when required
            if (UniversalRenderPipeline.asset)
            {
                if(UniversalRenderPipeline.asset.supportsCameraDepthTexture == false && required)
                {
                    EditorGUILayout.HelpBox("The render pipeline does not render the depth texture,\n\nwhich is required for the effect's current configuration", MessageType.Error);

                    GUILayout.Space(-32);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent("Fix", EditorGUIUtility.IconContent("d_tab_next").image), GUILayout.Width(60)))
                        {
                            UniversalRenderPipeline.asset.supportsCameraDepthTexture = true;
                        }
                        GUILayout.Space(8);
                    }
                    GUILayout.Space(11);
                }
            }
#endif
        }
#endif

#if PPS
        //Append the chosen mode to the title when overriden
        public static string ModeTitle(SerializedParameterOverride prop)
        {
            return ((prop.overrideState.boolValue) ? " (" + prop.value.enumDisplayNames[prop.value.intValue] + ")" : string.Empty);
        }
        
        public static void DisplayIntensityWarning(SerializedParameterOverride prop)
        {
            m_DisplayIntensityWarning(prop.value, prop.overrideState.boolValue);
        }
#endif

        //Append the chosen mode to the title when overriden
#if URP
        public static void DisplayIntensityWarning(SerializedDataParameter prop)
        {
            m_DisplayIntensityWarning(prop.value, prop.overrideState.boolValue);
        }

        public static string ModeTitle<T>(VolumeParameter prop)
        {
            return string.Empty;
        }
#endif

        private static void m_DisplayIntensityWarning(SerializedProperty prop, bool overrideState)
        {
            bool isZero = false;

            if (prop.type == "float") isZero = prop.floatValue == 0f;
            else if (prop.type == "int") isZero = prop.intValue == 0;
            
            m_DisplayIntensityWarning(overrideState, isZero);
            
        }
        private static void m_DisplayIntensityWarning(bool isOverriden, bool isZero)
        {
            if (!isOverriden)
            {
                EditorGUILayout.HelpBox("Main parameter hasn't been overriden. Effect may not be visible, unless overriden on another volume!", MessageType.Info);
            }
            else
            {
                if (isZero)
                {
                    EditorGUILayout.HelpBox("Main parameter is overridden with a 0 value. Effect will not be visible!", MessageType.Warning);
                }
            }
        }
        
        //Dummy overload, when using PPS
        public static void DisplaySetupWarning<T>(bool compatible2D = true)
        {
            bool state = true;
            DisplaySetupWarning<T>(ref state, compatible2D);
        }
        
        public static void DisplaySetupWarning<T>(ref bool state, bool compatible2D = true)
        {
#if URP
            ScriptableRendererData[] rendererDataList = (ScriptableRendererData[])typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(UniversalRenderPipeline.asset);
            int defaultRendererIndex = (int)typeof(UniversalRenderPipelineAsset).GetField("m_DefaultRendererIndex", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(UniversalRenderPipeline.asset);
            ScriptableRendererData forwardRenderer = rendererDataList[defaultRendererIndex];
            
            bool is2D = forwardRenderer.GetType() == typeof(Renderer2DData);
            #if !URP_12_0_OR_NEWER
            if (is2D)
            {
                EditorGUILayout.HelpBox("Support for the 2D renderer requires Unity 2021.2.0 or newer.", MessageType.Error);
                return;
            }
            #endif

            if (is2D && !compatible2D)
            {
                EditorGUILayout.HelpBox("This effect has limited or no practical purpose for 2D rendering", MessageType.Error);
            }
            
            if (state) return;

            EditorGUILayout.HelpBox("Effect has not been added to the default renderer's\n\"Renderer Features\" list. Will not render otherwise.", MessageType.Warning);

            GUILayout.Space(-32);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Add", EditorGUIUtility.IconContent("d_tab_next").image), GUILayout.Width(60)))
                {
                    AutoSetup.SetupEffect<T>();
                    state = true;
                }
                GUILayout.Space(8);
            }
            GUILayout.Space(11);
#endif

#if URP && PPS
			EditorGUILayout.HelpBox("Post Processing package and URP cannot both be installed.\n\nUninstall the Post-processing package...", MessageType.Error);
#endif
        }

        public static void DrawSunInfo()
        {
            if (!RenderSettings.sun) EditorGUILayout.HelpBox("No directional light is active and enabled", MessageType.Warning);
            else EditorGUILayout.HelpBox(string.Format("Using the \"{0}\" object as the light source (brightest)", RenderSettings.sun.name), MessageType.None);
        }
        
        private static bool IsVREnabled()
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            
            #if XR
            EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget buildTargetSettings);

            if (!buildTargetSettings) return false;
            
            return buildTargetSettings.SettingsForBuildTarget(buildTargetGroup).AssignedSettings.loaders.Count > 0;
            #else
            //Old-style VR system, likely no longer used, perhaps for Steam VR/OpenVR
            return UnityEditorInternal.VR.VREditor.GetVREnabledOnTargetGroup(buildTargetGroup);
            #endif
        }

        #if OCULUS
        private static OculusSettings oculusSettings;
        #endif
        private static bool MultiPassVREnabled()
        {
            #if XR && OCULUS
            if(!oculusSettings) EditorBuildSettings.TryGetConfigObject<OculusSettings>("Unity.XR.Oculus.Settings", out oculusSettings);

            if(oculusSettings && oculusSettings.m_StereoRenderingModeDesktop != (OculusSettings.StereoRenderingModeDesktop.MultiPass))
            #else
            if (PlayerSettings.stereoRenderingPath == StereoRenderingPath.SinglePass || PlayerSettings.stereoRenderingPath == StereoRenderingPath.Instancing)
            #endif
            {
                return false;
            }

            return true;
        }

        public static void DisplayVRWarning(bool requiresDepth = false)
        {
            //EditorGUILayout.LabelField("VR enabled: " + IsVREnabled());
            //EditorGUILayout.LabelField("Multi-pass enabled: " + MultiPassVREnabled());
            //EditorGUILayout.LabelField("Mode: " + PlayerSettings.stereoRenderingPath);
            
            if (IsVREnabled() == false) return;
            
            //Single Pass Instanced uses stereo matrices, which the built-in RP partially supports
            if (MultiPassVREnabled() == true) return;
            
            var message = "Not compatible with Single-Pass Stereo Rendering (VR)";
            
            #if PPS
            message += (requiresDepth ? "\n\nThis effect requires reading the depth texture, which only URP supports in VR" : "");
            #endif
            
            EditorGUILayout.HelpBox(message, MessageType.Error);
            
        }

        public enum Status
        {
            Ok,
            Warning,
            Error,
            Info
        }

        private const string HEADER_IMG_DATA =
            "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";
        private static Texture _HEADER_IMG;
        
        public static Texture HEADER_IMG
        {
            get
            {
                if (_HEADER_IMG == null) _HEADER_IMG = CreateIcon(HEADER_IMG_DATA);
                return _HEADER_IMG;
            }
        }
        
        public static Texture CreateIcon(string data)
        {
            byte[] bytes = System.Convert.FromBase64String(data);

            Texture2D icon = new Texture2D(32, 32, TextureFormat.RGBA32, false, false);
            icon.LoadImage(bytes, true);
            
            return icon;
        }

        public static void DrawStatusBox(GUIContent content, string status, SCPE_GUI.Status type, bool boxed = true)
        {
            
            using (new EditorGUILayout.HorizontalScope(EditorStyles.label))
            {
                if (content != null)
                {
                    content.text = "  " + content.text;
                    EditorGUILayout.LabelField(content, StatusContent);
                }
                DrawStatusString(status, type, boxed);
            }
        }

        public static bool DrawLabeledActionBox(GUIContent content, string buttonLabel, Texture buttonImage)
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.label))
            {
                if (content != null)
                {
                    content.text = "  " + content.text;
                    EditorGUILayout.LabelField(content, StatusContent);
                }

                if (GUILayout.Button(new GUIContent("  " + buttonLabel, buttonImage), EditorStyles.miniButton, GUILayout.MaxWidth(200f)))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool DrawActionBox(string text, Texture image = null)
        {
            if (text != string.Empty) text = " " + text;

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(" ");

                if (GUILayout.Button(new GUIContent(text, image), EditorStyles.miniButton, GUILayout.MaxWidth(200f)))
                {
                    return true;
                }

            }

            return false;
        }

        public static void DrawStatusString(string text, Status status, bool boxed = true)
        {
            GUIStyle guiStyle = EditorStyles.label;
            Color defaultTextColor = GUI.contentColor;

            //Personal skin
            if (EditorGUIUtility.isProSkin == false)
            {
                defaultTextColor = GUI.skin.customStyles[0].normal.textColor;
                guiStyle = new GUIStyle();

                GUI.skin.customStyles[0] = guiStyle;
            }


            //Grab icon and text color for status
            Texture icon = null;
            Color statusTextColor = Color.clear;

            StyleStatus(status, out statusTextColor, out icon);


            if (EditorGUIUtility.isProSkin == false)
            {
                GUI.skin.customStyles[0].normal.textColor = statusTextColor;
            }
            else
            {
                GUI.contentColor = statusTextColor;
            }

            if (boxed)
            {
                using (new EditorGUILayout.HorizontalScope(StatusBox))
                {
                    EditorGUILayout.LabelField(new GUIContent(" " + text, icon), guiStyle);
                }
            }
            else
            {
                EditorGUILayout.LabelField(new GUIContent(" " + text, icon), guiStyle);
            }


            if (EditorGUIUtility.isProSkin == false)
            {
                GUI.skin.customStyles[0].normal.textColor = defaultTextColor;
            }
            else
            {
                GUI.contentColor = defaultTextColor;
            }
        }

        public static void StyleStatus(Status status, out Color color, out Texture icon)
        {
            color = Color.clear;
            icon = null;

            float sin = Mathf.Sin((float)EditorApplication.timeSinceStartup * 3.14159274f * 2f) * 0.5f + 0.5f;

            switch (status)
            {
                case (Status)0: //Ok
                    {
                        color = Color.Lerp(new Color(97f / 255f, 255f / 255f, 66f / 255f), Color.green, sin);

                        icon = CheckMark;
                    }
                    break;
                case (Status)1: //Warning
                    {
                        color = Color.Lerp(new Color(252f / 255f, 174f / 255f, 78f / 255f), Color.yellow, sin);
                        icon = EditorGUIUtility.IconContent("console.warnicon.sml").image;
                    }
                    break;
                case (Status)2: //Error
                    {
                        color = Color.Lerp(new Color(255f / 255f, 112f / 255f, 112f / 255f), new Color(252f / 255f, 174f / 255f, 78f / 255f), sin);

                        icon = EditorGUIUtility.IconContent("CollabError").image;
                    }
                    break;
                case (Status)3: //Info
                    {
                        color = Color.Lerp(new Color(1f, 1f, 1f), new Color(0.9f, 0.9f, 0.9f), sin);
                        icon = EditorGUIUtility.IconContent("curvekeyframe").image;
                    }
                    break;
            }

            //Darken colors on personal skin
            if (EditorGUIUtility.isProSkin == false)
            {
                color = Color.Lerp(color, Color.black, (status != Status.Info) ? 0.5f : 0.1f);
            }
        }
        
        public static void DrawWindowHeader(float windowWidth, float windowHeight)
        {
            Rect headerRect = new Rect(0, -5, windowWidth, SCPE_GUI.HEADER_IMG.height);
            if (SCPE_GUI.HEADER_IMG)
            {
                UnityEngine.GUI.DrawTexture(headerRect, SCPE_GUI.HEADER_IMG, ScaleMode.ScaleToFit);
                GUILayout.Space(SCPE_GUI.HEADER_IMG.height / 4 + 65);
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b><size=24>SC Post Effects</size></b>\n<size=16>For Post Processing Stack</size>", Header);
            }
            
            GUILayout.Space(-5f);
        }
        
        public static GUIContent TargetPlatform()
        {
            GUIContent c = new GUIContent();

            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    {
                        c.text = "Android";
                        c.image = EditorGUIUtility.IconContent("BuildSettings.Android.Small").image;
                    }
                    break;
                case BuildTarget.iOS:
                    {
                        c.text = "iOS";
                        c.image = EditorGUIUtility.IconContent("BuildSettings.iPhone.Small").image;
                    }
                    break;
                case BuildTarget.tvOS:
                {
                    c.text = "tvOS";
                    c.image = EditorGUIUtility.IconContent("BuildSettings.iPhone.Small").image;
                }
                    break;
                case BuildTarget.PS4:
                    {
                        c.text = "Playstation 4";
                        c.image = EditorGUIUtility.IconContent("BuildSettings.PS4.Small").image;
                    }
                    break;
#if UNITY_2017_3_OR_NEWER
                case BuildTarget.StandaloneOSX:
                    {
                        c.text = "MacOS";
                        c.image = EditorGUIUtility.IconContent("BuildSettings.Standalone.Small").image;
                    }
                    break;
#else
                case BuildTarget.StandaloneOSXIntel:
                    {
                        c.text = "MacOS";
                        c.image = EditorGUIUtility.IconContent("BuildSettings.Standalone.Small").image;
                    }
                    break;
                case BuildTarget.StandaloneOSXIntel64:
                    {
                        c.text = "MacOS";
                        c.image = EditorGUIUtility.IconContent("BuildSettings.Standalone.Small").image;
                    }
                    break;
#endif
                case BuildTarget.StandaloneWindows:
                    {
                        c.text = "Windows";
                        c.image = EditorGUIUtility.IconContent("BuildSettings.Metro.Small").image;
                    }
                    break;
                case BuildTarget.StandaloneWindows64:
                    {
                        c.text = "Windows";
                        c.image = EditorGUIUtility.IconContent("BuildSettings.Metro.Small").image;
                    }
                    break;
                case BuildTarget.Switch:
                    {
                        c.text = "Nintendo Switch";
                        c.image = EditorGUIUtility.IconContent("BuildSettings.Switch.Small").image;
                    }
                    break;
                case BuildTarget.XboxOne:
                    {
                        c.text = "Xbox One";
                        c.image = EditorGUIUtility.IconContent("BuildSettings.XboxOne.Small").image;
                    }
                    break;
                case BuildTarget.WebGL:
                    {
                        c.text = "Web";
                        c.image = EditorGUIUtility.IconContent("BuildSettings.Web.Small").image;
                    }
                    break;
                case BuildTarget.WSAPlayer:
                    {
                        c.text = "Universal Windows Platform";
                        c.image = EditorGUIUtility.IconContent("BuildSettings.WP8.Small").image;
                    }
                    break;
                default:
                    {
                        c.text = "Unknown";
                        c.image = EditorGUIUtility.IconContent("console.erroricon.sml").image;
                    }
                    break;
            }

            return c;
        }

        public static class Installation
        {
            private static void BeginBackground(bool odd)
            {
                var prevColor = GUI.color;
                var prevBgColor = GUI.backgroundColor;
                
                var rect = EditorGUILayout.BeginHorizontal();
                
                GUI.color = odd ?  Color.gray * (EditorGUIUtility.isProSkin ? 0.90f : 1.7f) : Color.grey * (EditorGUIUtility.isProSkin ? 0.95f : 1.66f);
                
                //Background
                EditorGUI.DrawRect(rect, GUI.color);
                
                GUI.color = prevColor;
                GUI.backgroundColor = prevBgColor;
            }

            private static void EndBackground()
            {
                EditorGUILayout.EndHorizontal();
            }

            public static void DrawHeader(string label)
            {
                var prevColor = GUI.color;
                var prevBgColor = GUI.backgroundColor;
            
                var rect = EditorGUILayout.BeginVertical();
                rect.height *= 1.25f;
                rect.y -= rect.height * 0.25f;
            
                GUI.color = Color.gray * (EditorGUIUtility.isProSkin ? 1f : 1.7f);
            
                //Background
                EditorGUI.DrawRect(rect, GUI.color);
                
                GUI.color = prevColor;
                GUI.backgroundColor = prevBgColor;
            
                EditorGUILayout.LabelField(label, SCPE_GUI.Header);
                EditorGUILayout.EndHorizontal();
            }
            
            public static void DrawAssetVersion(bool odd = false)
            {
                string versionText;
                SCPE_GUI.Status versionStatus;

                if (AssetVersionCheck.queryStatus == AssetVersionCheck.QueryStatus.Fetching)
                {
                    versionStatus = SCPE_GUI.Status.Warning;
                    versionText = "Checking update server...";
                }
                else
                {
                    versionStatus = (AssetVersionCheck.IS_UPDATED) ? SCPE_GUI.Status.Ok : SCPE_GUI.Status.Warning;
                    versionText = (AssetVersionCheck.IS_UPDATED) ? "Latest version" : "New version available";
                }

                BeginBackground(odd);
                
                SCPE_GUI.DrawStatusBox(new GUIContent("Asset version " + SCPE.INSTALLED_VERSION, EditorGUIUtility.IconContent("cs Script Icon").image), versionText, versionStatus);
                
                EndBackground();
                
                if (!AssetVersionCheck.IS_UPDATED)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (SCPE_GUI.DrawActionBox("Update to " + AssetVersionCheck.fetchedVersionString, EditorGUIUtility.IconContent("BuildSettings.Standalone.Small").image))
                        {
                            SCPE.OpenAssetInPackageManager();
                        }
                    }
                }
            }

            public static void DrawUnityVersion(bool odd = false)
            {
                string versionText = null;
                versionText = (UnityVersionCheck.COMPATIBLE) ? "Compatible" : "Not compatible";
                versionText = (UnityVersionCheck.BETA) ? "Beta/alpha!" : versionText;
                SCPE_GUI.Status versionStatus;
                versionStatus = (UnityVersionCheck.COMPATIBLE) ? SCPE_GUI.Status.Ok : SCPE_GUI.Status.Error;
                versionStatus = (UnityVersionCheck.BETA) ? SCPE_GUI.Status.Warning : versionStatus;
                
                BeginBackground(odd);

                SCPE_GUI.DrawStatusBox(new GUIContent("Unity " + UnityVersionCheck.UnityVersion, EditorGUIUtility.IconContent("UnityLogo").image), versionText, versionStatus);
                
                EndBackground();
                
                if(UnityVersionCheck.BETA) EditorGUILayout.HelpBox("Alpha/beta version are not subject to support or fixes until release. Issues or errors may come and go.", MessageType.Warning);
            }

            public static void DrawPlatform(bool odd = false)
            {
                string compatibilityText = SCPE.IsCompatiblePlatform ? "Compatible" : "Unsupported";
                SCPE_GUI.Status compatibilityStatus = SCPE.IsCompatiblePlatform ? SCPE_GUI.Status.Ok : SCPE_GUI.Status.Error;

                BeginBackground(odd);

                SCPE_GUI.DrawStatusBox(TargetPlatform(), compatibilityText, compatibilityStatus);
                
                EndBackground();
            }

            public static void DrawColorSpace()
            {
                string colorText = (UnityEditor.PlayerSettings.colorSpace == ColorSpace.Linear) ? "Linear" : "Linear is recommended";
                SCPE_GUI.Status folderStatus = (UnityEditor.PlayerSettings.colorSpace == ColorSpace.Linear) ? SCPE_GUI.Status.Ok : SCPE_GUI.Status.Warning;

                SCPE_GUI.DrawStatusBox(new GUIContent("Color space", EditorGUIUtility.IconContent("d_PreTextureRGB").image), colorText, folderStatus);
            }

            public static void DrawPipeline(bool odd = false)
            {
                string pipelineText = string.Empty;
                string pipelineName = "Built-in Render pipeline";
                SCPE_GUI.Status compatibilityStatus = SCPE_GUI.Status.Info;

                switch (RenderPipelineInstallation.CurrentPipeline)
                {
                    case RenderPipelineInstallation.Pipeline.BuiltIn:
                        pipelineName = "Built-in Render pipeline";
                        break;
                    case RenderPipelineInstallation.Pipeline.URP:
                        pipelineName = "URP " + RenderPipelineInstallation.SRP_VERSION;
                        break;
                    case RenderPipelineInstallation.Pipeline.HDRP:
                        pipelineName = "HDRP " + RenderPipelineInstallation.SRP_VERSION;
                        break;
                }

                if (RenderPipelineInstallation.VersionStatus == RenderPipelineInstallation.Version.Compatible)
                {
                    pipelineText = "Compatible";
                    compatibilityStatus = SCPE_GUI.Status.Ok;
                }
                if (RenderPipelineInstallation.VersionStatus == RenderPipelineInstallation.Version.Outdated)
                {
                    pipelineText = "Outdated (Requires v" + RenderPipelineInstallation.MIN_URP_VERSION + "+)";
                    compatibilityStatus = Status.Error;
                }
                if (RenderPipelineInstallation.VersionStatus == RenderPipelineInstallation.Version.Incompatible)
                {
                    pipelineText = "Unsupported";
                    compatibilityStatus = SCPE_GUI.Status.Error;
                }

                BeginBackground(odd);

                SCPE_GUI.DrawStatusBox(new GUIContent(pipelineName, EditorGUIUtility.IconContent("d_Profiler.Rendering").image), pipelineText, compatibilityStatus);

                EndBackground();
                
                if (RenderPipelineInstallation.VersionStatus == RenderPipelineInstallation.Version.Outdated)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (SCPE_GUI.DrawActionBox("Update to " + RenderPipelineInstallation.LATEST_COMPATIBLE_VERSION, EditorGUIUtility.IconContent("BuildSettings.Standalone.Small").image))
                        {
                            RenderPipelineInstallation.UpdateToLatest();
                        }
                    }
                }
            }

            public static void DrawPostProcessing(bool odd = false)
            {
                string ppsLabel = "Built-in RP Post Processing";
                //Append current version
                if (PostProcessingInstallation.PPSVersionStatus != PostProcessingInstallation.VersionStatus.NotInstalled) ppsLabel += " (" + PostProcessingInstallation.PPS_VERSION + ")";
                
                string ppsText = "Unknown";
                SCPE_GUI.Status ppsStatus = SCPE_GUI.Status.Ok;

                switch (PostProcessingInstallation.PPSVersionStatus)
                {
                    case PostProcessingInstallation.VersionStatus.Compatible:
                    {
                        ppsText = "Installed (Package Manager)";
                        ppsStatus = Status.Ok;
                    }
                    break;
                    case PostProcessingInstallation.VersionStatus.InCompatible:
                    {
                        ppsText = "Incompatible (Requires v" + PostProcessingInstallation.MIN_PPS_VERSION + "+)";
                        ppsStatus = Status.Error;
                    } 
                    break;
                    case PostProcessingInstallation.VersionStatus.NotInstalled:
                    {
                        ppsText = "Not installed";
                        ppsStatus = Status.Error;
                    } 
                    break;
                    case PostProcessingInstallation.VersionStatus.Outdated:
                    {
                        ppsText = "Outdated";
                        ppsStatus = Status.Warning;
                    } 
                    break;
                }

                BeginBackground(odd);

                SCPE_GUI.DrawStatusBox(new GUIContent(ppsLabel, EditorGUIUtility.IconContent("Camera Gizmo").image), ppsText, ppsStatus);
                
                EndBackground();
                
                if (PostProcessingInstallation.PPSVersionStatus == PostProcessingInstallation.VersionStatus.NotInstalled)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (SCPE_GUI.DrawActionBox("Install v" + PostProcessingInstallation.LATEST_COMPATIBLE_VERSION, EditorGUIUtility.IconContent("BuildSettings.Standalone.Small").image))
                        {
                            PostProcessingInstallation.InstallFromPackageManager();
                        }

                    }

                } //End if-installed
                //When installed but outdated
                else
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (PostProcessingInstallation.PPSVersionStatus == PostProcessingInstallation.VersionStatus.Outdated)
                        {
                            if (SCPE_GUI.DrawActionBox("Update to " + PostProcessingInstallation.LATEST_COMPATIBLE_VERSION, EditorGUIUtility.IconContent("BuildSettings.Standalone.Small").image))
                            {
                                PostProcessingInstallation.InstallFromPackageManager();
                            }
                        }
                    }
                }
            }

            public static void DrawScriptPackage(bool odd = false)
            {
                SCPE_GUI.Status status = SCPE_GUI.Status.Ok;
                string statusLabel = "None installed";
                string label = "Effect scripts & shaders";
                
                if (Installer.ScriptPackages.PACKAGE_INSTALL_STATE == Installer.ScriptPackages.PackageInstallState.None)
                {
                    status = SCPE_GUI.Status.Error;
                }
                else
                {
                    label += " (" + Installer.ScriptPackages.PACKAGE_VERSION + ")";
                    if (Installer.ScriptPackages.PACKAGE_VERSION_STATE == Installer.ScriptPackages.PackageVersionState.Outdated)
                    {
                        status = SCPE_GUI.Status.Warning;
                        statusLabel = "Outdated";
                    }
                    else
                    {
                        status = SCPE_GUI.Status.Ok;
                        statusLabel = "Installed (and up-to-date)";
                    }
                }
                
                BeginBackground(odd);
    
                SCPE_GUI.DrawStatusBox(new GUIContent(label, EditorGUIUtility.IconContent("cs Script Icon").image), statusLabel, status);
                
                EndBackground();
            }
        }

#region Styles
        private static Texture _HelpIcon;
        public static Texture HelpIcon
        {
            get
            {
                if (_HelpIcon == null)
                {
                    _HelpIcon = EditorGUIUtility.FindTexture("_Help");
                }
                return _HelpIcon;
            }
        }

        private static Texture _InfoIcon;
        public static Texture InfoIcon
        {
            get
            {
                if (_InfoIcon == null)
                {
                    _InfoIcon = EditorGUIUtility.FindTexture("d_UnityEditor.InspectorWindow");
                }
                return _InfoIcon;
            }
        }

        private static GUIStyle _Footer;
        public static GUIStyle Footer
        {
            get
            {
                if (_Footer == null)
                {
                    _Footer = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        alignment = TextAnchor.LowerCenter,
                        wordWrap = true,
                        fontSize = 12
                    };
                }

                return _Footer;
            }
        }

        private static Texture _CheckMark;
        public static Texture CheckMark
        {
            get
            {
                if (_CheckMark == null)
                {
                    _CheckMark = EditorGUIUtility.IconContent("FilterSelectedOnly").image;
                }
                return _CheckMark;
            }
        }

        private static GUIStyle _StatusContent;
        public static GUIStyle StatusContent
        {
            get
            {
                if (_StatusContent == null)
                {
                    _StatusContent = new GUIStyle(EditorStyles.label)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        fixedWidth = 200f,
                        imagePosition = ImagePosition.ImageLeft
                    };
                }

                return _StatusContent;
            }
        }

        private static GUIStyle _StatusBox;
        public static GUIStyle StatusBox
        {
            get
            {
                if (_StatusBox == null)
                {
                    _StatusBox = new GUIStyle(EditorStyles.textArea)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        fixedWidth = 200f
                    };
                }

                return _StatusBox;
            }
        }

#region Toggles
        private static GUIStyle _ToggleButtonLeftNormal;
        public static GUIStyle ToggleButtonLeftNormal
        {
            get
            {
                if (_ToggleButtonLeftNormal == null)
                {
                    _ToggleButtonLeftNormal = new GUIStyle(EditorStyles.miniButtonLeft)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        stretchWidth = true,
                        richText = true,
                        wordWrap = false,
                        fixedHeight = 20f,
                        fixedWidth = 105f
                    };
                }

                return _ToggleButtonLeftNormal;
            }
        }
        private static GUIStyle _ToggleButtonLeftToggled;
        public static GUIStyle ToggleButtonLeftToggled
        {
            get
            {
                if (_ToggleButtonLeftToggled == null)
                {
                    _ToggleButtonLeftToggled = new GUIStyle(ToggleButtonLeftNormal);
                    _ToggleButtonLeftToggled.normal.background = _ToggleButtonLeftToggled.active.background;
                }

                return _ToggleButtonLeftToggled;
            }
        }

        private static GUIStyle _ToggleButtonRightNormal;
        public static GUIStyle ToggleButtonRightNormal
        {
            get
            {
                if (_ToggleButtonRightNormal == null)
                {
                    _ToggleButtonRightNormal = new GUIStyle(EditorStyles.miniButtonRight)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        stretchWidth = true,
                        richText = true,
                        wordWrap = false,
                        fixedHeight = 20f,
                        fixedWidth = 105f

                    };
                }

                return _ToggleButtonRightNormal;
            }
        }

        private static GUIStyle _ToggleButtonRightToggled;
        public static GUIStyle ToggleButtonRightToggled
        {
            get
            {
                if (_ToggleButtonRightToggled == null)
                {
                    _ToggleButtonRightToggled = new GUIStyle(ToggleButtonRightNormal);
                    _ToggleButtonRightToggled.normal.background = _ToggleButtonRightToggled.active.background;
                }

                return _ToggleButtonRightToggled;
            }
        }
#endregion

#region Buttons
        private static GUIStyle _Button;
        public static GUIStyle Button
        {
            get
            {
                if (_Button == null)
                {
                    _Button = new GUIStyle(UnityEngine.GUI.skin.button)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        stretchWidth = true,
                        richText = true,
                        wordWrap = true,
                        imagePosition = ImagePosition.ImageLeft,
                        padding = new RectOffset()
                        {
                            left = 14,
                            right = 14,
                            top = 8,
                            bottom = 8
                        }
                    };
                }

                return _Button;
            }
        }

        private static GUIStyle _DocButton;
        public static GUIStyle DocButton
        {
            get
            {
                if (_DocButton == null)
                {
                    _DocButton = new GUIStyle(EditorStyles.miniButton)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        stretchWidth = false,
                        richText = true,
                        wordWrap = true,
                        fixedHeight = 16f,
                        fixedWidth = 55f,
                        margin = new RectOffset()
                        {
                            left = 0,
#if URP
                            right = 0,
#else
                            right = 73,
#endif
                            top = 0,
                            bottom = 0
                        }

                    };
                }

                return _DocButton;
            }
        }
#endregion
        
        private static GUIStyle _Header;
        public static GUIStyle Header
        {
            get
            {
                if (_Header == null)
                {
                    _Header = new GUIStyle(GUI.skin.label)
                    {
                        richText = true,
                        alignment = TextAnchor.MiddleLeft,
                        wordWrap = true,
                        fontSize = 18,
                        fontStyle = FontStyle.Bold,
                        padding = new RectOffset()
                        {
                            left = 5,
                            right = 0,
                            top = -5,
                            bottom = 0
                        }
                    };
                }

                return _Header;
            }
        }
#endregion
    }
}