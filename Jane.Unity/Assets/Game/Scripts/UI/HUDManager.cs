using TMPro;
using UnityEngine;

namespace Jane.Unity
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private Camera hUDCamera;
        public Camera HUDCamera { get; set; }
        public TMP_Text currentRankText;
        public TMP_Text totalRankText;
        public Canvas hudCanvas;
        public Canvas targetCanvas;
        public Canvas standingsCanvas;
        public Canvas resultCanvas;
        [SerializeField] private RectTransform cursorRectTransform;
        [SerializeField] private RectTransform lineRectTransform;
        private RectTransform canvasRectTransform;

        public virtual Vector3 ViewportPosition
        {
            get
            {
                if (hUDCamera == null) { return new Vector3(0f, 0f, 1f); }

                Vector3 canvasPos = cursorRectTransform.anchoredPosition + (0.5f * canvasRectTransform.sizeDelta);
                return (new Vector3(canvasPos.x / canvasRectTransform.sizeDelta.x, canvasPos.y / canvasRectTransform.sizeDelta.y, 1f));
            }
        }

        public virtual Vector3 AimDirection
        {
            get
            {
                if (hUDCamera is null) { return -cursorRectTransform.forward; }

                return (hUDCamera.ViewportToWorldPoint(ViewportPosition) - hUDCamera.transform.position).normalized;
            }
        }

        private void Awake() => hudCanvas.TryGetComponent(out canvasRectTransform);

        public void CenterCursor() => SetViewportPosition(new Vector3(0.5f, 0.5f, 0.5f));

        public void SetViewportPosition(Vector3 viewportPosition)
        {
            // Set cursor position
            cursorRectTransform.anchoredPosition = Vector3.Scale(viewportPosition - new Vector3(0.5f, 0.5f, 0f), canvasRectTransform.sizeDelta);

            // Set line position
            lineRectTransform.anchoredPosition = 0.5f * cursorRectTransform.anchoredPosition;

            // Suppress "look rotation viewing vector is zero" warning.
            if (cursorRectTransform.anchoredPosition.magnitude > 0.001f)
            {
                lineRectTransform.localRotation = Quaternion.LookRotation(lineRectTransform.anchoredPosition, Vector3.up) * Quaternion.Euler(0f, -90f, 0f);

                lineRectTransform.sizeDelta = new Vector2((cursorRectTransform.position - lineRectTransform.position).magnitude * 2 * (1 / lineRectTransform.localScale.x),
                    lineRectTransform.sizeDelta.y);
            }
            else
            {
                lineRectTransform.sizeDelta = new Vector2(0f, lineRectTransform.sizeDelta.y);
            }
        }
    }
}
