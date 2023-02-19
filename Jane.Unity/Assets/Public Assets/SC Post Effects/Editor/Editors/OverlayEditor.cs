using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(Overlay))]
    sealed class OverlayEditor : VolumeComponentEditor
    {
        SerializedDataParameter overlayTex;
        SerializedDataParameter autoAspect;
        SerializedDataParameter blendMode;
        SerializedDataParameter intensity;
        SerializedDataParameter luminanceThreshold;
        SerializedDataParameter tiling;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Overlay>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<OverlayRenderer>();

            overlayTex = Unpack(o.Find(x => x.overlayTex));
            autoAspect = Unpack(o.Find(x => x.autoAspect));
            blendMode = Unpack(o.Find(x => x.blendMode));
            intensity = Unpack(o.Find(x => x.intensity));
            luminanceThreshold = Unpack(o.Find(x => x.luminanceThreshold));
            tiling = Unpack(o.Find(x => x.tiling));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("overlay");

            SCPE_GUI.DisplaySetupWarning<OverlayRenderer>(ref isSetup);
            PropertyField(overlayTex);

            if (overlayTex.overrideState.boolValue && overlayTex.value.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign a texture", MessageType.Info);
            }

            EditorGUILayout.Space();

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(luminanceThreshold);
            PropertyField(autoAspect);
            PropertyField(blendMode);
            PropertyField(tiling);
        }
    }
}