using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
using UnityEngine;
using Jane.Unity;

public class Booster : MonoBehaviour
{
    //SpaceshipController spaceshipController;
    SpaceshipInputManager spaceshipInputManager;

    [Header("Reference and Keys")]
    [SerializeField] private VisualEffect boosterImpactVFX;
    [SerializeField] private VisualEffect boosterLoopVFX;
    [SerializeField] private KeyCode boosterKey = KeyCode.LeftShift;

    [Space]
    //[Header("Booster Settings")]
    //[SerializeField] private float _boosterSpeed = 400f;
    //[SerializeField] private float _normalSpeed = 200f;
    [SerializeField] private float _warpRate = 0.05f;
    //[SerializeField] private bool _instantSpeed = true;

    public bool _isBoosterActive;

    private void Awake()
    {
        //spaceshipController = GetComponent<SpaceshipController>();
        spaceshipInputManager = GameObject.FindObjectOfType<SpaceshipInputManager>();
    }

    
    void Start()
    {
        boosterImpactVFX.Stop();
        boosterLoopVFX.Stop();
        boosterLoopVFX.SetFloat("WarpAmount", 0);
    }

    
    void Update()
    {
        //if(spaceshipController.canControl)
        if (spaceshipInputManager.CanControl())
        {
            if(Input.GetKeyDown(boosterKey))
            {
                _isBoosterActive = true;
                boosterImpactVFX.Play();
                StartCoroutine(ActivateBooster());
            }
            else if (Input.GetKeyUp(boosterKey))
            {
                _isBoosterActive= false;
                boosterImpactVFX.Stop();
                StartCoroutine(ActivateBooster());
            }
        }
        if (_isBoosterActive)
        {
            // Booster SpeedLine Left/Right Rotation management
            //if (spaceshipController.GetIsCursorLeft())
            if (spaceshipInputManager.IsCursorLeft())
            {
                boosterLoopVFX.SetBool("isLeft", true);
            }
            else
            {
                boosterLoopVFX.SetBool("isLeft", false);
            }
        }
    }
    
    IEnumerator ActivateBooster()
    {
        if (_isBoosterActive)
        {
            yield return new WaitForSeconds(0.6f);
            // if (_instantSpeed)
            // {
            //     spaceshipController.ChangeSpeedInstantly(_boosterSpeed);
            // }
            // spaceshipController.ChangeSpeed(_boosterSpeed);
            //spaceshipInputManager.SetBoost(1f);
            boosterLoopVFX.Play();

            float _warpAmount = boosterLoopVFX.GetFloat("WarpAmount");
            
            // for smooth SpeedLine 
            while(_warpAmount < 1) 
            {
                _warpAmount += _warpRate;
                boosterLoopVFX.SetFloat("WarpAmount", _warpAmount);
            }
        }
        else
        {
            yield return new WaitForSeconds(0.6f);
            // spaceshipController.ChangeSpeed(_normalSpeed);

            //spaceshipInputManager.SetBoost(0f);
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
