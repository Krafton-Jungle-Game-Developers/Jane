using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// This Script is for individual Gates, which are elements of CheckPointArr.
/// </summary>
public class Gate : MonoBehaviour
{
    [SerializeField] private VisualEffect gateActivateVFX;
    [SerializeField] private ParticleSystem swirlGreen;
    

    public void Activate()
    {
        // Probably some Activation VFX 
        gateActivateVFX.Play();

        swirlGreen.Play();
    }

    public void Deactivate()
    {
        gateActivateVFX.Stop();
        swirlGreen.Stop();
    }
}
