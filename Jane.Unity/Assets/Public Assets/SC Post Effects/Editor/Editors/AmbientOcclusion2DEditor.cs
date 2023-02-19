using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.PostProcessing;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(AmbientOcclusion2D))]
    public class AmbientOcclusion2DEditor : PostProcessEffectEditor<AmbientOcclusion2D>
    {
        SerializedParameterOverride aoOnly;
        SerializedParameterOverride intensity;
        SerializedParameterOverride luminanceThreshold;
        SerializedParameterOverride distance;
        SerializedParameterOverride blurAmount;
        SerializedParameterOverride iterations;
        SerializedParameterOverride downscaling;

        public override void OnEnable()
        {
            aoOnly = FindParameterOverride(x => x.aoOnly);
            intensity = FindParameterOverride(x => x.intensity);
            luminanceThreshold = FindParameterOverride(x => x.luminanceThreshold);
            distance = FindParameterOverride(x => x.distance);
            blurAmount = FindParameterOverride(x => x.blurAmount);
            iterations = FindParameterOverride(x => x.iterations);
            downscaling = FindParameterOverride(x => x.downscaling);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("ambient-occlusion-2d");

            SCPE_GUI.DisplayVRWarning();

            SCPE_GUI.DisplaySetupWarning<AmbientOcclusion2DRenderer>();

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(aoOnly);
            PropertyField(luminanceThreshold);
            PropertyField(distance);
            PropertyField(blurAmount);
            PropertyField(iterations);
            PropertyField(downscaling);

        }
    }
}