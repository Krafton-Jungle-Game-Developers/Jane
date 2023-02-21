using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
using UnityEngine;

public class Booster : MonoBehaviour
{
    SpaceshipController spaceshipController;
    [SerializeField] private VisualEffect boosterImpactVFX;
    [SerializeField] private VisualEffect boosterLoopVFX;
    [SerializeField] private KeyCode boosterKey = KeyCode.LeftShift;
    [SerializeField] private float boosterSpeed = 400f;
    [SerializeField] private float normalSpeed = 200f;
    [SerializeField] private float _warpRate = 0.02f;
    private bool _boosterActive;

    private void Awake()
    {
        spaceshipController = GetComponent<SpaceshipController>();
    }

    
    void Start()
    {
        boosterImpactVFX.Stop();
        boosterLoopVFX.Stop();
        boosterLoopVFX.SetFloat("WarpAmount", 0);
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
            boosterLoopVFX.Play();

            float _warpAmount = boosterLoopVFX.GetFloat("WarpAmount");
            while (_warpAmount < 1) 
            {
                _warpAmount += _warpRate;
                boosterLoopVFX.SetFloat("WarpAmount", _warpAmount);
            }
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            spaceshipController.ChangeSpeed(normalSpeed);

            float _warpAmount = boosterLoopVFX.GetFloat("WarpAmount");
            while (_warpAmount >= _warpRate)
            {
                _warpAmount -= _warpRate;
                boosterLoopVFX.SetFloat("WarpAmount", _warpAmount);
            }
            //boosterLoopVFX.SetFloat("WarpAmount", 0);

            boosterLoopVFX.Stop();
        }
    }
}
