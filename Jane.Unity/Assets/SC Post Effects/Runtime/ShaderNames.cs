using UnityEngine;

namespace SCPE
{
    public static class ShaderNames
    {
        public const string TEST = "Hidden/SC Post Effects/Test";
        public const string AO2D = "Hidden/SC Post Effects/Ambient Occlusion 2D";
        public const string BlackBars = ("Hidden/SC Post Effects/Black Bars");
        public const string Blur = "Hidden/SC Post Effects/Blur";
        public const string Caustics = "Hidden/SC Post Effects/Caustics";
        public const string CloudShadows = "Hidden/SC Post Effects/Cloud Shadows";
        public const string ColorSplit = "Hidden/SC Post Effects/Color Split";
        public const string Colorize = "Hidden/SC Post Effects/Colorize";
        public const string Danger = "Hidden/SC Post Effects/Danger";
        public const string Dithering = "Hidden/SC Post Effects/Dithering";
        public const string DoubleVision = "Hidden/SC Post Effects/Double Vision";
        public const string EdgeDetection = "Hidden/SC Post Effects/Edge Detection";
        public const string Fog = "Hidden/SC Post Effects/Fog";
        public const string Gradient = "Hidden/SC Post Effects/Gradient";
        public const string HueShift3D = "Hidden/SC Post Effects/3D Hue Shift";
        public const string Kaleidoscope = "Hidden/SC Post Effects/Kaleidoscope";
        public const string Kuwahara = "Hidden/SC Post Effects/Kuwahara";
        public const string LensFlares = "Hidden/SC Post Effects/Lensflares";
        public const string LightStreaks = "Hidden/SC Post Effects/Light Streaks";
        public const string LUT = "Hidden/SC Post Effects/LUT";
        public const string Mosaic = "Hidden/SC Post Effects/Mosaic";
        public const string Overlay = "Hidden/SC Post Effects/Overlay";
        public const string Pixelize = "Hidden/SC Post Effects/Pixelize";
        public const string Posterize = "Hidden/SC Post Effects/Posterize";
        public const string RadialBlur = "Hidden/SC Post Effects/Radial Blur";
        public const string Refraction = "Hidden/SC Post Effects/Refraction";
        public const string Ripples = "Hidden/SC Post Effects/Ripples";
        public const string Scanlines = "Hidden/SC Post Effects/Scanlines";
        public const string Sharpen = "Hidden/SC Post Effects/Sharpen";
        public const string Sketch = "Hidden/SC Post Effects/Sketch";
        public const string SpeedLines = "Hidden/SC Post Effects/SpeedLines";
        public const string Sunshafts = "Hidden/SC Post Effects/Sun Shafts";
        public const string TiltShift = "Hidden/SC Post Effects/Tilt Shift";
        public const string Tracers = "Hidden/SC Post Effects/Tracers";
        public const string Transition = "Hidden/SC Post Effects/Transition";
        public const string TubeDistortion = "Hidden/SC Post Effects/Tube Distortion";

        public const string DepthNormals = "Hidden/SC Post Effects/DepthNormals";
    }

    internal static class ShaderKeywords
    {
        public const string ReconstructedDepthNormals = "_RECONSTRUCT_NORMAL";
    }

    public static class ShaderParameters
    {
        public static int Params = Shader.PropertyToID("_Params");
    }

    internal static class TextureNames
    {
        public const string Main = "_MainTex";
        public const string DepthTexture = "_CameraDepthTexture";
        public const string DepthNormals = "_CameraDepthNormalsTexture";
        public const string FogSkyboxTex = "_SkyboxTex";
    }
}