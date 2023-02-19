using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(Refraction))]
    public sealed class RefractionEditor : PostProcessEffectEditor<Refraction>
    {
        SerializedParameterOverride refractionTex;
        SerializedParameterOverride convertNormalMap;
        SerializedParameterOverride amount;

        public override void OnEnable()
        {
            amount = FindParameterOverride(x => x.amount);
            convertNormalMap = FindParameterOverride(x => x.convertNormalMap);
            refractionTex = FindParameterOverride(x => x.refractionTex);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("refraction");

            SCPE_GUI.DisplaySetupWarning<RefractionRenderer>();

            PropertyField(amount);
            SCPE_GUI.DisplayIntensityWarning(amount);
            
            EditorGUILayout.Space();
            
            PropertyField(refractionTex);

            if (refractionTex.overrideState.boolValue && refractionTex.value.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign a texture", MessageType.Info);
            }

            PropertyField(convertNormalMap);

            EditorGUILayout.Space();

        }
    }
}