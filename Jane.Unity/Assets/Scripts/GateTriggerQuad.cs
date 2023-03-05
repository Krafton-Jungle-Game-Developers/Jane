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
    private NetworkPlayer networkPlayer;

    private void OnTriggerEnter(Collider other)
    {
        CheckPoints checkPoints = GetComponentInParent<CheckPoints>();
        if (other.gameObject.CompareTag("Player"))
        {
            if (checkPoints.nextGate == this.gameObject)
            {
                checkPoints.ControlGates();
            }
        }
        if (other.gameObject.GetComponent<NetworkPlayer>() != null)
        {
            other.gameObject.GetComponent<NetworkPlayer>().activeCheckpointIndex = checkPoints.idx;
            Debug.Log(other.gameObject.GetComponent<NetworkPlayer>().activeCheckpointIndex);
        }
    }
    public void Activate()
    {
        gateActivateVFX.Play();
        swirlGreen.Play();
    }

    public void Deactivate()
    {
        gateActivateVFX.Stop();
        swirlGreen.Stop();
    }
}
