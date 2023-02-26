using UnityEngine;

public class HUDCursor : MonoBehaviour
{
    [SerializeField] private Camera HUDCamera;
    [SerializeField] private Canvas canvas;
    private RectTransform canvasRectTransform;
    private RectTransform cursorRectTransform;
    private RectTransform lineRectTransform;
    
    public virtual Vector3 ViewportPosition
    {
        get
        {
            if (HUDCamera == null) { return new Vector3(0, 0, 1); }
            
            Vector3 canvasPos = cursorRectTransform.anchoredPosition + (0.5f * canvasRectTransform.sizeDelta);
            return (new Vector3(canvasPos.x / canvasRectTransform.sizeDelta.x, canvasPos.y / canvasRectTransform.sizeDelta.y, 1));
        }
    }

    public virtual Vector3 AimDirection
    {
        get
        {
            if (HUDCamera is null) { return -cursorRectTransform.forward; }

            return (HUDCamera.ViewportToWorldPoint(ViewportPosition) - HUDCamera.transform.position).normalized;
        }
    }

    private void Awake() => canvas.TryGetComponent(out canvasRectTransform);

    public void CenterCursor() => SetViewportPosition(new Vector3(0.5f, 0.5f, 0.5f));

    public void SetViewportPosition(Vector3 viewportPosition)
    {
        // Set cursor position
        cursorRectTransform.anchoredPosition = Vector3.Scale(viewportPosition - new Vector3(0.5f, 0.5f, 0), canvasRectTransform.sizeDelta);

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
            lineRectTransform.sizeDelta = new Vector2(0, lineRectTransform.sizeDelta.y);
        }
    }
}
