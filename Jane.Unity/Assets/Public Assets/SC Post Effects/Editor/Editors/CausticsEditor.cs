using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(Caustics))]
    sealed class CausticsEditor : VolumeComponentEditor
    {
        SerializedDataParameter causticsTexture;
        SerializedDataParameter intensity;
        SerializedDataParameter luminanceThreshold;
        SerializedDataParameter projectFromSun;

        SerializedDataParameter minHeight;
        SerializedDataParameter minHeightFalloff;
        SerializedDataParameter maxHeight;
        SerializedDataParameter maxHeightFalloff;
        
        SerializedDataParameter size;
        SerializedDataParameter speed;
        
        SerializedDataParameter distanceFade;
        SerializedDataParameter startFadeDistance;
        SerializedDataParameter endFadeDistance;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Caustics>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<CausticsRenderer>();

            causticsTexture = Unpack(o.Find(x =>x.causticsTexture));
            intensity = Unpack(o.Find(x =>x.intensity));
            luminanceThreshold = Unpack(o.Find(x =>x.luminanceThreshold));
            projectFromSun = Unpack(o.Find(x =>x.projectFromSun));

            minHeight = Unpack(o.Find(x =>x.minHeight));
            minHeightFalloff = Unpack(o.Find(x =>x.minHeightFalloff));
            maxHeight = Unpack(o.Find(x =>x.maxHeight));
            maxHeightFalloff = Unpack(o.Find(x =>x.maxHeightFalloff));

            size = Unpack(o.Find(x => x.size));
            speed = Unpack(o.Find(x =>x.speed));
            
            distanceFade = Unpack(o.Find(x =>x.distanceFade));
            startFadeDistance = Unpack(o.Find(x =>x.startFadeDistance));
            endFadeDistance = Unpack(o.Find(x =>x.endFadeDistance));
        }
        

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("caustics");

            SCPE_GUI.DisplaySetupWarning<CausticsRenderer>(ref isSetup, false);

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();

            PropertyField(causticsTexture);
            PropertyField(luminanceThreshold);
            PropertyField(projectFromSun);
            if (projectFromSun.value.boolValue) SCPE_GUI.DrawSunInfo();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Height filter", EditorStyles.boldLabel);
            PropertyField(minHeight);
            PropertyField(minHeightFalloff);
            PropertyField(maxHeight);
            PropertyField(maxHeightFalloff);
            
            EditorGUILayout.Space();

            PropertyField(size);
            PropertyField(speed);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Distance Fading", EditorStyles.boldLabel);
            PropertyField(distanceFade);
            if (distanceFade.value.boolValue)
            {
                PropertyField(startFadeDistance);
                PropertyField(endFadeDistance);
            }
        }
    }
}
