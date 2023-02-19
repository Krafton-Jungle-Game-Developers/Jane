using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(LightStreaks))]
    public class LightStreaksEditor : PostProcessEffectEditor<LightStreaks>
    {
        SerializedParameterOverride quality;
        SerializedParameterOverride debug;
        SerializedParameterOverride intensity;
        SerializedParameterOverride luminanceThreshold;
        SerializedParameterOverride direction;
        SerializedParameterOverride blur;
        SerializedParameterOverride iterations;
        SerializedParameterOverride downscaling;

        public override void OnEnable()
        {
            quality = FindParameterOverride(x => x.quality);
            debug = FindParameterOverride(x => x.debug);
            intensity = FindParameterOverride(x => x.intensity);
            luminanceThreshold = FindParameterOverride(x => x.luminanceThreshold);
            direction = FindParameterOverride(x => x.direction);
            blur = FindParameterOverride(x => x.blur);
            iterations = FindParameterOverride(x => x.iterations);
            downscaling = FindParameterOverride(x => x.downscaling);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("light-streaks");

            SCPE_GUI.DisplaySetupWarning<LightStreaksRenderer>();

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