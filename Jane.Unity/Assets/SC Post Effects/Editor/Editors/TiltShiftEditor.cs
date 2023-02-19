using UnityEditor;
using UnityEngine;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(TiltShift))]
    public sealed class TiltShiftEditor : PostProcessEffectEditor<TiltShift>
    {

        SerializedParameterOverride amount;
        SerializedParameterOverride mode;
        SerializedParameterOverride quality;
        SerializedParameterOverride areaSize;
        SerializedParameterOverride areaFalloff;
        SerializedParameterOverride offset;
        SerializedParameterOverride angle;

        public override void OnEnable()
        {
            amount = FindParameterOverride(x => x.amount);
            mode = FindParameterOverride(x => x.mode);
            quality = FindParameterOverride(x => x.quality);
            areaSize = FindParameterOverride(x => x.areaSize);
            areaFalloff = FindParameterOverride(x => x.areaFalloff);
            offset = FindParameterOverride(x => x.offset);
            angle = FindParameterOverride(x => x.angle);
        }

        public override string GetDisplayTitle()
        {
            return base.GetDisplayTitle() + SCPE_GUI.ModeTitle(mode);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("tilt-shift");

            SCPE_GUI.DisplaySetupWarning<TiltShiftRenderer>();

            PropertyField(amount, new GUIContent("Blur amount"));
            SCPE_GUI.DisplayIntensityWarning(amount);
            
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                var overrideRect = GUILayoutUtility.GetRect(17f, 17f, GUILayout.ExpandWidth(false));
                overrideRect.yMin += 4f;
                EditorUtilities.DrawOverrideCheckbox(overrideRect, mode.overrideState);
                using (new EditorGUI.DisabledGroupScope(mode.overrideState.boolValue == false))
                {
                    EditorGUILayout.PrefixLabel(mode.displayName);
                    mode.value.intValue = GUILayout.Toolbar(mode.value.intValue, mode.value.enumDisplayNames, GUILayout.Height(17f));
                }
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                var overrideRect = GUILayoutUtility.GetRect(17f, 17f, GUILayout.ExpandWidth(false));
                overrideRect.yMin += 4f;
                EditorUtilities.DrawOverrideCheckbox(overrideRect, quality.overrideState);
                using (new EditorGUI.DisabledGroupScope(quality.overrideState.boolValue == false))
                {
                    EditorGUILayout.PrefixLabel(quality.displayName);
                    quality.value.intValue = GUILayout.Toolbar(quality.value.intValue, quality.value.enumDisplayNames, GUILayout.Height(17f));
                }
            }
            
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Screen area", EditorStyles.boldLabel);
            PropertyField(areaSize, new GUIContent("Size"));
            PropertyField(areaFalloff, new GUIContent("Falloff"));
            if (mode.value.intValue == (int)TiltShift.TiltShiftMethod.Horizontal)
            {
                PropertyField(offset, new GUIContent("Offset"));
                PropertyField(angle, new GUIContent("Angleº"));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(" ");
                TiltShift.debug = GUILayout.Toggle(TiltShift.debug,"Visualize area", "Button");
            }

        }
    }
}