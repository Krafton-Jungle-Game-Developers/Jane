using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraMovement : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.04f;
    [SerializeField] private float frequency = 100f;
    private Vector3 initialPosition; //초기 위치

    [SerializeField]private Camera _playerCamera;
    // Start is called before the first frame update
    void Start()
    {
        _playerCamera = GetComponentInParent<Camera>();
        initialPosition = _playerCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float x = initialPosition.x + Mathf.Sin(Time.time * frequency) * amplitude;
        float y = initialPosition.y + Mathf.Cos(Time.time * frequency) * amplitude;
        float z = initialPosition.z;
        transform.position = new Vector3(x, y, z);
    }
}
