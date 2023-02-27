#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Reach
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIManagerAudio))]
    public class UIManagerAudioEditor : Editor
    {
        private UIManagerAudio uimaTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            uimaTarget = (UIManagerAudio)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = ReachEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = ReachEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            var UIManagerAsset = serializedObject.FindProperty("UIManagerAsset");
            var audioMixer = serializedObject.FindProperty("audioMixer");
            var audioSource = serializedObject.FindProperty("audioSource");
            var masterSlider = serializedObject.FindProperty("masterSlider");
            var musicSlider = serializedObject.FindProperty("musicSlider");
            var SFXSlider = serializedObject.FindProperty("SFXSlider");
            var UISlider = serializedObject.FindProperty("UISlider");

            ReachEditorHandler.DrawHeader(customSkin, "Core Header", 6);
            ReachEditorHandler.DrawProperty(UIManagerAsset, customSkin, "UI Manager");
            ReachEditorHandler.DrawProperty(audioMixer, customSkin, "Audio Mixer");
            ReachEditorHandler.DrawProperty(audioSource, customSkin, "Audio Source");
            ReachEditorHandler.DrawProperty(masterSlider, customSkin, "Master Slider");
            ReachEditorHandler.DrawProperty(musicSlider, customSkin, "Music Slider");
            ReachEditorHandler.DrawProperty(SFXSlider, customSkin, "SFX Slider");
            ReachEditorHandler.DrawProperty(UISlider, customSkin, "UI Slider");

            if (Application.isPlaying == true)
                return;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif