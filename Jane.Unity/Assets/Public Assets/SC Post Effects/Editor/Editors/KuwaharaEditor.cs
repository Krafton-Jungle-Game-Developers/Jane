using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace SCPE
{
    [VolumeComponentEditor(typeof(Kuwahara))]
    sealed class KuwaharaEditor : VolumeComponentEditor
    {
        SerializedDataParameter mode;
        SerializedDataParameter radius;

        SerializedDataParameter startFadeDistance;
        SerializedDataParameter endFadeDistance;

        private bool isOrthographic = false;
        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Kuwahara>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<KuwaharaRenderer>();

            mode = Unpack(o.Find(x => x.mode));
            radius = Unpack(o.Find(x => x.radius));
            startFadeDistance = Unpack(o.Find(x => x.startFadeDistance));
            endFadeDistance = Unpack(o.Find(x => x.endFadeDistance));

            if (Camera.current) isOrthographic = Camera.current.orthographic;
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("kuwahara");

            SCPE_GUI.DisplaySetupWarning<KuwaharaRenderer>(ref isSetup);

            SCPE_GUI.ShowDepthTextureWarning(mode.value.intValue == 1);

            EditorGUI.BeginDisabledGroup(isOrthographic);
            
            PropertyField(radius);
            SCPE_GUI.DisplayIntensityWarning(radius);
            
            EditorGUILayout.Space();
            
            PropertyField(mode);
            EditorGUI.EndDisabledGroup();

            if (isOrthographic)
            {
                mode.value.intValue = 0;
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