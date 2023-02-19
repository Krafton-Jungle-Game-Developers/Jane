using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(Kaleidoscope))]
    sealed class KaleidoscopeEditor : VolumeComponentEditor
    {
        SerializedDataParameter radialSplits;
        SerializedDataParameter horizontalSplits;
        SerializedDataParameter verticalSplits;
        SerializedDataParameter center;
        SerializedDataParameter maintainAspectRatio;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Kaleidoscope>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<KaleidoscopeRenderer>();

            radialSplits = Unpack(o.Find(x => x.radialSplits));
            horizontalSplits = Unpack(o.Find(x => x.horizontalSplits));
            verticalSplits = Unpack(o.Find(x => x.verticalSplits));
            center = Unpack(o.Find(x => x.center));
            maintainAspectRatio = Unpack(o.Find(x => x.maintainAspectRatio));
        }


        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("kaleidoscope");

            SCPE_GUI.DisplaySetupWarning<KaleidoscopeRenderer>(ref isSetup);

            PropertyField(radialSplits);
            SCPE_GUI.DisplayIntensityWarning(radialSplits);
            
            PropertyField(horizontalSplits);
            PropertyField(verticalSplits);

            EditorGUILayout.Space();
            
            PropertyField(center);
            PropertyField(maintainAspectRatio);
        }
    }
}