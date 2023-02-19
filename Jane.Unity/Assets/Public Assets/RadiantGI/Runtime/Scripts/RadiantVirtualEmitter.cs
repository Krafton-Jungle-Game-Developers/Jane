using UnityEngine;
using System.Collections.Generic;

namespace RadiantGI.Universal {

    [ExecuteInEditMode]
    public class RadiantVirtualEmitter : MonoBehaviour {

        [Header("GI Color")]
        [ColorUsage(showAlpha: false, hdr: true)]
        public Color color = new Color(1, 1, 1);
        [Tooltip("Enable this option to add the emission color of the material used by this object to the global illumination.")]
        public bool addMaterialEmission;
        [Tooltip("The renderer from which synchronize the emission color")]
        public Renderer targetRenderer;
        [Tooltip("Optionally specify the material for the emission color")]
        public Material material;
        public string emissionPropertyName = "_EmissionColor";
        [Tooltip("Useful in case the gameobject uses more than one material")]
        public int materialIndex;
        public float intensity = 1f;
        public float range = 10f;

        [Header("Area Of Influence")]
        public Vector3 boxCenter;
        public Vector3 boxSize = new Vector3(25, 25, 25);
        public bool boundsInLocalSpace = true;

        int emissionNameId;
        Renderer thisRenderer;

        static List<Material> sharedMaterials = new List<Material>();

        private void OnValidate() {
            intensity = Mathf.Max(0, intensity);
            range = Mathf.Max(0, range);
        }

        void OnEnable() {
            emissionNameId = Shader.PropertyToID(emissionPropertyName);
            thisRenderer = GetComponentInChildren<Renderer>();
            RadiantRenderFeature.RegisterVirtualEmitter(this);
        }

        void OnDisable() {
            RadiantRenderFeature.UnregisterVirtualEmitter(this);
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = new Color(0, 1f, 0, 0.75F);
            Gizmos.DrawWireSphere(transform.position, range);
        }


        public Color GetGIColor() {
            Color sum = color;
            if (addMaterialEmission) {
                Material mat = material;
                if (mat == null) {
                    Renderer r = targetRenderer != null ? targetRenderer : thisRenderer;
                    if (r != null) {
                        if (materialIndex == 0) {
                            mat = r.sharedMaterial;
                        } else {
                            r.GetSharedMaterials(sharedMaterials);
                            if (materialIndex < sharedMaterials.Count) {
                                mat = sharedMaterials[materialIndex];
                            }
                        }
                    }
                    if (mat != null && mat.HasProperty(emissionNameId)) {
                        sum += mat.GetColor(emissionNameId);
                    }
                }
            }
            return sum * intensity;
        }


        public Vector4 GetGIColorAndRange() {
            Color giColor = GetGIColor();
            return new Vector4(giColor.r, giColor.g, giColor.b, range);
        }

        /// <summary>
        /// Returns emitter area of influence in world space
        /// </summary>
        /// <returns></returns>
        public Bounds GetBounds() {
            Bounds bounds = new Bounds(boxCenter, boxSize);
            if (boundsInLocalSpace) {
                bounds.center += transform.position;
            }
            return bounds;
        }


        /// <summary>
        /// Sets emitter area of influence in world space
        /// </summary>
        /// <param name="bounds"></param>
        public void SetBounds(Bounds bounds) {
            if (boundsInLocalSpace) {
                bounds.center -= transform.position;
            }
            boxCenter = bounds.center;
            boxSize = bounds.size;
        }

    }

}
