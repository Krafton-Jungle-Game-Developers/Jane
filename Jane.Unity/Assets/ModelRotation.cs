using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelRotation : MonoBehaviour
{
    [SerializeField] private Vector3 _modelVec;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.Rotate(_modelVec,Space.Self);
    }
}
