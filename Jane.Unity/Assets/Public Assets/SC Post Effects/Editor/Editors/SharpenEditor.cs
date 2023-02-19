using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(Sharpen))]
    sealed class SharpenEditor : VolumeComponentEditor
    {
        SerializedDataParameter mode;
        SerializedDataParameter amount;
        SerializedDataParameter radius;
        SerializedDataParameter contrast;
        
        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Sharpen>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<SharpenRenderer>();

            mode = Unpack(o.Find(x => x.mode));
            amount = Unpack(o.Find(x => x.amount));
            radius = Unpack(o.Find(x => x.radius));
            contrast = Unpack(o.Find(x => x.contrast));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("sharpen");

            SCPE_GUI.DisplaySetupWarning<SharpenRenderer>(ref isSetup);

            PropertyField(mode);
            
            EditorGUILayout.Space();
            
            PropertyField(amount);
            SCPE_GUI.DisplayIntensityWarning(amount);
            
            PropertyField(radius);
            
            if (mode.value.intValue == (int)Sharpen.Method.ContrastAdaptive)
            {
                PropertyField(contrast);
            }
        }
    }
}