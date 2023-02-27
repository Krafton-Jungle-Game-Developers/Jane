#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CustomEditor(typeof(PanelManager))]
    public class PanelManagerEditor : Editor
    {
        private GUISkin customSkin;
        private PanelManager pmTarget;
        private int currentTab;

        private void OnEnable()
        {
            pmTarget = (PanelManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            ReachEditorHandler.DrawComponentHeader(customSkin, "Panel Manager Top Header");

            GUIContent[] toolbarTabs = new GUIContent[2];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Settings");

            currentTab = ReachEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 1;

            GUILayout.EndHorizontal();

            var panels = serializedObject.FindProperty("panels");
            var currentPanelIndex = serializedObject.FindProperty("currentPanelIndex");
            
            var cullPanels = serializedObject.FindProperty("cullPanels");
            var onPanelChanged = serializedObject.FindProperty("onPanelChanged");
            var initializeButtons = serializedObject.FindProperty("initializeButtons");
            var bypassAnimationOnEnable = serializedObject.FindProperty("bypassAnimationOnEnable");
            var panelMode = serializedObject.FindProperty("panelMode");
            var animationSpeed = serializedObject.FindProperty("animationSpeed");

            switch (currentTab)
            {
                case 0:
                    ReachEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    if (pmTarget.currentPanelIndex > pmTarget.panels.Count - 1) { pmTarget.currentPanelIndex = 0; }
                    if (pmTarget.panels.Count != 0)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.BeginHorizontal();

                        GUI.enabled = false;
                        EditorGUILayout.LabelField(new GUIContent("Current Panel:"), customSkin.FindStyle("Text"), GUILayout.Width(82));
                        GUI.enabled = true;
                        EditorGUILayout.LabelField(new GUIContent(pmTarget.panels[currentPanelIndex.intValue].panelName), customSkin.FindStyle("Text"));

                        GUILayout.EndHorizontal();
                        GUILayout.Space(2);

                        if (Application.isPlaying == true) { GUI.enabled = false; }

                        currentPanelIndex.intValue = EditorGUILayout.IntSlider(currentPanelIndex.intValue, 0, pmTarget.panels.Count - 1);

                        if (Application.isPlaying == false && pmTarget.panels[currentPanelIndex.intValue].panelObject != null)
                        {
                            for (int i = 0; i < pmTarget.panels.Count; i++)
                            {
                                if (i == currentPanelIndex.intValue)
                                {
                                    var tempCG = pmTarget.panels[currentPanelIndex.intValue].panelObject.GetComponent<CanvasGroup>();
                                    if (tempCG != null) { tempCG.alpha = 1; }
                                }

                                else if (pmTarget.panels[i].panelObject != null)
                                {
                                    var tempCG = pmTarget.panels[i].panelObject.GetComponent<CanvasGroup>();
                                    if (tempCG != null) { tempCG.alpha = 0; }
                                }
                            }
                        }

                        if (pmTarget.panels[pmTarget.currentPanelIndex].panelObject != null && GUILayout.Button("Select Current Panel", customSkin.button)) { Selection.activeObject = pmTarget.panels[pmTarget.currentPanelIndex].panelObject; }
                        GUI.enabled = true;
                        GUILayout.EndVertical();

                    }

                    else { EditorGUILayout.HelpBox("Panel List is empty. Create a new item to see more options.", MessageType.Info); }

                    GUILayout.BeginVertical();
                    EditorGUI.indentLevel = 1;

                    EditorGUILayout.PropertyField(panels, new GUIContent("Panel Items"), true);

                    EditorGUI.indentLevel = 0;
                    GUILayout.EndVertical();

                    ReachEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onPanelChanged, new GUIContent("On Panel Changed"), true);
                    break;

                case 1:
                    ReachEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    cullPanels.boolValue = ReachEditorHandler.DrawToggle(cullPanels.boolValue, customSkin, "Cull Panels", "Disables unused panels.");
                    initializeButtons.boolValue = ReachEditorHandler.DrawToggle(initializeButtons.boolValue, customSkin, "Initialize Buttons", "Automatically adds necessary events to buttons.");
                    bypassAnimationOnEnable.boolValue = ReachEditorHandler.DrawToggle(bypassAnimationOnEnable.boolValue, customSkin, "Bypass Animation On Enable");
                    ReachEditorHandler.DrawProperty(panelMode, customSkin, "Panel Mode");
                    ReachEditorHandler.DrawProperty(animationSpeed, customSkin, "Animation Speed");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif