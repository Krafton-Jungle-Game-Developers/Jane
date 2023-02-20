using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
        float distanceFromCenter = Vector2.Distance(screenCenter, Input.mousePosition);
        Vector3 direction = Input.mousePosition - screenCenter;

        if (distanceFromCenter < radius)
        {
            cursorRectTransform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane);
        }
        else if (distanceFromCenter >= radius)
        {
            cursorRectTransform.position = screenCenter + (direction.normalized * radius);
        }
    }
}