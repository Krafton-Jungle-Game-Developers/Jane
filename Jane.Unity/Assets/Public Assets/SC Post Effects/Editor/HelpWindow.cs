// SC Post Effects
// Staggart Creations
// http://staggart.xyz

using System;
using UnityEngine;
using UnityEditor;
#if PPS
using UnityEngine.Rendering.PostProcessing;
#endif

namespace SCPE
{
    public class HelpWindow : EditorWindow
    {
        public static bool blur;

        [MenuItem("Window/SC Post Effects", false, 0)]
        public static void ExecuteMenuItem()
        {
            HelpWindow.ShowWindow();
        }

        //Window properties
        private static int width = 450;
        private static int height = 500;

        private enum Tabs
        {
            Installation,
            Support
        }
        private Tabs tabID;

        //Check if latest version has been pulled from backend and package manager
        private static bool installationVerified;

        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow<HelpWindow>(true, "Asset Window", true);

            //Open somewhat in the center of the screen
            editorWindow.position = new Rect((Screen.currentResolution.width / 2) - (width * 0.5f), 150f, width, height);

            //Fixed size
            editorWindow.maxSize = new Vector2(width, height);
            editorWindow.minSize = new Vector2(width, height);

            editorWindow.Show();
        }
        
        //Store values in the volatile SessionState
        static void InitInstallation()
        {
            Installer.Initialize();
            installationVerified = true;
        }

        private Vector2 scrollPos;

        private void OnEnable()
        {
            Installer.Initialize();
            installationVerified = true;
        }

        void OnGUI()
        {
            var rect = EditorGUILayout.BeginVertical();
            
            DrawHeader();

            DrawTabs();
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            {
                if (tabID == Tabs.Installation)
                {
                    if (!installationVerified) InitInstallation();

                    EditorGUILayout.Space();

                    InstallerWindow.DrawInstallationScreen();

                    EditorGUILayout.Space();
                }

                if (tabID == Tabs.Support) DrawSupport();
            }
            EditorGUILayout.EndScrollView();
            
            GUILayout.Space(-5f);
            
            EditorGUILayout.LabelField("", UnityEngine.GUI.skin.horizontalSlider);

            DrawActionButtons();

            DrawFooter();
            
                            
            var prevColor = GUI.color;
            var prevBgColor = GUI.backgroundColor;
            
            if (blur)
            {
                rect.width = HelpWindow.width;
                rect.height = HelpWindow.height;
                
                GUI.color = new Color(0,0,0, 0.66f);

                //Background
                EditorGUI.DrawRect(rect, GUI.color);
            }
            
            GUI.color = prevColor;
            GUI.backgroundColor = prevBgColor;
            
            EditorGUILayout.EndVertical();
        }

        void DrawHeader()
        {
            SCPE_GUI.DrawWindowHeader(width, height);

            //GUILayout.Label("Version: " + SCPE.INSTALLED_VERSION, SCPE_GUI.Footer);
            //GUILayout.Space(5);
        }

        void DrawTabs()
        {
            GUIContent[] content = new GUIContent[]
            {
                new GUIContent(" Installation", EditorGUIUtility.IconContent("Assembly Icon").image), 
                //new GUIContent("  Quick setup", EditorGUIUtility.IconContent("Prefab Icon").image), 
                new GUIContent("  Support", EditorGUIUtility.IconContent("PointLight Gizmo").image)
            };
            tabID = (Tabs)GUILayout.Toolbar((int)tabID, content, GUILayout.MaxHeight(27f));
        }

