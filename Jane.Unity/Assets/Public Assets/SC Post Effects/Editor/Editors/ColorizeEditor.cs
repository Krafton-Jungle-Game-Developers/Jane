using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(Colorize))]
    public sealed class ColorizeEditor : PostProcessEffectEditor<Colorize>
    {
        SerializedParameterOverride mode;
        SerializedParameterOverride intensity;
        SerializedParameterOverride colorRamp;

        public override void OnEnable()
        {
            mode = FindParameterOverride(x => x.mode);
            intensity = FindParameterOverride(x => x.intensity);
            colorRamp = FindParameterOverride(x => x.colorRamp);
        }

        public override string GetDisplayTitle()
        {
            return "Colorize (" + mode.value.enumDisplayNames[mode.value.intValue] + ")";
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("colorize");

            SCPE_GUI.DisplaySetupWarning<ColorizeRenderer>();

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(mode);
            PropertyField(colorRamp);

            if (colorRamp.value.objectReferenceValue)
            {
                SCPE.CheckGradientImportSettings(colorRamp.value.objectReferenceValue);
            }
        }
    }
}