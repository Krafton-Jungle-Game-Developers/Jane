using UnityEngine;

namespace RadiantGI.Demos {

    public class ShootGlowingBalls : MonoBehaviour {

        public int count;
        public Transform center;

        public GameObject glowingBall;

        // Start is called before the first frame update
        void Start() {
            for (int k = 0; k < count; k++) {
                GameObject go = Instantiate(glowingBall, center.position + Vector3.right * Random.Range(-4, 4) + Vector3.up * (5f + k), Quaternion.identity);

                Color color = Random.ColorHSV();
                float cr = Random.value;
                if (cr < 0.33f) color.r *= 0.2f;
                else if (cr < 0.66f) color.g *= 0.2f;
                else color.b *= 0.2f;

                Renderer r = go.GetComponent<Renderer>();
                r.transform.localScale = Vector3.one * Random.Range(0.65f, 1f);
                r.material.color = color;
                r.material.SetColor("_EmissionColor", color * 2f);
            }
        }

    }

}