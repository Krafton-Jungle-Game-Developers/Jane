#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HotkeyEvent))]
    public class HotkeyEventEditor : Editor
    {
        private HotkeyEvent heTarget;
        private GUISkin customSkin;
        private int latestTabIndex;
        private string searchString;
        Vector2 scrollPosition = Vector2.zero;

        private void OnEnable()
        {
            heTarget = (HotkeyEvent)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }

            if (!Application.isPlaying && heTarget.hotkeyType == HotkeyEvent.HotkeyType.Dynamic) { heTarget.UpdateVisual(); }
        }

        public override void OnInspectorGUI()
        {
            ReachEditorHandler.DrawComponentHeader(customSkin, "Hotkey Event Top Header");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            latestTabIndex = ReachEditorHandler.DrawTabs(latestTabIndex, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                latestTabIndex = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab Resources")))
                latestTabIndex = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                latestTabIndex = 2;

            GUILayout.EndHorizontal();

            var hotkeyType = serializedObject.FindProperty("hotkeyType");
            var controllerPreset = serializedObject.FindProperty("controllerPreset");
            var hotkey = serializedObject.FindProperty("hotkey");
            var keyID = serializedObject.FindProperty("keyID");
            var hotkeyLabel = serializedObject.FindProperty("hotkeyLabel");

            var iconParent = serializedObject.FindProperty("iconParent");
            var textParent = serializedObject.FindProperty("textParent");
            var iconObj = serializedObject.FindProperty("iconObj");
            var labelObj = serializedObject.FindProperty("labelObj");
            var textObj = serializedObject.FindProperty("textObj");
            var normalCG = serializedObject.FindProperty("normalCG");
            var highlightCG = serializedObject.FindProperty("highlightCG");

            var useSounds = serializedObject.FindProperty("useSounds");
            var useLocalization = serializedObject.FindProperty("useLocalization");
            var normalOpacity = serializedObject.FindProperty("normalOpacity");
            var fadingMultiplier = serializedObject.FindProperty("fadingMultiplier");

            var onHotkeyPress = serializedObject.FindProperty("onHotkeyPress");

            switch (latestTabIndex)
            {
                case 0:
                    ReachEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    ReachEditorHandler.DrawPropertyPlainCW(hotkeyType, customSkin, "Hotkey Type", 82);
                 
                    if (heTarget.hotkeyType == HotkeyEvent.HotkeyType.Dynamic) 
                    { 
                        EditorGUILayout.HelpBox("Dynamic: UI will adapt itself to the current controller scheme (if available). Recommended if you want to specify the hotkey in UI.", MessageType.Info);
                        ReachEditorHandler.DrawProperty(controllerPreset, customSkin, "Default Preset");
                        if (labelObj.objectReferenceValue != null) { ReachEditorHandler.DrawProperty(hotkeyLabel, customSkin, "Hotkey Label"); }

                        GUILayout.BeginHorizontal(EditorStyles.helpBox);
                        EditorGUILayout.LabelField(new GUIContent("Key ID"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                        EditorGUILayout.PropertyField(keyID, new GUIContent(""));
                        heTarget.showOutputOnEditor = GUILayout.Toggle(heTarget.showOutputOnEditor, new GUIContent("", "See output"), GUILayout.Width(15), GUILayout.Height(18));
                        GUILayout.EndHorizontal();

                        // Search for keys
                        if (heTarget.controllerPreset != null)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            EditorGUILayout.LabelField(new GUIContent("Search for keys in " + heTarget.controllerPreset.name), customSkin.FindStyle("Text"));

                            GUILayout.BeginHorizontal();
                            searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
                            if (!string.IsNullOrEmpty(searchString) && GUILayout.Button(new GUIContent("", "Clear search bar"), GUI.skin.FindStyle("ToolbarSeachCancelButton"))) { searchString = ""; GUI.FocusControl(null); }
                            GUILayout.EndHorizontal();

                            if (!string.IsNullOrEmpty(searchString))
                            {
                                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(132));
                                GUILayout.BeginVertical();

                                for (int i = 0; i < heTarget.controllerPreset.items.Count; i++)
                                {
                                    if (heTarget.controllerPreset.items[i].itemID.ToLower().Contains(searchString.ToLower()))
                                    {
                                        if (GUILayout.Button(new GUIContent(heTarget.controllerPreset.items[i].itemID), customSkin.button))
                                        {
                                            heTarget.keyID = heTarget.controllerPreset.items[i].itemID;
                                            searchString = "";
                                            GUI.FocusControl(null);
                                            EditorUtility.SetDirty(heTarget);
                                        }
                                    }
                                }

                                GUILayout.EndVertical();
                                GUILayout.EndScrollView();
                            }

                            GUILayout.EndVertical();

                            if (heTarget.showOutputOnEditor == true)
                            {
                                GUI.enabled = false;
                                for (int i = 0; i < heTarget.controllerPreset.items.Count; i++)
                                {
                                    if (heTarget.controllerPreset.items[i].itemID == heTarget.keyID)
                                    {
                                        EditorGUILayout.LabelField(new GUIContent("Output for " + heTarget.controllerPreset.items[i].itemID), customSkin.FindStyle("Text"));
                                        EditorGUILayout.LabelField(new GUIContent("[Type] " + heTarget.controllerPreset.items[i].itemType.ToString()), customSkin.FindStyle("Text"));
                                        if (heTarget.controllerPreset.items[i].itemType == ControllerPreset.ItemType.Text) { EditorGUILayout.LabelField(new GUIContent("[Text] " + heTarget.controllerPreset.items[i].itemText), customSkin.FindStyle("Text")); }
                                        else if (heTarget.controllerPreset.items[i].itemIcon != null) { GUILayout.Box(ReachEditorHandler.TextureFromSprite(heTarget.controllerPreset.items[i].itemIcon), GUILayout.Width(32), GUILayout.Height(32)); }
                                    }
                                }
                                GUI.enabled = true;
                            }
                        }

                        if (Application.isPlaying == false && GUILayout.Button(new GUIContent("Update Visual UI"), customSkin.button)) 
                        { 
                            heTarget.UpdateVisual();
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }

                    else 
                    {
                        EditorGUILayout.HelpBox("Custom: You can manually change the content, but UI will not automatically adapt to the current controller scheme. " +
                            "Recommended if you don't need to specify the hotkey in UI.", MessageType.Info); 
                    }

                    GUILayout.EndVertical();

                    ReachEditorHandler.DrawHeader(customSkin, "Customization Header", 10);
                    EditorGUILayout.PropertyField(hotkey, new GUIContent("Hotkey"), true);
                    ReachEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onHotkeyPress, new GUIContent("On Hotkey Press"), true);
                    break;

                case 1:
                    ReachEditorHandler.DrawHeader(customSkin, "Core Header", 6);

                    if (heTarget.hotkeyType == HotkeyEvent.HotkeyType.Dynamic)
                    {
                        ReachEditorHandler.DrawProperty(iconParent, customSkin, "Icon Parent");
                        ReachEditorHandler.DrawProperty(textParent, customSkin, "Text Parent");
                        ReachEditorHandler.DrawProperty(iconObj, customSkin, "Icon Object");
                        ReachEditorHandler.DrawProperty(labelObj, customSkin, "Label Object");
                        ReachEditorHandler.DrawProperty(textObj, customSkin, "Text Object");
                        ReachEditorHandler.DrawProperty(normalCG, customSkin, "Normal CG");
                        ReachEditorHandler.DrawProperty(highlightCG, customSkin, "Highlight CG");
                    }

                    else 
                    {
                        EditorGUILayout.HelpBox("Hotkey Type is set to Custom.", MessageType.Info);
                    }

                    break;

                case 2:
                    ReachEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    useSounds.boolValue = ReachEditorHandler.DrawToggle(useSounds.boolValue, customSkin, "Use Sounds");
                    useLocalization.boolValue = ReachEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
                    ReachEditorHandler.DrawProperty(normalOpacity, customSkin, "Normal Opacity", "Sets the normal state (non-highlighted) opacity.");
                    ReachEditorHandler.DrawProperty(fadingMultiplier, customSkin, "Fading Multiplier", "Set the animation fade multiplier.");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif