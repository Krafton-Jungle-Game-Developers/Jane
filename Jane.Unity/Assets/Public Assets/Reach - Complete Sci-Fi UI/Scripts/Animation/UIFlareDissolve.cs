using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Reach
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    public class UIFlareDissolve : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private RectTransform widthReference;
        private Image baseImage;
        private RectTransform baseRect;

        [Header("Settings")]
        [SerializeField] private bool multiplyWidth = true;
        [SerializeField] [Range(0, 1)] private float startScale = 0.25f;
        [SerializeField] [Range(0, 1)] private float endScale = 1;
        [SerializeField] [Range(1, 10)] private float curveSpeed = 4;
        [SerializeField] [Range(0.5f, 6)] private float fadingSpeed = 2;
        [SerializeField] private AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));
        [SerializeField] private AnimationCurve fadingCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));

        // Helpers
        bool isInitialized = false;
        float defaultWidth = 300;
        float widthMultiplier = 80;

        void Awake()
        {
            baseRect = GetComponent<RectTransform>();
            baseRect.sizeDelta = new Vector2(0, baseRect.sizeDelta.y);
            baseRect.localScale = new Vector3(0, 1, 1);

            baseImage = GetComponent<Image>();
            baseImage.color = new Color(baseImage.color.r, baseImage.color.g, baseImage.color.b, 0);
          
            isInitialized = true;
        }

        void OnEnable()
        {
            if (isInitialized == false)
                return;

            StartCoroutine("FirstPhase");
        }

        IEnumerator FirstPhase()
        {
            float elapsedTime = 0;
            float startWidth = 0;
            float parentWidth = defaultWidth;

            if (multiplyWidth == false) { widthMultiplier = 0; }
            if (widthReference != null) { parentWidth = widthReference.sizeDelta.x + widthMultiplier; }
            baseRect.sizeDelta = new Vector2(0, baseRect.sizeDelta.y);
            baseRect.localScale = new Vector3(startScale, 1, 1);
            baseImage.color = new Color(baseImage.color.r, baseImage.color.g, baseImage.color.b, 1);

            while (baseRect.sizeDelta.x < parentWidth - 0.5f)
            {
                elapsedTime += Time.unscaledDeltaTime;

                baseRect.sizeDelta = new Vector2(Mathf.Lerp(startWidth, parentWidth, animationCurve.Evaluate(elapsedTime * curveSpeed)), baseRect.sizeDelta.y);
                baseRect.localScale = new Vector3(Mathf.Lerp(startScale, endScale, animationCurve.Evaluate(elapsedTime * curveSpeed)), baseRect.localScale.y, baseRect.localScale.z);

                yield return null;
            }

            baseRect.sizeDelta = new Vector2(parentWidth, baseRect.sizeDelta.y);
            baseRect.localScale = new Vector3(1, 1, 1);

            elapsedTime = 0;
            startWidth = baseRect.sizeDelta.x;
            float endWidth = baseRect.sizeDelta.x + widthMultiplier;
            float alphaValue = baseImage.color.a;

            while (baseImage.color.a > 0)
            {
                elapsedTime += Time.unscaledDeltaTime;

                baseRect.sizeDelta = new Vector2(Mathf.Lerp(startWidth, endWidth, animationCurve.Evaluate(elapsedTime * curveSpeed)), baseRect.sizeDelta.y);
                baseImage.color = new Color(baseImage.color.r, baseImage.color.g, baseImage.color.b, Mathf.Lerp(alphaValue, 0, fadingCurve.Evaluate(elapsedTime * fadingSpeed)));

                yield return null;
            }

            baseImage.color = new Color(baseImage.color.r, baseImage.color.g, baseImage.color.b, 0);
        }
    }
}