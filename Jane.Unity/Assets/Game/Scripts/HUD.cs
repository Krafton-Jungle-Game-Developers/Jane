using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class HUD : MonoBehaviour
{
    public RectTransform cursorRectTransform;
    public RectTransform lineRectTransform;
    public Canvas canvas;
    public Camera camera;
    public float radius;

    private void Start()
    {
        /*Cursor.lockState = CursorLockMode.Locked;*/
        Cursor.visible = false;
    }
    void Update()
    {
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
        float distanceFromCenter = Vector3.Distance(screenCenter, Input.mousePosition);

        Debug.Log($"mouse position:{Input.mousePosition},  cursor position:{cursorRectTransform.position},  distance:{distanceFromCenter}");
        if (distanceFromCenter < radius)
        {
            cursorRectTransform.position = Input.mousePosition;
        }
        else if (distanceFromCenter >= radius)
        {
            Mathf.Clamp(distanceFromCenter, 0f, radius);
            cursorRectTransform.position = 
        }
    }
}
