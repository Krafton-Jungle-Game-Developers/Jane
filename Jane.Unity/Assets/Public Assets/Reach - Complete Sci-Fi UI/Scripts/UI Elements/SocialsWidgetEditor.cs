#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SocialsWidget))]
    public class SocialsWidgetEditor : Editor
    {
        private SocialsWidget swTarget;
        private GUISkin customSkin;
        private int currentTab;

        private void OnEnable()
        {
            swTarget = (SocialsWidget)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            ReachEditorHandler.DrawComponentHeader(customSkin, "Socials Widget Top Header");

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

            var socials = serializedObject.FindProperty("socials");

            var itemPreset = serializedObject.FindProperty("itemPreset");
            var itemParent = serializedObject.FindProperty("itemParent");
            var buttonPreset = serializedObject.FindProperty("buttonPreset");
            var buttonParent = serializedObject.FindProperty("buttonParent");
            var background = serializedObject.FindProperty("background");

            var useLocalization = serializedObject.FindProperty("useLocalization");
            var timer = serializedObject.FindProperty("timer");
            var tintSpeed = serializedObject.FindProperty("tintSpeed");
            var tintCurve = serializedObject.FindProperty("tintCurve");

            switch (currentTab)
            {
                case 0:
                    ReachEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.PropertyField(socials, new GUIContent("Socials"), true);
                    EditorGUI.indentLevel = 0;
                    break;

                case 1:
                    ReachEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    ReachEditorHandler.DrawProperty(itemPreset, customSkin, "Item Preset");
                    ReachEditorHandler.DrawProperty(itemParent, customSkin, "Item Parent");
                    ReachEditorHandler.DrawProperty(buttonPreset, customSkin, "Button Preset");
                    ReachEditorHandler.DrawProperty(buttonParent, customSkin, "Button Parent");
                    ReachEditorHandler.DrawProperty(background, customSkin, "Background");
                    break;

                case 2:
                    ReachEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    useLocalization.boolValue = ReachEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
                    ReachEditorHandler.DrawProperty(timer, customSkin, "Timer");
                    ReachEditorHandler.DrawProperty(tintSpeed, customSkin, "Tint Speed");
                    ReachEditorHandler.DrawProperty(tintCurve, customSkin, "Tint Curve");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif