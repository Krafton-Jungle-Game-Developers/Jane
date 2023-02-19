using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(RadialBlur))]
    public sealed class RadialBlurEditor : PostProcessEffectEditor<RadialBlur>
    {
        SerializedParameterOverride amount;
        SerializedParameterOverride center;
        SerializedParameterOverride angle;
        SerializedParameterOverride iterations;

        public override void OnEnable()
        {
            amount = FindParameterOverride(x => x.amount);
            center = FindParameterOverride(x => x.center);
            angle = FindParameterOverride(x => x.angle);
            iterations = FindParameterOverride(x => x.iterations);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("radial-blur");

            SCPE_GUI.DisplaySetupWarning<RadialBlurRenderer>();

            PropertyField(amount);
            SCPE_GUI.DisplayIntensityWarning(amount);
            
            EditorGUILayout.Space();
            
            PropertyField(center);
            PropertyField(angle);
            PropertyField(iterations);
        }
    }
}