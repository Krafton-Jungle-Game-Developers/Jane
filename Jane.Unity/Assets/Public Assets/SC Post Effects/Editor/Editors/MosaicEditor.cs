using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(Mosaic))]
    sealed class MosaicEditor : VolumeComponentEditor
    {
        SerializedDataParameter mode;
        SerializedDataParameter size;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Mosaic>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<MosaicRenderer>();

            mode = Unpack(o.Find(x => x.mode));
            size = Unpack(o.Find(x => x.size));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("mosaic");

            SCPE_GUI.DisplaySetupWarning<MosaicRenderer>(ref isSetup);

            PropertyField(size);
            SCPE_GUI.DisplayIntensityWarning(size);
            
            EditorGUILayout.Space();
            
            PropertyField(mode);
        }
    }
}