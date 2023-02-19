using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(Dithering))]
    public sealed class DitheringEditor : PostProcessEffectEditor<Dithering>
    {
        SerializedParameterOverride intensity;
        SerializedParameterOverride tiling;
        SerializedParameterOverride luminanceThreshold;
        SerializedParameterOverride lut;
#if DITHERING_WORLD_PROJECTION
        SerializedParameterOverride worldProjected;
#endif

        public override void OnEnable()
        {
            lut = FindParameterOverride(x => x.lut);
            intensity = FindParameterOverride(x => x.intensity);
            tiling = FindParameterOverride(x => x.tiling);
            luminanceThreshold = FindParameterOverride(x => x.luminanceThreshold);
#if DITHERING_WORLD_PROJECTION
            worldProjected = FindParameterOverride(x => x.worldProjected);
#endif
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("dithering");

            SCPE_GUI.DisplaySetupWarning<DitheringRenderer>();

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(lut);

            if (lut.overrideState.boolValue && lut.value.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign a pattern texture", MessageType.Info);
            }

            EditorGUILayout.Space();

            PropertyField(luminanceThreshold);
#if DITHERING_WORLD_PROJECTION
            PropertyField(worldProjected);
#endif
            PropertyField(tiling);
        }
    }
}