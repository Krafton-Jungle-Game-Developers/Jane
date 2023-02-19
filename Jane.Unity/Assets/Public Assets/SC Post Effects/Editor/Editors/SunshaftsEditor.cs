using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(Sunshafts))]
    public sealed class SunshaftsEditor : PostProcessEffectEditor<Sunshafts>
    {
        SerializedParameterOverride useCasterColor;
        SerializedParameterOverride useCasterIntensity;

        SerializedParameterOverride resolution;
        SerializedParameterOverride sunThreshold;
        SerializedParameterOverride blendMode;
        SerializedParameterOverride sunColor;
        SerializedParameterOverride sunShaftIntensity;
        SerializedParameterOverride falloff;

        SerializedParameterOverride length;
        SerializedParameterOverride highQuality;

        public override void OnEnable()
        {
            useCasterColor = FindParameterOverride(x => x.useCasterColor);
            useCasterIntensity = FindParameterOverride(x => x.useCasterIntensity);

            resolution = FindParameterOverride(x => x.resolution);
            sunThreshold = FindParameterOverride(x => x.sunThreshold);
            blendMode = FindParameterOverride(x => x.blendMode);
            sunColor = FindParameterOverride(x => x.sunColor);
            sunShaftIntensity = FindParameterOverride(x => x.sunShaftIntensity);
            falloff = FindParameterOverride(x => x.falloff);
            length = FindParameterOverride(x => x.length);
            highQuality = FindParameterOverride(x => x.highQuality);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("sunshafts");

            SCPE_GUI.DisplayVRWarning();

            SCPE_GUI.DisplaySetupWarning<SunshaftsRenderer>();
            
            SCPE_GUI.DrawSunInfo();

            if (useCasterIntensity.value.boolValue == false)
            {
                PropertyField(sunShaftIntensity);
                SCPE_GUI.DisplayIntensityWarning(sunShaftIntensity);
            }
                
            EditorUtilities.DrawHeaderLabel("Quality");
            PropertyField(resolution);
            PropertyField(highQuality, new GUIContent("High quality"));

            EditorGUILayout.Space();

            EditorUtilities.DrawHeaderLabel("Use values from caster");
            PropertyField(useCasterColor, new GUIContent("Color"));
            if (useCasterColor.value.boolValue == false) PropertyField(sunColor);
            PropertyField(useCasterIntensity, new GUIContent("Intensity"));

            EditorGUILayout.Space();

            EditorUtilities.DrawHeaderLabel("Sunshafts");
            PropertyField(blendMode);
            PropertyField(sunThreshold);
            PropertyField(falloff);
            PropertyField(length);

        }

    }
}