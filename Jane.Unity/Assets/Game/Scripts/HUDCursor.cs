using UnityEngine;
public class HUDCursor : MonoBehaviour
{
    [SerializeField] private Camera HUDCamera;
    [SerializeField] private Canvas canvas;
    private RectTransform canvasRectTransform;
    private RectTransform cursorRectTransform;
    private RectTransform lineRectTransform;

    [Tooltip("The distance from the camera to maintain when operating on a world space canvas.")]
    [SerializeField] private float worldSpaceDistanceFromCamera = 0.5f;

    public virtual Vector3 ViewportPosition
    {
        get
        {
            if (HUDCamera == null) { return new Vector3(0, 0, 1); }

            bool worldSpace = (canvas == null) || (canvas.renderMode == RenderMode.WorldSpace);
            if (worldSpace) { return (HUDCamera.WorldToViewportPoint(cursorRectTransform.position)); }

            Vector3 canvasPos = cursorRectTransform.anchoredPosition + (0.5f * canvasRectTransform.sizeDelta);
            return (new Vector3(canvasPos.x / canvasRectTransform.sizeDelta.x, canvasPos.y / canvasRectTransform.sizeDelta.y, 1));
        }
    }

    public virtual Vector3 AimDirection
    {
        get
        {
            if (HUDCamera == null)
            {
                return -cursorRectTransform.forward;
            }

            bool worldSpace = (canvas == null) || (canvas.renderMode == RenderMode.WorldSpace);
            if (worldSpace)
            {
                return (cursorRectTransform.position - HUDCamera.transform.position).normalized;
            }

            return (HUDCamera.ViewportToWorldPoint(ViewportPosition) - HUDCamera.transform.position).normalized;
        }
    }

    private void Awake()
    {
        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }
    }
    
    public void CenterCursor()
    {
        SetViewportPosition(new Vector3(0.5f, 0.5f, worldSpaceDistanceFromCamera));
    }
    
    public void SetViewportPosition(Vector3 viewportPosition)
    {
        bool worldSpace = (canvas == null) || (canvas.renderMode == RenderMode.WorldSpace);

        // World space
        if (worldSpace && HUDCamera != null)
        {
            viewportPosition.z = worldSpaceDistanceFromCamera;

            // Set the cursor position
            if (cursorRectTransform != null)
            {
                cursorRectTransform.position = HUDCamera.ViewportToWorldPoint(viewportPosition);
                cursorRectTransform.position = HUDCamera.transform.position + (cursorRectTransform.position - HUDCamera.transform.position).normalized * worldSpaceDistanceFromCamera;
                cursorRectTransform.LookAt(HUDCamera.transform, HUDCamera.transform.up);
            }

            // Set the line position
            if (lineRectTransform != null)
            {
                Vector3 centerPos = HUDCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, worldSpaceDistanceFromCamera));
                centerPos = HUDCamera.transform.position + (centerPos - HUDCamera.transform.position).normalized * worldSpaceDistanceFromCamera;


                lineRectTransform.position = 0.5f * centerPos + 0.5f * cursorRectTransform.position;
                lineRectTransform.LookAt(HUDCamera.transform,
                                            Vector3.Cross(HUDCamera.transform.forward, (cursorRectTransform.position - lineRectTransform.position).normalized));

                lineRectTransform.sizeDelta = new Vector2((cursorRectTransform.localPosition - lineRectTransform.localPosition).magnitude * 2 * (1 / lineRectTransform.localScale.x),
                                                            lineRectTransform.sizeDelta.y);
            }
        }
        // Screen/camera space
        else
        {
            // Set the cursor position
            if (cursorRectTransform != null)
            {
                cursorRectTransform.anchoredPosition = Vector3.Scale(viewportPosition - new Vector3(0.5f, 0.5f, 0), canvasRectTransform.sizeDelta);
            }

            // Set the line position
            if (lineRectTransform != null)
            {
                lineRectTransform.anchoredPosition = 0.5f * cursorRectTransform.anchoredPosition;

                if (cursorRectTransform.anchoredPosition.magnitude > 0.001f) // Otherwise will get look rotation viewing vector is zero warning
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
    }
}