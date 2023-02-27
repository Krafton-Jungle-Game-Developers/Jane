#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LobbyPlayer))]
    public class LobbyPlayerEditor : Editor
    {
        private LobbyPlayer lpTarget;
        private GUISkin customSkin;
        public bool showResources;

        private void OnEnable()
        {
            lpTarget = (LobbyPlayer)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var playerPicture = serializedObject.FindProperty("playerPicture");
            var playerName = serializedObject.FindProperty("playerName");
            var additionalText = serializedObject.FindProperty("additionalText");
            var currentState = serializedObject.FindProperty("currentState");

            var emptyParent = serializedObject.FindProperty("emptyParent");
            var readyParent = serializedObject.FindProperty("readyParent");
            var notReadyParent = serializedObject.FindProperty("notReadyParent");
            var playerIndicatorReady = serializedObject.FindProperty("playerIndicatorReady");
            var playerIndicatorNotReady = serializedObject.FindProperty("playerIndicatorNotReady");
            var pictureReadyImg = serializedObject.FindProperty("pictureReadyImg");
            var pictureNotReadyImg = serializedObject.FindProperty("pictureNotReadyImg");
            var nameReadyTMP = serializedObject.FindProperty("nameReadyTMP");
            var nameNotReadyTMP = serializedObject.FindProperty("nameNotReadyTMP");
            var adtReadyTMP = serializedObject.FindProperty("adtReadyTMP");
            var adtNotReadyTMP = serializedObject.FindProperty("adtNotReadyTMP");

            var onEmpty = serializedObject.FindProperty("onEmpty");
            var onReady = serializedObject.FindProperty("onReady");
            var onUnready = serializedObject.FindProperty("onUnready");

            ReachEditorHandler.DrawHeader(customSkin, "Options Header", 6);
            ReachEditorHandler.DrawProperty(playerPicture, customSkin, "Player Picture");
            ReachEditorHandler.DrawProperty(playerName, customSkin, "Player Name");
            ReachEditorHandler.DrawProperty(additionalText, customSkin, "Additional Text");
            ReachEditorHandler.DrawProperty(currentState, customSkin, "Current State");

            ReachEditorHandler.DrawHeader(customSkin, "Core Header", 10);
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            showResources = GUILayout.Toggle(showResources, new GUIContent("Show Resources", "Current state: " + showResources.ToString()), customSkin.FindStyle("Toggle"));
            showResources = GUILayout.Toggle(showResources, new GUIContent("", "Current state: " + showResources.ToString()), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();

            if (showResources == true)
            {
                ReachEditorHandler.DrawProperty(emptyParent, customSkin, "Empty Parent");
                ReachEditorHandler.DrawProperty(readyParent, customSkin, "Ready Parent");
                ReachEditorHandler.DrawProperty(notReadyParent, customSkin, "Not Ready Parent");
                ReachEditorHandler.DrawProperty(playerIndicatorReady, customSkin, "Indicator Ready");
                ReachEditorHandler.DrawProperty(playerIndicatorNotReady, customSkin, "Indicator Not Ready");
                ReachEditorHandler.DrawProperty(pictureReadyImg, customSkin, "Picture Ready");
                ReachEditorHandler.DrawProperty(pictureNotReadyImg, customSkin, "Picture Not Ready");
                ReachEditorHandler.DrawProperty(nameReadyTMP, customSkin, "Name Ready");
                ReachEditorHandler.DrawProperty(nameNotReadyTMP, customSkin, "Name Not Ready");
                ReachEditorHandler.DrawProperty(adtReadyTMP, customSkin, "Adt. Ready");
                ReachEditorHandler.DrawProperty(adtNotReadyTMP, customSkin, "Adt. Not Ready");
            }

            ReachEditorHandler.DrawHeader(customSkin, "Events Header", 10);
            EditorGUILayout.PropertyField(onEmpty, new GUIContent("On Empty"), true);
            EditorGUILayout.PropertyField(onReady, new GUIContent("On Ready"), true);
            EditorGUILayout.PropertyField(onUnready, new GUIContent("On Unready"), true);

            if (Application.isPlaying == false) { Repaint(); }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif