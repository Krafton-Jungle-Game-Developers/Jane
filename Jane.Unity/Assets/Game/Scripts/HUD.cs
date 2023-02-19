using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public RectTransform cursorRectTransform;
    public RectTransform lineRectTransform;
    public Canvas canvas;
    public Camera camera;


    private void Start()
    {
        /*Cursor.lockState = CursorLockMode.Locked;*/
        Cursor.visible = false;
    }
    void Update()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        cursorRectTransform.position += new Vector3(x, y, 0.0f);
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
        Vector3 crosshairOrigin = cursorRectTransform.position - screenCenter;

        if (crosshairOrigin.magnitude > 100.0f)
        {
            crosshairOrigin.Normalize();
            crosshairOrigin *= 10.0f;
        }

        cursorRectTransform.position = crosshairOrigin + screenCenter;
    }
}
