#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    public class InitReach
    {
        [InitializeOnLoad]
        public class InitOnLoad
        {
            static InitOnLoad()
            {
                if (!EditorPrefs.HasKey("ReachUI.Installed"))
                {
                    EditorPrefs.SetInt("ReachUI.Installed", 1);
                    EditorUtility.DisplayDialog("Hello there!", "Thank you for purchasing Reach UI." +
                        "\r\rTo use the UI Manager, go to Tools > Reach UI > Show UI Manager." +
                        "\r\rIf you need help, feel free to contact us through our support channels.", "Got it!");
                }

                if (!EditorPrefs.HasKey("ReachUI.HasCustomEditorData"))
                {
                    string darkPath = AssetDatabase.GetAssetPath(Resources.Load("ReachEditor-Dark"));
                    string lightPath = AssetDatabase.GetAssetPath(Resources.Load("ReachEditor-Light"));

                    EditorPrefs.SetString("ReachUI.CustomEditorDark", darkPath);
                    EditorPrefs.SetString("ReachUI.CustomEditorLight", lightPath);
                    EditorPrefs.SetInt("ReachUI.HasCustomEditorData", 1);
                }
            }
        }
    }
}
#endif