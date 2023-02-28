using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelRotation : MonoBehaviour
{
    private Quaternion _modelQuat;
    // Start is called before the first frame update
    void Start()
    {
        _modelQuat = transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _modelQuat.x += 1f;
    }
}
