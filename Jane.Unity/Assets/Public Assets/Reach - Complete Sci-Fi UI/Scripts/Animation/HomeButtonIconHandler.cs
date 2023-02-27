using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Reach
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public class HomeButtonIconHandler : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private ButtonManager buttonManager;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private HorizontalLayoutGroup layoutGroup;

        [Header("Settings")]
        [Tooltip("Destroys the gradient component from icon which is used only for editor preview.")]
        [SerializeField] private bool disableGradient = true;
        [SerializeField] [Range(0.5f, 10)] private float curveSpeed = 1f;
        [SerializeField] [Range(1, 10)] private float fadingSpeed = 8;
        [SerializeField] private AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));
        [SerializeField] private AnimationCurve fadingCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));

        RectTransform rect;
        RectTransform buttonRect;
        float targetValue;

        void Start()
        {
            if (buttonManager == null) { gameObject.SetActive(false); return; }
            if (layoutGroup == null) { layoutGroup = buttonManager.mainLayout; }
            if (canvasGroup == null) { canvasGroup = gameObject.GetComponent<CanvasGroup>(); }
            if (disableGradient == true && buttonManager.enableIcon == true && buttonManager.normalImageObj != null)
            {
                UIGradient gradient = buttonManager.normalImageObj.GetComponent<UIGradient>();
                if (gradient != null) { Destroy(gradient); }
            }

            buttonManager.onHover.AddListener(delegate { DoIn(); });
            buttonManager.onLeave.AddListener(delegate { DoOut(); });
            buttonManager.onSelect.AddListener(delegate { DoIn(); });
            buttonManager.onDeselect.AddListener(delegate { DoOut(); });

            buttonRect = buttonManager.GetComponent<RectTransform>();
            rect = gameObject.GetComponent<RectTransform>();
            targetValue = rect.sizeDelta.x;
            rect.sizeDelta = new Vector2(0, rect.sizeDelta.y);
            canvasGroup.alpha = 0;

            layoutGroup.childControlWidth = true;
            gameObject.SetActive(false);
        }

        void DoIn()
        {
            if (buttonManager.enableIcon == false)
                return;

            gameObject.SetActive(true);
            layoutGroup.childControlWidth = false;

            StopCoroutine("AnimIn");
            StopCoroutine("AnimOut");
            StartCoroutine("AnimIn");
        }

        void DoOut()
        {
            if (buttonManager.enableIcon == false || gameObject.activeInHierarchy == false)
                return;

            StopCoroutine("AnimIn");
            StopCoroutine("AnimOut");
            StartCoroutine("AnimOut");
        }

        IEnumerator AnimIn()
        {
            float startingPoint = rect.sizeDelta.x;
            float elapsedTime = 0;
            float cgValue = canvasGroup.alpha;
           
            // Making sure that the layout is initialized
            layoutGroup.childControlWidth = true;
            yield return new WaitForSecondsRealtime(0.01f);
            layoutGroup.childControlWidth = false;

            while ((rect.sizeDelta.x < targetValue - 0.1f) && (canvasGroup.alpha < 0.99f))
            {
                elapsedTime += Time.unscaledDeltaTime;
                rect.sizeDelta = new Vector2(Mathf.Lerp(startingPoint, targetValue, animationCurve.Evaluate(elapsedTime * curveSpeed)), rect.sizeDelta.y);
                canvasGroup.alpha = Mathf.Lerp(cgValue, 1, fadingCurve.Evaluate(elapsedTime * fadingSpeed));
                LayoutRebuilder.ForceRebuildLayoutImmediate(buttonRect);
                yield return null;
            }

            canvasGroup.alpha = 1;
            rect.sizeDelta = new Vector2(targetValue, rect.sizeDelta.y);
            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonRect);
        }

        IEnumerator AnimOut()
        {
            float startingPoint = rect.sizeDelta.x;
            float elapsedTime = 0;
            float cgValue = canvasGroup.alpha;

            while ((rect.sizeDelta.x > 0.1f) && (canvasGroup.alpha > 0.01f))
            {
                elapsedTime += Time.unscaledDeltaTime;
                rect.sizeDelta = new Vector2(Mathf.Lerp(startingPoint, 0, animationCurve.Evaluate(elapsedTime * curveSpeed)), rect.sizeDelta.y);
                canvasGroup.alpha = Mathf.Lerp(cgValue, 0, fadingCurve.Evaluate(elapsedTime * fadingSpeed));
                LayoutRebuilder.ForceRebuildLayoutImmediate(buttonRect);
                yield return null;
            }

            rect.sizeDelta = new Vector2(0, rect.sizeDelta.y);
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
    }
}