        void DrawSupport()
        {
            EditorGUILayout.HelpBox("Please view the documentation for further details about this package and its workings.", MessageType.Info);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("<b><size=12>Documentation</size></b>\n<i>Usage instructions</i>", SCPE_GUI.Button))
                {
                    Application.OpenURL(SCPE.DOC_URL);
                }
                if (GUILayout.Button("<b><size=12>Effect details</size></b>\n<i>View effect examples</i>", SCPE_GUI.Button))
                {
                    Application.OpenURL(SCPE.DOC_URL + "?section=effects");
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.HelpBox("\nIf you have any questions, or ran into issues, please get in touch.\n", MessageType.Info);

            EditorGUILayout.Space();

            //Buttons box
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("<b><size=12>Email</size></b>\n<i>Contact</i>", SCPE_GUI.Button))
                {
                    Application.OpenURL("mailto:contact@staggart.xyz");
                }
                if (GUILayout.Button("<b><size=12>Twitter</size></b>\n<i>Follow developments</i>", SCPE_GUI.Button))
                {
                    Application.OpenURL("https://twitter.com/search?q=staggart%20creations");
                }
                if (GUILayout.Button("<b><size=12>Forum</size></b>\n<i>Join the discussion</i>", SCPE_GUI.Button))
                {
                    Application.OpenURL(SCPE.FORUM_URL);
                }
            }
            EditorGUILayout.EndHorizontal();

        }


        private void DrawActionButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(new GUIContent("<b><size=12>Quick setup</size></b>\n<i>Enable post processing</i>", EditorGUIUtility.IconContent("Prefab Icon").image), SCPE_GUI.Button, GUILayout.Height(45f)))
                {
                    QuickSetupWindow.ShowWindow();
                }
                    
                if (GUILayout.Button(new GUIContent("<b><size=12>Asset store</size></b>\n<i>Write a review</i>", EditorGUIUtility.IconContent("Favorite Icon").image), SCPE_GUI.Button, GUILayout.Height(45f))) SCPE.OpenStorePage();
            }
        }

        private void DrawFooter()
        {
            EditorGUILayout.Space();
            GUILayout.Label("- Staggart Creations -", SCPE_GUI.Footer);
            EditorGUILayout.Space();

        }
        

    }//SCPE_Window Class

    public class QuickSetupWindow : EditorWindow
    {
        private const int width = 400;
        private const int height = 100;
            
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow<QuickSetupWindow>(true, "Quick setup", true);

            //Open somewhat in the center of the screen
            editorWindow.position = new Rect((Screen.currentResolution.width / 2) - (width * 0.5f), (Screen.currentResolution.height / 2), width, height);

            //Fixed size
            editorWindow.maxSize = new Vector2(width, height);
            editorWindow.minSize = new Vector2(width, height);

            editorWindow.Show();

            HelpWindow.blur = true;
        }

        private void OnDestroy()
        {
            HelpWindow.blur = false;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            var needCamSetup = true;
            
            Camera mainCamera = (Camera.main) ? Camera.main : GameObject.FindObjectOfType<Camera>();
            #if PPS

            if (mainCamera)
            {
                PostProcessLayer layer = mainCamera.GetComponent<PostProcessLayer>();

                if (layer) needCamSetup = false;
            }
            #endif
            
            #if URP
            if (mainCamera)
            {
                UnityEngine.Rendering.Universal.UniversalAdditionalCameraData data = mainCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                if (data)
                {
                    needCamSetup = !data.renderPostProcessing;
                }
            }
            #endif
            
            string pipelineText = needCamSetup ? "Needs setup" : "Post processing enabled";
            SCPE_GUI.Status compatibilityStatus = needCamSetup ? SCPE_GUI.Status.Warning : SCPE_GUI.Status.Ok;
            
            SCPE_GUI.DrawStatusBox(new GUIContent("Camera set up", EditorGUIUtility.IconContent("d_Profiler.Rendering").image), pipelineText, compatibilityStatus);

            if (needCamSetup)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (SCPE_GUI.DrawActionBox("Enable post-processing", EditorGUIUtility.IconContent("SceneLoadIn").image))
                    {
                        AutoSetup.SetupCamera();
                    }
                }
            }
            
            //Volume setup
            EditorGUILayout.BeginHorizontal();
            {
                if (SCPE_GUI.DrawLabeledActionBox(new GUIContent("Global Post Processing volume"), "Create", EditorGUIUtility.IconContent("SceneLoadIn").image))
                {
                    AutoSetup.SetupGlobalVolume();
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
        }
    }
}
