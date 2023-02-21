using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
using UnityEngine;

public class Booster : MonoBehaviour
{
    SpaceshipController spaceshipController;
    [SerializeField] VisualEffect boosterImpactVFX;
    [SerializeField] KeyCode boosterKey = KeyCode.LeftShift;
    [SerializeField] float boosterSpeed = 400f;
    [SerializeField] float normalSpeed = 200f;
    private bool _boosterActive;

    private void Awake()
    {
        spaceshipController = GetComponent<SpaceshipController>();
    }

    
    void Start()
    {
        boosterImpactVFX.Stop();
    }

    
    void Update()
    {
        if(Input.GetKeyDown(boosterKey))
        {
            _boosterActive = true;
            boosterImpactVFX.Play();
            StartCoroutine(ActivateBooster());
        }
        else if (Input.GetKeyUp(boosterKey))
        {
            _boosterActive= false;
            boosterImpactVFX.Stop();
            StartCoroutine(ActivateBooster());
        }
    }
    
    IEnumerator ActivateBooster()
    {
        if (_boosterActive)
        {
            yield return new WaitForSeconds(0.6f);
            spaceshipController.ChangeSpeed(boosterSpeed);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            spaceshipController.ChangeSpeed(normalSpeed);
        }
    }
}
