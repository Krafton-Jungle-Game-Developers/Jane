using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace SCPE
{
    [VolumeComponentEditor(typeof(TiltShift))]
    sealed class TiltShiftEditor : VolumeComponentEditor
    {
        SerializedDataParameter amount;
        SerializedDataParameter mode;
        SerializedDataParameter quality;
        SerializedDataParameter areaSize;
        SerializedDataParameter areaFalloff;
        SerializedDataParameter offset;
        SerializedDataParameter angle;
        
        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<TiltShift>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<TiltShiftRenderer>();

            amount = Unpack(o.Find(x => x.amount));
            mode = Unpack(o.Find(x => x.mode));
            quality = Unpack(o.Find(x => x.quality));
            areaSize = Unpack(o.Find(x => x.areaSize));
            areaFalloff = Unpack(o.Find(x => x.areaFalloff));
            offset = Unpack(o.Find(x => x.offset));
            angle = Unpack(o.Find(x => x.angle));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("tilt-shift");

            SCPE_GUI.DisplaySetupWarning<TiltShiftRenderer>(ref isSetup);

            PropertyField(amount, new GUIContent("Blur amount"));
            SCPE_GUI.DisplayIntensityWarning(amount);
            
            EditorGUILayout.Space();
 
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawOverrideCheckbox(mode);
                using (new EditorGUI.DisabledGroupScope(mode.overrideState.boolValue == false))
                {
                    EditorGUILayout.PrefixLabel(mode.displayName);
                    mode.value.intValue = GUILayout.Toolbar(mode.value.intValue, mode.value.enumDisplayNames, GUILayout.Height(17f));
                }
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawOverrideCheckbox(quality);
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