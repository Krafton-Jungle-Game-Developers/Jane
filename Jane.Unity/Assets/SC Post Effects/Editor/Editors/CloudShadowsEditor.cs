using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(CloudShadows))]
    public sealed class CloudShadowsEditor : PostProcessEffectEditor<CloudShadows>
    {
        SerializedParameterOverride texture;
        SerializedParameterOverride density;
        
        SerializedParameterOverride size;
        SerializedParameterOverride speed;
        SerializedParameterOverride direction;
        SerializedParameterOverride projectFromSun;
        
        SerializedParameterOverride startFadeDistance;
        SerializedParameterOverride endFadeDistance;

        public override void OnEnable()
        {
            texture = FindParameterOverride(x => x.texture);
            size = FindParameterOverride(x => x.size);
            density = FindParameterOverride(x => x.density);
            speed = FindParameterOverride(x => x.speed);
            direction = FindParameterOverride(x => x.direction);
            projectFromSun = FindParameterOverride(x => x.projectFromSun);
            
            startFadeDistance = FindParameterOverride(x => x.startFadeDistance);
            endFadeDistance = FindParameterOverride(x => x.endFadeDistance);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("cloud-shadows");

            SCPE_GUI.DisplayVRWarning(true);
            
            SCPE_GUI.DisplaySetupWarning<CloudShadowsRenderer>();

            PropertyField(density);
            SCPE_GUI.DisplayIntensityWarning(density);
            
            EditorGUILayout.Space();
            
            PropertyField(texture);

            PropertyField(size);
            PropertyField(speed);
            PropertyField(direction);
            PropertyField(projectFromSun);
            if(projectFromSun.value.boolValue) SCPE_GUI.DrawSunInfo();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Distance Fading");
            PropertyField(startFadeDistance);
            PropertyField(endFadeDistance);
        }
    }
}