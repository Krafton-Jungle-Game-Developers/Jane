using UnityEngine.Rendering.Universal;
using UnityEditor.Rendering;
using UnityEditor;
using UnityEngine;

namespace SCPE
{
    [VolumeComponentEditor(typeof(BlackBars))]
    sealed class BlackBarsEditor : VolumeComponentEditor
    {
        SerializedDataParameter mode;
        SerializedDataParameter size;
        SerializedDataParameter maxSize;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<BlackBars>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<BlackBarsRenderer>();

            mode = Unpack(o.Find(x => x.mode));
            size = Unpack(o.Find(x => x.size));
            maxSize = Unpack(o.Find(x => x.maxSize));
        }
        
        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("black-bars");

            SCPE_GUI.DisplaySetupWarning<BlackBarsRenderer>(ref isSetup);

            PropertyField(mode);
            SCPE_GUI.DisplayIntensityWarning(size);
            
            EditorGUILayout.Space();

            PropertyField(size);
            PropertyField(maxSize);
        }
    }
}