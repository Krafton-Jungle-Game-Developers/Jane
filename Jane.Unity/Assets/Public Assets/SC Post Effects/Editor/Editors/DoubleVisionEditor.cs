using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(DoubleVision))]
    sealed class DoubleVisionEditor : VolumeComponentEditor
    {
        SerializedDataParameter mode;
        SerializedDataParameter intensity;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<DoubleVision>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<DoubleVisionRenderer>();

            mode = Unpack(o.Find(x => x.mode));
            intensity = Unpack(o.Find(x => x.intensity));
        }


        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("double-vision");

            SCPE_GUI.DisplaySetupWarning<DoubleVisionRenderer>(ref isSetup);

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(mode);
        }
    }
}
