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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        // Probably some Activation VFX 
        gateActivateVFX.Play();
        
    }

    public void Deactivate()
    {
        gateActivateVFX.Stop();

    }
}
