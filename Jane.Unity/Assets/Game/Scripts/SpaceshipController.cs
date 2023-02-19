using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    public float forwardSpeed;
    public float strafeSpeed;
    public float hoverSpeed;
    private float _activeForwardSpeed;
    private float _activeStrafeSpeed;
    private float _activeHoverSpeed;
    [SerializeField] private float _forwardAcceleration;
    [SerializeField] private float _strafeAcceleration;
    [SerializeField] private float _hoverAcceleration;

    public float lookRateSpeed;
    private Vector2 lookInput, screenCenter, mouseDistance;

    private float rollInput;
    public float rollSpeed;
    public float rollAcceleration;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;

        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        MouseSteeringUpdate();
        MovementUpdate();
    }

    private void MouseSteeringUpdate()
    {
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);

        rollInput = Mathf.Lerp(rollInput, Input.GetAxisRaw("Roll"), rollAcceleration * Time.deltaTime);

        transform.Rotate(-mouseDistance.y * lookRateSpeed, mouseDistance.x * lookRateSpeed, rollInput * rollSpeed, Space.Self);
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
