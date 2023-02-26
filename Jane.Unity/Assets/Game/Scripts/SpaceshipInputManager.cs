using UnityEngine;

public class SpaceshipInputManager : MonoBehaviour
{
    [Header("Control Scheme")]
    protected bool inputEnabled = true;
    [Tooltip("Whether the vehicle should yaw when rolling.")]
    [SerializeField] private bool linkYawAndRoll = false;
    [Tooltip("How much the vehicle should yaw when rolling.")]
    [SerializeField] private float yawRollRatio = 1;

    [Header("Auto Roll")]
    [SerializeField] private bool autoRollEnabled = true;
    [SerializeField] private float autoRollStrength = 0.04f;
    [SerializeField] private float maxAutoRoll = 0.2f;
    private float lastRollTime;

    [Header("Mouse Steering")]
    [Tooltip("Whether the mouse position should control the steering.")]
    [SerializeField] private bool mouseSteeringEnabled = true;
    [SerializeField] private MouseSteeringType mouseSteeringType;

    public virtual void SetMouseInputEnabled(bool setEnabled)
    {
        mouseSteeringEnabled = setEnabled;
    }

    [SerializeField] private string mouseDeltaXAxisName = "Mouse X";
    [SerializeField] private string mouseDeltaYAxisName = "Mouse Y";
    
    [Header("Mouse Screen Position Settings")]
    [Tooltip("The fraction of the viewport (based on the screen width) around the screen center inside which the mouse position does not affect the ship steering.")]
    [SerializeField] private float mouseDeadRadius = 0;
    [Tooltip("How far the mouse reticule is allowed to get from the screen center.")]
    [SerializeField] private float maxReticleDistanceFromCenter = 0.475f;
    [SerializeField] private float reticleMovementSpeed = 20f;
    [Tooltip("How much the ship pitches (local X axis rotation) based on the mouse distance from screen center.")]
    [SerializeField] private AnimationCurve mousePositionInputCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private bool centerMouseOnInputEnabled = true;

    [Header("Mouse Delta Position Settings")]
    [SerializeField] private float mouseDeltaPositionSensitivity = 0.75f;
    [SerializeField] private AnimationCurve mouseDeltaPositionInputCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Keyboard Steering")]
    [Tooltip("Invert the pitch (local X rotation) input.")]
    [SerializeField] private bool keyboardVerticalInverted = false;
    [Tooltip("Whether keyboard steering is enabled.")]
    [SerializeField] private bool keyboardSteeringEnabled = false;
    [Tooltip("The custom input controls for the pitch (local X axis rotation) steering.")]
    [SerializeField] private CustomInput pitchAxisInput = new CustomInput("Flight Controls", "Pitch", "Vertical");
    [Tooltip("The custom input controls for the yaw (local Y axis rotation) steering.")]
    [SerializeField] private CustomInput yawAxisInput = new CustomInput("Flight Controls", "Yaw", "Horizontal");
    [Tooltip("The custom input controls for the roll (local Z axis rotation) steering.")]
    [SerializeField] private CustomInput rollAxisInput = new CustomInput("Flight Controls", "Roll", "Roll");

    [Header("Throttle")]
    [Tooltip("The custom input controls for increasing the throttle.")]
    [SerializeField] private CustomInput throttleUpInput = new CustomInput("Flight Controls", "Throttle Up", KeyCode.Z);
    [Tooltip("The custom input controls for decreasing the throttle.")]
    [SerializeField] private CustomInput throttleDownInput = new CustomInput("Flight Controls", "Throttle Down", KeyCode.X);
    [Tooltip("How fast the throttle increases/decreases for each input axis.")]
    [SerializeField] private float throttleSensitivity = 1;
    [Tooltip("Whether to set the throttle value using the Throttle Axis Input value.")]
    [SerializeField] private bool setThrottle = false;
    [Tooltip("The custom input axis for controlling the throttle.")]
    [SerializeField] private CustomInput throttleAxisInput = new CustomInput("Flight Controls", "Move Forward/Backward", "Vertical");
    [Tooltip("The custom input axis for strafing the ship vertically.")]
    [SerializeField] private CustomInput strafeVerticalInput = new CustomInput("Flight Controls", "Strafe Vertical", "Strafe Vertical");
    [Tooltip("The custom input axis for strafing the ship horizontally.")]
    [SerializeField] private CustomInput strafeHorizontalInput = new CustomInput("Flight Controls", "Strafe Horizontal", "Strafe Horizontal");

