#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CustomEditor(typeof(UIManagerText))]
    public class UIManagerTextEditor : Editor
    {
        private UIManagerText uimtTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            uimtTarget = (UIManagerText)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var UIManagerAsset = serializedObject.FindProperty("UIManagerAsset");
            var fontType = serializedObject.FindProperty("fontType");
            var colorType = serializedObject.FindProperty("colorType");
            var useCustomColor = serializedObject.FindProperty("useCustomColor");
            var useCustomAlpha = serializedObject.FindProperty("useCustomAlpha");
            var useCustomFont = serializedObject.FindProperty("useCustomFont");

            ReachEditorHandler.DrawHeader(customSkin, "Core Header", 6);
            ReachEditorHandler.DrawProperty(UIManagerAsset, customSkin, "UI Manager");

            ReachEditorHandler.DrawHeader(customSkin, "Options Header", 10);

            if (uimtTarget.UIManagerAsset != null)
            {
                if (useCustomFont.boolValue == true) { GUI.enabled = false; }
                ReachEditorHandler.DrawProperty(fontType, customSkin, "Font Type");
                GUI.enabled = true;
                ReachEditorHandler.DrawProperty(colorType, customSkin, "Color Type");
                useCustomColor.boolValue = ReachEditorHandler.DrawToggle(useCustomColor.boolValue, customSkin, "Use Custom Color");
                if (useCustomColor.boolValue == true) { GUI.enabled = false; }
                useCustomAlpha.boolValue = ReachEditorHandler.DrawToggle(useCustomAlpha.boolValue, customSkin, "Use Custom Alpha");
                GUI.enabled = true;
                useCustomFont.boolValue = ReachEditorHandler.DrawToggle(useCustomFont.boolValue, customSkin, "Use Custom Font");
            }

            else { EditorGUILayout.HelpBox("UI Manager should be assigned.", MessageType.Error); }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif