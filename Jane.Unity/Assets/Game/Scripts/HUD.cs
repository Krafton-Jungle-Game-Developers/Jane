using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
//using UnityEngine.InputSystem.LowLevel;

public class HUD : MonoBehaviour
{
    public RectTransform cursorRectTransform;
    public RectTransform lineRectTransform;
    public Canvas canvas;
    public Camera camera;
    public float radius;

    private Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
    private Vector3 relativeScreenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);

    private void Start()
    {
        /*Cursor.lockState = CursorLockMode.Locked;*/
        Cursor.visible = false;
    }
    void Update()
    {
        float distanceFromCenter = Vector2.Distance(relativeScreenCenter, Input.mousePosition);
        Vector3 direction = Input.mousePosition - relativeScreenCenter;

        if (distanceFromCenter < radius)
        {
            cursorRectTransform.position = screenCenter + direction;
        }
        else if (distanceFromCenter >= radius)
        {
            cursorRectTransform.position = screenCenter + (direction.normalized * radius);
            relativeScreenCenter += (distanceFromCenter - radius) * (direction.normalized);
        }

        lineRectTransform.position = 0.5f * screenCenter + 0.5f * cursorRectTransform.position;
        lineRectTransform.LookAt(cursorRectTransform, Vector3.Cross(cursorRectTransform.up, (cursorRectTransform.position - lineRectTransform.position).normalized));
        if(lineRectTransform.anchoredPosition.x < 0f)
        {
            lineRectTransform.rotation = Quaternion.Euler(0f, 0f, lineRectTransform.rotation.eulerAngles.x);
        }
        else
        {
            lineRectTransform.rotation = Quaternion.Euler(0f, 180f, lineRectTransform.rotation.eulerAngles.x);
        }
        lineRectTransform.sizeDelta = new Vector2((cursorRectTransform.localPosition - lineRectTransform.localPosition).magnitude * 2 * (1 / lineRectTransform.localScale.x), lineRectTransform.sizeDelta.y);
    }
}