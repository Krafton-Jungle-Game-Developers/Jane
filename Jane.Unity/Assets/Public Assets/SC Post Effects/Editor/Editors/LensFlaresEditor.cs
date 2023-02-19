using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace SCPE
{
    [VolumeComponentEditor(typeof(LensFlares))]
    sealed class LensFlaresEditor : VolumeComponentEditor
    {
        SerializedDataParameter intensity;
        SerializedDataParameter luminanceThreshold;
        SerializedDataParameter maskTex;
        SerializedDataParameter chromaticAbberation;
        SerializedDataParameter colorTex;

        //Flares
        SerializedDataParameter iterations;
        SerializedDataParameter distance;
        SerializedDataParameter falloff;

        //Halo
        SerializedDataParameter haloSize;
        SerializedDataParameter haloWidth;

        //Blur
        SerializedDataParameter blur;
        SerializedDataParameter passes;


        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<LensFlares>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<LensFlaresRenderer>();

            intensity = Unpack(o.Find(x => x.intensity));
            luminanceThreshold = Unpack(o.Find(x => x.luminanceThreshold));
            maskTex = Unpack(o.Find(x => x.maskTex));
            chromaticAbberation = Unpack(o.Find(x => x.chromaticAbberation));
            colorTex = Unpack(o.Find(x => x.colorTex));

            //Flares
            iterations = Unpack(o.Find(x => x.iterations));
            distance = Unpack(o.Find(x => x.distance));
            falloff = Unpack(o.Find(x => x.falloff));

            //Halo
            haloSize = Unpack(o.Find(x => x.haloSize));
            haloWidth = Unpack(o.Find(x => x.haloWidth));

            //Blur
            blur = Unpack(o.Find(x => x.blur));
            passes = Unpack(o.Find(x => x.passes));
        }
        
        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("lens-flares");

            SCPE_GUI.DisplaySetupWarning<LensFlaresRenderer>(ref isSetup);

            SCPE_GUI.DisplayVRWarning();

            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();
            
            PropertyField(luminanceThreshold);

            //Flares
            PropertyField(iterations);
            if(iterations.value.intValue > 1) PropertyField(distance);
            PropertyField(falloff);

            //Halo
            PropertyField(haloSize);
            PropertyField(haloWidth);

            PropertyField(maskTex);
            PropertyField(chromaticAbberation);
            PropertyField(colorTex, new GUIContent("Gradient"));
            if (colorTex.value.objectReferenceValue)
            {
                SCPE.CheckGradientImportSettings(colorTex.value.objectReferenceValue);
            }

            //Blur
            PropertyField(blur);
            PropertyField(passes);
        }
    }
}