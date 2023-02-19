using UnityEditor;
using UnityEditor.Rendering;

namespace SCPE
{
    [VolumeComponentEditor(typeof(Posterize))]
    sealed class PosterizeEditor : VolumeComponentEditor
    {
        SerializedDataParameter hsvMode;
        SerializedDataParameter levels;
        SerializedDataParameter hue;
        SerializedDataParameter saturation;
        SerializedDataParameter value;
        
        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<Posterize>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<PosterizeRenderer>();

            hsvMode = Unpack(o.Find(x => x.hsvMode));
            levels = Unpack(o.Find(x => x.levels));
            hue = Unpack(o.Find(x => x.hue));
            saturation = Unpack(o.Find(x => x.saturation));
            value = Unpack(o.Find(x => x.value));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("posterize");

            SCPE_GUI.DisplaySetupWarning<PosterizeRenderer>(ref isSetup);

            PropertyField(hsvMode);
            if (hsvMode.value.boolValue)
            {
                PropertyField(hue);
                PropertyField(saturation);
                PropertyField(value);
            }
            else
            {
                PropertyField(levels);
            }
        }
    }
}