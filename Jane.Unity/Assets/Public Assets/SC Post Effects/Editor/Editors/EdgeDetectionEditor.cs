using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(EdgeDetection))]
    public sealed class EdgeDetectionEditor : PostProcessEffectEditor<EdgeDetection>
    {
        SerializedParameterOverride mode;

        SerializedParameterOverride sensitivityDepth;
        SerializedParameterOverride sensitivityNormals;
        SerializedParameterOverride lumThreshold;

        SerializedParameterOverride edgeExp;
        SerializedParameterOverride edgeSize;

        SerializedParameterOverride edgesOnly;
        SerializedParameterOverride edgeColor;
        SerializedParameterOverride edgeOpacity;

        SerializedParameterOverride invertFadeDistance;
        SerializedParameterOverride distanceFade;
        SerializedParameterOverride startFadeDistance;
        SerializedParameterOverride endFadeDistance;
        SerializedParameterOverride sobelThin;

        private static bool showHelp;

        public override string GetDisplayTitle()
        {
            return "Edge Detection (" + mode.value.enumDisplayNames[mode.value.intValue] + ")";
        }

        public override void OnEnable()
        {
            mode = FindParameterOverride(x => x.mode);
            sensitivityDepth = FindParameterOverride(x => x.sensitivityDepth);
            sensitivityNormals = FindParameterOverride(x => x.sensitivityNormals);
            lumThreshold = FindParameterOverride(x => x.lumThreshold);
            edgeExp = FindParameterOverride(x => x.edgeExp);
            edgeSize = FindParameterOverride(x => x.edgeSize);
            edgesOnly = FindParameterOverride(x => x.debug);
            edgeColor = FindParameterOverride(x => x.edgeColor);
            edgeOpacity = FindParameterOverride(x => x.edgeOpacity);
            invertFadeDistance = FindParameterOverride(x => x.invertFadeDistance);
            distanceFade = FindParameterOverride(x => x.distanceFade);
            startFadeDistance = FindParameterOverride(x => x.startFadeDistance);
            endFadeDistance = FindParameterOverride(x => x.endFadeDistance);
            sobelThin = FindParameterOverride(x => x.sobelThin);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("edge-detection");

            SCPE_GUI.DisplayVRWarning();

            SCPE_GUI.DisplaySetupWarning<EdgeDetectionRenderer>();

            PropertyField(edgeOpacity);
            SCPE_GUI.DisplayIntensityWarning(edgeOpacity);
            
            EditorGUILayout.Space();
            
            PropertyField(edgesOnly);
            
            EditorGUILayout.Space();

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