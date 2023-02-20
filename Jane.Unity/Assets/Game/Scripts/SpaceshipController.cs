using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float mouseSensitivity;
    private float _xRotation;
    private float _yRotation;

    public float forwardSpeed = 25f;
    public float strafeSpeed = 15f;
    public float hoverSpeed = 15f;
    private float _activeForwardSpeed;
    private float _activeStrafeSpeed;
    private float _activeHoverSpeed;
    [SerializeField] private float _forwardAcceleration = 5f;
    [SerializeField] private float _strafeAcceleration = 2f;
    [SerializeField] private float _hoverAcceleration = 2f;

    public float lookRateSpeed = 0.5f;
    private Vector3 _lookInput, _screenCenter, _mouseDistance;

    private float _rollInput;
    public float rollSpeed = 5f ;
    public float rollAcceleration = 0.5f;
    public int steerVersion = 1;
    private Transform _camTransform;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _screenCenter = new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        //_camTransform = GetComponentInChildren<Camera>().transform;
    }

    void Update()
    {
        MouseSteeringUpdate();
        MovementUpdate();
    }

    private void MouseSteeringUpdate()
    {
        // Consistent method
        if (steerVersion == 1)
        {
            _lookInput = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            Debug.Log($"mouse input:{_mouseDistance}");

            _mouseDistance.x = (_lookInput.x - _screenCenter.x) / _screenCenter.y;
            _mouseDistance.y = (_lookInput.y - _screenCenter.y) / _screenCenter.y;

            _mouseDistance = Vector3.ClampMagnitude(_mouseDistance, 1f);

            _rollInput = Mathf.Lerp(_rollInput, Input.GetAxisRaw("Roll"), rollAcceleration * Time.deltaTime);

            transform.Rotate(-_mouseDistance.y * lookRateSpeed, _mouseDistance.x * lookRateSpeed, _rollInput * rollSpeed, Space.Self);
        }
        // Raw input method
        else if (steerVersion == 2)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

            _yRotation -= mouseX;
            _xRotation -= mouseY;

            _mouseDistance = new Vector3(mouseX, mouseX, 0f);

            _rollInput = Mathf.Lerp(_rollInput, Input.GetAxisRaw("Roll"), rollAcceleration * Time.deltaTime);

            transform.rotation = Quaternion.Euler(0, _yRotation, 0);
            transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        }
    }
    private void MovementUpdate()
    {
        _activeForwardSpeed = Mathf.Lerp(_activeForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed, Time.deltaTime * _forwardAcceleration);
        _activeStrafeSpeed = Mathf.Lerp(_activeStrafeSpeed, Input.GetAxisRaw("Horizontal") * strafeSpeed, Time.deltaTime * _strafeAcceleration);
        _activeHoverSpeed = Mathf.Lerp(_activeHoverSpeed, Input.GetAxisRaw("Hover") * hoverSpeed, Time.deltaTime * _hoverAcceleration);

        transform.position += (transform.forward * _activeForwardSpeed * Time.deltaTime) +
                              (transform.right * _activeStrafeSpeed * Time.deltaTime) +
                              (transform.up * _activeHoverSpeed * Time.deltaTime);
    }
}
