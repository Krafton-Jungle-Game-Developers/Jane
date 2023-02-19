// SC Post Effects
// Staggart Creations
// http://staggart.xyz

using System;
using UnityEditor;
using UnityEngine;

namespace SCPE
{
    public class InstallerWindow : EditorWindow
    {
        //Window properties
        private static readonly int width = 450;
        private static readonly int height = 575;
        private Vector2 scrollPos;

        
#if SCPE_DEV
        [MenuItem("SC Post Effects Installer/Open", false, 0)]
#endif
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(InstallerWindow), false, " Installer", true);

            editorWindow.titleContent.image = EditorGUIUtility.IconContent("_Popup").image;
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.ShowAuxWindow();

            //Open somewhat in the center of the screen
            editorWindow.position = new Rect((Screen.currentResolution.width / 2) - (width * 0.5f), (Screen.currentResolution.height / 2)  - (height * 0.7f), width, height);

            //Fixed size
            editorWindow.maxSize = new Vector2(width, height);
            editorWindow.minSize = new Vector2(width, height);

            Init();

            editorWindow.Show();
        }

        private void OnEnable()
        {
            Installer.Initialize();
        }

        private static void Init()
        {
            Installer.Initialize();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
        
        private void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                this.ShowNotification(new GUIContent(" Compiling...", EditorGUIUtility.IconContent("cs Script Icon").image));
            }
            else
            {
                this.RemoveNotification();
            }
            
            if (SCPE_GUI.HEADER_IMG)
            {
                Rect headerRect = new Rect(0, -5, width, SCPE_GUI.HEADER_IMG.height);
                UnityEngine.GUI.DrawTexture(headerRect, SCPE_GUI.HEADER_IMG, ScaleMode.ScaleToFit);
                GUILayout.Space(SCPE_GUI.HEADER_IMG.height - 10);
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b><size=24>SC Post Effects</size></b>\n<size=16>For Post Processing Stack</size>", SCPE_GUI.Header);
            }

            GUILayout.Space(15f);

            DrawInstallationScreen();
            
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("- Staggart Creations -", SCPE_GUI.Footer);

        }

        public static void DrawInstallationScreen()
        {
            DrawPreInstallSection();
            DrawEffectInstallSection();
            DrawDemoInstallSection();
        }

        public static void DrawPreInstallSection()
        {
            SCPE_GUI.Installation.DrawHeader("Project compatibility");
            
            using (new EditorGUILayout.VerticalScope(EditorStyles.textArea))
            {
                EditorGUILayout.Space();

                SCPE_GUI.Installation.DrawAssetVersion(true);
                SCPE_GUI.Installation.DrawUnityVersion();
                SCPE_GUI.Installation.DrawPlatform(true);
                //SCPE_GUI.Installation.DrawColorSpace();
                SCPE_GUI.Installation.DrawPipeline();
                

                EditorGUILayout.Space();
            }
            
            EditorGUILayout.Space();
        }

        public static void DrawEffectInstallSection()
        {
            SCPE_GUI.Installation.DrawHeader("Effect installation");
            
        #if URP && PPS
            EditorGUILayout.HelpBox("\nBoth the Post-processing and URP packages are installed (through the Package Manager).\n\n" + "This combination is not possible, uninstall the Post-proccesing package.\n", MessageType.Error);
        #else
            
            var postProcessingInstalled = false;
            var urpInstalled = false;
            
            #if PPS
            postProcessingInstalled = true;
            #endif
            #if URP
            urpInstalled = true;
            #endif

            string buttonPrefix = "Unpack";
            if (Installer.ScriptPackages.PACKAGE_VERSION_STATE == Installer.ScriptPackages.PackageVersionState.Outdated) buttonPrefix = "Update";
            if (Installer.ScriptPackages.PACKAGE_INSTALL_STATE == Installer.ScriptPackages.PackageInstallState.None) buttonPrefix = "Install";
           
            using (new EditorGUILayout.VerticalScope(EditorStyles.textArea))
            {
                EditorGUILayout.Space();

                bool allowUnpacking = true;
                if (RenderPipelineInstallation.CurrentPipeline == RenderPipelineInstallation.Pipeline.BuiltIn)
                {
                    SCPE_GUI.Installation.DrawPostProcessing();

                    allowUnpacking = (PostProcessingInstallation.PPSVersionStatus == PostProcessingInstallation.VersionStatus.Compatible || PostProcessingInstallation.PPSVersionStatus == PostProcessingInstallation.VersionStatus.Outdated) && postProcessingInstalled;
                }

                if (RenderPipelineInstallation.CurrentPipeline == RenderPipelineInstallation.Pipeline.URP)
                {
                    SCPE_GUI.DrawStatusBox(new GUIContent("SRP Volume System", EditorGUIUtility.IconContent("Camera Gizmo").image), "Installed with URP", SCPE_GUI.Status.Ok);

                    allowUnpacking = RenderPipelineInstallation.VersionStatus == RenderPipelineInstallation.Version.Compatible && urpInstalled;
                }

                EditorGUI.BeginDisabledGroup(!allowUnpacking);
                {
                    SCPE_GUI.Installation.DrawScriptPackage();
                    
                    if (SCPE_GUI.DrawActionBox(buttonPrefix + " files", EditorGUIUtility.IconContent("SceneLoadIn").image))
                    {
                        if (RenderPipelineInstallation.CurrentPipeline == RenderPipelineInstallation.Pipeline.BuiltIn)
                        {
                            Installer.ScriptPackages.UnpackPPSScripts();
                        }
                        if (RenderPipelineInstallation.CurrentPipeline == RenderPipelineInstallation.Pipeline.URP)
                        {
                            Installer.ScriptPackages.UnpackURPFiles();
                        } }

                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();
            }
            
        #endif
            
            EditorGUILayout.Space();
        }

        public static void DrawDemoInstallSection()
        {
            var postProcessingInstalled = false;
            var urpInstalled = false;
            
            #if PPS
            postProcessingInstalled = true;
            #endif
            #if URP
            urpInstalled = true;
            #endif
            
            using (new EditorGUI.DisabledGroupScope(RenderPipelineInstallation.CurrentPipeline == RenderPipelineInstallation.Pipeline.BuiltIn && !postProcessingInstalled))
            {
                SCPE_GUI.Installation.DrawHeader("Demo content");
                
                using (new EditorGUILayout.VerticalScope(EditorStyles.textArea))
                {
                    EditorGUILayout.Space();

                    if (SCPE_GUI.DrawLabeledActionBox(new GUIContent("Sample textures", ""), "Unpack files", EditorGUIUtility.IconContent("SceneLoadIn").image))
                    {
                        Installer.Demo.InstallSamples();
                    }
                    EditorGUILayout.HelpBox("Contains textures, to be used with specific effects", MessageType.None);
                    
                    EditorGUI.BeginDisabledGroup(RenderPipelineInstallation.VersionStatus != RenderPipelineInstallation.Version.Compatible || urpInstalled);

                    if (SCPE_GUI.DrawLabeledActionBox(new GUIContent("Demo scenes", ""), "Unpack files", EditorGUIUtility.IconContent("SceneLoadIn").image))
                    {
                        Installer.Demo.InstallScenes();
                    }
                    EditorGUILayout.HelpBox("Scenes showcasing volume blending", MessageType.None);


                    EditorGUI.EndDisabledGroup();

                    #if URP
                    EditorGUILayout.HelpBox("Demo scenes aren't available for URP, due to both practical and technical constraints", MessageType.None);
                    #endif
                    
                    EditorGUILayout.Space();
                }
                
            }
        }
    }

}
