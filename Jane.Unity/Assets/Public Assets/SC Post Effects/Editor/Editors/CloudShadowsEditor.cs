using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(CloudShadows))]
    sealed class CloudShadowsEditor : VolumeComponentEditor
    {
        SerializedDataParameter texture;
        SerializedDataParameter size;
        SerializedDataParameter density;
        SerializedDataParameter speed;
        SerializedDataParameter direction;
        SerializedDataParameter projectFromSun;
        
        SerializedDataParameter startFadeDistance;
        SerializedDataParameter endFadeDistance;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<CloudShadows>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<CloudShadowsRenderer>();

            texture = Unpack(o.Find(x => x.texture));
            size = Unpack(o.Find(x => x.size));
            density = Unpack(o.Find(x => x.density));
            speed = Unpack(o.Find(x => x.speed));
            direction = Unpack(o.Find(x => x.direction));
            projectFromSun = Unpack(o.Find(x => x.projectFromSun));
            startFadeDistance = Unpack(o.Find(x => x.startFadeDistance));
            endFadeDistance = Unpack(o.Find(x => x.endFadeDistance));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("cloud-shadows");

            //SCPE_GUI.DisplayVRWarning(true);

            SCPE_GUI.DisplaySetupWarning<CloudShadowsRenderer>(ref isSetup, false);

            if (CloudShadows.isOrtho) EditorGUILayout.HelpBox("Not available for orthographic cameras", MessageType.Warning);

            PropertyField(density);
            SCPE_GUI.DisplayIntensityWarning(density);

            EditorGUILayout.Space();

            PropertyField(texture);
            PropertyField(size);
            PropertyField(speed);
            PropertyField(direction);
            PropertyField(projectFromSun);
            if (projectFromSun.value.boolValue) SCPE_GUI.DrawSunInfo();
            
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Distance Fading");
            PropertyField(startFadeDistance);
            PropertyField(endFadeDistance);
        }
    }
}
