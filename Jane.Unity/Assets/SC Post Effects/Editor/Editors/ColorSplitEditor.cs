using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(ColorSplit))]
    public sealed class ColorSplitEditor : PostProcessEffectEditor<ColorSplit>
    {
        SerializedParameterOverride mode;
        SerializedParameterOverride offset;

        public override void OnEnable()
        {
            mode = FindParameterOverride(x => x.mode);
            offset = FindParameterOverride(x => x.offset);
        }

        public override string GetDisplayTitle()
        {
            return base.GetDisplayTitle() + SCPE_GUI.ModeTitle(mode);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("color-split");

            SCPE_GUI.DisplaySetupWarning<ColorSplitRenderer>();

            PropertyField(offset);
            SCPE_GUI.DisplayIntensityWarning(offset);
            
            EditorGUILayout.Space();
            
            PropertyField(mode);
        }
    }
}