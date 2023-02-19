using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(SpeedLines))]
    sealed class SpeedLinesEditor : VolumeComponentEditor
    {
        SerializedDataParameter intensity;
        SerializedDataParameter size;
        SerializedDataParameter falloff;
        SerializedDataParameter noiseTex;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<SpeedLines>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<SpeedLinesRenderer>();

            intensity = Unpack(o.Find(x => x.intensity));
            size = Unpack(o.Find(x => x.size));
            falloff = Unpack(o.Find(x => x.falloff));
            noiseTex = Unpack(o.Find(x => x.noiseTex));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("speed-lines");

            SCPE_GUI.DisplaySetupWarning<SpeedLinesRenderer>(ref isSetup);

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(noiseTex);
            PropertyField(size);
            PropertyField(falloff);
        }
    }
}