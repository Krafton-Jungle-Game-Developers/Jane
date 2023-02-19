using UnityEngine.Rendering.Universal;
using UnityEditor.Rendering;
using UnityEditor;
using UnityEngine;

namespace SCPE
{
    [VolumeComponentEditor(typeof(AmbientOcclusion2D))]
    sealed class AmbientOcclusion2DEditor : VolumeComponentEditor
    {
        SerializedDataParameter aoOnly;
        SerializedDataParameter intensity;
        SerializedDataParameter luminanceThreshold;
        SerializedDataParameter distance;
        SerializedDataParameter blurAmount;
        SerializedDataParameter iterations;
        SerializedDataParameter downscaling;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<AmbientOcclusion2D>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<AmbientOcclusion2DRenderer>();

            aoOnly = Unpack(o.Find(x => x.aoOnly));
            intensity = Unpack(o.Find(x => x.intensity));
            luminanceThreshold = Unpack(o.Find(x => x.luminanceThreshold));
            distance = Unpack(o.Find(x => x.distance));
            blurAmount = Unpack(o.Find(x => x.blurAmount));
            iterations = Unpack(o.Find(x => x.iterations));
            downscaling = Unpack(o.Find(x => x.downscaling));
        }
        
        public override void OnInspectorGUI()
        {
            #if !URP_12_0_OR_NEWER && !SCPE_DEV
            EditorGUILayout.HelpBox("Only compatible with Unity 2021.2.0 or newer", MessageType.Error);
            #else
            SCPE_GUI.DisplayDocumentationButton("ambient-occlusion-2d");

            SCPE_GUI.DisplaySetupWarning<AmbientOcclusion2DRenderer>(ref isSetup);

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(aoOnly);
            PropertyField(luminanceThreshold);
            PropertyField(distance);
            PropertyField(blurAmount);
            PropertyField(iterations);
            PropertyField(downscaling);
            #endif
        }
    }
}