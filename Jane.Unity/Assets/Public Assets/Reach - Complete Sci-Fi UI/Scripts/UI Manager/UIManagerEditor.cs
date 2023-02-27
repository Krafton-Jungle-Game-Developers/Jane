#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;

namespace Michsky.UI.Reach
{
    [CustomEditor(typeof(UIManager))]
    [System.Serializable]
    public class UIManagerEditor : Editor
    {
        GUISkin customSkin;
        private UIManager uimTarget;

        protected static float foldoutItemSpace = 2;
        protected static float foldoutTopSpace = 5;
        protected static float foldoutBottomSpace = 2;

        protected static bool showAch = false;
        protected static bool showAudio = false;
        protected static bool showColors = false;
        protected static bool showFonts = false;
        protected static bool showLocalization = false;
        protected static bool showLogo = false;
        protected static bool showSplashScreen = false;

        private void OnEnable()
        {
            uimTarget = (UIManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            if (customSkin == null)
            {
                EditorGUILayout.HelpBox("Editor resources are missing. You can manually fix this by deleting " +
                    "Reach UI > Editor folder and then re-import the package. \n\nIf you're still seeing this " +
                    "dialog even after the re-import, contact me with this ID: " + UIManager.buildID, MessageType.Error);

                if (GUILayout.Button("Contact")) { Email(); }
                return;
            }

            // Foldout style
            GUIStyle foldoutStyle = customSkin.FindStyle("UIM Foldout");

            // UIM Header
            ReachEditorHandler.DrawHeader(customSkin, "UIM Header", 8);

            #region Achievements
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showAch = EditorGUILayout.Foldout(showAch, "Achievements", true, foldoutStyle);
            showAch = GUILayout.Toggle(showAch, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showAch)
            {
                var achievementLibrary = serializedObject.FindProperty("achievementLibrary");
                var commonColor = serializedObject.FindProperty("commonColor");
                var rareColor = serializedObject.FindProperty("rareColor");
                var legendaryColor = serializedObject.FindProperty("legendaryColor");

                ReachEditorHandler.DrawProperty(achievementLibrary, customSkin, "Achievement Library");
                if (uimTarget.achievementLibrary != null && GUILayout.Button("Show Library", customSkin.button)) { Selection.activeObject = uimTarget.achievementLibrary; }
                ReachEditorHandler.DrawProperty(commonColor, customSkin, "Common Color");
                ReachEditorHandler.DrawProperty(rareColor, customSkin, "Rare Color");
                ReachEditorHandler.DrawProperty(legendaryColor, customSkin, "Legendary Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Audio
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showAudio = EditorGUILayout.Foldout(showAudio, "Audio", true, foldoutStyle);
            showAudio = GUILayout.Toggle(showAudio, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showAudio)
            {
                var hoverSound = serializedObject.FindProperty("hoverSound");
                var clickSound = serializedObject.FindProperty("clickSound");

                ReachEditorHandler.DrawProperty(hoverSound, customSkin, "Hover Sound");
                ReachEditorHandler.DrawProperty(clickSound, customSkin, "Click Sound");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Colors
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showColors = EditorGUILayout.Foldout(showColors, "Colors", true, foldoutStyle);
            showColors = GUILayout.Toggle(showColors, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showColors)
            {
                var accentColor = serializedObject.FindProperty("accentColor");
                var accentColorInvert = serializedObject.FindProperty("accentColorInvert");
                var primaryColor = serializedObject.FindProperty("primaryColor");
                var secondaryColor = serializedObject.FindProperty("secondaryColor");
                var negativeColor = serializedObject.FindProperty("negativeColor");
                var backgroundColor = serializedObject.FindProperty("backgroundColor");
                var altBackgroundColor = serializedObject.FindProperty("altBackgroundColor");

                ReachEditorHandler.DrawProperty(accentColor, customSkin, "Accent Color");
                ReachEditorHandler.DrawProperty(accentColorInvert, customSkin, "Accent Match");
                ReachEditorHandler.DrawProperty(primaryColor, customSkin, "Primary Color");
                ReachEditorHandler.DrawProperty(secondaryColor, customSkin, "Secondary Color");
                ReachEditorHandler.DrawProperty(negativeColor, customSkin, "Negative Color");
                ReachEditorHandler.DrawProperty(backgroundColor, customSkin, "Background Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Fonts
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showFonts = EditorGUILayout.Foldout(showFonts, "Fonts", true, foldoutStyle);
            showFonts = GUILayout.Toggle(showFonts, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showFonts)
            {
                var fontLight = serializedObject.FindProperty("fontLight");
                var fontRegular = serializedObject.FindProperty("fontRegular");
                var fontMedium = serializedObject.FindProperty("fontMedium");
                var fontSemiBold = serializedObject.FindProperty("fontSemiBold");
                var fontBold = serializedObject.FindProperty("fontBold");
                var customFont = serializedObject.FindProperty("customFont");

                ReachEditorHandler.DrawProperty(fontLight, customSkin, "Light Font");
                ReachEditorHandler.DrawProperty(fontRegular, customSkin, "Regular Font");
                ReachEditorHandler.DrawProperty(fontMedium, customSkin, "Medium Font");
                ReachEditorHandler.DrawProperty(fontSemiBold, customSkin, "Semibold Font");
                ReachEditorHandler.DrawProperty(fontBold, customSkin, "Bold Font");
                ReachEditorHandler.DrawProperty(customFont, customSkin, "Custom Font");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Localization
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showLocalization = EditorGUILayout.Foldout(showLocalization, "Localization", true, foldoutStyle);
            showLocalization = GUILayout.Toggle(showLocalization, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showLocalization)
            {
                var enableLocalization = serializedObject.FindProperty("enableLocalization");
                var localizationSettings = serializedObject.FindProperty("localizationSettings");

                enableLocalization.boolValue = ReachEditorHandler.DrawToggle(enableLocalization.boolValue, customSkin, "Enable Localization (Beta)");

                if (enableLocalization.boolValue == true)
                {
                    ReachEditorHandler.DrawPropertyCW(localizationSettings, customSkin, "Localization Settings", 130);
                    
                    if (uimTarget.localizationSettings != null)
                    {
                        if (GUILayout.Button("Open Localization Settings", customSkin.button)) { Selection.activeObject = uimTarget.localizationSettings; }
                        EditorGUILayout.HelpBox("Localization is enabled. You can use the localization settings asset to manage localization.", MessageType.Info);
                    }
                    else { EditorGUILayout.HelpBox("Localization is enabled, but 'Localization Settings' is missing.", MessageType.Warning); }
                }
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Logo
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showLogo = EditorGUILayout.Foldout(showLogo, "Logo", true, foldoutStyle);
            showLogo = GUILayout.Toggle(showLogo, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showLogo)
            {
                var brandLogo = serializedObject.FindProperty("brandLogo");
                var brandLogoColor = serializedObject.FindProperty("brandLogoColor");
                var gameLogo = serializedObject.FindProperty("gameLogo");
                var gameLogoColor = serializedObject.FindProperty("gameLogoColor");

                ReachEditorHandler.DrawProperty(brandLogo, customSkin, "Brand Logo");
                ReachEditorHandler.DrawProperty(brandLogoColor, customSkin, "Brand Logo Color");
                ReachEditorHandler.DrawProperty(gameLogo, customSkin, "Game Logo");
                ReachEditorHandler.DrawProperty(gameLogoColor, customSkin, "Game Logo Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Splash Screen
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showSplashScreen = EditorGUILayout.Foldout(showSplashScreen, "Splash Screen", true, foldoutStyle);
            showSplashScreen = GUILayout.Toggle(showSplashScreen, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showSplashScreen)
            {
                var enableSplashScreen = serializedObject.FindProperty("enableSplashScreen");
                var pakType = serializedObject.FindProperty("pakType");
                var pakText = serializedObject.FindProperty("pakText");
                var pakLocalizationText = serializedObject.FindProperty("pakLocalizationText");

                enableSplashScreen.boolValue = ReachEditorHandler.DrawToggle(enableSplashScreen.boolValue, customSkin, "Enable Splash Screen");

                if (enableSplashScreen.boolValue == false) { GUI.enabled = false; }
                ReachEditorHandler.DrawProperty(pakType, customSkin, "Press Any Key Type");

                if (pakType.enumValueIndex == 0)
                {
                    if (uimTarget.enableLocalization == true) 
                    {
                        ReachEditorHandler.DrawProperty(pakLocalizationText, customSkin, "Press Any Key Text");
                        EditorGUILayout.HelpBox("Localization formatting: {StringKey}" + "\nDefault: PAK_Part1 {PAK_Key} PAK_Part3"
                            + "\nDefault output: Press [Any Key] To Start", MessageType.Info); 
                    }

                    else
                    {
                        ReachEditorHandler.DrawProperty(pakText, customSkin, "Press Any Key Text");
                        EditorGUILayout.HelpBox("Formatting: {Key Text}" + "\nSample: Press {Any Key} To Start", MessageType.Info);
                    }
                }
            }

            GUI.enabled = true;
            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Settings
            ReachEditorHandler.DrawHeader(customSkin, "Options Header", 14);

            var enableDynamicUpdate = serializedObject.FindProperty("enableDynamicUpdate");
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(-2);
            GUILayout.BeginHorizontal();
            enableDynamicUpdate.boolValue = GUILayout.Toggle(enableDynamicUpdate.boolValue, new GUIContent("Enable Dynamic Update"), customSkin.FindStyle("Toggle"));
            enableDynamicUpdate.boolValue = GUILayout.Toggle(enableDynamicUpdate.boolValue, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(4);

            if (enableDynamicUpdate.boolValue == true)
            {
                EditorGUILayout.HelpBox("When this option is enabled, all objects connected to this manager will be dynamically updated synchronously. " +
                    "Basically; consumes more resources, but allows dynamic changes at runtime/editor.", MessageType.Info);
            }

            else
            {
                EditorGUILayout.HelpBox("When this option is disabled, all objects connected to this manager will be updated only once on awake. " +
                    "Basically; has better performance, but it's static.", MessageType.Info);
            }

            GUILayout.EndVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset to defaults", customSkin.button)) { ResetToDefaults(); }
            GUILayout.EndHorizontal();
            #endregion

            #region Integrations
            ReachEditorHandler.DrawHeader(customSkin, "Integrations Header", 16);
            if (GUILayout.Button("Assembly Definition Patch", customSkin.button)) { Application.OpenURL("https://docs.michsky.com/docs/reach-ui/others/"); }
            #endregion

            #region Support
            ReachEditorHandler.DrawHeader(customSkin, "Support Header", 16);
            GUILayout.BeginVertical();
         
            GUILayout.BeginHorizontal();
            GUILayout.Label("Need help? Contact me via:", customSkin.FindStyle("Text"));
            GUILayout.EndHorizontal();
          
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Discord", customSkin.button)) { Discord(); }
            if (GUILayout.Button("Twitter", customSkin.button)) { Twitter(); }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Documentation", customSkin.button)) { Docs(); }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("E-mail", customSkin.button)) { Email(); }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("ID: " + UIManager.buildID);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(6);
            #endregion

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }

        void Discord() { Application.OpenURL("https://discord.gg/VXpHyUt"); }
        void Docs() { Application.OpenURL("https://docs.michsky.com/docs/reach-ui/"); }
        void Email() { Application.OpenURL("https://www.michsky.com/contact/"); }
        void Twitter() { Application.OpenURL("https://www.twitter.com/michskyHQ"); }

        void ResetToDefaults()
        {
            if (EditorUtility.DisplayDialog("Reset to defaults", "Are you sure you want to reset UI Manager values to default?", "Yes", "Cancel"))
            {
                try
                {
                    Preset defaultPreset = Resources.Load<Preset>("RUIM Presets/Default");
                    defaultPreset.ApplyTo(Resources.Load("Reach UI Manager"));
                   
                    Selection.activeObject = null;
                    Selection.activeObject = Resources.Load("Reach UI Manager");
                   
                    Debug.Log("<b>[UI Manager]</b> Resetting successful.");
                }

                catch { Debug.LogWarning("<b>[UI Manager]</b> Resetting failed. Default preset seems to be missing."); }
            }
        }
    }
}
#endif