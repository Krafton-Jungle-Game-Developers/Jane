using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(ColorSplit))]
    sealed class ColorSplitEditor : VolumeComponentEditor
    {
        SerializedDataParameter mode;
        SerializedDataParameter offset;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<ColorSplit>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<ColorSplitRenderer>();

            mode = Unpack(o.Find(x => x.mode));
            offset = Unpack(o.Find(x => x.offset));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("color-split");

            SCPE_GUI.DisplaySetupWarning<ColorSplitRenderer>(ref isSetup);

            PropertyField(offset);
            SCPE_GUI.DisplayIntensityWarning(offset);
            
            EditorGUILayout.Space();
            
            PropertyField(mode);
            
        }
    }
}
