using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleHealthBar : MonoBehaviour
{
    [SerializeField] private Image _bar;
    [SerializeField] private float _visibleAngle = 180.0f;
    private Destroyable playerDestroyable;


    void Start()
    {
        playerDestroyable = GameObject.FindGameObjectWithTag("Player").GetComponent<Destroyable>();

    }

    void Update()
    {
        HealthChange(playerDestroyable.durability);
    }

    private void HealthChange(int _playerHP)
    {
        float amount = (_playerHP / playerDestroyable.maxHealth) * _visibleAngle / 360;
        _bar.fillAmount = amount;
    }

}
