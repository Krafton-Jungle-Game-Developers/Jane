using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.SceneManagement;
using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

namespace SCPE
{
    [PostProcessEditor(typeof(Fog))]
    public sealed class FogEditor : PostProcessEffectEditor<Fog>
    {
        SerializedParameterOverride useSceneSettings;
        SerializedParameterOverride fogMode;
        SerializedParameterOverride fogDensity;
        SerializedParameterOverride fogStartDistance;
        SerializedParameterOverride fogEndDistance;

        SerializedParameterOverride colorMode;
#if PPS_DEV
        SerializedParameterOverride skyboxMipLevel;
#endif
        SerializedParameterOverride fogColor;
        SerializedParameterOverride fogColorGradient;
        SerializedParameterOverride gradientDistance;
        SerializedParameterOverride gradientUseFarClipPlane;

        SerializedParameterOverride distanceFog;
        SerializedParameterOverride distanceDensity;
        SerializedParameterOverride useRadialDistance;

        SerializedParameterOverride skyboxInfluence;

        SerializedParameterOverride enableDirectionalLight;
        SerializedParameterOverride useLightDirection;
        SerializedParameterOverride useLightColor;
        SerializedParameterOverride useLightIntensity;
        SerializedParameterOverride lightColor;
        SerializedParameterOverride lightDirection;
        SerializedParameterOverride lightIntensity;

        SerializedParameterOverride heightFog;
        SerializedParameterOverride height;
        SerializedParameterOverride heightDensity;

        SerializedParameterOverride heightFogNoise;
        SerializedParameterOverride heightNoiseTex;
        SerializedParameterOverride heightNoiseSize;
        SerializedParameterOverride heightNoiseStrength;
        SerializedParameterOverride heightNoiseSpeed;

        SerializedParameterOverride lightScattering;
        SerializedParameterOverride scatterIntensity;
        SerializedParameterOverride scatterDiffusion;
        SerializedParameterOverride scatterThreshold;
        SerializedParameterOverride scatterSoftKnee;

        private float animSpeed = 4f;
        AnimBool m_showControls;
        AnimBool m_showHeight;
        AnimBool m_showSun;
        AnimBool m_showScattering;

        public override void OnEnable()
        {
            useSceneSettings = FindParameterOverride(x => x.useSceneSettings);
            fogMode = FindParameterOverride(x => x.fogMode);
            fogDensity = FindParameterOverride(x => x.globalDensity);
            fogStartDistance = FindParameterOverride(x => x.fogStartDistance);
            fogDensity = FindParameterOverride(x => x.globalDensity);
            fogEndDistance = FindParameterOverride(x => x.fogEndDistance);
            colorMode = FindParameterOverride(x => x.colorSource);
#if PPS_DEV
            skyboxMipLevel = FindParameterOverride(x => x.skyboxMipLevel);
#endif
            fogColor = FindParameterOverride(x => x.fogColor);
            fogColorGradient = FindParameterOverride(x => x.fogColorGradient);
            gradientDistance = FindParameterOverride(x => x.gradientDistance);
            gradientUseFarClipPlane = FindParameterOverride(x => x.gradientUseFarClipPlane);
            distanceFog = FindParameterOverride(x => x.distanceFog);
            distanceDensity = FindParameterOverride(x => x.distanceDensity);
            useRadialDistance = FindParameterOverride(x => x.useRadialDistance);
            heightFog = FindParameterOverride(x => x.heightFog);
            height = FindParameterOverride(x => x.height);
            heightDensity = FindParameterOverride(x => x.heightDensity);
            heightFogNoise = FindParameterOverride(x => x.heightFogNoise);
            heightNoiseTex = FindParameterOverride(x => x.heightNoiseTex);
            heightNoiseSize = FindParameterOverride(x => x.heightNoiseSize);
            heightNoiseStrength = FindParameterOverride(x => x.heightNoiseStrength);
            heightNoiseSpeed = FindParameterOverride(x => x.heightNoiseSpeed);

            skyboxInfluence = FindParameterOverride(x => x.skyboxInfluence);

            enableDirectionalLight = FindParameterOverride(x => x.enableDirectionalLight);
            useLightDirection = FindParameterOverride(x => x.useLightDirection);
            useLightColor = FindParameterOverride(x => x.useLightColor);
            useLightIntensity = FindParameterOverride(x => x.useLightIntensity);
            lightColor = FindParameterOverride(x => x.lightColor);
            lightDirection = FindParameterOverride(x => x.lightDirection);
            lightIntensity = FindParameterOverride(x => x.lightIntensity);


            lightScattering = FindParameterOverride(x => x.lightScattering);
            scatterIntensity = FindParameterOverride(x => x.scatterIntensity);
            scatterDiffusion = FindParameterOverride(x => x.scatterDiffusion);
            scatterThreshold = FindParameterOverride(x => x.scatterThreshold);
            scatterSoftKnee = FindParameterOverride(x => x.scatterSoftKnee);


            m_showControls = new AnimBool(true);
            m_showControls.valueChanged.AddListener(Repaint);
            m_showControls.speed = animSpeed;

            m_showHeight = new AnimBool(true);
            m_showHeight.valueChanged.AddListener(Repaint);
            m_showHeight.speed = animSpeed;

            m_showSun = new AnimBool(true);
            m_showSun.valueChanged.AddListener(Repaint);
            m_showSun.speed = animSpeed;

            m_showScattering = new AnimBool(true);
            m_showScattering.valueChanged.AddListener(Repaint);
            m_showScattering.speed = animSpeed;
        }

