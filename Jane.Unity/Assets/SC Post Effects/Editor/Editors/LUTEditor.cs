using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(LUT))]
    public sealed class LUTEditor : PostProcessEffectEditor<LUT>
    {
        SerializedParameterOverride mode;
        SerializedParameterOverride intensity;
        SerializedParameterOverride lutNear;
        SerializedParameterOverride lutFar;
        SerializedParameterOverride invert;
        SerializedParameterOverride startFadeDistance;
        SerializedParameterOverride endFadeDistance;
        
        SerializedParameterOverride vibranceRGBBalance;
        SerializedParameterOverride vibrance;

        public override void OnEnable()
        {
            mode = FindParameterOverride(x => x.mode);
            intensity = FindParameterOverride(x => x.intensity);
            lutNear = FindParameterOverride(x => x.lutNear);
            lutFar = FindParameterOverride(x => x.lutFar);
            invert = FindParameterOverride(x => x.invert);
            
            startFadeDistance = FindParameterOverride(x => x.startFadeDistance);
            endFadeDistance = FindParameterOverride(x => x.endFadeDistance);
            
            vibranceRGBBalance = FindParameterOverride(x => x.vibranceRGBBalance);
            vibrance = FindParameterOverride(x => x.vibrance);
        }

        public override string GetDisplayTitle()
        {
            return "LUT Color Grading" + ((mode.value.intValue == 0) ? "" : " (2-way distance blend)");
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("lut");

            SCPE_GUI.DisplaySetupWarning<LUTRenderer>();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(new GUIContent("Open LUT Extracter", EditorGUIUtility.IconContent("d_PreTextureRGB").image, 
                    "Extract a LUT from the bottom-left corner of a screenshot"), 
                    EditorStyles.miniButton, GUILayout.Height(20f), GUILayout.Width(150f)))
                {
                    LUTExtracterWindow.ShowWindow();
                }
                
                GUILayout.FlexibleSpace();
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
        void CheckLUTImportSettings(SerializedParameterOverride tex)
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