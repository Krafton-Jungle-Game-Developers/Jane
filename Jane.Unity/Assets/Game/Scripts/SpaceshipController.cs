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
    private float _activeRollSpeed;
    [SerializeField] private float _rollBackSpeed = 5f;
    [SerializeField] private AnimationCurve curve;
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
        RollUpdate();
    }

    private void MouseSteeringUpdate()
    {
        // Consistent method
        _lookInput = cursorRectTransform.position;

        _mouseDistance.x = (_lookInput.x - _screenCenter.x) / _screenCenter.y;
        _mouseDistance.y = (_lookInput.y - _screenCenter.y) / _screenCenter.y;

        _mouseDistance = Vector3.ClampMagnitude(_mouseDistance, 1f);

        if (_mouseDistance.magnitude < 0.1f)
        {
            return;
        }

        transform.Rotate(-_mouseDistance.y * lookRateSpeed, _mouseDistance.x * lookRateSpeed, 0f, Space.Self);
    }

    float t;
    private void RollUpdate()
    {
        float rollInput = Input.GetAxisRaw("Roll");
        _activeRollSpeed = Mathf.Lerp(_activeRollSpeed, rollInput, rollAcceleration * Time.deltaTime);
        if(Mathf.Abs(rollInput) < 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f), Mathf.Exp(_rollBackSpeed * 0.1f) * Time.deltaTime);
        }
        transform.Rotate(0f, 0f, _activeRollSpeed * rollSpeed, Space.Self);
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

    // Method to Instantly change activeForwardSpeed 
    // by outside Elements
    // Currently used for BoosterImpact in BOOSTER.cs
    public void ChangeSpeedInstantly(float newSpeed)
    {
        _activeForwardSpeed = newSpeed;
    }

    // Method to Access Cursor X Position (true if Left)
    // by outside Elements
    // Currently used for BOOSTER.cs
    public bool GetIsCursorLeft()
    {
        if (_mouseDistance.x < 0 )
        {
            // Cursor is Left
            return true;
        } 
        else
        {
            return false;
        }
    }
}
