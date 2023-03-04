using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TargetBoxGenerator : MonoBehaviour
{
    public Camera mainCam;
    public CheckPoints checkPoints;
    public List<GameObject> targetList = new List<GameObject>();
    public List<GameObject> onScreenObjects = new List<GameObject>();
    public List<GameObject> offScreenObjects = new List<GameObject>();
    public GameObject targetCanvasPrefab;

    [SerializeField]
    private Vector2 minSize = new Vector2(100, 100);
    [SerializeField]
    private Vector2 sizeMargin = new Vector2(-50, -50);

    void Start()
    {
        foreach (GameObject checkPoint in checkPoints.checkPointArr)
        {
            targetList.Add(checkPoint);
        }
        for (int i = 0; i < targetList.Count; i++)
        {
            onScreenObjects.Add(Instantiate(targetCanvasPrefab.GetComponentInChildren<HUDTargetBox>().gameObject));
            onScreenObjects[i].transform.parent = transform;
        }
        SetNextTargetBox(0);
    }

    void Update()
    {
        UpdateTargetBox();
    }

    private void UpdateTargetBox()
    {
        int i = checkPoints.idx;
        bool isInView = targetList[i].GetComponent<Renderer>().isVisible;

        if (isInView)
        {
            Rect targetRect = GetBoundsInScreenSpace(targetList[i], mainCam);
            RectTransform targetRectTransform = onScreenObjects[i].gameObject.GetComponent<RectTransform>();

            targetRectTransform.position = new Vector2(targetRect.center.x, targetRect.center.y);
            targetRectTransform.sizeDelta = new Vector2(Mathf.Max(targetRect.width / 2, minSize.x), Mathf.Max(targetRect.height / 2, minSize.y)) + sizeMargin;
        }
        else
        {
        }
    }

    public void ResetTargetBox()
    {
        foreach(GameObject obj in onScreenObjects)
        {
            obj.SetActive(false);
        }
    }
    public void SetNextTargetBox(int i)
    {
        ResetTargetBox();
        onScreenObjects[i].SetActive(true);
    }

    public static Rect GetBoundsInScreenSpace(GameObject targetObj, Camera camera)
    {
        Bounds targetBounds = targetObj.GetComponent<Renderer>().bounds;
        Vector3[] screenSpaceCorners = new Vector3[8];

        screenSpaceCorners[0] = camera.WorldToScreenPoint(new Vector3(targetBounds.center.x + targetBounds.extents.x, targetBounds.center.y + targetBounds.extents.y, targetBounds.center.z + targetBounds.extents.z));
        screenSpaceCorners[1] = camera.WorldToScreenPoint(new Vector3(targetBounds.center.x + targetBounds.extents.x, targetBounds.center.y + targetBounds.extents.y, targetBounds.center.z - targetBounds.extents.z));
        screenSpaceCorners[2] = camera.WorldToScreenPoint(new Vector3(targetBounds.center.x + targetBounds.extents.x, targetBounds.center.y - targetBounds.extents.y, targetBounds.center.z + targetBounds.extents.z));
        screenSpaceCorners[3] = camera.WorldToScreenPoint(new Vector3(targetBounds.center.x + targetBounds.extents.x, targetBounds.center.y - targetBounds.extents.y, targetBounds.center.z - targetBounds.extents.z));
        screenSpaceCorners[4] = camera.WorldToScreenPoint(new Vector3(targetBounds.center.x - targetBounds.extents.x, targetBounds.center.y + targetBounds.extents.y, targetBounds.center.z + targetBounds.extents.z));
        screenSpaceCorners[5] = camera.WorldToScreenPoint(new Vector3(targetBounds.center.x - targetBounds.extents.x, targetBounds.center.y + targetBounds.extents.y, targetBounds.center.z - targetBounds.extents.z));
        screenSpaceCorners[6] = camera.WorldToScreenPoint(new Vector3(targetBounds.center.x - targetBounds.extents.x, targetBounds.center.y - targetBounds.extents.y, targetBounds.center.z + targetBounds.extents.z));
        screenSpaceCorners[7] = camera.WorldToScreenPoint(new Vector3(targetBounds.center.x - targetBounds.extents.x, targetBounds.center.y - targetBounds.extents.y, targetBounds.center.z - targetBounds.extents.z));

        float min_x = screenSpaceCorners[0].x;
        float min_y = screenSpaceCorners[0].y;
        float max_x = screenSpaceCorners[0].x;
        float max_y = screenSpaceCorners[0].y;

        for (int i = 1; i < 8; i++)
        {
            if (screenSpaceCorners[i].x < min_x)
            {
                min_x = screenSpaceCorners[i].x;
            }
            if (screenSpaceCorners[i].y < min_y)
            {
                min_y = screenSpaceCorners[i].y;
            }
            if (screenSpaceCorners[i].x > max_x)
            {
                max_x = screenSpaceCorners[i].x;
            }
            if (screenSpaceCorners[i].y > max_y)
            {
                max_y = screenSpaceCorners[i].y;
            }
        }
        return Rect.MinMaxRect(min_x, min_y, max_x, max_y);
    }

}
