#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CustomEditor(typeof(ChapterManager))]
    public class ChapterManagerEditor : Editor
    {
        private GUISkin customSkin;
        private ChapterManager cmTarget;
        private int currentTab;

        private void OnEnable()
        {
            cmTarget = (ChapterManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            ReachEditorHandler.DrawComponentHeader(customSkin, "Chapters Top Header");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            currentTab = ReachEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab Resources")))
                currentTab = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 2;

            GUILayout.EndHorizontal();

            var chapters = serializedObject.FindProperty("chapters");
            var currentChapterIndex = serializedObject.FindProperty("currentChapterIndex");
            var onChapterPanelChanged = serializedObject.FindProperty("onChapterPanelChanged");

            var chapterPreset = serializedObject.FindProperty("chapterPreset");
            var chapterParent = serializedObject.FindProperty("chapterParent");
            var previousButton = serializedObject.FindProperty("previousButton");
            var nextButton = serializedObject.FindProperty("nextButton");
            var progressFill = serializedObject.FindProperty("progressFill");

            var showLockedChapters = serializedObject.FindProperty("showLockedChapters");
            var setPanelAuto = serializedObject.FindProperty("setPanelAuto");
            var checkChapterData = serializedObject.FindProperty("checkChapterData");
            var useLocalization = serializedObject.FindProperty("useLocalization");
            var backgroundStretch = serializedObject.FindProperty("backgroundStretch");
            var stretchCurveSpeed = serializedObject.FindProperty("stretchCurveSpeed");
            var stretchCurve = serializedObject.FindProperty("stretchCurve");
            var maxStretch = serializedObject.FindProperty("maxStretch");
            var barCurveSpeed = serializedObject.FindProperty("barCurveSpeed");
            var barCurve = serializedObject.FindProperty("barCurve");
            var animationSpeed = serializedObject.FindProperty("animationSpeed");

            switch (currentTab)
            {
                case 0:
                    ReachEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    if (cmTarget.chapters.Count != 0)
                    {
                        if (Application.isPlaying == true) { GUI.enabled = false; }
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.BeginHorizontal();
                        GUI.enabled = false;
                        EditorGUILayout.LabelField(new GUIContent("Current Chapter:"), customSkin.FindStyle("Text"), GUILayout.Width(94));
                        GUI.enabled = true;
                        
                        if (setPanelAuto.boolValue == true) { GUI.enabled = false; }
                        EditorGUILayout.LabelField(new GUIContent(cmTarget.chapters[currentChapterIndex.intValue].chapterID), customSkin.FindStyle("Text"));
                       
                        GUILayout.EndHorizontal();
                        GUILayout.Space(2);

                        currentChapterIndex.intValue = EditorGUILayout.IntSlider(currentChapterIndex.intValue, 0, cmTarget.chapters.Count - 1);

                        GUI.enabled = true;
                        if (setPanelAuto.boolValue == true) { EditorGUILayout.HelpBox("'Set Panel Automatically' is enabled. Current chapter will be set automatically based on default chapter states.", MessageType.Info); }
                        GUILayout.EndVertical();
                    }

                    else { EditorGUILayout.HelpBox("Chapter list is empty. Create a new item to see more options.", MessageType.Info); }

                    GUILayout.BeginVertical();
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.PropertyField(chapters, new GUIContent("Chapters"), true);
                    EditorGUI.indentLevel = 0;
                    GUILayout.EndVertical();

                    ReachEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onChapterPanelChanged, new GUIContent("On Chapter Panel Changed"), true);
                    break;

                case 1:
                    ReachEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    ReachEditorHandler.DrawProperty(chapterPreset, customSkin, "Chapter Preset");
                    ReachEditorHandler.DrawProperty(chapterParent, customSkin, "Chapter Parent");
                    ReachEditorHandler.DrawProperty(previousButton, customSkin, "Previous Button");
                    ReachEditorHandler.DrawProperty(nextButton, customSkin, "Next Button");
                    ReachEditorHandler.DrawProperty(progressFill, customSkin, "Progress Fill");
                    break;

                case 2:
                    ReachEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    showLockedChapters.boolValue = ReachEditorHandler.DrawToggle(showLockedChapters.boolValue, customSkin, "Show Locked Chapters");
                    setPanelAuto.boolValue = ReachEditorHandler.DrawToggle(setPanelAuto.boolValue, customSkin, "Set Panel Automatically");
                    checkChapterData.boolValue = ReachEditorHandler.DrawToggle(checkChapterData.boolValue, customSkin, "Check For Chapter Data");
                    useLocalization.boolValue = ReachEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization");

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-2);
                    GUILayout.BeginHorizontal();

                    backgroundStretch.boolValue = GUILayout.Toggle(backgroundStretch.boolValue, new GUIContent("Background Stretch"), customSkin.FindStyle("Toggle"));
                    backgroundStretch.boolValue = GUILayout.Toggle(backgroundStretch.boolValue, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
                    
                    GUILayout.EndHorizontal();
                    GUILayout.Space(4);

                    if (backgroundStretch.boolValue == true) 
                    {
                        ReachEditorHandler.DrawProperty(maxStretch, customSkin, "Max Stretch");
                        ReachEditorHandler.DrawProperty(stretchCurveSpeed, customSkin, "Curve Speed");
                        ReachEditorHandler.DrawProperty(stretchCurve, customSkin, "Stretch Curve");
                    }

                    GUILayout.EndVertical();

                    ReachEditorHandler.DrawProperty(barCurveSpeed, customSkin, "Bar Curve Speed");
                    ReachEditorHandler.DrawProperty(barCurve, customSkin, "Bar Curve");
                    ReachEditorHandler.DrawProperty(animationSpeed, customSkin, "Panel Speed");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif