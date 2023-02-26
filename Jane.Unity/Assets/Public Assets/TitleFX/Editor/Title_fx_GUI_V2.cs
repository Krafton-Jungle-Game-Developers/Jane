using UnityEngine;
using UnityEditor;

public class Title_fx_GUI_V2 : ShaderGUI
{

    MaterialEditor editor;
    MaterialProperty[] properties;
    bool TargetMode;

    //get preperties function
    MaterialProperty FindProperty(string name)
    {
        return FindProperty(name, properties);
    }
    //

    ////
    static GUIContent staticLabel = new GUIContent();
    static GUIContent MakeLabel(MaterialProperty property, string tooltip = null)
    {
        staticLabel.text = property.displayName;
        staticLabel.tooltip = tooltip;
        return staticLabel;
    }
    ////

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        this.editor = editor;
        this.properties = properties;
        DoMain();

    }


    // GUI FUNCTION	
    void DoMain()
    {
        //--- Logo
        Texture2D myGUITexture = (Texture2D)Resources.Load("TitleFX_PACK");
        GUILayout.Label(myGUITexture, EditorStyles.centeredGreyMiniLabel);

        //LABELS
        GUILayout.Label("/---------------/ TITLE FX PACK | Shader B /---------------/", EditorStyles.centeredGreyMiniLabel);
        GUILayout.Label("MAIN MAP", EditorStyles.helpBox);
        
        // Main
        // get properties
        MaterialProperty _Main = ShaderGUI.FindProperty("_Main", properties);
        //Add to GUI
        editor.TexturePropertySingleLine(MakeLabel(_Main, "Main Map"), _Main);

        //Color
        MaterialProperty _MainColor = FindProperty("_MainColor");
        editor.ShaderProperty(_MainColor, MakeLabel(_MainColor));
/*
        //Outline Map
        // get properties
        MaterialProperty _Outline = ShaderGUI.FindProperty("_Outline", properties);

        //Add to GUI
        editor.TexturePropertySingleLine(MakeLabel(_Outline, "Outline Map"), _Outline);
        //Color
        MaterialProperty _OutlineColor = FindProperty("_OutlineColor");
        editor.ShaderProperty(_OutlineColor, MakeLabel(_OutlineColor));
        */
        


        GUILayout.Label("MASK MAPS", EditorStyles.helpBox);

        // MASK
        // get properties
        MaterialProperty _MainMASK = ShaderGUI.FindProperty("_MainMASK", properties);
        //Add to GUI
        editor.TexturePropertySingleLine(MakeLabel(_MainMASK, "Main MASK Map"), _MainMASK);

        //Transition
        MaterialProperty _TransitionFactor = FindProperty("_TransitionFactor");
        editor.ShaderProperty(_TransitionFactor, MakeLabel(_TransitionFactor));

        // MASK DETAILS
        // get properties
        MaterialProperty _DetailsMASK = ShaderGUI.FindProperty("_DetailsMASK", properties);
        //Add to GUI
        editor.TexturePropertySingleLine(MakeLabel(_DetailsMASK, "Details MASK Map"), _DetailsMASK);

        //Details Distortion
        MaterialProperty _DetailsMaskDistortionMult = FindProperty("_DetailsMaskDistortionMult");
        editor.ShaderProperty(_DetailsMaskDistortionMult, MakeLabel(_DetailsMaskDistortionMult));

        GUILayout.Label("MASKS TILING", EditorStyles.helpBox);
        //TILING
        MaterialProperty _XTiling = FindProperty("_XTiling");
        editor.ShaderProperty(_XTiling, MakeLabel(_XTiling));

        MaterialProperty _YTiling = FindProperty("_YTiling");
        editor.ShaderProperty(_YTiling, MakeLabel(_YTiling));

        GUILayout.Label("SETTING", EditorStyles.helpBox);

        //DIRECTION
        MaterialProperty _InverseDirection = FindProperty("_InverseDirection");
        editor.ShaderProperty(_InverseDirection, MakeLabel(_InverseDirection));

        MaterialProperty _UpDownDirection = FindProperty("_UpDownDirection");
        editor.ShaderProperty(_UpDownDirection, MakeLabel(_UpDownDirection));

        MaterialProperty _TransitionSpeed = FindProperty("_TransitionSpeed");
        editor.ShaderProperty(_TransitionSpeed, MakeLabel(_TransitionSpeed));


        GUILayout.Label("Animation", EditorStyles.helpBox);
        MaterialProperty _AutoManualAnimation = FindProperty("_AutoManualAnimation");
        editor.ShaderProperty(_AutoManualAnimation, MakeLabel(_AutoManualAnimation));

        MaterialProperty _Animation_Factor = FindProperty("_Animation_Factor");
        editor.ShaderProperty(_Animation_Factor, MakeLabel(_Animation_Factor));


        // MASK VIGNETTE
        GUILayout.Label("Vignette Mask", EditorStyles.helpBox);
        MaterialProperty _VignetteMaskFallof = FindProperty("_VignetteMaskFallof");
        editor.ShaderProperty(_VignetteMaskFallof, MakeLabel(_VignetteMaskFallof));

        MaterialProperty _VignetteMaskSize = FindProperty("_VignetteMaskSize");
        editor.ShaderProperty(_VignetteMaskSize, MakeLabel(_VignetteMaskSize));

        /*
        GUILayout.Label("Colors Settings", EditorStyles.helpBox);

        MaterialProperty _SurfaceOpacity = FindProperty("_SurfaceOpacity");
        editor.ShaderProperty(_SurfaceOpacity, MakeLabel(_SurfaceOpacity));

        MaterialProperty _ColorsHue = FindProperty("_ColorsHue");
        editor.ShaderProperty(_ColorsHue, MakeLabel(_ColorsHue));

        GUILayout.Label("Outline Settings", EditorStyles.helpBox);
        MaterialProperty _OutlineMult = FindProperty("_OutlineMult");
        editor.ShaderProperty(_OutlineMult, MakeLabel(_OutlineMult));

        MaterialProperty _TransitionOutlineMult = FindProperty("_TransitionOutlineMult");
        editor.ShaderProperty(_TransitionOutlineMult, MakeLabel(_TransitionOutlineMult));

        GUILayout.Label("Image Distortion Settings", EditorStyles.helpBox);
        MaterialProperty _ChromaticAberationMult = FindProperty("_ChromaticAberationMult");
        editor.ShaderProperty(_ChromaticAberationMult, MakeLabel(_TransitionOutlineMult));

        MaterialProperty _BlurMult = FindProperty("_BlurMult");
        editor.ShaderProperty(_BlurMult, MakeLabel(_BlurMult));

        GUILayout.Label("Displacement Settings", EditorStyles.helpBox);
        MaterialProperty _PushMult = FindProperty("_PushMult");
        editor.ShaderProperty(_PushMult, MakeLabel(_PushMult));

        GUILayout.Label("Dynamic Settings", EditorStyles.helpBox);
        MaterialProperty _WallOpening = FindProperty("_WallOpening");
        editor.ShaderProperty(_WallOpening, MakeLabel(_WallOpening));

        MaterialProperty _SpeedAnimatedMode = FindProperty("_SpeedAnimatedMode");
        editor.ShaderProperty(_SpeedAnimatedMode, MakeLabel(_SpeedAnimatedMode));

        MaterialProperty _AnimatedOpening = FindProperty("_AnimatedOpening");
        editor.ShaderProperty(_AnimatedOpening, MakeLabel(_AnimatedOpening));


        GUILayout.Label("Debug Settings", EditorStyles.helpBox);
        MaterialProperty _DebugColor = FindProperty("_DebugColor");
        editor.ShaderProperty(_DebugColor, MakeLabel(_DebugColor));

        */


    }
}