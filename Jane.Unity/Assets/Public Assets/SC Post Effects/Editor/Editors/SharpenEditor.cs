using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(Sharpen))]
    public sealed class SharpenEditor : PostProcessEffectEditor<Sharpen>
    {
        SerializedParameterOverride mode;
        SerializedParameterOverride amount;
        SerializedParameterOverride radius;
        SerializedParameterOverride contrast;

        public override void OnEnable()
        {
            mode = FindParameterOverride(x => x.mode);
            amount = FindParameterOverride(x => x.amount);
            radius = FindParameterOverride(x => x.radius);
            contrast = FindParameterOverride(x => x.contrast);
        }
        
        public override string GetDisplayTitle()
        {
            return "Sharpen (" + mode.value.enumDisplayNames[mode.value.intValue] + ")";
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("sharpen");

            SCPE_GUI.DisplaySetupWarning<SharpenRenderer>();

            PropertyField(mode);
            
            EditorGUILayout.Space();

            PropertyField(amount);
            SCPE_GUI.DisplayIntensityWarning(amount);
            
            PropertyField(radius);

            if (mode.value.intValue == (int)Sharpen.Method.ContrastAdaptive)
            {
                PropertyField(contrast);
            }
        }
    }
}