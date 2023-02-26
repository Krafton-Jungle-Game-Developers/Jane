using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    [Range(0f, 89f)] public float pitchAngle = 15f;
    [Range(0f, 89f)] public float rollAngle = 15f;
    public bool autoRoll = true;

    private Rigidbody _rb;
    private Vector2 _screenCenter;
    private Vector3 _currentMovementSpeed;
    private Vector3 playerInput;
    private Vector2 pitchYawInput;

    private float rollInput;
    private RectTransform cursorRectTransform;
    public bool canControl = true;

    private void Start()
    {
        TryGetComponent(out _rb);
        if (cursorRectTransform is null) { cursorRectTransform = GameObject.Find("Cursor").GetComponent<RectTransform>(); }
        _screenCenter = new Vector2(Screen.width, Screen.height) / 2f;
    }

    private void Update() => UpdatePlayerInput();

    private void FixedUpdate() => UpdatePhysics();

    private void UpdatePlayerInput()
    {
        if (canControl is false) { return; }

        playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Hover"), Input.GetAxisRaw("Vertical"));
        rollInput = Input.GetAxisRaw("Roll");

        Vector2 cursorLocation = cursorRectTransform.position;
        pitchYawInput.x = (cursorLocation.x - _screenCenter.x) / _screenCenter.y;
        pitchYawInput.y = (cursorLocation.y - _screenCenter.y) / _screenCenter.y;
        pitchYawInput = Vector2.ClampMagnitude(pitchYawInput, 1f);
    }

    private void UpdatePhysics()
    {
        float pitchAngle = pitchYawInput.magnitude >= 0.1f ? -pitchYawInput.y * lookRateSpeed : 0f;
        float yawAngle = pitchYawInput.magnitude >= 0.1f ? pitchYawInput.x * lookRateSpeed : 0f;
        float rollAngle = Mathf.Abs(rollInput) >= 0.1f ? rollInput * rollSpeed : 0f;

        transform.Rotate(new Vector3(pitchAngle, yawAngle, rollAngle));


        Vector3 targetVelocity = transform.TransformDirection(playerInput * maxMovementSpeed);
        
        _rb.velocity = Vector3.Lerp(_rb.velocity, targetVelocity, movementLerpSpeed * Time.deltaTime);
    }

    public void ChangeSpeed(float newSpeed)
    {
        //forwardSpeed = newSpeed;
        //Debug.Log("Change Speed : " + forwardSpeed);
    }
    
    public void ChangeSpeedInstantly(float newSpeed)
    {
        // _activeForwardSpeed = newSpeed;
    }

    public bool GetIsCursorLeft() => pitchYawInput.x < 0;
}
