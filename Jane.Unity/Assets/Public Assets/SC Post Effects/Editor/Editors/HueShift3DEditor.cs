using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(HueShift3D))]
    sealed class HueShift3DEditor : VolumeComponentEditor
    {
        
        SerializedDataParameter colorSource;
        SerializedDataParameter gradientTex;
        SerializedDataParameter intensity;
        SerializedDataParameter speed;
        SerializedDataParameter size;
        SerializedDataParameter geoInfluence;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<HueShift3D>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<HueShift3DRenderer>();

            colorSource = Unpack(o.Find(x => x.colorSource));
            gradientTex = Unpack(o.Find(x => x.gradientTex));
            intensity = Unpack(o.Find(x => x.intensity));
            speed = Unpack(o.Find(x => x.speed));
            size = Unpack(o.Find(x => x.size));
            geoInfluence = Unpack(o.Find(x => x.geoInfluence));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("hue-shift-3d");

            SCPE_GUI.DisplaySetupWarning<HueShift3DRenderer>(ref isSetup);

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(colorSource);
            if(colorSource.value.intValue == (int)HueShift3D.ColorSource.GradientTexture) PropertyField(gradientTex);
            PropertyField(speed);
            PropertyField(size);

            PropertyField(geoInfluence);
            if (HueShift3D.isOrtho) EditorGUILayout.HelpBox("Not available for orthographic cameras", MessageType.None);
        }
    }
}