#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.UI.Reach
{
    public class ToolsMenu : Editor
    {
        static string objectPath;

        static void GetObjectPath()
        {
            objectPath = AssetDatabase.GetAssetPath(Resources.Load("Reach UI Manager"));
            objectPath = objectPath.Replace("Resources/Reach UI Manager.asset", "").Trim();
            objectPath = objectPath + "Prefabs/";
        }

        static void MakeSceneDirty(GameObject source, string sourceName)
        {
            if (Application.isPlaying == false)
            {
                Undo.RegisterCreatedObjectUndo(source, sourceName);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        static void ShowErrorDialog()
        {
            EditorUtility.DisplayDialog("Reach UI", "Cannot create the object due to missing manager file. " +
                    "Make sure you have 'Reach UI Manager' file in Reach UI > Resources folder.", "Dismiss");
        }

        static void UpdateCustomEditorPath()
        {
            string darkPath = AssetDatabase.GetAssetPath(Resources.Load("ReachEditor-Dark"));
            string lightPath = AssetDatabase.GetAssetPath(Resources.Load("ReachEditor-Light"));

            EditorPrefs.SetString("ReachUI.CustomEditorDark", darkPath);
            EditorPrefs.SetString("ReachUI.CustomEditorLight", lightPath);
        }

        [MenuItem("Tools/Reach UI/Show UI Manager %#M")]
        static void ShowManager()
        {
            Selection.activeObject = Resources.Load("Reach UI Manager");

            if (Selection.activeObject == null)
                Debug.Log("<b>[Reach UI]</b>Can't find an asset called 'Reach UI Manager'. Make sure you have 'Reach UI Manager' in: Reach UI > Editor > Resources");
        }

        static void CreateObject(string resourcePath)
        {
            try
            {
                GetObjectPath();
                UpdateCustomEditorPath();
                GameObject clone = Instantiate(AssetDatabase.LoadAssetAtPath(objectPath + resourcePath + ".prefab", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;

                try
                {
                    if (Selection.activeGameObject == null)
                    {
                        var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                        clone.transform.SetParent(canvas.transform, false);
                    }

                    else { clone.transform.SetParent(Selection.activeGameObject.transform, false); }

                    clone.name = clone.name.Replace("(Clone)", "").Trim();
                    MakeSceneDirty(clone, clone.name);
                }

                catch
                {
                    CreateCanvas();
                    var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                    clone.transform.SetParent(canvas.transform, false);
                    clone.name = clone.name.Replace("(Clone)", "").Trim();
                    MakeSceneDirty(clone, clone.name);
                }

                Selection.activeObject = clone;
            }

            catch { ShowErrorDialog(); }
        }

        [MenuItem("GameObject/Reach UI/Canvas", false, 8)]
        static void CreateCanvas()
        {
            try
            {
                GetObjectPath();
                UpdateCustomEditorPath();
                GameObject clone = Instantiate(AssetDatabase.LoadAssetAtPath(objectPath + "UI Elements/Canvas/Canvas" + ".prefab", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Selection.activeObject = clone;
                MakeSceneDirty(clone, clone.name);
            }

            catch { ShowErrorDialog(); }
        }

        [MenuItem("GameObject/Reach UI/Button/Button", false, 8)]
        static void CreateButtonMain() { CreateObject("UI Elements/Button/Button"); }

        [MenuItem("GameObject/Reach UI/Button/Button (Basic)", false, 8)]
        static void CreateButtonBasic() { CreateObject("UI Elements/Button/Button (Basic)"); }

        [MenuItem("GameObject/Reach UI/Button/Button (Icon Sway)", false, 8)]
        static void CreateButtonIS() { CreateObject("UI Elements/Button/Button (Icon Sway)"); }

        [MenuItem("GameObject/Reach UI/Button/Button (Panel)", false, 8)]
        static void CreateButtonPanel() { CreateObject("UI Elements/Button/Button (Panel)"); }

        [MenuItem("GameObject/Reach UI/Button/Button (Spot)", false, 8)]
        static void CreateButtonSpot() { CreateObject("UI Elements/Button/Button (Spot)"); }

        [MenuItem("GameObject/Reach UI/Dropdown/Standard", false, 8)]
        static void CreateDropdown() { CreateObject("UI Elements/Dropdown/Dropdown"); }

        [MenuItem("GameObject/Reach UI/HUD/Feed Notification", false, 8)]
        static void CreateHudFN() { CreateObject("HUD/Feed Notification"); }

        [MenuItem("GameObject/Reach UI/HUD/Health Bar", false, 8)]
        static void CreateHudHealthBar() { CreateObject("HUD/Health Bar"); }

        [MenuItem("GameObject/Reach UI/HUD/Minimap", false, 8)]
        static void CreateHudMinimap() { CreateObject("HUD/Minimap"); }

        [MenuItem("GameObject/Reach UI/HUD/Quest Item", false, 8)]
        static void CreateHudQuestItem() { CreateObject("HUD/Quest Item"); }

        [MenuItem("GameObject/Reach UI/Input/Hotkey Indicator", false, 8)]
        static void CreateHotkeyIndicator() { CreateObject("UI Elements/Input/Hotkey Indicator"); }

        [MenuItem("GameObject/Reach UI/Input Field/Standard", false, 8)]
        static void CreateInputField() { CreateObject("UI Elements/Input Field/Input Field"); }

        [MenuItem("GameObject/Reach UI/Modal Window/Standard", false, 8)]
        static void CreateModalWindow() { CreateObject("UI Elements/Modal Window/Modal Window"); }

        [MenuItem("GameObject/Reach UI/Modal Window/Custom Content", false, 8)]
        static void CreateModalWindowCC() { CreateObject("UI Elements/Modal Window/Modal Window (Custom Content)"); }

        [MenuItem("GameObject/Reach UI/Panels/Credits", false, 8)]
        static void CreateCredits() { CreateObject("Panels/Credits"); }

        [MenuItem("GameObject/Reach UI/Progress Bar/Standard", false, 8)]
        static void CreateProgressBar() { CreateObject("UI Elements/Progress Bar/Progress Bar"); }

        [MenuItem("GameObject/Reach UI/Scrollbar/Horizontal", false, 8)]
        static void CreateScrollbarHorizontal() { CreateObject("UI Elements/Scrollbar/Scrollbar Horizontal"); }

        [MenuItem("GameObject/Reach UI/Scrollbar/Vertical", false, 8)]
        static void CreateScrollbarVertical() { CreateObject("UI Elements/Scrollbar/Scrollbar Vertical"); }

        [MenuItem("GameObject/Reach UI/Selectors/Horizontal Selector", false, 8)]
        static void CreateHorizontalSelector() { CreateObject("UI Elements/Selectors/Horizontal Selector"); }

        [MenuItem("GameObject/Reach UI/Selectors/Mode Selector", false, 8)]
        static void CreateModeSelector() { CreateObject("UI Elements/Selectors/Mode Selector"); }

        [MenuItem("GameObject/Reach UI/Settings/Settings Element (Dropdown)", false, 8)]
        static void CreateSettingsDropdownt() { CreateObject("UI Elements/Settings/Settings Element (Dropdown Alt)"); }

        [MenuItem("GameObject/Reach UI/Settings/Settings Element (Horizontal Selector)", false, 8)]
        static void CreateSettingsHS() { CreateObject("UI Elements/Settings/Settings Element (Horizontal Selector)"); }

        [MenuItem("GameObject/Reach UI/Settings/Settings Element (Slider)", false, 8)]
        static void CreateSettingsSlider() { CreateObject("UI Elements/Settings/Settings Element (Slider)"); }

        [MenuItem("GameObject/Reach UI/Settings/Settings Element (Switch)", false, 8)]
        static void CreateSettingsSwitch() { CreateObject("UI Elements/Settings/Settings Element (Switch)"); }

        [MenuItem("GameObject/Reach UI/Slider/Standard", false, 8)]
        static void CreateSlider() { CreateObject("UI Elements/Slider/Slider"); }

        [MenuItem("GameObject/Reach UI/Switch/Standard", false, 8)]
        static void CreateSwitch() { CreateObject("UI Elements/Switch/Switch"); }

        [MenuItem("GameObject/Reach UI/Text/Text (TMP)", false, 8)]
        static void CreateText() { CreateObject("UI Elements/Text/Text (TMP)"); }

        [MenuItem("GameObject/Reach UI/Text/Panel Header", false, 8)]
        static void CreatePanelText() { CreateObject("UI Elements/Text/Panel Header"); }

        [MenuItem("GameObject/Reach UI/Widgets/News Slider", false, 8)]
        static void CreateNewsSlider() { CreateObject("UI Elements/Widgets/News Slider/News Slider"); }

        [MenuItem("GameObject/Reach UI/Widgets/Socials", false, 8)]
        static void CreateSocialsWidget() { CreateObject("UI Elements/Widgets/Socials/Socials Widget"); }
    }
}
#endif