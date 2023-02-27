#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ButtonManager))]
    public class ButtonManagerEditor : Editor
    {
        private ButtonManager buttonTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            buttonTarget = (ButtonManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            ReachEditorHandler.DrawComponentHeader(customSkin, "Button Top Header");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            buttonTarget.latestTabIndex = ReachEditorHandler.DrawTabs(buttonTarget.latestTabIndex, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                buttonTarget.latestTabIndex = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab Resources")))
                buttonTarget.latestTabIndex = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                buttonTarget.latestTabIndex = 2;

            GUILayout.EndHorizontal();

            var normalCG = serializedObject.FindProperty("normalCG");
            var highlightCG = serializedObject.FindProperty("highlightCG");
            var disabledCG = serializedObject.FindProperty("disabledCG");
            var audioManager = serializedObject.FindProperty("audioManager");
            var normalTextObj = serializedObject.FindProperty("normalTextObj");
            var highlightTextObj = serializedObject.FindProperty("highlightTextObj");
            var disabledTextObj = serializedObject.FindProperty("disabledTextObj");
            var normalImageObj = serializedObject.FindProperty("normalImageObj");
            var highlightImageObj = serializedObject.FindProperty("highlightImageObj");
            var disabledImageObj = serializedObject.FindProperty("disabledImageObj");

            var buttonIcon = serializedObject.FindProperty("buttonIcon");
            var buttonText = serializedObject.FindProperty("buttonText");
            var iconScale = serializedObject.FindProperty("iconScale");
            var textSize = serializedObject.FindProperty("textSize");

            var autoFitContent = serializedObject.FindProperty("autoFitContent");
            var padding = serializedObject.FindProperty("padding");
            var spacing = serializedObject.FindProperty("spacing");
            var disabledLayout = serializedObject.FindProperty("disabledLayout");
            var normalLayout = serializedObject.FindProperty("normalLayout");
            var highlightedLayout = serializedObject.FindProperty("highlightedLayout");
            var mainLayout = serializedObject.FindProperty("mainLayout");
            var mainFitter = serializedObject.FindProperty("mainFitter");
            var targetFitter = serializedObject.FindProperty("targetFitter");
            var targetRect = serializedObject.FindProperty("targetRect");

            var isInteractable = serializedObject.FindProperty("isInteractable");
            var enableIcon = serializedObject.FindProperty("enableIcon");
            var enableText = serializedObject.FindProperty("enableText");
            var useCustomIconSize = serializedObject.FindProperty("useCustomIconSize");
            var useCustomTextSize = serializedObject.FindProperty("useCustomTextSize");
            var useUINavigation = serializedObject.FindProperty("useUINavigation");
            var navigationMode = serializedObject.FindProperty("navigationMode");
            var wrapAround = serializedObject.FindProperty("wrapAround");
            var selectOnUp = serializedObject.FindProperty("selectOnUp");
            var selectOnDown = serializedObject.FindProperty("selectOnDown");
            var selectOnLeft = serializedObject.FindProperty("selectOnLeft");
            var selectOnRight = serializedObject.FindProperty("selectOnRight");
            var checkForDoubleClick = serializedObject.FindProperty("checkForDoubleClick");
            var useLocalization = serializedObject.FindProperty("useLocalization");
            var useSounds = serializedObject.FindProperty("useSounds");
            var doubleClickPeriod = serializedObject.FindProperty("doubleClickPeriod");
            var fadingMultiplier = serializedObject.FindProperty("fadingMultiplier");
            var useCustomContent = serializedObject.FindProperty("useCustomContent");

            var onClick = serializedObject.FindProperty("onClick");
            var onDoubleClick = serializedObject.FindProperty("onDoubleClick");
            var onHover = serializedObject.FindProperty("onHover");
            var onLeave = serializedObject.FindProperty("onLeave");

            switch (buttonTarget.latestTabIndex)
            {
                case 0:
                    ReachEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    if (useCustomContent.boolValue == false)
                    {
                        if (buttonTarget.normalImageObj != null || buttonTarget.highlightImageObj != null)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Space(-3);

                            enableIcon.boolValue = ReachEditorHandler.DrawTogglePlain(enableIcon.boolValue, customSkin, "Enable Icon");

                            GUILayout.Space(4);

                            if (enableIcon.boolValue == true)
                            {
                                ReachEditorHandler.DrawPropertyCW(buttonIcon, customSkin, "Button Icon", 80);
                                ReachEditorHandler.DrawPropertyCW(iconScale, customSkin, "Icon Scale", 80);
                                if (enableText.boolValue == true) { ReachEditorHandler.DrawPropertyCW(spacing, customSkin, "Spacing", 80); }
                            }

                            GUILayout.EndVertical();
                        }

                        if (buttonTarget.normalTextObj != null || buttonTarget.highlightTextObj != null)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Space(-3);

                            enableText.boolValue = ReachEditorHandler.DrawTogglePlain(enableText.boolValue, customSkin, "Enable Text");

                            GUILayout.Space(4);

                            if (enableText.boolValue == true)
                            {
                                ReachEditorHandler.DrawPropertyCW(buttonText, customSkin, "Button Text", 80);
                                if (useCustomTextSize.boolValue == false) { ReachEditorHandler.DrawPropertyCW(textSize, customSkin, "Text Size", 80); }
                            }

                            GUILayout.EndVertical();
                        }

                        if (Application.isPlaying == false) { buttonTarget.UpdateUI(); }
                    }

                    else { EditorGUILayout.HelpBox("'Use Custom Content' is enabled. Content is now managed manually.", MessageType.Info); }

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);

                    autoFitContent.boolValue = ReachEditorHandler.DrawTogglePlain(autoFitContent.boolValue, customSkin, "Auto-Fit Content", "Sets the width based on the button content.");

                    GUILayout.Space(4);

                    if (autoFitContent.boolValue == true)
                    {
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);
                        EditorGUI.indentLevel = 1;
                        EditorGUILayout.PropertyField(padding, new GUIContent(" Padding"), true);
                        EditorGUI.indentLevel = 0;
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.EndVertical();

                    isInteractable.boolValue = ReachEditorHandler.DrawToggle(isInteractable.boolValue, customSkin, "Is Interactable");

                    if (Application.isPlaying == true && GUILayout.Button("Update UI", customSkin.button)) { buttonTarget.UpdateUI(); }

                    ReachEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onClick, new GUIContent("On Click"), true);
                    EditorGUILayout.PropertyField(onDoubleClick, new GUIContent("On Double Click"), true);
                    EditorGUILayout.PropertyField(onHover, new GUIContent("On Hover"), true);
                    EditorGUILayout.PropertyField(onLeave, new GUIContent("On Leave"), true);
                    break;

                case 1:
                    ReachEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    ReachEditorHandler.DrawProperty(normalCG, customSkin, "Normal CG");
                    ReachEditorHandler.DrawProperty(highlightCG, customSkin, "Highlight CG");
                    ReachEditorHandler.DrawProperty(disabledCG, customSkin, "Disabled CG");

                    if (enableText.boolValue == true)
                    {
                        ReachEditorHandler.DrawProperty(normalTextObj, customSkin, "Normal Text");
                        ReachEditorHandler.DrawProperty(highlightTextObj, customSkin, "Highlighted Text");
                        ReachEditorHandler.DrawProperty(disabledTextObj, customSkin, "Disabled Text");
                    }

                    if (enableIcon.boolValue == true)
                    {
                        ReachEditorHandler.DrawProperty(normalImageObj, customSkin, "Normal Icon");
                        ReachEditorHandler.DrawProperty(highlightImageObj, customSkin, "Highlight Icon");
                        ReachEditorHandler.DrawProperty(disabledImageObj, customSkin, "Disabled Icon");
                    }

                    ReachEditorHandler.DrawProperty(disabledLayout, customSkin, "Disabled Layout");
                    ReachEditorHandler.DrawProperty(normalLayout, customSkin, "Normal Layout");
                    ReachEditorHandler.DrawProperty(highlightedLayout, customSkin, "Highlighted Layout");

                    if (autoFitContent.boolValue == true)
                    {
                        ReachEditorHandler.DrawProperty(mainLayout, customSkin, "Main Layout");
                        ReachEditorHandler.DrawProperty(mainFitter, customSkin, "Main Fitter");
                        ReachEditorHandler.DrawProperty(targetFitter, customSkin, "Target Fitter");
                        ReachEditorHandler.DrawProperty(targetRect, customSkin, "Target Rect");
                    }

                    if (useSounds.boolValue == true) { ReachEditorHandler.DrawProperty(audioManager, customSkin, "Audio Manager"); }
                    break;

                case 2:
                    ReachEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    ReachEditorHandler.DrawProperty(fadingMultiplier, customSkin, "Fading Multiplier", "Set the animation fade multiplier.");
                    ReachEditorHandler.DrawProperty(doubleClickPeriod, customSkin, "Double Click Period");
                    isInteractable.boolValue = ReachEditorHandler.DrawToggle(isInteractable.boolValue, customSkin, "Is Interactable");
                    useCustomContent.boolValue = ReachEditorHandler.DrawToggle(useCustomContent.boolValue, customSkin, "Use Custom Content", "Bypasses inspector values and allows manual editing.");
                    if (useCustomContent.boolValue == true || enableText.boolValue == false) { GUI.enabled = false; }
                    useCustomTextSize.boolValue = ReachEditorHandler.DrawToggle(useCustomTextSize.boolValue, customSkin, "Use Custom Text Size");
                    useLocalization.boolValue = ReachEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
                    GUI.enabled = true;
                    checkForDoubleClick.boolValue = ReachEditorHandler.DrawToggle(checkForDoubleClick.boolValue, customSkin, "Check For Double Click");
                    useSounds.boolValue = ReachEditorHandler.DrawToggle(useSounds.boolValue, customSkin, "Use Button Sounds");
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);

                    useUINavigation.boolValue = ReachEditorHandler.DrawTogglePlain(useUINavigation.boolValue, customSkin, "Use UI Navigation", "Enables controller navigation.");

                    GUILayout.Space(4);

                    if (useUINavigation.boolValue == true)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        ReachEditorHandler.DrawPropertyPlain(navigationMode, customSkin, "Navigation Mode");

                        if (buttonTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Horizontal)
                        {
                            EditorGUI.indentLevel = 1;
                            wrapAround.boolValue = ReachEditorHandler.DrawToggle(wrapAround.boolValue, customSkin, "Wrap Around");
                            EditorGUI.indentLevel = 0;
                        }

                        else if (buttonTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Vertical)
                        {
                            wrapAround.boolValue = ReachEditorHandler.DrawTogglePlain(wrapAround.boolValue, customSkin, "Wrap Around");
                        }

                        else if (buttonTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Explicit)
                        {
                            EditorGUI.indentLevel = 1;
                            ReachEditorHandler.DrawPropertyPlain(selectOnUp, customSkin, "Select On Up");
                            ReachEditorHandler.DrawPropertyPlain(selectOnDown, customSkin, "Select On Down");
                            ReachEditorHandler.DrawPropertyPlain(selectOnLeft, customSkin, "Select On Left");
                            ReachEditorHandler.DrawPropertyPlain(selectOnRight, customSkin, "Select On Right");
                            EditorGUI.indentLevel = 0;
                        }

                        GUILayout.EndVertical();
                    }

                    GUILayout.EndVertical();
                    buttonTarget.UpdateUI();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif