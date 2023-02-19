using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(TubeDistortion))]
    sealed class TubeDistortionEditor : VolumeComponentEditor
    {
        SerializedDataParameter mode;
        SerializedDataParameter amount;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<TubeDistortion>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<TubeDistortionRenderer>();

            mode = Unpack(o.Find(x => x.mode));
            amount = Unpack(o.Find(x => x.amount));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("tube-distortion");

            SCPE_GUI.DisplaySetupWarning<TubeDistortionRenderer>(ref isSetup);

            PropertyField(amount);
            SCPE_GUI.DisplayIntensityWarning(amount);
            
            EditorGUILayout.Space();
            
            PropertyField(mode);
        }
    }
}