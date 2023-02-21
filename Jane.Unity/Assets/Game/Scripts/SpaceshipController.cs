using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    /*    [SerializeField] private Rigidbody _rb;
        [SerializeField] private float mouseSensitivity;
        public int steerVersion = 1;
    */

    [SerializeField] private float lookRateSpeed = 0.5f;
    [SerializeField] private float forwardSpeed = 200f, strafeSpeed = 50f, hoverSpeed = 50f, rollSpeed = 5f;
    private float _activeForwardSpeed, _activeStrafeSpeed, _activeHoverSpeed;
    [SerializeField] private float _forwardAcceleration = 2f, _strafeAcceleration = 2f, _hoverAcceleration = 2f, rollAcceleration = 0.5f;
    private float _rollInput;
    private Vector3 _lookInput, _screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f), _mouseDistance;

    public RectTransform cursorRectTransform;


    void Start()
    {
        /* _rb = GetComponent<Rigidbody>(); */        
    }

    void Update()
    {
        MouseSteeringUpdate();
        MovementUpdate();
        Debug.Log("Active Forward Speed : " + _activeForwardSpeed);
    }

    private void MouseSteeringUpdate()
    {
        // Consistent method
        _lookInput = cursorRectTransform.position;

        _mouseDistance.x = (_lookInput.x - _screenCenter.x) / _screenCenter.y;
        _mouseDistance.y = (_lookInput.y - _screenCenter.y) / _screenCenter.y;

        _mouseDistance = Vector3.ClampMagnitude(_mouseDistance, 1f);

        _rollInput = Mathf.Lerp(_rollInput, Input.GetAxisRaw("Roll"), rollAcceleration * Time.deltaTime);

        transform.Rotate(-_mouseDistance.y * lookRateSpeed, _mouseDistance.x * lookRateSpeed, _rollInput * rollSpeed, Space.Self);

        // Raw input method
/*        else if (steerVersion == 2)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

            _yRotation -= mouseX;
            _xRotation -= mouseY;

            _mouseDistance = new Vector3(mouseX, mouseX, 0f);

            _rollInput = Mathf.Lerp(_rollInput, Input.GetAxisRaw("Roll"), rollAcceleration * Time.deltaTime);

            transform.rotation = Quaternion.Euler(0, _yRotation, 0);
            transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        }*/
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


    // Method for Controlling Speed by outside elements 
    // Currently used by BOOSTER.cs 
    public void ChangeSpeed(float newSpeed)
    {
        forwardSpeed= newSpeed;
        Debug.Log("Change Speed : " + forwardSpeed);
    }
}
