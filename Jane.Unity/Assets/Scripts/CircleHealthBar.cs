using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleHealthBar : MonoBehaviour
{
    [SerializeField] private Image _bar;
    [SerializeField] private float _visibleAngle = 45.0f;
    private Destroyable playerDestroyable;


    void Start()
    {
        playerDestroyable = GameObject.FindGameObjectWithTag("Player").GetComponent<Destroyable>();
        _visibleAngle = _bar.fillAmount;
    }

    void Update()
    {
        HealthChange(playerDestroyable.durability);
    }

    private void HealthChange(float _playerHP)
    {
        float amount = (_playerHP / playerDestroyable.maxHealth) * _visibleAngle;

        _bar.fillAmount = amount;
    }

}
