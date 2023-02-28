using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipInputManager : MonoBehaviour
{
    private SpaceshipInputAction spaceshipInput;
    private GeneralInputAction generalInput;

    private float acceleration, roll;
    private Vector2 steering, strafing;
    
    [Header("Auto Roll")]
    [SerializeField] private bool autoRollEnabled = true;
    [SerializeField] private float autoRollStrength = 0.04f;
    [SerializeField] private float maxAutoRoll = 0.2f;
    private float lastRollTime;

    [SerializeField] private bool mouseEnabled = true;
    [SerializeField] private bool inputEnabled = true;
    [SerializeField] private bool steeringEnabled = true;
    [SerializeField] private bool movementEnabled = true;

    [Header("Mouse Screen Position Settings")]
    [Tooltip("The fraction of the viewport (based on the screen width) around the screen center inside which the mouse position does not affect the ship steering.")]
    [SerializeField] private float mouseDeadRadius = 0.1f;
    [Tooltip("How far the mouse reticule is allowed to get from the screen center.")]
    [SerializeField] private float maxReticleDistanceFromCenter = 0.475f;
    [SerializeField] private float reticleMovementSpeed = 1;
    [Tooltip("How much the ship pitches (local X axis rotation) based on the mouse distance from screen center.")]
    [SerializeField] private AnimationCurve mousePositionInputCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private bool centerCursorOnInputEnabled = true;

    [Header("Mouse Delta Position Settings")]
    [SerializeField] private float mouseDeltaPositionSensitivity = 0.75f;
    [SerializeField] private AnimationCurve mouseDeltaPositionInputCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Throttle")]
    [SerializeField] private bool setThrottle = false;
    [SerializeField] private float throttleSensitivity = 1;

    // The rotation, translation and boost inputs that are updated each frame
    private Vector3 mouseSteeringInputs = Vector3.zero;
    private Vector3 steeringInputs = Vector3.zero;

    private Vector3 movementInputs = Vector3.zero;
    private Vector3 boostInputs = Vector3.zero;
    
    [Header("Boost")]
    [SerializeField] private float boostChangeSpeed = 3;
    private Vector3 boostTarget = Vector3.zero;

    [SerializeField] private SpaceshipEngine spaceEngine;

    [SerializeField] private HUDCursor hudCursor;
    private Vector3 reticuleViewportPosition = new (0.5f, 0.5f, 0);

    private void Awake()
    {
        generalInput = new();
        spaceshipInput = new();

        spaceshipInput.SpaceshipControls.Steer.performed += ctx => steering = ctx.ReadValue<Vector2>();
        spaceshipInput.SpaceshipControls.Strafe.performed += ctx => strafing = ctx.ReadValue<Vector2>();
        spaceshipInput.SpaceshipControls.Roll.performed += ctx => GetRollInput(ctx.ReadValue<float>());
        spaceshipInput.SpaceshipControls.Throttle.performed += ctx => acceleration = ctx.ReadValue<float>();
        spaceshipInput.SpaceshipControls.Boost.performed += ctx => boostTarget.z = ctx.ReadValue<float>();
    }

    private void Update()
    {
        if (inputEnabled) { InputUpdate(); }
    }

    private void GetRollInput(float rollAmount)
    {
        roll = rollAmount;
    }

    private void OnEnable()
    {
        generalInput.Enable();
        spaceshipInput.Enable();
    }

    private void OnDisable()
    {
        generalInput.Disable();
        spaceshipInput.Disable();
    }
    
    public void EnableInput()
    {
        inputEnabled = true;

        if (centerCursorOnInputEnabled && hudCursor != null)
        {
            hudCursor.CenterCursor();
        }
    }
    
    public void DisableInput()
    {
        inputEnabled = false;
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
            steeringInputs = Vector3.zero;
            spaceEngine.SetSteeringInputs(steeringInputs);
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
            // Set movement to zero
            movementInputs = Vector3.zero;
            spaceEngine.SetMovementInputs(movementInputs);

            // Set boost to zero
            boostTarget = Vector3.zero;
            boostInputs = Vector3.zero;
            spaceEngine.SetBoostInputs(boostInputs);
        }
    }

    private void UpdateReticulePosition(Vector3 mouseDelta)
    {
        // Add the delta 
        reticuleViewportPosition += new Vector3(mouseDelta.x / Screen.width, mouseDelta.y / Screen.height, 0) * reticleMovementSpeed;

        // Center it
        Vector3 centeredReticuleViewportPosition = reticuleViewportPosition - new Vector3(0.5f, 0.5f, 0);

        // Prevent distortion before clamping
        centeredReticuleViewportPosition.x *= (float)Screen.width / Screen.height;

        // Clamp
        centeredReticuleViewportPosition = Vector3.ClampMagnitude(centeredReticuleViewportPosition, maxReticleDistanceFromCenter);

        // Convert back to proper viewport
        centeredReticuleViewportPosition.x /= (float)Screen.width / Screen.height;

        reticuleViewportPosition = centeredReticuleViewportPosition + new Vector3(0.5f, 0.5f, 0);
    }
    
    private void MouseSteeringUpdate()
    {
        mouseSteeringInputs = Vector3.zero;

        if (!mouseEnabled) return;

        Vector3 screenInputs = Vector3.zero;
        Vector3 centeredViewportPos = reticuleViewportPosition - new Vector3(0.5f, 0.5f, 0);

        centeredViewportPos.x *= (float)Screen.width / Screen.height;
        float amount = Mathf.Max(centeredViewportPos.magnitude - mouseDeadRadius, 0) / (maxReticleDistanceFromCenter - mouseDeadRadius);
        centeredViewportPos.x /= (float)Screen.width / Screen.height;

        screenInputs = mousePositionInputCurve.Evaluate(amount) * centeredViewportPos.normalized;

        mouseSteeringInputs.x = -screenInputs.y;
        mouseSteeringInputs.y = screenInputs.x;
        
        if (hudCursor != null)
        {
            hudCursor.SetViewportPosition(reticuleViewportPosition);
        }
    }
    
    private void MovementUpdate()
    {
        // Forward / backward movement
        Vector3 movementInputs = spaceEngine.movementInputs;

        if (setThrottle)
        {
            movementInputs.z = acceleration;
        }
        else
        {
            movementInputs.z += acceleration * throttleSensitivity * Time.deltaTime;
        }

        // Left / right movement
        movementInputs.x = strafing.x;

        // Up / down movement
        movementInputs.y = strafing.y;

        spaceEngine.SetMovementInputs(movementInputs);

        boostInputs = Vector3.Lerp(boostInputs, boostTarget, boostChangeSpeed * Time.deltaTime);
        if (boostInputs.magnitude < 0.0001f) boostInputs = Vector3.zero;
        spaceEngine.SetBoostInputs(boostInputs);
    }
    
    private void OnRoll(float rollAmount)
    {
        if (Mathf.Abs(rollAmount) > 0.0001f)
        {
            lastRollTime = Time.time;
        }
    }
    
    public void SetBoost(float boostAmount)
    {
        boostTarget = new Vector3(0f, 0f, boostAmount);
    }
    
    private void AutoRoll()
    {
        if (Time.time - lastRollTime < 0.5f) return;

        // Project the forward vector down
        Vector3 flattenedFwd = spaceEngine.transform.forward;
        flattenedFwd.y = 0;
        flattenedFwd.Normalize();

        // Get the right
        Vector3 right = Vector3.Cross(Vector3.up, flattenedFwd);

        float angle = Vector3.Angle(right, spaceEngine.transform.right);

        if (Vector3.Dot(spaceEngine.transform.up, right) > 0)
        {
            angle *= -1;
        }

        steeringInputs.z = Mathf.Clamp(angle * -1 * autoRollStrength, -1, 1);
        steeringInputs.z *= maxAutoRoll;
        steeringInputs.z *= 1 - Mathf.Abs(Vector3.Dot(spaceEngine.transform.forward, Vector3.up));
    }
    
    private void InputUpdate()
    {
        // Pitch
        steeringInputs.x = Mathf.Clamp(-steering.y, -1, 1);
        // Roll
        steeringInputs.z = Mathf.Clamp(roll, -1, 1);
        // Yaw
        steeringInputs.y = Mathf.Clamp(steering.x, -1f, 1f);

        UpdateReticulePosition(generalInput.GeneralControls.MouseDelta.ReadValue<Vector2>());
        MouseSteeringUpdate();

        steeringInputs = new Vector3(Mathf.Abs(steeringInputs.x) > Mathf.Abs(mouseSteeringInputs.x) ? steeringInputs.x : mouseSteeringInputs.x,
                                        Mathf.Abs(steeringInputs.y) > Mathf.Abs(mouseSteeringInputs.y) ? steeringInputs.y : mouseSteeringInputs.y,
                                        Mathf.Abs(steeringInputs.z) > Mathf.Abs(mouseSteeringInputs.z) ? steeringInputs.z : mouseSteeringInputs.z);

        if (Mouse.current == null || !mouseEnabled)
        {
            hudCursor.CenterCursor();
            reticuleViewportPosition = new Vector3(0.5f, 0.5f, 0);
        }
        
        OnRoll(steeringInputs.z);

        MovementUpdate();

        if (autoRollEnabled) AutoRoll();

        spaceEngine.SetSteeringInputs(steeringInputs);
    }
}
