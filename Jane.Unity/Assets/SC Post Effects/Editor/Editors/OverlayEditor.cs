using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(Overlay))]
    public sealed class OverlayEditor : PostProcessEffectEditor<Overlay>
    {
        SerializedParameterOverride overlayTex;
        SerializedParameterOverride autoAspect;
        SerializedParameterOverride blendMode;
        SerializedParameterOverride intensity;
        SerializedParameterOverride luminanceThreshold;
        SerializedParameterOverride tiling;

        public override void OnEnable()
        {
            overlayTex = FindParameterOverride(x => x.overlayTex);
            autoAspect = FindParameterOverride(x => x.autoAspect);
            blendMode = FindParameterOverride(x => x.blendMode);
            intensity = FindParameterOverride(x => x.intensity);
            luminanceThreshold = FindParameterOverride(x => x.luminanceThreshold);
            tiling = FindParameterOverride(x => x.tiling);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("overlay");

            SCPE_GUI.DisplaySetupWarning<OverlayRenderer>();

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(overlayTex);

            if (overlayTex.overrideState.boolValue && overlayTex.value.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign a texture", MessageType.Info);
            }

            EditorGUILayout.Space();

            PropertyField(luminanceThreshold);
            PropertyField(autoAspect);
            PropertyField(blendMode);
            PropertyField(tiling);
        }
    }
}