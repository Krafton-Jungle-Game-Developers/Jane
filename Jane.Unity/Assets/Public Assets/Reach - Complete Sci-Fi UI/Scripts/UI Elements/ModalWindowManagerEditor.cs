#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.UI.Reach
{
    [CustomEditor(typeof(ModalWindowManager))]
    public class ModalWindowManagerEditor : Editor
    {
        private GUISkin customSkin;
        private ModalWindowManager mwTarget;
        private int currentTab;

        private void OnEnable()
        {
            mwTarget = (ModalWindowManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            ReachEditorHandler.DrawComponentHeader(customSkin, "Modal WIndow Top Header");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            currentTab = ReachEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab Resources")))
                currentTab = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 2;

            GUILayout.EndHorizontal();

            var windowIcon = serializedObject.FindProperty("windowIcon");
            var windowTitle = serializedObject.FindProperty("windowTitle");
            var windowDescription = serializedObject.FindProperty("windowDescription");

            var titleKey = serializedObject.FindProperty("titleKey");
            var descriptionKey = serializedObject.FindProperty("descriptionKey");

            var onConfirm = serializedObject.FindProperty("onConfirm");
            var onCancel = serializedObject.FindProperty("onCancel");
            var onOpen = serializedObject.FindProperty("onOpen");
            var onClose = serializedObject.FindProperty("onClose");

            var icon = serializedObject.FindProperty("icon");
            var titleText = serializedObject.FindProperty("titleText");
            var descriptionText = serializedObject.FindProperty("descriptionText");
            var confirmButton = serializedObject.FindProperty("confirmButton");
            var cancelButton = serializedObject.FindProperty("cancelButton");
            var mwAnimator = serializedObject.FindProperty("mwAnimator");

            var closeBehaviour = serializedObject.FindProperty("closeBehaviour");
            var startBehaviour = serializedObject.FindProperty("startBehaviour");
            var useCustomContent = serializedObject.FindProperty("useCustomContent");
            var closeOnCancel = serializedObject.FindProperty("closeOnCancel");
            var closeOnConfirm = serializedObject.FindProperty("closeOnConfirm");
            var showCancelButton = serializedObject.FindProperty("showCancelButton");
            var showConfirmButton = serializedObject.FindProperty("showConfirmButton");
            var useLocalization = serializedObject.FindProperty("useLocalization");
            var animationSpeed = serializedObject.FindProperty("animationSpeed");

            switch (currentTab)
            {
                case 0:
                    ReachEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    if (useCustomContent.boolValue == false)
                    {
                        if (mwTarget.windowIcon != null) 
                        {
                            ReachEditorHandler.DrawProperty(icon, customSkin, "Icon");
                            if (Application.isPlaying == false) { mwTarget.windowIcon.sprite = mwTarget.icon; }
                        }

                        if (mwTarget.windowTitle != null) 
                        {
                            ReachEditorHandler.DrawProperty(titleText, customSkin, "Title");
                            if (Application.isPlaying == false) { mwTarget.windowTitle.text = titleText.stringValue; }
                        }

                        if (mwTarget.windowDescription != null) 
                        {
                            GUILayout.BeginHorizontal(EditorStyles.helpBox);
                            EditorGUILayout.LabelField(new GUIContent("Description"), customSkin.FindStyle("Text"), GUILayout.Width(-3));
                            EditorGUILayout.PropertyField(descriptionText, new GUIContent(""), GUILayout.Height(70));
                            GUILayout.EndHorizontal();
                            if (Application.isPlaying == false) { mwTarget.windowDescription.text = descriptionText.stringValue; }
                        }
                    }

                    else { EditorGUILayout.HelpBox("'Use Custom Content' is enabled.", MessageType.Info); }

                    GUILayout.BeginHorizontal();
                    if (mwTarget.showConfirmButton == true && mwTarget.confirmButton != null && GUILayout.Button("Edit Confirm Button", customSkin.button)) { Selection.activeObject = mwTarget.confirmButton; }
                    if (mwTarget.showCancelButton == true && mwTarget.cancelButton != null && GUILayout.Button("Edit Cancel Button", customSkin.button)) { Selection.activeObject = mwTarget.cancelButton; }
                    GUILayout.EndHorizontal();

                    if (Application.isPlaying == false)
                    {
                        if (mwTarget.GetComponent<CanvasGroup>().alpha == 0 && GUILayout.Button("Set Visible", customSkin.button))
                        {
                            mwTarget.GetComponent<CanvasGroup>().alpha = 1;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }

                        else if (mwTarget.GetComponent<CanvasGroup>().alpha == 1 && GUILayout.Button("Set Invisible", customSkin.button))
                        {
                            mwTarget.GetComponent<CanvasGroup>().alpha = 0;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }

                    if (mwTarget.useCustomContent == false && mwTarget.useLocalization == true)
                    {
                        ReachEditorHandler.DrawHeader(customSkin, "Languages Header", 10);
                        ReachEditorHandler.DrawProperty(titleKey, customSkin, "Title Key", "Used for localization.");
                        ReachEditorHandler.DrawProperty(descriptionKey, customSkin, "Description Key", "Used for localization.");
                    }

                    ReachEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onConfirm, new GUIContent("On Confirm"), true);
                    EditorGUILayout.PropertyField(onCancel, new GUIContent("On Cancel"), true);
                    EditorGUILayout.PropertyField(onOpen, new GUIContent("On Open"), true);
                    EditorGUILayout.PropertyField(onClose, new GUIContent("On Close"), true);
                    break;

                case 1:
                    ReachEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    ReachEditorHandler.DrawProperty(windowIcon, customSkin, "Icon Object");
                    ReachEditorHandler.DrawProperty(windowTitle, customSkin, "Title Object");
                    ReachEditorHandler.DrawProperty(windowDescription, customSkin, "Description Object");
                    ReachEditorHandler.DrawProperty(confirmButton, customSkin, "Confirm Button");
                    ReachEditorHandler.DrawProperty(cancelButton, customSkin, "Cancel Button");
                    ReachEditorHandler.DrawProperty(mwAnimator, customSkin, "Animator");
                    break;

                case 2:
                    ReachEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    ReachEditorHandler.DrawProperty(animationSpeed, customSkin, "Animation Speed");
                    ReachEditorHandler.DrawProperty(startBehaviour, customSkin, "Start Behaviour");
                    ReachEditorHandler.DrawProperty(closeBehaviour, customSkin, "Close Behaviour");
                    useCustomContent.boolValue = ReachEditorHandler.DrawToggle(useCustomContent.boolValue, customSkin, "Use Custom Content", "Bypasses inspector values and allows manual editing.");
                    closeOnCancel.boolValue = ReachEditorHandler.DrawToggle(closeOnCancel.boolValue, customSkin, "Close Window On Cancel");
                    closeOnConfirm.boolValue = ReachEditorHandler.DrawToggle(closeOnConfirm.boolValue, customSkin, "Close Window On Confirm");
                    showCancelButton.boolValue = ReachEditorHandler.DrawToggle(showCancelButton.boolValue, customSkin, "Show Cancel Button");
                    showConfirmButton.boolValue = ReachEditorHandler.DrawToggle(showConfirmButton.boolValue, customSkin, "Show Confirm Button");
                    useLocalization.boolValue = ReachEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif