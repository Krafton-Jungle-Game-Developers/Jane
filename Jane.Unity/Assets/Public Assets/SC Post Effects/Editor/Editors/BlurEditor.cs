using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Rendering.PostProcessing;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(Blur))]
    public class BlurEditor : PostProcessEffectEditor<Blur>
    {
        SerializedParameterOverride mode;
        SerializedParameterOverride highQuality;
        SerializedParameterOverride amount;
        SerializedParameterOverride iterations;
        SerializedParameterOverride downscaling;
        
        SerializedParameterOverride distanceFade;
        SerializedParameterOverride startFadeDistance;
        SerializedParameterOverride endFadeDistance;

        public override void OnEnable()
        {
            mode = FindParameterOverride(x => x.mode);
            highQuality = FindParameterOverride(x => x.highQuality);
            amount = FindParameterOverride(x => x.amount);
            iterations = FindParameterOverride(x => x.iterations);
            downscaling = FindParameterOverride(x => x.downscaling);
            
            distanceFade = FindParameterOverride(x => x.distanceFade);
            startFadeDistance = FindParameterOverride(x => x.startFadeDistance);
            endFadeDistance = FindParameterOverride(x => x.endFadeDistance);
        }

        public override string GetDisplayTitle()
        {
            return base.GetDisplayTitle() + " (" + mode.value.enumDisplayNames[mode.value.intValue] + ")";
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("blur");

            SCPE_GUI.DisplaySetupWarning<BlurRenderer>();

            PropertyField(amount);
            SCPE_GUI.DisplayIntensityWarning(amount);
            
            EditorGUILayout.Space();
            
            PropertyField(mode);
            PropertyField(highQuality);
            PropertyField(iterations);
            PropertyField(downscaling);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Distance Fading");
            PropertyField(distanceFade);
            if (distanceFade.value.boolValue)
            {
                PropertyField(startFadeDistance);
                PropertyField(endFadeDistance);
            }


        }
    }
}