using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleSpeedBar : MonoBehaviour
{
    [SerializeField] private Image _bar;
    [SerializeField] private float _visibleAngle = 45.0f;

    private Rigidbody _playerRB;
    private Vector3 _playerSpeed;
    [SerializeField] float _maxSpeed = 310f;

    void Start()
    {
        _playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        _playerSpeed = _playerRB.velocity;
    }

    
    void Update()
    {
        if (_playerRB != null)
        {
            _playerSpeed = _playerRB.velocity;
            DisplaySpeedChange(_playerSpeed.magnitude);
        }
    }

    private void DisplaySpeedChange(float _speed)
    {
        float amount = (_speed / _maxSpeed) * _visibleAngle;

        _bar.fillAmount = amount;

        //if (_playerHP < playerDestroyable.maxHealth / 3)
        //{
        //    _lowHealthScreenMat.SetFloat("_FullScreenIntensity", 0.2f);
        //}
        //else if (_playerHP >= playerDestroyable.maxHealth / 3)
        //{
        //    _lowHealthScreenMat.SetFloat("_FullScreenIntensity", 0f);
        //}
    }
}
