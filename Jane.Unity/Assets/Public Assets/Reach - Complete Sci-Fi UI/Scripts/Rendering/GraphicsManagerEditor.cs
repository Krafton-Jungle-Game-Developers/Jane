#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GraphicsManager))]
    public class GraphicsManagerEditor : Editor
    {
        private GraphicsManager gmTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            gmTarget = (GraphicsManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var resolutionDropdown = serializedObject.FindProperty("resolutionDropdown");

            var initializeResolutions = serializedObject.FindProperty("initializeResolutions");

            ReachEditorHandler.DrawHeader(customSkin, "Core Header", 6);
            ReachEditorHandler.DrawPropertyCW(resolutionDropdown, customSkin, "Resolution Dropdown", 132);

            ReachEditorHandler.DrawHeader(customSkin, "Options Header", 10);
            initializeResolutions.boolValue = ReachEditorHandler.DrawToggle(initializeResolutions.boolValue, customSkin, "Initialize Resolutions");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif