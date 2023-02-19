using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RadiantGI.Universal {

    [ExecuteInEditMode, VolumeComponentMenu("Kronnect/Radiant Global Illumination")]
    public class RadiantGlobalIllumination : VolumeComponent, IPostProcessComponent {

        public enum DebugView {
            None,
            Albedo,
            Normals,
            Specular,
            Depth,
            Raycast = 20,
            DownscaledHalf = 30,
            DownscaledQuarter = 40,
            ReflectiveShadowMap = 41,
            UpscaleToHalf = 50,
            TemporalAccumulationBuffer = 60,
            FinalGI = 70
        }


        [Tooltip("Intensity of the indirect lighting.")]
        public FloatParameter indirectIntensity = new FloatParameter(0);

        [Tooltip("Distance attenuation applied to indirect lighting. Reduces indirect intensity by square of distance.")]
        public ClampedFloatParameter indirectDistanceAttenuation = new ClampedFloatParameter(0, 0, 1);

        [Tooltip("Maximum brightness of indirect source.")]
        public FloatParameter indirectMaxSourceBrightness = new FloatParameter(8);

        [Tooltip("Determines how much influence has the surface normal map when receiving indirect lighting.")]
        public ClampedFloatParameter normalMapInfluence = new ClampedFloatParameter(1f, 0, 1);

        [Tooltip("Add one ray bounce.")]
        public BoolParameter rayBounce = new BoolParameter(false);

        [Tooltip("Only in forward rendering mode: uses pixel luma to enhance results by adding variety to the effect based on the perceptual brigthness. Set this value to 0 to disable this feature.")]
        public FloatParameter lumaInfluence = new FloatParameter(0f);

        [Tooltip("Intensity of the near field obscurance effect. Darkens surfaces occluded by other nearby surfaces.")]
        public FloatParameter nearFieldObscurance = new FloatParameter(0);

        [Tooltip("Spread or radius of the near field obscurance effect")]
        public ClampedFloatParameter nearFieldObscuranceSpread = new ClampedFloatParameter(0.2f, 0.1f, 1f);

        [Tooltip("Enable user-defined light emitters in the scene.")]
        public BoolParameter virtualEmitters = new BoolParameter(false);

        [Tooltip("Number of rays per pixel")]
        public ClampedIntParameter rayCount = new ClampedIntParameter(1, 1, 4);

        [Tooltip("Max ray length. Increasing this value may also require increasing the 'Max Samples' value to avoid losing quality.")]
        public FloatParameter rayMaxLength = new FloatParameter(8);

        [Tooltip("Max samples taken during raymarch.")]
        public IntParameter rayMaxSamples = new IntParameter(32);

        [Tooltip("Jitter adds a random offset to the ray direction to reduce banding. Useful when using low sample count.")]
        public FloatParameter rayJitter = new FloatParameter(0);

        [Tooltip("The assumed thickness for any geometry. Used to determine if ray crosses a surface.")]
        public FloatParameter thickness = new FloatParameter(1f);

        [Tooltip("Improves raymarch accuracy by using binary search.")]
        public BoolParameter rayBinarySearch = new BoolParameter(true);

        [Tooltip("In case a ray miss a target, reuse rays from previous frames.")]
        public BoolParameter fallbackReuseRays = new BoolParameter(false);

        [Tooltip("If a ray misses a target, reuse result from history buffer. This value is the intensity of the previous color in case the ray misses the target.")]
        public ClampedFloatParameter rayReuse = new ClampedFloatParameter(0, 0, 1);

        [Tooltip("In case a ray miss a target, use nearby probes if they're available.")]
        public BoolParameter fallbackReflectionProbes = new BoolParameter(false);

        [Tooltip("Custom global probe intensity multiplier. Note that each probe has also an intensity property.")]
        public FloatParameter probesIntensity = new FloatParameter(1f);

        [Tooltip("In case a ray miss a target, use reflective shadow map data from the main directional light. You need to add the ReflectiveShadowMap script to the directional light to use this feature.")]
        public BoolParameter fallbackReflectiveShadowMap = new BoolParameter(false);

        public ClampedFloatParameter reflectiveShadowMapIntensity = new ClampedFloatParameter(0.8f, 0, 1);

        [Tooltip("Reduces resolution of all GI stages improving performance")]
        public ClampedFloatParameter downsampling = new ClampedFloatParameter(1, 1, 4);

        [Tooltip("Raytracing accuracy. Reducing this value will shrink the depth buffer used during raytracing, improving performance in exchange of accuracy.")]
        public ClampedIntParameter raytracerAccuracy = new ClampedIntParameter(8, 1, 8);

        [Tooltip("Extra blur passes")]
        public ClampedIntParameter smoothing = new ClampedIntParameter(3, 0, 4);

        [Tooltip("Uses motion vectors to blend into a history buffer to reduce flickering. Only applies in play mode.")]
        public BoolParameter temporalReprojection = new BoolParameter(true);

        [Tooltip("Reaction speed to screen changes. Higher values reduces ghosting but also the smoothing.")]
        public FloatParameter temporalResponseSpeed = new FloatParameter(12);

        [Tooltip("Reaction speed to camera position change. Higher values reduces ghosting when camera moves.")]
        public FloatParameter temporalCameraTranslationResponse = new FloatParameter(100);

        [Tooltip("Difference in depth with current frame to discard history buffer when reusing rays.")]
        public FloatParameter temporalDepthRejection = new FloatParameter(1f);

        [Tooltip("Allowed difference in color between history and current GI buffers.")]
        public ClampedFloatParameter temporalChromaThreshold = new ClampedFloatParameter(0.2f, 0, 1f);

        [Tooltip("Renders the effect also in edit mode (when not in play-mode).")]
        public BoolParameter showInEditMode = new BoolParameter(true);

        [Tooltip("Renders the effect also in Scene View.")]
        public BoolParameter showInSceneView = new BoolParameter(true);

        [Tooltip("Computes GI emitted by objects with a minimum luminosity.")]
        public FloatParameter brightnessThreshold = new FloatParameter(0f);

        [Tooltip("Maximum GI brightness.")]
        public FloatParameter brightnessMax = new FloatParameter(8f);

        [Tooltip("Attenuates GI brightness from nearby surfaces.")]
        public FloatParameter nearCameraAttenuation = new FloatParameter(0);

        [Tooltip("Adjusted color saturation for the computed GI.")]
        public ClampedFloatParameter saturation = new ClampedFloatParameter(1, 0, 2);

        [Tooltip("Applies GI only inside the post processing volume (use only if the volume is local)")]
        public BoolParameter limitToVolumeBounds = new BoolParameter(false);

        [Tooltip("Enables stencil check during GI composition. This option let you exclude GI over certain objects that also use stencil buffer.")]
        public BoolParameter stencilCheck = new BoolParameter(false);

        public IntParameter stencilValue = new IntParameter(1);

        [Serializable] public sealed class CompareFunctionParameter : VolumeParameter<CompareFunction> { }
        public CompareFunctionParameter stencilCompareFunction = new CompareFunctionParameter { value = CompareFunction.NotEqual };

        [Tooltip("Integration with URP native screen space ambient occlusion (also with HBAO in Lit AO mode). Amount of ambient occlusion that influences indirect lighting created by Radiant.")]
        public ClampedFloatParameter aoInfluence = new ClampedFloatParameter(0f, 0, 1f);

        [Serializable]
        public sealed class DebugViewParameter : VolumeParameter<DebugView> { }

        public DebugViewParameter debugView = new DebugViewParameter { value = DebugView.None };

        public BoolParameter compareMode = new BoolParameter(false);

        public BoolParameter compareSameSide = new BoolParameter(false);

        public ClampedFloatParameter comparePanning = new ClampedFloatParameter(0.25f, 0, 0.5f);

        public ClampedFloatParameter compareLineAngle = new ClampedFloatParameter(1.4f, -Mathf.PI, Mathf.PI);

        public ClampedFloatParameter compareLineWidth = new ClampedFloatParameter(0.002f, 0.0001f, 0.05f);


        public bool IsActive() => indirectIntensity.value > 0 || compareMode.value;

        public bool IsTileCompatible() => true;

        void OnValidate() {
            indirectIntensity.value = Mathf.Max(0, indirectIntensity.value);
            indirectMaxSourceBrightness.value = Mathf.Max(0, indirectMaxSourceBrightness.value);
            temporalResponseSpeed.value = Mathf.Max(0, temporalResponseSpeed.value);
            temporalDepthRejection.value = Mathf.Max(0, temporalDepthRejection.value);
            rayMaxLength.value = Mathf.Max(0.1f, rayMaxLength.value);
            rayMaxSamples.value = Mathf.Max(2, rayMaxSamples.value);
            rayJitter.value = Mathf.Max(0, rayJitter.value);
            lumaInfluence.value = Mathf.Max(0, lumaInfluence.value);
            thickness.value = Mathf.Max(0.1f, thickness.value);
            brightnessThreshold.value = Mathf.Max(0, brightnessThreshold.value);
            brightnessMax.value = Mathf.Max(0, brightnessMax.value);
            nearCameraAttenuation.value = Mathf.Max(0, nearCameraAttenuation.value);
            nearFieldObscurance.value = Mathf.Max(0, nearFieldObscurance.value);
        }

        void Reset() {
            RadiantRenderFeature.needRTRefresh = true;
        }


    }
}
