using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

/// <summary>
/// Used by GateTriggerQuad Objects inside gate prefab. 
/// </summary>
public class GateTriggerQuad : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CheckPoints checkPointsArr = GetComponentInParent<CheckPoints>();

        checkPointsArr.ControlGates();
        Destroy(gameObject);
    }
}