        public override string GetDisplayTitle()
        {
            return "Screen-Space Fog (" + colorMode.value.enumDisplayNames[colorMode.value.intValue] + ")";
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("fog");

            SCPE_GUI.DisplayVRWarning(true);

            SCPE_GUI.DisplaySetupWarning<FogRenderer>();

            if (RenderSettings.fog)
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.HelpBox("Fog is currently enabled in the active scene, resulting in an overlapping fog effect", MessageType.Warning);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Disable scene fog"))
                        {
                            RenderSettings.fog = false;
							EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                        GUILayout.FlexibleSpace();
                    }


                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
            }
            PropertyField(useSceneSettings);
            EditorGUILayout.Space();

            m_showControls.target = !useSceneSettings.value.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_showControls.faded))
            {
                PropertyField(fogMode);
                PropertyField(fogStartDistance);
                if (fogMode.value.intValue == 1)
                {
                    PropertyField(fogEndDistance);
                }
                else
                {
                    PropertyField(fogDensity);
                }

                PropertyField(colorMode);
                if (colorMode.value.intValue == 0)
                {
                    PropertyField(fogColor);
                }
                else if (colorMode.value.intValue == 1) //gradient
                {
                    PropertyField(fogColorGradient);

                    if (fogColorGradient.value.objectReferenceValue)
                    {
                        SCPE.CheckGradientImportSettings(fogColorGradient.value.objectReferenceValue);

                        GUILayout.Space(5f);
                        //Gradient preview
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Near", GUILayout.MaxWidth(32f));
                            Rect rect = GUILayoutUtility.GetRect(1, 12, "TextField");
                            EditorGUI.DrawTextureTransparent(rect, fogColorGradient.value.objectReferenceValue as Texture2D);
                            EditorGUILayout.LabelField("Far", GUILayout.MaxWidth(30f));
                        }

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUI.BeginDisabledGroup(!gradientUseFarClipPlane.value.boolValue);
                            {
                                PropertyField(gradientDistance, new GUIContent("Distance"));
                            }
                            EditorGUI.EndDisabledGroup();
                            gradientUseFarClipPlane.overrideState.boolValue = true;
                            gradientUseFarClipPlane.value.boolValue = !GUILayout.Toggle(!gradientUseFarClipPlane.value.boolValue, new GUIContent("Automatic", "Distance will be set by the camera's far clipping plane"), EditorStyles.miniButton);
                        }
                        EditorGUILayout.EndHorizontal();

                        //EditorGUILayout.HelpBox("In scene view, the gradient will appear to look different due to the camera's dynamic far clipping value", MessageType.None);
                    }
                }
                else if (colorMode.value.intValue == 2) //Skybox
                {
#if !LWRP_5_7_2_OR_NEWER
                    if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset)
                    {
                        if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.name.Contains("LWRP")) EditorGUILayout.HelpBox("This feature requires atleast LWRP version 5.7.2. If you're using this version, rerun the installer through the Help window", MessageType.Error);
                    }
#endif
                    if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset)
                    {
                        if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.name.Contains("HDR")) EditorGUILayout.HelpBox("This feature is not supported in the HDRP", MessageType.Error);
                    }
#if PPS_DEV  //Hide parameter, implementation isn't ideal and will cause pixel marching, won't look good in any case
                    PropertyField(skyboxMipLevel);
#endif
                }
            }
            EditorGUILayout.EndFadeGroup();

            PropertyField(distanceFog);
            if (colorMode.value.intValue == 1 && distanceFog.value.boolValue == false && heightFog.value.boolValue)
            {
                EditorGUILayout.HelpBox("Distance fog must be enabled when using the Gradient Texture color mode. Height fog only will not be colored correctly", MessageType.Warning);
            }
            if (distanceFog.value.boolValue)
            {
                PropertyField(useRadialDistance, new GUIContent("Radial"));
                PropertyField(distanceDensity);
            }

            //Always exclude skybox for skybox color mode
            PropertyField(skyboxInfluence, new GUIContent("Influence"));

            PropertyField(enableDirectionalLight);

            m_showSun.target = enableDirectionalLight.value.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_showSun.faded))
            {
                SCPE_GUI.DrawSunInfo();

                PropertyField(useLightDirection);
                if (!useLightDirection.value.boolValue) PropertyField(lightDirection);
                PropertyField(useLightColor);
                if (!useLightColor.value.boolValue) PropertyField(lightColor);
                PropertyField(useLightIntensity);
                if (!useLightIntensity.value.boolValue) PropertyField(lightIntensity);
            }
            EditorGUILayout.EndFadeGroup();

            PropertyField(heightFog);
            m_showHeight.target = heightFog.value.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_showHeight.faded))
            {
                if (RuntimeUtilities.isSinglePassStereoSelected)
                {
                    EditorGUILayout.HelpBox("Not supported in VR", MessageType.Warning);
                }
                PropertyField(height);
                PropertyField(heightDensity);

                PropertyField(heightFogNoise);
                if (heightFogNoise.value.boolValue)
                {
#if UNITY_2018_1_OR_NEWER
                    //EditorGUILayout.HelpBox("Fog noise is currently bugged when using the Post Processing installed through the Package Manager.", MessageType.Warning);
#endif
                    PropertyField(heightNoiseTex);
                    if (heightNoiseTex.value.objectReferenceValue)
                    {
                        PropertyField(heightNoiseSize);
                        PropertyField(heightNoiseStrength);
                        PropertyField(heightNoiseSpeed);
                    }

                }
            }
            EditorGUILayout.EndFadeGroup();

            PropertyField(lightScattering);
            m_showScattering.target = lightScattering.value.boolValue;
            if (EditorGUILayout.BeginFadeGroup(m_showScattering.faded))
            {
                PropertyField(scatterIntensity);
                PropertyField(scatterThreshold);
                PropertyField(scatterDiffusion);
                PropertyField(scatterSoftKnee);
            }
            EditorGUILayout.EndFadeGroup();

        }
    }
}