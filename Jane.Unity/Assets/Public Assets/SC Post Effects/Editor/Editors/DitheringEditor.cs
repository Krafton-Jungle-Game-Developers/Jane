using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(Dithering))]
    sealed class DitheringEditor : VolumeComponentEditor
    {
        SerializedDataParameter intensity;
        SerializedDataParameter tiling;
        SerializedDataParameter luminanceThreshold;
        SerializedDataParameter lut;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Dithering>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<DitheringRenderer>();

            intensity = Unpack(o.Find(x => x.intensity));
            tiling = Unpack(o.Find(x => x.tiling));
            luminanceThreshold = Unpack(o.Find(x => x.luminanceThreshold));
            lut = Unpack(o.Find(x => x.lut));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("dithering");

            SCPE_GUI.DisplaySetupWarning<DitheringRenderer>(ref isSetup);

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(lut);

            if (lut.overrideState.boolValue && lut.value.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign a pattern texture", MessageType.Info);
            }

            EditorGUILayout.Space();

            PropertyField(luminanceThreshold);
            PropertyField(tiling);
        }
    }
}
