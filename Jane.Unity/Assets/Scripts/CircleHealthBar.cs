using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleHealthBar : MonoBehaviour
{
    [SerializeField] private Image _bar;
    [SerializeField] private float _visibleAngle = 45.0f;
    private Destroyable playerDestroyable;

    [SerializeField] private Material _lowHealthScreenMat;

    void Start()
    {
        playerDestroyable = GameObject.FindGameObjectWithTag("Player").GetComponent<Destroyable>();
        _visibleAngle = _bar.fillAmount;
        _lowHealthScreenMat.SetFloat("_FullScreenIntensity", 0f);
    }

    void Update()
    {
        if(playerDestroyable != null) { 
        HealthChange(playerDestroyable.durability);
        }
    }

    private void HealthChange(float _playerHP)
    {
        float amount = (_playerHP / playerDestroyable.maxHealth) * _visibleAngle;

        _bar.fillAmount = amount;

        if (_playerHP < playerDestroyable.maxHealth / 3 )
        {
            _lowHealthScreenMat.SetFloat("_FullScreenIntensity", 0.2f);
        } 
        else if (_playerHP >= playerDestroyable.maxHealth / 3 )
        {
            _lowHealthScreenMat.SetFloat("_FullScreenIntensity", 0f);
        }
    }

}
