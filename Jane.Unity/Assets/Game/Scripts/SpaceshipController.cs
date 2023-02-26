using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    private Rigidbody _rb;

    [Header("Movement")]
    [SerializeField] private float yawRotation = 500f;
    [SerializeField] private float pitchRotation = 1000f;
    [SerializeField] private float rollRotation = 1000f;
    [SerializeField] private Vector3 maxMovementSpeed = new Vector3(400f, 400f, 400f);
    private Vector3 _currentMovementSpeed = Vector3.zero;
    [SerializeField, Range(0.001f, 0.999f)] private float forwardDeceleration, strafeDeceleration, hoverDeceleration;
    private Vector3 playerInput;
    private Vector2 pitchYawInput;
    private float rollInput;
    private Vector3 glide = new Vector3(0f, 0f, 0f);

    [SerializeField] private float lookRateSpeed = 0.5f;
    [SerializeField] private float rollSpeed = 5f;
    private float _activeForwardSpeed, _activeStrafeSpeed, _activeHoverSpeed;
    [SerializeField] private float _forwardAcceleration = 2f, _strafeAcceleration = 2f, _hoverAcceleration = 2f, rollAcceleration = 0.5f;
    private float _activeRollSpeed;
    [SerializeField] private float _rollBackSpeed = 5f;
    private Vector3 _lookInput, _screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f), _mouseDistance;

    public RectTransform cursorRectTransform;
    [HideInInspector] public bool canControl;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (canControl)
        {
        }
    }

    void FixedUpdate()
    {
        MouseSteeringUpdate();
        RollUpdate();
        MyInput();
        MovementUpdate();
    }

    private void MyInput()
    {
        playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Hover"), Input.GetAxisRaw("Vertical"));
        Debug.Log($"{playerInput}");
        rollInput = Input.GetAxisRaw("Roll");

        Vector3 mouseLocation = cursorRectTransform.position;
        pitchYawInput.x = (mouseLocation.x - _screenCenter.x) / _screenCenter.y;
        pitchYawInput.y = (mouseLocation.y - _screenCenter.y) / _screenCenter.y;
        pitchYawInput = Vector3.ClampMagnitude(pitchYawInput, 1f);
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

    private void RollUpdate()
    {
        float rollInput = Input.GetAxisRaw("Roll");
        /*        _rb.AddRelativeTorque(Vector3.forward * rollRotation * rollInput * Time.deltaTime);
                _rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYawInput.y, -1f, 1f) * pitchRotation * Time.deltaTime);
                _rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYawInput.x, -1f, 1f) * yawRotation * Time.deltaTime);
        */        /*        float xAngle = transform.rotation.eulerAngles.x;
                        float yAngle = transform.rotation.eulerAngles.y;


                */
        _activeRollSpeed = Mathf.Lerp(_activeRollSpeed, rollInput, rollAcceleration * Time.deltaTime);
        /*        if (Mathf.Abs(rollInput) < 0.1f && _mouseDistance.magnitude < 0.1f && (0f < Mathf.Abs(xAngle) && Mathf.Abs(xAngle) < 60f))
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(xAngle, yAngle, 0f),
                                                         _rollBackSpeed * Mathf.Abs((xAngle) - 60f) / 60f * Time.deltaTime);
                }
                else if (Mathf.Abs(rollInput) < 0.1f && _mouseDistance.magnitude < 0.1f && (300f < Mathf.Abs(xAngle) && Mathf.Abs(xAngle) < 360f))
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(xAngle, yAngle, 0f),
                                                         _rollBackSpeed * Mathf.Abs((xAngle) - 300f) / 60f * Time.deltaTime);
                }
        */
        transform.Rotate(0f, 0f, _activeRollSpeed * rollSpeed, Space.Self);
        if (Mathf.Abs(rollInput) < 0.1f)
        {
            transform.Rotate(0f, 0f, 0f, Space.Self);
        }

    }

    private void MovementUpdate()
    {

        Vector3 nextMovementSpeed = Vector3.Scale(playerInput, maxMovementSpeed * 100f);

        nextMovementSpeed = Vector3.Lerp(_currentMovementSpeed, nextMovementSpeed, 5f * Time.deltaTime);
        _currentMovementSpeed = nextMovementSpeed;

        _rb.AddRelativeForce(nextMovementSpeed);
    }

    // Method for Controlling Speed by outside elements 
    // Currently used by BOOSTER.cs 
    public void ChangeSpeed(float newSpeed)
    {
        /*        forwardSpeed= newSpeed;
        */        //Debug.Log("Change Speed : " + forwardSpeed);
    }

    // Method to Instantly change activeForwardSpeed 
    // by outside Elements
    // Currently used for BoosterImpact in BOOSTER.cs
    public void ChangeSpeedInstantly(float newSpeed)
    {
        /*        _activeForwardSpeed = newSpeed;
        */
    }

    // Method to Access Cursor X Position (true if Left)
    // by outside Elements
    // Currently used for BOOSTER.cs
    public bool GetIsCursorLeft()
    {
        if (_mouseDistance.x < 0)
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
