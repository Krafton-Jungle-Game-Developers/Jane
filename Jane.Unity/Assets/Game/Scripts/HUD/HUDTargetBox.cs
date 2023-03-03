using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HUDTargetBox : MonoBehaviour
{
    public Texture checkpointTransform;
    public RectTransform checkpointRectTransform;
    public int targetBoxWidth, targetBoxHeight;
    public GameObject target;
    public Camera camera;

    private Vector3 checkpointScreenPos;
    private Vector3 opponentScreenPos;
    private float distance;

    void Update()
    {
/*        checkpointScreenPos = camera.WorldToScreenPoint(target.transform.position);
*//*        checkpointRectTransform.anchoredPosition = camera.ViewportToWorldPoint(transform.position);
        Debug.Log($"{checkpointRectTransform.anchoredPosition}");
        checkpointRectTransform.transform.localScale
*//*        distance = Vector3.Distance(target.transform.position, camera.transform.position);
*/    }

    private void OnGUI()
    {
/*        Vector3 centeredViewportPos = GetViewportPosition(position, hudCamera, true);
*/
        /*        if (target.GetComponent<Renderer>().isVisible)
                {
                    Debug.Log("working");
                    float posX = checkpointScreenPos.x - (targetBoxWidth / 2) / distance;
                    float posY = Screen.height - checkpointScreenPos.y - (targetBoxHeight / 2) / distance;
                    float width = targetBoxWidth / distance;
                    float height = targetBoxHeight / distance;

                    GUI.Box(new Rect(posX, posY, width, height), checkpointTransform);
                }
        */
    }
}
