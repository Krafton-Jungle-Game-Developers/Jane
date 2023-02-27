#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HUDManager))]
    public class HUDManagerEditor : Editor
    {
        private HUDManager hmTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            hmTarget = (HUDManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var HUDPanel = serializedObject.FindProperty("HUDPanel");

            var fadeSpeed = serializedObject.FindProperty("fadeSpeed");
            var defaultBehaviour = serializedObject.FindProperty("defaultBehaviour");

            var onSetVisible = serializedObject.FindProperty("onSetVisible");
            var onSetInvisible = serializedObject.FindProperty("onSetInvisible");

            ReachEditorHandler.DrawHeader(customSkin, "Core Header", 6);
            ReachEditorHandler.DrawProperty(HUDPanel, customSkin, "HUD Panel");

            ReachEditorHandler.DrawHeader(customSkin, "Options Header", 10);
            ReachEditorHandler.DrawProperty(fadeSpeed, customSkin, "Fade Speed", "Sets the fade animation speed.");
            ReachEditorHandler.DrawProperty(defaultBehaviour, customSkin, "Default Behaviour");

            ReachEditorHandler.DrawHeader(customSkin, "Events Header", 10);
            EditorGUILayout.PropertyField(onSetVisible, new GUIContent("On Set Visible"), true);
            EditorGUILayout.PropertyField(onSetInvisible, new GUIContent("On Set Invisible"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif