using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Used by GateTriggerQuad Objects inside gate prefab. 
/// </summary>
public class GateTriggerQuad : MonoBehaviour
{
    [SerializeField] private VisualEffect gateActivateVFX;
    [SerializeField] private ParticleSystem swirlGreen;
    public int gateNumber;

    private void OnTriggerEnter(Collider other)
    {
        CheckPoints checkPoints = GetComponentInParent<CheckPoints>();
        if (other.gameObject.CompareTag("Player"))
        {
            if (checkPoints.nextGate == this.gameObject)
            {
                checkPoints.ControlGates(gateNumber);
            }
        }
        if (other.gameObject.GetComponent<NetworkPlayer>() != null)
        {
            other.gameObject.GetComponent<NetworkPlayer>().activeCheckpointIndex = gateNumber;
        }
    }
    public void Activate()
    {
        if (gateActivateVFX != null)
        {
            gateActivateVFX.Play();
        }
        /*        swirlGreen.Play();
        */
    }

    public void Deactivate()
    {
        if (gateActivateVFX != null)
        {
            gateActivateVFX.Stop();
        }
        /*        swirlGreen.Stop();
        */
    }
}
