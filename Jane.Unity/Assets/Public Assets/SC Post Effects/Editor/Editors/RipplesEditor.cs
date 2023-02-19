using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(Ripples))]
    public sealed class RipplesEditor : PostProcessEffectEditor<Ripples>
    {
        SerializedParameterOverride mode;

        SerializedParameterOverride strength;
        SerializedParameterOverride distance;
        SerializedParameterOverride speed;
        SerializedParameterOverride width;
        SerializedParameterOverride height;

        public override void OnEnable()
        {
            strength = FindParameterOverride(x => x.strength);
            mode = FindParameterOverride(x => x.mode);
            distance = FindParameterOverride(x => x.distance);
            speed = FindParameterOverride(x => x.speed);
            width = FindParameterOverride(x => x.width);
            height = FindParameterOverride(x => x.height);
        }

        public override string GetDisplayTitle()
        {
            return "Ripples (" + (Ripples.RipplesMode)mode.value.enumValueIndex + ")";
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("ripples");

            SCPE_GUI.DisplaySetupWarning<RipplesRenderer>();

            PropertyField(strength);
            SCPE_GUI.DisplayIntensityWarning(strength);
            
            EditorGUILayout.Space();
            
            PropertyField(mode);
            PropertyField(distance);
            PropertyField(speed);

            //If Radial
            if (mode.value.intValue == 0)
            {
                EditorGUILayout.Space();
                PropertyField(width);
                PropertyField(height);

            }
        }
    }
}