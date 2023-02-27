#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PauseMenuManager))]
    public class PauseMenuManagerEditor : Editor
    {
        private PauseMenuManager pmmTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            pmmTarget = (PauseMenuManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var pauseMenuCanvas = serializedObject.FindProperty("pauseMenuCanvas");
            var continueButton = serializedObject.FindProperty("continueButton");
            var panelManager = serializedObject.FindProperty("panelManager");
            var background = serializedObject.FindProperty("background");

            var setTimeScale = serializedObject.FindProperty("setTimeScale");
            var inputBlockDuration = serializedObject.FindProperty("inputBlockDuration");
            var menuCursorState = serializedObject.FindProperty("menuCursorState");
            var gameCursorState = serializedObject.FindProperty("gameCursorState");
            var hotkey = serializedObject.FindProperty("hotkey");

            var onOpen = serializedObject.FindProperty("onOpen");
            var onClose = serializedObject.FindProperty("onClose");

            if (pmmTarget.pauseMenuCanvas != null)
            {
                ReachEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                GUILayout.BeginHorizontal();

                if (Application.isPlaying == false)
                {
                    if (pmmTarget.pauseMenuCanvas.gameObject.activeSelf == false && GUILayout.Button("Show Pause Menu", customSkin.button))
                    {
                        pmmTarget.pauseMenuCanvas.gameObject.SetActive(true);
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }

                    else if (pmmTarget.pauseMenuCanvas.gameObject.activeSelf == true && GUILayout.Button("Hide Pause Menu", customSkin.button))
                    {
                        pmmTarget.pauseMenuCanvas.gameObject.SetActive(false);
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                }

                if (GUILayout.Button("Select Object", customSkin.button)) { Selection.activeObject = pmmTarget.pauseMenuCanvas; }
                GUILayout.EndHorizontal();
            }

            ReachEditorHandler.DrawHeader(customSkin, "Core Header", 10);
            ReachEditorHandler.DrawProperty(pauseMenuCanvas, customSkin, "Pause Canvas");
            ReachEditorHandler.DrawProperty(continueButton, customSkin, "Continue Button");
            ReachEditorHandler.DrawProperty(panelManager, customSkin, "Panel Manager");
            ReachEditorHandler.DrawProperty(background, customSkin, "Background");

            ReachEditorHandler.DrawHeader(customSkin, "Options Header", 10);
            setTimeScale.boolValue = ReachEditorHandler.DrawToggle(setTimeScale.boolValue, customSkin, "Set Time Scale", "Sets the time scale depending on the pause menu state.");
            ReachEditorHandler.DrawProperty(inputBlockDuration, customSkin, "Input Block Duration", "Block input in specific amount of time to provide smooth visuals.");
            ReachEditorHandler.DrawProperty(menuCursorState, customSkin, "Menu Cursor State");
            ReachEditorHandler.DrawProperty(gameCursorState, customSkin, "Game Cursor State");
            EditorGUILayout.PropertyField(hotkey, new GUIContent("Hotkey"), true);

            ReachEditorHandler.DrawHeader(customSkin, "Events Header", 10);
            EditorGUILayout.PropertyField(onOpen, new GUIContent("On Open"), true);
            EditorGUILayout.PropertyField(onClose, new GUIContent("On Close"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif