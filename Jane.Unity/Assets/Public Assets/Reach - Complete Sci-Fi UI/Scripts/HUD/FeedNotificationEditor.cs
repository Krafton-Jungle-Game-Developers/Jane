#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FeedNotification))]
    public class FeedNotificationHeader : Editor
    {
        private FeedNotification fnTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            fnTarget = (FeedNotification)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var icon = serializedObject.FindProperty("icon");
            var notificationText = serializedObject.FindProperty("notificationText");
            var localizationKey = serializedObject.FindProperty("localizationKey");

            var itemAnimator = serializedObject.FindProperty("itemAnimator");
            var iconObj = serializedObject.FindProperty("iconObj");
            var textObj = serializedObject.FindProperty("textObj");

            var useLocalization = serializedObject.FindProperty("useLocalization");
            var updateOnAnimate = serializedObject.FindProperty("updateOnAnimate");
            var minimizeAfter = serializedObject.FindProperty("minimizeAfter");
            var defaultState = serializedObject.FindProperty("defaultState");
            var afterMinimize = serializedObject.FindProperty("afterMinimize");

            var onDestroy = serializedObject.FindProperty("onDestroy");

            ReachEditorHandler.DrawHeader(customSkin, "Content Header", 6);
            ReachEditorHandler.DrawProperty(icon, customSkin, "Icon");
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Notification Text"), customSkin.FindStyle("Text"), GUILayout.Width(-3));
            EditorGUILayout.PropertyField(notificationText, new GUIContent(""), GUILayout.Height(70));
            GUILayout.EndHorizontal();
            ReachEditorHandler.DrawProperty(localizationKey, customSkin, "Localization Key");

            ReachEditorHandler.DrawHeader(customSkin, "Core Header", 10);
            ReachEditorHandler.DrawProperty(itemAnimator, customSkin, "Animator");
            ReachEditorHandler.DrawProperty(iconObj, customSkin, "Icon Object");
            ReachEditorHandler.DrawProperty(textObj, customSkin, "Text Object");

            ReachEditorHandler.DrawHeader(customSkin, "Options Header", 10);
            useLocalization.boolValue = ReachEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
            updateOnAnimate.boolValue = ReachEditorHandler.DrawToggle(updateOnAnimate.boolValue, customSkin, "Update On Animate");
            ReachEditorHandler.DrawProperty(minimizeAfter, customSkin, "Minimize After");
            ReachEditorHandler.DrawProperty(defaultState, customSkin, "Default State");
            ReachEditorHandler.DrawProperty(afterMinimize, customSkin, "After Minimize");

            ReachEditorHandler.DrawHeader(customSkin, "Events Header", 10);
            EditorGUILayout.PropertyField(onDestroy, new GUIContent("On Destroy"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif