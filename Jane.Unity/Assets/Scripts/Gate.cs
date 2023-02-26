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
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activateSFX;
    [SerializeField] private AudioClip gateClearSFX;    // Used for user Feedback

    public void Activate()
    {
        audioSource.clip = activateSFX;

        // Probably some Activation VFX 
        gateActivateVFX.Play();
        audioSource.Play();
        swirlGreen.Play();
    }

    public void Deactivate()
    {
        audioSource.clip = gateClearSFX;
        
        audioSource.Play();
        gateActivateVFX.Stop();
        swirlGreen.Stop();
    }
}
