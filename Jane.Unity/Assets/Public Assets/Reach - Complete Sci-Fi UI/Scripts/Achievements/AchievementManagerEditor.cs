#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AchievementManager))]
    public class AchievementManagerEditor : Editor
    {
        private AchievementManager amTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            amTarget = (AchievementManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var UIManagerAsset = serializedObject.FindProperty("UIManagerAsset");
            var allParent = serializedObject.FindProperty("allParent");
            var commonParent = serializedObject.FindProperty("commonParent");
            var rareParent = serializedObject.FindProperty("rareParent");
            var legendaryParent = serializedObject.FindProperty("legendaryParent");
            var achievementPreset = serializedObject.FindProperty("achievementPreset");
            var totalUnlockedObj = serializedObject.FindProperty("totalUnlockedObj");
            var totalValueObj = serializedObject.FindProperty("totalValueObj");
            var commonUnlockedObj = serializedObject.FindProperty("commonUnlockedObj");
            var commonlTotalObj = serializedObject.FindProperty("commonlTotalObj");
            var rareUnlockedObj = serializedObject.FindProperty("rareUnlockedObj");
            var rareTotalObj = serializedObject.FindProperty("rareTotalObj");
            var legendaryUnlockedObj = serializedObject.FindProperty("legendaryUnlockedObj");
            var legendaryTotalObj = serializedObject.FindProperty("legendaryTotalObj");

            var useLocalization = serializedObject.FindProperty("useLocalization");
            var useAlphabeticalOrder = serializedObject.FindProperty("useAlphabeticalOrder");

            ReachEditorHandler.DrawHeader(customSkin, "Content Header", 6);
            ReachEditorHandler.DrawProperty(UIManagerAsset, customSkin, "UI Manager");

            if (amTarget.UIManagerAsset != null)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent("Library Preset"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                GUI.enabled = false;
                amTarget.UIManagerAsset.achievementLibrary = EditorGUILayout.ObjectField(amTarget.UIManagerAsset.achievementLibrary, typeof(AchievementLibrary), true) as AchievementLibrary;
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            ReachEditorHandler.DrawHeader(customSkin, "Core Header", 10);
            ReachEditorHandler.DrawProperty(achievementPreset, customSkin, "Achievement Preset");
            ReachEditorHandler.DrawProperty(allParent, customSkin, "All Parent");
            ReachEditorHandler.DrawProperty(commonParent, customSkin, "Common Parent");
            ReachEditorHandler.DrawProperty(rareParent, customSkin, "Rare Parent");
            ReachEditorHandler.DrawProperty(legendaryParent, customSkin, "Legendary Parent");
            ReachEditorHandler.DrawProperty(totalUnlockedObj, customSkin, "Total Unlocked");
            ReachEditorHandler.DrawProperty(totalValueObj, customSkin, "Total Value");
            ReachEditorHandler.DrawProperty(commonUnlockedObj, customSkin, "Common Unlocked");
            ReachEditorHandler.DrawProperty(commonlTotalObj, customSkin, "Commonl Total");
            ReachEditorHandler.DrawProperty(rareUnlockedObj, customSkin, "Rare Unlocked");
            ReachEditorHandler.DrawProperty(rareTotalObj, customSkin, "Rare Total");
            ReachEditorHandler.DrawProperty(legendaryUnlockedObj, customSkin, "Legendary Unlocked");
            ReachEditorHandler.DrawProperty(legendaryTotalObj, customSkin, "Legendary Total");

            ReachEditorHandler.DrawHeader(customSkin, "Options Header", 10);
            useLocalization.boolValue = ReachEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
            useAlphabeticalOrder.boolValue = ReachEditorHandler.DrawToggle(useAlphabeticalOrder.boolValue, customSkin, "Use Alphabetical Order");

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif