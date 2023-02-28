using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    private Rigidbody _rb;

    [Header("Movement")]
    [SerializeField] private Vector3 maxMovementSpeed = new Vector3(400f, 400f, 400f);
    [SerializeField] private Vector3 maxSteeringForce = new Vector3(16f, 16f, 25f);
    private Vector3 _currentMovementSpeed = Vector3.zero;
    private Vector3 playerInput;

    [SerializeField] private float lookRateSpeed = 0.5f;
    [SerializeField] private float rollSpeed = 5f;
    [SerializeField] private float rollAcceleration = 0.5f;
    private float _activeRollSpeed;
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
            MyInput();
        }
    }

    void FixedUpdate()
    {
        MouseSteeringUpdate();
        RollUpdate();
        MovementUpdate();
    }

    private void MyInput()
    {
        playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Hover"), Input.GetAxisRaw("Vertical"));
    }

    private void MouseSteeringUpdate()
    {
        // Consistent method
        _lookInput = cursorRectTransform.position;

        _mouseDistance.y = (_lookInput.x - _screenCenter.x) / _screenCenter.y;
        _mouseDistance.x = -((_lookInput.y - _screenCenter.y) / _screenCenter.y);

        _mouseDistance = Vector3.ClampMagnitude(_mouseDistance, 1f);

        if (_mouseDistance.magnitude < 0.1f)
        {
            return;
        }
        _rb.AddRelativeTorque(lookRateSpeed * Vector3.Scale(_mouseDistance, maxSteeringForce), ForceMode.Acceleration);
    }

    private void RollUpdate()
    {
        float rollInput = Input.GetAxisRaw("Roll");
        _activeRollSpeed = Mathf.Lerp(_activeRollSpeed, rollInput, rollAcceleration * Time.deltaTime);
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
