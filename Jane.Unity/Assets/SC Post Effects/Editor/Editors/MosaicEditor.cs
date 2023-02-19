using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(Mosaic))]
    public sealed class MosaicEditor : PostProcessEffectEditor<Mosaic>
    {
        SerializedParameterOverride mode;
        SerializedParameterOverride size;

        public override void OnEnable()
        {
            mode = FindParameterOverride(x => x.mode);
            size = FindParameterOverride(x => x.size);
        }

        public override string GetDisplayTitle()
        {
            return base.GetDisplayTitle() + SCPE_GUI.ModeTitle(mode);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("mosaic");

            SCPE_GUI.DisplaySetupWarning<MosaicRenderer>();

            PropertyField(size);
            SCPE_GUI.DisplayIntensityWarning(size);
            
            EditorGUILayout.Space();
            
            PropertyField(mode);
        }
    }
}