    // The rotation, translation and boost inputs that are updated each frame
    private Vector3 steeringInputs = Vector3.zero;
    private Vector3 movementInputs = Vector3.zero;
    private Vector3 boostInputs = Vector3.zero;

    private bool steeringEnabled = true;
    private bool movementEnabled = true;

    [Header("Boost")]
    [SerializeField] private CustomInput boostInput = new CustomInput("Flight Controls", "Boost", KeyCode.Tab);
    // Reference to the engines component on the current vehicle
    private SpaceshipMovementManager spaceVehicleEngines;
    private HUDCursor hudCursor;
    private Vector3 reticleViewportPosition = new(0.5f, 0.5f, 0);
    
    private bool Initialize()
    {
        if (!base.Initialize(vehicle)) return false;
        
        spaceVehicleEngines = GetComponent<SpaceshipMovementManager>();
        hudCursor = GetComponentInChildren<HUDCursor>();
        
        return true;
    }

    private void Update()
    {
        if (inputEnabled) { InputUpdate(); }
    }

    private void InputUpdate()
    {
        UpdateReticlePosition(reticleMovementSpeed * new Vector3(Input.GetAxis(mouseDeltaXAxisName), Input.GetAxis(mouseDeltaYAxisName), 0));

        if (steeringEnabled)
        {
            if (mouseSteeringEnabled)
            {
                MouseSteeringUpdate();
            }
            else if (keyboardSteeringEnabled)
            {
                KeyboardSteeringUpdate();
            }
            
            spaceVehicleEngines.SetSteeringInputs(steeringInputs);
        }

        if (movementEnabled)
        {
            MovementUpdate();
            spaceVehicleEngines.SetMovementInputs(movementInputs);
        }

        if (autoRollEnabled) AutoRoll();
    }

    public void DisableInput()
    {
        inputEnabled = false;

        // Reset the space vehicle engines to idle
        if (spaceVehicleEngines != null)
        {
            steeringInputs = Vector3.zero;
            spaceVehicleEngines.SetSteeringInputs(steeringInputs);
            
            movementInputs = Vector3.zero;
            spaceVehicleEngines.SetMovementInputs(movementInputs);
            
            boostInputs = Vector3.zero;
            spaceVehicleEngines.SetBoostInputs(boostInputs);
        }
    }

    public void EnableInput()
    {
        inputEnabled = true;

        if (centerMouseOnInputEnabled && hudCursor != null)
        {
            hudCursor.CenterCursor();
        }
    }
    
    public virtual void EnableSteering()
    {
        steeringEnabled = true;
    }
    
    public virtual void DisableSteering(bool clearCurrentValues)
    {
        steeringEnabled = false;

        if (clearCurrentValues)
        {
            // Set steering to zero
            steeringInputs = Vector3.zero;
            spaceVehicleEngines.SetSteeringInputs(steeringInputs);
        }
    }
    
    public virtual void EnableMovement()
    {
        movementEnabled = true;
    }
    
    public virtual void DisableMovement(bool clearCurrentValues)
    {
        movementEnabled = false;

        if (clearCurrentValues)
        {
            movementInputs = Vector3.zero;
            spaceVehicleEngines.SetMovementInputs(movementInputs);
            
            boostInputs = Vector3.zero;
            spaceVehicleEngines.SetBoostInputs(boostInputs);
        }
    }

    private void UpdateReticlePosition(Vector3 mouseDelta)
    {
        // Add the delta 
        reticleViewportPosition += new Vector3(mouseDelta.x / Screen.width, mouseDelta.y / Screen.height, 0);

        // Center it
        Vector3 centeredreticleViewportPosition = reticleViewportPosition - new Vector3(0.5f, 0.5f, 0);

        // Prevent distortion before clamping
        centeredreticleViewportPosition.x *= (float)Screen.width / Screen.height;

        // Clamp
        centeredreticleViewportPosition = Vector3.ClampMagnitude(centeredreticleViewportPosition, maxReticleDistanceFromCenter);

        // Convert back to proper viewport
        centeredreticleViewportPosition.x /= (float)Screen.width / Screen.height;

        reticleViewportPosition = centeredreticleViewportPosition + new Vector3(0.5f, 0.5f, 0);
    }
    
