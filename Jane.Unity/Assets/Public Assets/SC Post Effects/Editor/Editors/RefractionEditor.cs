using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(Refraction))]
    sealed class RefractionEditor : VolumeComponentEditor
    {
        SerializedDataParameter refractionTex;
        SerializedDataParameter convertNormalMap;
        SerializedDataParameter amount;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Refraction>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<RefractionRenderer>();

            refractionTex = Unpack(o.Find(x => x.refractionTex));
            convertNormalMap = Unpack(o.Find(x => x.convertNormalMap));
            amount = Unpack(o.Find(x => x.amount));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("refraction");

            SCPE_GUI.DisplaySetupWarning<RefractionRenderer>(ref isSetup);

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