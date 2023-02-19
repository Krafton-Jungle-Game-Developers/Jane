using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(Danger))]
    public sealed class DangerEditor : PostProcessEffectEditor<Danger>
    {
        SerializedParameterOverride overlayTex;
        SerializedParameterOverride color;
        SerializedParameterOverride intensity;
        SerializedParameterOverride size;

        public override void OnEnable()
        {
            overlayTex = FindParameterOverride(x => x.overlayTex);
            color = FindParameterOverride(x => x.color);
            intensity = FindParameterOverride(x => x.intensity);
            size = FindParameterOverride(x => x.size);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("danger");

            SCPE_GUI.DisplaySetupWarning<DangerRenderer>();

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(overlayTex);

            if (overlayTex.overrideState.boolValue && overlayTex.value.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign a texture", MessageType.Info);
            }

            PropertyField(color);
            PropertyField(size);
        }
    }
}