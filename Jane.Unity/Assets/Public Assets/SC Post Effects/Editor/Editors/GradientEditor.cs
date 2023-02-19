using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(Gradient))]
    sealed class GradientEditor : VolumeComponentEditor
    {
        SerializedDataParameter intensity;
        SerializedDataParameter input;
        SerializedDataParameter color1;
        SerializedDataParameter color2;
        SerializedDataParameter rotation;
        SerializedDataParameter gradientTex;
        SerializedDataParameter mode;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Gradient>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<GradientRenderer>();

            intensity = Unpack(o.Find(x => x.intensity));
            input = Unpack(o.Find(x => x.input));
            color1 = Unpack(o.Find(x => x.color1));
            color2 = Unpack(o.Find(x => x.color2));
            rotation = Unpack(o.Find(x => x.rotation));
            gradientTex = Unpack(o.Find(x => x.gradientTex));
            mode = Unpack(o.Find(x => x.mode));
        }


        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("gradient");

            SCPE_GUI.DisplaySetupWarning<GradientRenderer>(ref isSetup);

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