using SCPE;
using UnityEditor;
using UnityEngine.Rendering.PostProcessing;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(BlackBars))]
    public class BlackBarsEditor : PostProcessEffectEditor<BlackBars>
    {
        SerializedParameterOverride mode;
        SerializedParameterOverride size;
        SerializedParameterOverride maxSize;

        public override void OnEnable()
        {
            mode = FindParameterOverride(x => x.mode);
            size = FindParameterOverride(x => x.size);
            maxSize = FindParameterOverride(x => x.maxSize);
        }

        public override string GetDisplayTitle()
        {
            return "Black Bars (" + (BlackBars.Direction)mode.value.enumValueIndex + ")";
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("black-bars");

            SCPE_GUI.DisplaySetupWarning<BlackBarsRenderer>();

            PropertyField(size);
            SCPE_GUI.DisplayIntensityWarning(size);
            
            EditorGUILayout.Space();
            
            PropertyField(mode);
            PropertyField(maxSize);
        }
    }
}