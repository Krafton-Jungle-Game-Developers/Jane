#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SettingsElement))]
    public class SettingsElementEditor : Editor
    {
        private SettingsElement seTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            seTarget = (SettingsElement)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var highlightCG = serializedObject.FindProperty("highlightCG");
            var audioManager = serializedObject.FindProperty("audioManager");

            var isInteractable = serializedObject.FindProperty("isInteractable");
            var useSounds = serializedObject.FindProperty("useSounds");
            var useUINavigation = serializedObject.FindProperty("useUINavigation");
            var navigationMode = serializedObject.FindProperty("navigationMode");
            var selectOnUp = serializedObject.FindProperty("selectOnUp");
            var selectOnDown = serializedObject.FindProperty("selectOnDown");
            var selectOnLeft = serializedObject.FindProperty("selectOnLeft");
            var selectOnRight = serializedObject.FindProperty("selectOnRight");
            var wrapAround = serializedObject.FindProperty("wrapAround");
            var fadingMultiplier = serializedObject.FindProperty("fadingMultiplier");

            var onClick = serializedObject.FindProperty("onClick");
            var onHover = serializedObject.FindProperty("onHover");
            var onLeave = serializedObject.FindProperty("onLeave");

            ReachEditorHandler.DrawHeader(customSkin, "Core Header", 6);
            ReachEditorHandler.DrawProperty(highlightCG, customSkin, "Highlight CG");
            if (useSounds.boolValue == false) { GUI.enabled = false; }
            ReachEditorHandler.DrawProperty(audioManager, customSkin, "Audio Manager");
            GUI.enabled = true;

            ReachEditorHandler.DrawHeader(customSkin, "Options Header", 10);
            isInteractable.boolValue = ReachEditorHandler.DrawToggle(isInteractable.boolValue, customSkin, "Is Interactable");
            useSounds.boolValue = ReachEditorHandler.DrawToggle(useSounds.boolValue, customSkin, "Use Sounds");

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(-3);

            useUINavigation.boolValue = ReachEditorHandler.DrawTogglePlain(useUINavigation.boolValue, customSkin, "Use UI Navigation", "Enables controller navigation.");

            GUILayout.Space(4);

            if (useUINavigation.boolValue == true)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                ReachEditorHandler.DrawPropertyPlain(navigationMode, customSkin, "Navigation Mode");

                if (seTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Horizontal)
                {
                    EditorGUI.indentLevel = 1;
                    wrapAround.boolValue = ReachEditorHandler.DrawToggle(wrapAround.boolValue, customSkin, "Wrap Around");
                    EditorGUI.indentLevel = 0;
                }

                else if (seTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Vertical)
                {
                    wrapAround.boolValue = ReachEditorHandler.DrawTogglePlain(wrapAround.boolValue, customSkin, "Wrap Around");
                }

                else if (seTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Explicit)
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
            ReachEditorHandler.DrawProperty(fadingMultiplier, customSkin, "Fading Multiplier", "Set the animation fade multiplier.");

            ReachEditorHandler.DrawHeader(customSkin, "Events Header", 10);
            EditorGUILayout.PropertyField(onClick, new GUIContent("On Click"), true);
            EditorGUILayout.PropertyField(onHover, new GUIContent("On Hover"), true);
            EditorGUILayout.PropertyField(onLeave, new GUIContent("On Leave"), true);

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif