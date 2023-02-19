using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(Scanlines))]
    sealed class ScanlinesEditor : VolumeComponentEditor
    {
        SerializedDataParameter intensity;
        SerializedDataParameter amount;
        SerializedDataParameter speed;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Scanlines>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<ScanlinesRenderer>();

            intensity = Unpack(o.Find(x => x.intensity));
            amount = Unpack(o.Find(x => x.amount));
            speed = Unpack(o.Find(x => x.speed));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("scanlines");

            SCPE_GUI.DisplaySetupWarning<ScanlinesRenderer>(ref isSetup);

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(amount);
            PropertyField(speed);
        }
    }
}