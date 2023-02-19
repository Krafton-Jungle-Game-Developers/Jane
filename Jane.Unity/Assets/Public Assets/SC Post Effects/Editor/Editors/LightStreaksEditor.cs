using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(LightStreaks))]
    sealed class LightStreaksEditor : VolumeComponentEditor
    {
        SerializedDataParameter quality;
        SerializedDataParameter debug;
        SerializedDataParameter intensity;
        SerializedDataParameter luminanceThreshold;
        SerializedDataParameter direction;
        SerializedDataParameter blur;
        SerializedDataParameter iterations;
        SerializedDataParameter downscaling;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<LightStreaks>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<LightStreaksRenderer>();

            quality = Unpack(o.Find(x => x.quality));
            debug = Unpack(o.Find(x => x.debug));
            intensity = Unpack(o.Find(x => x.intensity));
            luminanceThreshold = Unpack(o.Find(x => x.luminanceThreshold));
            direction = Unpack(o.Find(x => x.direction));
            blur = Unpack(o.Find(x => x.blur));
            iterations = Unpack(o.Find(x => x.iterations));
            downscaling = Unpack(o.Find(x => x.downscaling));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("light-streaks");

            SCPE_GUI.DisplaySetupWarning<LightStreaksRenderer>(ref isSetup);
            
            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(quality);
            PropertyField(debug);
            PropertyField(luminanceThreshold);
            PropertyField(direction);
            PropertyField(blur);
            PropertyField(iterations);
            PropertyField(downscaling);
        }
    }
}
