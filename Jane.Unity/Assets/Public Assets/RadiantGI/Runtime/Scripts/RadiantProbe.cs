using UnityEngine;

namespace RadiantGI.Universal {

    [ExecuteInEditMode]
    public class RadiantProbe : MonoBehaviour {

        ReflectionProbe probe;

        void OnEnable() {
            probe = GetComponent<ReflectionProbe>();
            RadiantRenderFeature.RegisterReflectionProbe(probe);
        }

        void OnDisable() {
            RadiantRenderFeature.UnregisterReflectionProbe(probe);
        }
    }

}