    private void MouseSteeringUpdate()
    {
        Vector3 inputs = Vector3.zero;
        Vector3 centeredViewportPos = reticleViewportPosition - new Vector3(0.5f, 0.5f, 0);

        centeredViewportPos.x *= (float)Screen.width / Screen.height;

        float amount = Mathf.Max(centeredViewportPos.magnitude - mouseDeadRadius, 0) / (maxReticleDistanceFromCenter - mouseDeadRadius);

        centeredViewportPos.x /= (float)Screen.width / Screen.height;

        inputs = mousePositionInputCurve.Evaluate(amount) * centeredViewportPos.normalized;
        
        Pitch(-inputs.y);
        
        if (linkYawAndRoll)
        {
            Roll(-inputs.x);
            Yaw(Mathf.Clamp(-steeringInputs.z * yawRollRatio, -1f, 1f));
        }
        else
        {
            Roll(rollAxisInput.FloatValue());
            Yaw(inputs.x);
        }

        hudCursor.SetViewportPosition(reticleViewportPosition);
    }
    
    private void KeyboardSteeringUpdate()
    {
        Pitch((keyboardVerticalInverted ? -1 : 1) * pitchAxisInput.FloatValue());
        
        if (linkYawAndRoll)
        {
            Roll(-yawAxisInput.FloatValue());
            Yaw(Mathf.Clamp(-steeringInputs.z * yawRollRatio, -1f, 1f));
        }
        else
        {
            Roll(rollAxisInput.FloatValue());
            Yaw(yawAxisInput.FloatValue());
        }
    }
    
    private void MovementUpdate()
    {
        // Forward / backward movement
        movementInputs = spaceVehicleEngines.movementInputs;

        if (setThrottle)
        {
            movementInputs.z = throttleAxisInput.FloatValue();
        }
        else
        {
            if (throttleUpInput.Pressed())
            {
                movementInputs.z += throttleSensitivity * Time.deltaTime;
            }
            else if (throttleDownInput.Pressed())
            {
                movementInputs.z -= throttleSensitivity * Time.deltaTime;
            }
        }

        // Left / right movement
        movementInputs.x = strafeHorizontalInput.FloatValue();

        // Up / down movement
        movementInputs.y = strafeVerticalInput.FloatValue();

        // Boost
        if (boostInput.Down()) { SetBoost(1); }
        else if (boostInput.Up()) { SetBoost(0); }
    }

    private void Pitch(float pitchAmount) => steeringInputs.x = Mathf.Clamp(pitchAmount, -1, 1);
    private void Yaw(float yawAmount) => steeringInputs.y = Mathf.Clamp(yawAmount, -1, 1);
    public void Roll(float rollAmount)
    {
        steeringInputs.z = Mathf.Clamp(rollAmount, -1, 1);

        if (Mathf.Abs(rollAmount) > 0.0001f)
        {
            lastRollTime = Time.time;
        }
    }

    public void SetBoost(float boostAmount)
    {
        boostInputs = new Vector3(0f, 0f, boostAmount);
        spaceVehicleEngines.SetBoostInputs(boostInputs);
    }

    private void AutoRoll()
    {
        if (Time.time - lastRollTime < 0.5f) return;

        // Project the forward vector down
        Vector3 flattenedFwd = spaceVehicleEngines.transform.forward;
        flattenedFwd.y = 0;
        flattenedFwd.Normalize();

        // Get the right
        Vector3 right = Vector3.Cross(Vector3.up, flattenedFwd);

        float angle = Vector3.Angle(right, spaceVehicleEngines.transform.right);

        if (Vector3.Dot(spaceVehicleEngines.transform.up, right) > 0)
        {
            angle *= -1;
        }

        Vector3 steeringInputs = spaceVehicleEngines.steeringInputs;
        steeringInputs.z = Mathf.Clamp(angle * -1 * autoRollStrength, -1, 1);

        steeringInputs.z *= maxAutoRoll;

        steeringInputs.z *= 1 - Mathf.Abs(Vector3.Dot(spaceVehicleEngines.transform.forward, Vector3.up));

        spaceVehicleEngines.SetSteeringInputs(steeringInputs);
    }
}
