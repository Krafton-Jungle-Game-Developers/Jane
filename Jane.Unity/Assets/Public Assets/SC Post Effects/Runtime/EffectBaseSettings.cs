using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace SCPE
{
    [System.Serializable]
    public class EffectBaseSettings
    {
        [Flags]
        public enum CameraTypeFlags
        {
            None = 0,
            [InspectorName("Game (Base)")]
            GameBase = 1,
            [InspectorName("Game (Overlay)")]
            GameOverlay = 2,
            [InspectorName("Scene View")]
            SceneView = 4,
            [InspectorName("Preview")]
            Preview = 8,
            [InspectorName("Reflections")]
            Reflection = 16,
            [InspectorName("Hidden (HideFlags)")]
            Hidden = 32
        }

        [HideInInspector] //Can't execute after post-processing right now
        public RenderPassEvent injectionPoint = RenderPassEvent.BeforeRenderingPostProcessing;

        [Tooltip("Effect will render, even if the camera has post-processing disabled")]
        public bool alwaysEnable;
        
        [Tooltip("Configure which camera types the effect is allowed to render on when using camera stacking" +
                 "\n\nNote that some depth-based effects will not work with camera stacking, due to how the stacking system handles the depth texture")]
        public CameraTypeFlags cameraTypes = CameraTypeFlags.GameBase | CameraTypeFlags.SceneView;

        public EffectBaseSettings(bool enableInSceneView = true)
        {
            if(!enableInSceneView) this.cameraTypes = CameraTypeFlags.GameBase | CameraTypeFlags.GameOverlay;
            else this.cameraTypes = CameraTypeFlags.GameBase | CameraTypeFlags.SceneView;
        }
    }
}