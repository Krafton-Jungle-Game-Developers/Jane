using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(Kaleidoscope))]
    public sealed class KaleidoscopeEditor : PostProcessEffectEditor<Kaleidoscope>
    {
        SerializedParameterOverride radialSplits;
        SerializedParameterOverride horizontalSplits;
        SerializedParameterOverride verticalSplits;
        SerializedParameterOverride center;
        SerializedParameterOverride maintainAspectRatio;

        public override void OnEnable()
        {
            radialSplits = FindParameterOverride(x => x.radialSplits);
            horizontalSplits = FindParameterOverride(x => x.horizontalSplits);
            verticalSplits = FindParameterOverride(x => x.verticalSplits);
            center = FindParameterOverride(x => x.center);
            maintainAspectRatio = FindParameterOverride(x => x.maintainAspectRatio);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("kaleidoscope");

            SCPE_GUI.DisplaySetupWarning<KaleidoscopeRenderer>();

            PropertyField(radialSplits);
            SCPE_GUI.DisplayIntensityWarning(radialSplits);
            
            PropertyField(horizontalSplits);
            PropertyField(verticalSplits);

            EditorGUILayout.Space();
            
            PropertyField(center);
            PropertyField(maintainAspectRatio);
        }
    }
}