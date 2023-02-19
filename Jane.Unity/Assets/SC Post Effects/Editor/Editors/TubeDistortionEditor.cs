using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(TubeDistortion))]
    public sealed class TubeDistortionEditor : PostProcessEffectEditor<TubeDistortion>
    {
        SerializedParameterOverride mode;
        SerializedParameterOverride amount;
        SerializedParameterOverride luminanceThreshold;
        SerializedParameterOverride lut;

        public override void OnEnable()
        {
            mode = FindParameterOverride(x => x.mode);
            amount = FindParameterOverride(x => x.amount);
        }

        public override string GetDisplayTitle()
        {
            return base.GetDisplayTitle() + SCPE_GUI.ModeTitle(mode);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("tube-distortion");

            SCPE_GUI.DisplaySetupWarning<TubeDistortionRenderer>();

            PropertyField(amount);
            SCPE_GUI.DisplayIntensityWarning(amount);
            
            EditorGUILayout.Space();
            
            PropertyField(mode);
        }
    }
}