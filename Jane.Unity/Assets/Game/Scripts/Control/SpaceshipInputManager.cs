using UnityEngine;
using UnityEngine.InputSystem;

namespace Jane.Unity
{
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
        [SerializeField] private float mouseDeadRadius = 0.1f;
        [SerializeField] private float maxReticleDistanceFromCenter = 0.475f;
        [SerializeField] private float reticleMovementSpeed = 1f;
        [SerializeField] private AnimationCurve mousePositionInputCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private bool centerCursorOnInputEnabled = true;

        private Vector3 mouseSteeringInputs = Vector3.zero;
        private Vector3 steeringInputs = Vector3.zero;
        private Vector3 movementInputs = Vector3.zero;
        private Vector3 boostInputs = Vector3.zero;

        [Header("Boost")]
        [SerializeField] private float boostChangeSpeed = 3f;
        private Vector3 boostTarget = Vector3.zero;

        [SerializeField] private SpaceshipEngine spaceEngine;
        public SpaceshipEngine SpaceEngine
        {
            get => spaceEngine;
            set => spaceEngine = value;
        }
        [SerializeField] private HUDManager hudCursor;

        public HUDManager HUDCursor
        {
            get => hudCursor;
            set => hudCursor = value;
        }
        private Vector3 reticuleViewportPosition = new(0.5f, 0.5f, 0f);

        private void Awake()
        {
            generalInput = new();
            spaceshipInput = new();

            spaceshipInput.SpaceshipControls.Steer.performed += ctx => steering = ctx.ReadValue<Vector2>();
            spaceshipInput.SpaceshipControls.Strafe.performed += ctx => strafing = ctx.ReadValue<Vector2>();
            spaceshipInput.SpaceshipControls.Roll.performed += ctx => SetRollInput(ctx.ReadValue<float>());
            spaceshipInput.SpaceshipControls.Throttle.performed += ctx => acceleration = ctx.ReadValue<float>();
            spaceshipInput.SpaceshipControls.Boost.performed += ctx => boostTarget.z = ctx.ReadValue<float>();
        }

        private void Update()
        {
            if (inputEnabled) { UpdateInput(); }
        }

        private void SetRollInput(float rollAmount) => roll = rollAmount;

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
        public void DisableInput() => inputEnabled = false;
        public void EnableSteering() => steeringEnabled = true;
        public void DisableSteering(bool clearCurrentValues)
        {
            steeringEnabled = false;

            if (clearCurrentValues)
            {
                steeringInputs = Vector3.zero;
                spaceEngine.SetSteeringInputs(steeringInputs);
            }
        }
        public void EnableMovement() => movementEnabled = true;
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
            reticuleViewportPosition += new Vector3(mouseDelta.x / Screen.width, mouseDelta.y / Screen.height, 0f) * reticleMovementSpeed;

            // Center it
            Vector3 centeredReticuleViewportPosition = reticuleViewportPosition - new Vector3(0.5f, 0.5f, 0f);

            // Prevent distortion before clamping
            centeredReticuleViewportPosition.x *= (float)Screen.width / Screen.height;

            // Clamp
            centeredReticuleViewportPosition = Vector3.ClampMagnitude(centeredReticuleViewportPosition, maxReticleDistanceFromCenter);

            // Convert back to proper viewport
            centeredReticuleViewportPosition.x /= (float)Screen.width / Screen.height;

            reticuleViewportPosition = centeredReticuleViewportPosition + new Vector3(0.5f, 0.5f, 0f);
        }

        private void UpdateSteering()
        {
            mouseSteeringInputs = Vector3.zero;

            if (!mouseEnabled) { return; }

            Vector3 centeredViewportPos = reticuleViewportPosition - new Vector3(0.5f, 0.5f, 0f);

            centeredViewportPos.x *= (float)Screen.width / Screen.height;
            float amount = Mathf.Max(centeredViewportPos.magnitude - mouseDeadRadius, 0f) / (maxReticleDistanceFromCenter - mouseDeadRadius);
            centeredViewportPos.x /= (float)Screen.width / Screen.height;

            Vector3 screenInputs = mousePositionInputCurve.Evaluate(amount) * centeredViewportPos.normalized;

            mouseSteeringInputs.x = -screenInputs.y;
            mouseSteeringInputs.y = screenInputs.x;

            hudCursor.SetViewportPosition(reticuleViewportPosition);
        }

        private void UpdateMovement()
        {
            Vector3 movementInputs = spaceEngine.movementInputs;

            movementInputs.z += acceleration * Time.deltaTime;
            movementInputs.x = strafing.x;
            movementInputs.y = strafing.y;

            spaceEngine.SetMovementInputs(movementInputs);

            boostInputs = Vector3.Lerp(boostInputs, boostTarget, boostChangeSpeed * Time.deltaTime);
            if (boostInputs.magnitude < 0.0001f) { boostInputs = Vector3.zero; }
            spaceEngine.SetBoostInputs(boostInputs);
        }

        private void OnRoll(float rollAmount)
        {
            if (Mathf.Abs(rollAmount) > 0.0001f) { lastRollTime = Time.time; }
        }

        public void SetBoost(float boostAmount) => boostTarget = new Vector3(0f, 0f, boostAmount);

        private void AutoRoll()
        {
            if (Time.time - lastRollTime < 0.5f) { return; }

            Vector3 flattenedFwd = spaceEngine.transform.forward;
            flattenedFwd.y = 0f;
            flattenedFwd.Normalize();

            Vector3 right = Vector3.Cross(Vector3.up, flattenedFwd);
            float angle = Vector3.Angle(right, spaceEngine.transform.right);

            if (Vector3.Dot(spaceEngine.transform.up, right) > 0f) { angle *= -1f; }

            steeringInputs.z = Mathf.Clamp(angle * -1f * autoRollStrength, -1f, 1f);
            steeringInputs.z *= maxAutoRoll;
            steeringInputs.z *= 1 - Mathf.Abs(Vector3.Dot(spaceEngine.transform.forward, Vector3.up));
        }

        private void UpdateInput()
        {
            // Pitch
            steeringInputs.x = Mathf.Clamp(-steering.y, -1f, 1f);
            // Roll
            steeringInputs.z = Mathf.Clamp(roll, -1f, 1f);
            // Yaw
            steeringInputs.y = Mathf.Clamp(steering.x, -1f, 1f);

            UpdateReticulePosition(generalInput.GeneralControls.MouseDelta.ReadValue<Vector2>());
            UpdateSteering();

            steeringInputs = new Vector3(Mathf.Abs(steeringInputs.x) > Mathf.Abs(mouseSteeringInputs.x) ? steeringInputs.x : mouseSteeringInputs.x,
                                            Mathf.Abs(steeringInputs.y) > Mathf.Abs(mouseSteeringInputs.y) ? steeringInputs.y : mouseSteeringInputs.y,
                                            Mathf.Abs(steeringInputs.z) > Mathf.Abs(mouseSteeringInputs.z) ? steeringInputs.z : mouseSteeringInputs.z);

            if (mouseEnabled is false)
            {
                hudCursor.CenterCursor();
                reticuleViewportPosition = new Vector3(0.5f, 0.5f, 0f);
            }

            OnRoll(steeringInputs.z);

            UpdateMovement();

            if (autoRollEnabled) { AutoRoll(); }

            spaceEngine.SetSteeringInputs(steeringInputs);
        }
    }

}
