using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(SpeedLines))]
    public sealed class SpeedLinesEditor : PostProcessEffectEditor<SpeedLines>
    {
        SerializedParameterOverride intensity;
        SerializedParameterOverride size;
        SerializedParameterOverride falloff;
        SerializedParameterOverride noiseTex;

        public override void OnEnable()
        {
            intensity = FindParameterOverride(x => x.intensity);
            size = FindParameterOverride(x => x.size);
            falloff = FindParameterOverride(x => x.falloff);
            noiseTex = FindParameterOverride(x => x.noiseTex);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("speed-lines");

            SCPE_GUI.DisplaySetupWarning<SpeedLinesRenderer>();

            PropertyField(intensity);            
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(noiseTex);
            PropertyField(size);
            PropertyField(falloff);
        }
    }
}