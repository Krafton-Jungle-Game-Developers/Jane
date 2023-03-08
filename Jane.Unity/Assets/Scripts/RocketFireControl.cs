using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to control spawnrate by speed 
public class RocketFireControl : MonoBehaviour
{
    private Rigidbody _playerRB;
    private Vector3 _currentSpeed;
    private ParticleSystem rocketFire;
    private float _simSpeed;
    private float _minSimSpeed = 3f;
    [SerializeField] private float _correction = 100.0f;
    
    void Start()
    {
        _playerRB = GetComponentInParent<Rigidbody>();
        rocketFire = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (_playerRB != null)
        {
            _currentSpeed = _playerRB.velocity;
            _simSpeed = _currentSpeed.magnitude / _correction;
            SetFireSpawnRate();
        }
    }

    public void SetFireSpawnRate()
    {
        if (_playerRB != null )
        {
            var _main = rocketFire.main;

            if (_simSpeed < _minSimSpeed)
            {
                _main.simulationSpeed = _minSimSpeed;
            } 
            else
            {
                _main.simulationSpeed = _simSpeed;

            }
        }
    }
}
