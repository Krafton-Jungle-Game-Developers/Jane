using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Reach
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Reach UI/Animation/UI Popup")]
    public class UIPopup : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool playOnEnable = true;
        [Tooltip("Enables content size fitter mode.")]
        [SerializeField] private bool fitterMode = false;
        [SerializeField] private StartBehaviour startBehaviour;

        [Header("Animation")]
        [SerializeField] private AnimationMode animationMode;
        [Range(0.5f, 10)] public float curveSpeed = 4f;
        [SerializeField] private AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));

        // Helpers
        RectTransform rect;
        CanvasGroup cg;
        Vector2 rectHelper;
        bool isInitialized = false;
        bool isFitterInitialized;

        public enum AnimationMode { Scale, Horizontal, Vertical }
        public enum StartBehaviour { Default, Disabled }

        void Start()
        {
            if (startBehaviour == StartBehaviour.Disabled)
                gameObject.SetActive(false);
        }

        void OnEnable()
        {
            if (isInitialized == false) { Initialize(); }
            if (playOnEnable == true) { PlayIn(); }
        }

        private void Initialize()
        {
            if (rect == null) { rect = GetComponent<RectTransform>(); }
            if (cg == null) { cg = GetComponent<CanvasGroup>(); }

            isInitialized = true;
        }

        public void ResetFitterData()
        {
            isFitterInitialized = false;
        }

        public void PlayIn()
        {
            gameObject.SetActive(true);

            if (fitterMode == true && isFitterInitialized == false)
            {
                cg.alpha = 0;
                StartCoroutine("InitFitter");
                return;
            }

            if (animationMode == AnimationMode.Scale && cg != null)
            {
                if (gameObject.activeInHierarchy == true) { StopCoroutine("ScaleIn"); StopCoroutine("ScaleOut"); StartCoroutine("ScaleIn"); }
                else { cg.alpha = 1; rect.localScale = new Vector3(1, 1, 1); }
            }

            else if (animationMode == AnimationMode.Horizontal && cg != null)
            {
                if (gameObject.activeInHierarchy == true) { StopCoroutine("HorizontalIn"); StopCoroutine("HorizontalOut"); StartCoroutine("HorizontalIn"); }
                else { cg.alpha = 1; }
            }

            else if (animationMode == AnimationMode.Vertical && cg != null)
            {
                if (gameObject.activeInHierarchy == true) { StopCoroutine("VerticalIn"); StopCoroutine("VerticalOut"); StartCoroutine("VerticalIn"); }
                else { cg.alpha = 1; }
            }
        }

        public void PlayOut()
        {
            if (animationMode == AnimationMode.Scale && cg != null)
            {
                if (gameObject.activeInHierarchy == true) { StopCoroutine("ScaleIn"); StopCoroutine("ScaleOut"); StartCoroutine("ScaleOut"); }
                else { cg.alpha = 0; rect.localScale = new Vector3(0, 0, 0); gameObject.SetActive(false); }
            }

            else if (animationMode == AnimationMode.Horizontal && cg != null)
            {
                if (gameObject.activeInHierarchy == true) { StopCoroutine("HorizontalIn"); StopCoroutine("HorizontalOut"); StartCoroutine("HorizontalOut"); }
                else { cg.alpha = 0; gameObject.SetActive(false); }
            }

            else if (animationMode == AnimationMode.Vertical && cg != null)
            {
                if (gameObject.activeInHierarchy == true) { StopCoroutine("VerticalIn"); StopCoroutine("VerticalOut"); StartCoroutine("VerticalOut"); }
                else { cg.alpha = 0; gameObject.SetActive(false); }
            }
        }

        IEnumerator InitFitter()
        {
            yield return new WaitForSecondsRealtime(0.04f);

            ContentSizeFitter csf = GetComponent<ContentSizeFitter>();
            csf.enabled = false;

            rectHelper = rect.sizeDelta;
            isFitterInitialized = true;
            PlayIn();
        }

        IEnumerator ScaleIn()
        {
            float elapsedTime = 0;
            float startingPoint = 0;
            float smoothValue = 0;

            rect.localScale = new Vector3(0, 0, 0);
            cg.alpha = 0;

            while (rect.localScale.x < 0.99)
            {
                elapsedTime += Time.unscaledDeltaTime;
                smoothValue = Mathf.Lerp(startingPoint, 1, animationCurve.Evaluate(elapsedTime * curveSpeed));
               
                rect.localScale = new Vector3(smoothValue, smoothValue, smoothValue);
                cg.alpha = smoothValue;
               
                yield return null;
            }

            cg.alpha = 1;
            rect.localScale = new Vector3(1, 1, 1);
        }

        IEnumerator ScaleOut()
        {
            float elapsedTime = 0;
            float startingPoint = 1;
            float smoothValue = 0;

            rect.localScale = new Vector3(1, 1, 1);
            cg.alpha = 1;

            while (rect.localScale.x > 0.01)
            {
                elapsedTime += Time.unscaledDeltaTime;       
                smoothValue = Mathf.Lerp(startingPoint, 0, animationCurve.Evaluate(elapsedTime * curveSpeed));
               
                rect.localScale = new Vector3(smoothValue, smoothValue, smoothValue);
                cg.alpha = smoothValue;
             
                yield return null;
            }

            cg.alpha = 0;
            rect.localScale = new Vector3(0, 0, 0);
            gameObject.SetActive(false);
        }

        IEnumerator HorizontalIn()
        {
            float elapsedTime = 0;
            Vector2 startPos = new Vector2(0, rect.sizeDelta.y);
            Vector2 endPos;
            if (fitterMode == false) { endPos = rect.sizeDelta; }
            else { endPos = rectHelper; }
            rect.sizeDelta = startPos;

            while (rect.sizeDelta.x <= endPos.x - 0.1f)
            {
                elapsedTime += Time.unscaledDeltaTime;

                cg.alpha += Time.unscaledDeltaTime * (curveSpeed * 2);
                rect.sizeDelta = Vector2.Lerp(startPos, endPos, animationCurve.Evaluate(elapsedTime * curveSpeed));
               
                yield return null;
            }

            cg.alpha = 1;
            rect.sizeDelta = endPos;
        }

        IEnumerator HorizontalOut()
        {
            float elapsedTime = 0;
            Vector2 startPos = rect.sizeDelta;
            Vector2 endPos = new Vector2(0, rect.sizeDelta.y);

            while (rect.sizeDelta.x >= 0.1f)
            {
                elapsedTime += Time.unscaledDeltaTime;

                cg.alpha -= Time.unscaledDeltaTime * (curveSpeed * 2);
                rect.sizeDelta = Vector2.Lerp(startPos, endPos, animationCurve.Evaluate(elapsedTime * curveSpeed));
               
                yield return null;
            }

            cg.alpha = 0;
            rect.sizeDelta = endPos;
            rect.gameObject.SetActive(false);
        }

        IEnumerator VerticalIn()
        {
            float elapsedTime = 0;
            Vector2 startPos = new Vector2(rect.sizeDelta.x, 0);
            Vector2 endPos;
            if (fitterMode == false) { endPos = rect.sizeDelta; }
            else { endPos = rectHelper; }
            rect.sizeDelta = startPos;

            while (rect.sizeDelta.y <= endPos.y - 0.1f)
            {
                elapsedTime += Time.unscaledDeltaTime;

                cg.alpha += Time.unscaledDeltaTime * (curveSpeed * 2);
                rect.sizeDelta = Vector2.Lerp(startPos, endPos, animationCurve.Evaluate(elapsedTime * curveSpeed));
             
                yield return null;
            }

            cg.alpha = 1;
            rect.sizeDelta = endPos;
        }

        IEnumerator VerticalOut()
        {
            float elapsedTime = 0;
            Vector2 startPos = rect.sizeDelta;
            Vector2 endPos = new Vector2(rect.sizeDelta.x, 0);

            while (rect.sizeDelta.y >= 0.1f)
            {
                elapsedTime += Time.unscaledDeltaTime;

                cg.alpha -= Time.unscaledDeltaTime * (curveSpeed * 2);
                rect.sizeDelta = Vector2.Lerp(startPos, endPos, animationCurve.Evaluate(elapsedTime * curveSpeed));
             
                yield return null;
            }

            cg.alpha = 0;
            rect.sizeDelta = endPos;
            rect.gameObject.SetActive(false);
        }
    }
}