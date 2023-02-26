using UnityEngine;

public class SpaceshipInputManager : MonoBehaviour
{
    [Header("Control Scheme")]
    private bool inputEnabled = true;
    private bool steeringEnabled = true;
    private bool movementEnabled = true;

    [Header("Mouse Steering")]
    [Tooltip("Whether the mouse position should control the steering.")]
    [SerializeField] private bool mouseSteeringEnabled = true;
    private string mouseDeltaXAxis = "Mouse X";
    private string mouseDeltaYAxis = "Mouse Y";

    [Header("Mouse Screen Position Settings")]
    [Tooltip("The fraction of the viewport (based on the screen width) around the screen center inside which the mouse position does not affect the ship steering.")]
    [SerializeField] private float mouseDeadRadius = 0f;
    [Tooltip("How far the mouse reticule is allowed to get from the screen center.")]
    [SerializeField] private float maxReticleDistanceFromCenter = 0.475f;
    [SerializeField] private float reticleMovementSpeed = 20f;
    [Tooltip("How much the ship pitches (local X axis rotation) based on the mouse distance from screen center.")]
    [SerializeField] private AnimationCurve mousePositionInputCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private bool centerMouseOnInputEnabled = true;
    
    private string rollAxisInput = "Roll";
    [SerializeField] private float yawRollRatio = 1f;

    [Header("Throttle")]
    [SerializeField] private KeyCode throttleUpKey = KeyCode.W;
    [SerializeField] private KeyCode throttleDownKey = KeyCode.S;
    [SerializeField] private float throttleSensitivity = 1f;

    [Header("Strafe")]
    private string strafeVerticalInput = "Strafe Vertical";
    private string strafeHorizontalInput = "Strafe Horizontal";
    
    [Header("Boost")]
    [SerializeField] private KeyCode boostInputKey = KeyCode.LeftShift;

    private Vector3 steeringInputs = Vector3.zero;
    private Vector3 movementInputs = Vector3.zero;
    private Vector3 boostInputs = Vector3.zero;

    private SpaceshipMovementManager movementManager;
    private HUDCursor hudCursor;
    private Vector3 reticleViewportPosition = new(0.5f, 0.5f, 0f);

    private void Awake()
    {
        movementManager = GetComponent<SpaceshipMovementManager>();
        hudCursor = GetComponentInChildren<HUDCursor>();
    }

    private void Update()
    {
        if (inputEnabled) { ProcessInput(); }
    }

    private void ProcessInput()
    {
        UpdateReticlePosition(new Vector3(Input.GetAxis(mouseDeltaXAxis), Input.GetAxis(mouseDeltaYAxis), 0f));

        if (steeringEnabled)
        {
            ProcessMouseSteering();
            movementManager.SetSteeringInputs(steeringInputs);
        }

        if (movementEnabled)
        {
            ProcessMovementInput();
            movementManager.SetMovementInputs(movementInputs);
        }
    }

    private void UpdateReticlePosition(Vector3 mouseDelta)
    {
        // Add the delta 
        reticleViewportPosition += new Vector3(mouseDelta.x / Screen.width, mouseDelta.y / Screen.height, 0);

        // Center it
        Vector3 centeredReticleViewportPosition = reticleViewportPosition - new Vector3(0.5f, 0.5f, 0);

        // Prevent distortion before clamping
        centeredReticleViewportPosition.x *= (float)Screen.width / Screen.height;

        // Clamp
        centeredReticleViewportPosition = Vector3.ClampMagnitude(centeredReticleViewportPosition, maxReticleDistanceFromCenter);

        // Convert back to proper viewport
        centeredReticleViewportPosition.x /= (float)Screen.width / Screen.height;

        reticleViewportPosition = centeredReticleViewportPosition + new Vector3(0.5f, 0.5f, 0f);
    }

    private void ProcessMouseSteering()
    {
        if (mouseSteeringEnabled is false) { return; }

        Vector3 screenInputs = Vector3.zero;
        Vector3 centeredViewportPos = reticleViewportPosition - new Vector3(0.5f, 0.5f, 0f);

        centeredViewportPos.x *= (float)Screen.width / Screen.height;

        float amount = Mathf.Max(centeredViewportPos.magnitude - mouseDeadRadius, 0) / (maxReticleDistanceFromCenter - mouseDeadRadius);

        centeredViewportPos.x /= (float)Screen.width / Screen.height;

        screenInputs = mousePositionInputCurve.Evaluate(amount) * centeredViewportPos.normalized;

        Pitch(-screenInputs.y);
        Roll(Input.GetAxis(rollAxisInput));
        Yaw(screenInputs.x);

        hudCursor.SetViewportPosition(reticleViewportPosition);
    }

    private void ProcessMovementInput()
    {
        movementInputs = movementManager.movementInputs;

        if (Input.GetKey(throttleUpKey)) { movementInputs.z += throttleSensitivity * Time.deltaTime; }
        else if (Input.GetKey(throttleDownKey)) { movementInputs.z -= throttleSensitivity * Time.deltaTime; }
        
        movementInputs.x = Input.GetAxis(strafeHorizontalInput);
        movementInputs.y = Input.GetAxis(strafeVerticalInput);
        
        if (Input.GetKeyDown(boostInputKey)) { SetBoost(1); }
        else if (Input.GetKeyUp(boostInputKey)) { SetBoost(0); }
    }

    public void EnableInput()
    {
        inputEnabled = true;

        if (centerMouseOnInputEnabled && hudCursor != null)
        {
            hudCursor.CenterCursor();
        }
    }
    public void DisableInput()
    {
        inputEnabled = false;

        // Reset the space vehicle engines to idle
        if (movementManager != null)
        {
            steeringInputs = Vector3.zero;
            movementManager.SetSteeringInputs(steeringInputs);

            movementInputs = Vector3.zero;
            movementManager.SetMovementInputs(movementInputs);

            boostInputs = Vector3.zero;
            movementManager.SetBoostInputs(boostInputs);
        }
    }
    public void EnableSteering()
    {
        steeringEnabled = true;
    }
    public void DisableSteering(bool clearCurrentValues)
    {
        steeringEnabled = false;

        if (clearCurrentValues)
        {
            // Set steering to zero
            steeringInputs = Vector3.zero;
            movementManager.SetSteeringInputs(steeringInputs);
        }
    }
    public void EnableMovement()
    {
        movementEnabled = true;
    }
    public void DisableMovement(bool clearCurrentValues)
    {
        movementEnabled = false;

        if (clearCurrentValues)
        {
            movementInputs = Vector3.zero;
            movementManager.SetMovementInputs(movementInputs);

            boostInputs = Vector3.zero;
            movementManager.SetBoostInputs(boostInputs);
        }
    }

    private void Pitch(float pitchAmount) => steeringInputs.x = Mathf.Clamp(pitchAmount, -1, 1);
    private void Yaw(float yawAmount) => steeringInputs.y = Mathf.Clamp(yawAmount, -1, 1);
    public void Roll(float rollAmount) => steeringInputs.z = Mathf.Clamp(rollAmount, -1, 1);

    public void SetBoost(float boostAmount)
    {
        boostInputs = new Vector3(0f, 0f, boostAmount);
        movementManager.SetBoostInputs(boostInputs);
    }
}
