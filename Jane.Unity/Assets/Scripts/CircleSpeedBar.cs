using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleSpeedBar : MonoBehaviour
{
    [SerializeField] private Image _bar;
    [SerializeField] private float _visibleAngle;

    private Rigidbody _playerRB;
    private float _playerSpeed;
    [SerializeField] float _maxSpeed;

    void Start()
    {
        _playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        _playerSpeed = _playerRB.velocity.magnitude;
    }

    
    void Update()
    {
        if (_playerRB != null)
        {
            _playerSpeed = _playerRB.velocity.magnitude;
            DisplaySpeedChange(_playerSpeed);
        }
    }

    private void DisplaySpeedChange(float _speed)
    {
        float amount = (_speed / _maxSpeed) * (_visibleAngle/360);
        amount = Mathf.Clamp(amount, 0, 0.23f);

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
