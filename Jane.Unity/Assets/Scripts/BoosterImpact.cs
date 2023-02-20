using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
using UnityEngine;

public class BoosterImpact : MonoBehaviour
{

    [SerializeField] VisualEffect boosterImpactVFX;
    [SerializeField] KeyCode boosterKey = KeyCode.LeftShift;
    private bool _boosterActive;
    
    // Start is called before the first frame update
    void Start()
    {
        boosterImpactVFX.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(boosterKey))
        {
            _boosterActive = true;
            StartCoroutine(ActivateImpactParticles());
        }
        else
        {
            _boosterActive= false;
            StartCoroutine(ActivateImpactParticles());
        }
    }
    
    IEnumerator ActivateImpactParticles()
    {
        if (_boosterActive)
        {
            boosterImpactVFX.Play();
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            boosterImpactVFX.Stop();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
