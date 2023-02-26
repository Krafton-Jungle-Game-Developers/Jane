using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpeedGate : MonoBehaviour
{
    [SerializeField] private VisualEffect gateActivateVFX;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CheckPoints checkPointsArr = GetComponentInParent<CheckPoints>();

            checkPointsArr.ControlGates();
            Destroy(gameObject);
        }
    }

    IEnumerator Warp()
    {
        yield return null;
    }
}


