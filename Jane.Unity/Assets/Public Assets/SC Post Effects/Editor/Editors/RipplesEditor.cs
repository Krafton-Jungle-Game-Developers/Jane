using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(Ripples))]
    sealed class RipplesEditor : VolumeComponentEditor
    {
        SerializedDataParameter mode;

        SerializedDataParameter strength;
        SerializedDataParameter distance;
        SerializedDataParameter speed;
        SerializedDataParameter width;
        SerializedDataParameter height;
        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Ripples>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<RipplesRenderer>();

            strength = Unpack(o.Find(x => x.strength));
            mode = Unpack(o.Find(x => x.mode));
            distance = Unpack(o.Find(x => x.distance));
            speed = Unpack(o.Find(x => x.speed));
            width = Unpack(o.Find(x => x.width));
            height = Unpack(o.Find(x => x.height));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("ripples");

            SCPE_GUI.DisplaySetupWarning<RipplesRenderer>(ref isSetup);

            PropertyField(strength);
            SCPE_GUI.DisplayIntensityWarning(strength);
            
            EditorGUILayout.Space();
            
            PropertyField(mode);
            PropertyField(distance);
            PropertyField(speed);

            //If Radial
            if (mode.value.intValue == 0)
            {
                EditorGUILayout.Space();
                PropertyField(width);
                PropertyField(height);
            }
        }
    }
}