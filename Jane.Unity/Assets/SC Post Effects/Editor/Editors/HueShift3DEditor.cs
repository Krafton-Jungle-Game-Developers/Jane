using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(HueShift3D))]
    public sealed class HueShift3DEditor : PostProcessEffectEditor<HueShift3D>
    {
        SerializedParameterOverride colorSource;
        SerializedParameterOverride gradientTex;
        SerializedParameterOverride intensity;
        SerializedParameterOverride speed;
        SerializedParameterOverride size;
        SerializedParameterOverride geoInfluence;

        public override void OnEnable()
        {
            colorSource = FindParameterOverride(x => x.colorSource);
            gradientTex = FindParameterOverride(x => x.gradientTex);
            intensity = FindParameterOverride(x => x.intensity);
            speed = FindParameterOverride(x => x.speed);
            size = FindParameterOverride(x => x.size);
            geoInfluence = FindParameterOverride(x => x.geoInfluence);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("hue-shift-3d");

            SCPE_GUI.DisplaySetupWarning<HueShift3DRenderer>();

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(colorSource);
            if(colorSource.value.intValue == (int)HueShift3D.ColorSource.GradientTexture) PropertyField(gradientTex);
            PropertyField(speed);
            PropertyField(size);

            EditorGUI.BeginDisabledGroup(HueShift3D.isOrtho || GraphicsSettings.renderPipelineAsset != null);
            {
                PropertyField(geoInfluence);
                if (HueShift3D.isOrtho) EditorGUILayout.HelpBox("Not available for orthographic cameras", MessageType.None);
                if (GraphicsSettings.renderPipelineAsset != null) EditorGUILayout.HelpBox("Not available when using a scriptable render pipeline", MessageType.None);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}