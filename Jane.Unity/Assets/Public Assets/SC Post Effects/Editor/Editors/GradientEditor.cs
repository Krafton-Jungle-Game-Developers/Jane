using UnityEditor;
using UnityEditor.Rendering.PostProcessing;


namespace SCPE
{
    [PostProcessEditor(typeof(Gradient))]
    public class GradientEditor : PostProcessEffectEditor<Gradient>
    {
        SerializedParameterOverride intensity;
        SerializedParameterOverride input;
        SerializedParameterOverride color1;
        SerializedParameterOverride color2;
        SerializedParameterOverride rotation;
        SerializedParameterOverride gradientTex;
        SerializedParameterOverride mode;

        public override void OnEnable()
        {
            intensity = FindParameterOverride(x => x.intensity);
            input = FindParameterOverride(x => x.input);
            color1 = FindParameterOverride(x => x.color1);
            color2 = FindParameterOverride(x => x.color2);
            rotation = FindParameterOverride(x => x.rotation);
            gradientTex = FindParameterOverride(x => x.gradientTex);
            mode = FindParameterOverride(x => x.mode);
        }

        public override string GetDisplayTitle()
        {
            return base.GetDisplayTitle() + SCPE_GUI.ModeTitle(mode);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("gradient");

            SCPE_GUI.DisplaySetupWarning<GradientRenderer>();

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(input);

            //If Radial
            if (input.value.intValue == 1)
            {
                PropertyField(gradientTex);

                if (gradientTex.value.objectReferenceValue)
                {
                    SCPE.CheckGradientImportSettings(gradientTex.value.objectReferenceValue);
                }

            }
            else
            {
                PropertyField(color1);
                PropertyField(color2);
            }

            PropertyField(mode);
            PropertyField(rotation);

        }

    }
}