using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace SCPE
{
    [VolumeComponentEditor(typeof(LUT))]
    sealed class LUTEditor : VolumeComponentEditor
    {
        LUT effect;
        SerializedDataParameter mode;
        SerializedDataParameter intensity;
        SerializedDataParameter lutNear;
        SerializedDataParameter lutFar;
        SerializedDataParameter invert;
        SerializedDataParameter startFadeDistance;
        SerializedDataParameter endFadeDistance;
        
        SerializedDataParameter vibranceRGBBalance;
        SerializedDataParameter vibrance;
        
        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            effect = (LUT)target;
            var o = new PropertyFetcher<LUT>(serializedObject);

            isSetup = AutoSetup.ValidEffectSetup<LUTRenderer>();

            mode = Unpack(o.Find(x => x.mode));
            intensity = Unpack(o.Find(x => x.intensity));
            lutNear = Unpack(o.Find(x => x.lutNear));
            lutFar = Unpack(o.Find(x => x.lutFar));
            invert = Unpack(o.Find(x => x.invert));
            startFadeDistance = Unpack(o.Find(x => x.startFadeDistance));
            endFadeDistance = Unpack(o.Find(x => x.endFadeDistance));
            vibranceRGBBalance = Unpack(o.Find(x => x.vibranceRGBBalance));
            vibrance = Unpack(o.Find(x => x.vibrance));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("lut");

            SCPE_GUI.DisplaySetupWarning<LUTRenderer>(ref isSetup);
            
            using (new EditorGUILayout.HorizontalScope())
            {
                //GUILayout.FlexibleSpace();

                if (GUILayout.Button(new GUIContent("Open LUT Extracter", EditorGUIUtility.IconContent("d_PreTextureRGB").image,
                    "Extract a LUT from the bottom-left corner of a screenshot"),
                    EditorStyles.miniButton, GUILayout.Height(20f), GUILayout.Width(150f)))
                {
                    LUTExtracterWindow.ShowWindow();
                }
            }

            EditorGUILayout.Space();
            
            PropertyField(intensity);
            SCPE_GUI.DisplayIntensityWarning(intensity);
            
            EditorGUILayout.Space();

            CheckLUTImportSettings(lutNear);
            if (mode.value.intValue == (int)LUT.Mode.DistanceBased) CheckLUTImportSettings(lutFar);

            PropertyField(mode);
            if (mode.value.intValue == (int)LUT.Mode.DistanceBased)
            {
                PropertyField(startFadeDistance);
                PropertyField(endFadeDistance);
            }
            
            EditorGUILayout.Space();
            
            PropertyField(lutNear, new GUIContent(mode.value.intValue == 0 ? "Look up Texture" : "Near"));
            if (mode.value.intValue == (int)LUT.Mode.DistanceBased)
            {
                PropertyField(lutFar);
            }
            
            EditorGUILayout.Space();

            PropertyField(invert);
            
            EditorGUILayout.Space();
            
            PropertyField(vibrance);
            PropertyField(vibranceRGBBalance);
        }

        // Checks import settings on the lut, offers to fix them if invalid
        void CheckLUTImportSettings(SerializedDataParameter tex)
        {
            if (tex != null)
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex.value.objectReferenceValue));

                if (importer != null) // Fails when using a non-persistent texture
                {
                    bool valid = importer.anisoLevel == 0
                        && importer.mipmapEnabled == false
                        && importer.sRGBTexture == false
                        && (importer.textureCompression == TextureImporterCompression.Uncompressed)
                        && importer.wrapMode == TextureWrapMode.Clamp;

                    if (!valid)
                    {
                        EditorGUILayout.HelpBox("\"" + tex.value.objectReferenceValue.name + "\" has invalid LUT import settings.", MessageType.Warning);

                        GUILayout.Space(-32);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Fix", GUILayout.Width(60)))
                            {
                                SetLUTImportSettings(importer);
                                AssetDatabase.Refresh();
                            }
                            GUILayout.Space(8);
                        }
                        GUILayout.Space(11);
                    }
                }
                else
                {
                    tex.value.objectReferenceValue = null;
                }
            }
        }

        void SetLUTImportSettings(TextureImporter importer)
        {
            importer.textureType = TextureImporterType.Default;
            // importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.sRGBTexture = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.anisoLevel = 0;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }

    }
}