using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(Kuwahara))]
    public class KuwaharaEditor : PostProcessEffectEditor<Kuwahara>
    {
        SerializedParameterOverride mode;
        SerializedParameterOverride radius;

        SerializedParameterOverride startFadeDistance;
        SerializedParameterOverride endFadeDistance;

        private bool isOrthographic = false;

        public override void OnEnable()
        {
            mode = FindParameterOverride(x => x.mode);
            radius = FindParameterOverride(x => x.radius);
            startFadeDistance = FindParameterOverride(x => x.startFadeDistance);
            endFadeDistance = FindParameterOverride(x => x.endFadeDistance);

            if (Camera.current) isOrthographic = Camera.current.orthographic;
        }

        public override string GetDisplayTitle()
        {
            return "Kuwahara" + ((mode.value.intValue == 0) ? "" : " (Depth Fade)");
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("kuwahara");

            SCPE_GUI.DisplaySetupWarning<KuwaharaRenderer>();

            EditorGUI.BeginDisabledGroup(isOrthographic);
            
            PropertyField(radius);
            SCPE_GUI.DisplayIntensityWarning(radius);
            
            EditorGUILayout.Space();
            
            PropertyField(mode);
            EditorGUI.EndDisabledGroup();

            if (isOrthographic)
            {
                EditorGUILayout.HelpBox("Depth fade is disabled for orthographic cameras", MessageType.Info);
            }
            
            if (mode.value.intValue == (int)Kuwahara.KuwaharaMode.DepthFade)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Distance Fading");
                
                PropertyField(startFadeDistance);
                PropertyField(endFadeDistance);
            }
        }
    }
}