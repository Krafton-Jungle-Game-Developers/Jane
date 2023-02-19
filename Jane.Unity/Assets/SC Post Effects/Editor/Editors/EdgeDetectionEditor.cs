using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(EdgeDetection))]
    sealed class EdgeDetectionEditor : VolumeComponentEditor
    {
        EdgeDetection effect;
        SerializedDataParameter mode;
        SerializedDataParameter debug;

        SerializedDataParameter sensitivityDepth;
        SerializedDataParameter sensitivityNormals;
        SerializedDataParameter lumThreshold;

        SerializedDataParameter edgeExp;
        SerializedDataParameter edgeSize;

        SerializedDataParameter edgeColor;
        SerializedDataParameter edgeOpacity;

        SerializedDataParameter invertFadeDistance;
        SerializedDataParameter distanceFade;
        SerializedDataParameter startFadeDistance;
        SerializedDataParameter endFadeDistance;
        SerializedDataParameter sobelThin;
        
        private bool isSetup;
        public override void OnEnable()
        {
            base.OnEnable();

            effect = (EdgeDetection)target;
            var o = new PropertyFetcher<EdgeDetection>(serializedObject);

            isSetup = AutoSetup.ValidEffectSetup<EdgeDetectionRenderer>();

            mode = Unpack(o.Find(x => x.mode));
            debug = Unpack(o.Find(x => x.debug));

            sensitivityDepth = Unpack(o.Find(x => x.sensitivityDepth));
            sensitivityNormals = Unpack(o.Find(x => x.sensitivityNormals));
            lumThreshold = Unpack(o.Find(x => x.lumThreshold));

            edgeExp = Unpack(o.Find(x => x.edgeExp));
            edgeSize = Unpack(o.Find(x => x.edgeSize));

            edgeColor = Unpack(o.Find(x => x.edgeColor));
            edgeOpacity = Unpack(o.Find(x => x.edgeOpacity));

            invertFadeDistance = Unpack(o.Find(x => x.invertFadeDistance));
            distanceFade = Unpack(o.Find(x => x.distanceFade));
            startFadeDistance = Unpack(o.Find(x => x.startFadeDistance));
            endFadeDistance = Unpack(o.Find(x => x.endFadeDistance));
            sobelThin = Unpack(o.Find(x => x.sobelThin));

        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("edge-detection");

            SCPE_GUI.DisplaySetupWarning<EdgeDetectionRenderer>(ref isSetup, false);

            SCPE_GUI.ShowDepthTextureWarning();

            PropertyField(edgeOpacity);

            SCPE_GUI.DisplayIntensityWarning(edgeOpacity);
            
            EditorGUILayout.Space();

            PropertyField(debug);

            PropertyField(mode);

            if (mode.overrideState.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                switch (mode.value.intValue)
                {
                    case 0:
                        EditorGUILayout.HelpBox("Checks pixels for differences between geometry normals and their distance from the camera", MessageType.None);
                        break;
                    case 1:
                        EditorGUILayout.HelpBox("Same as Depth Normals but also uses vertical sampling for improved accuracy", MessageType.None);
                        break;
                    case 2:
                        EditorGUILayout.HelpBox("Draws edges only where neighboring pixels greatly differ in their depth value.", MessageType.None);
                        break;
                    case 3:
                        EditorGUILayout.HelpBox("Creates an edge where the luminance value of a pixel differs from its neighbors, past the threshold", MessageType.None);
                        break;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.LabelField("Method options");
            if (mode.value.intValue < 2)
            {
                PropertyField(sensitivityDepth);
                PropertyField(sensitivityNormals);
            }
            else if (mode.value.intValue == 2)
            {
                PropertyField(edgeExp);
            }
            else
            {
                // lum based mode
                PropertyField(lumThreshold);
            }
            if (mode.value.intValue == 2)
            {
                PropertyField(sobelThin);
            }
            
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Distance Fading");

            PropertyField(distanceFade);
            if (distanceFade.value.boolValue)
            {
                PropertyField(invertFadeDistance);
                PropertyField(startFadeDistance);
                PropertyField(endFadeDistance);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Edge settings");
            PropertyField(edgeColor);
            PropertyField(edgeSize);
        }
    }
}
