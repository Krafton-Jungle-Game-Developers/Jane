using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    public float forwardSpeed = 25f;
    public float strafeSpeed = 15f;
    public float hoverSpeed = 15f;
    private float _activeForwardSpeed;
    private float _activeStrafeSpeed;
    private float _activeHoverSpeed;
    [SerializeField] private float _forwardAcceleration = 5f;
    [SerializeField] private float _strafeAcceleration = 2f;
    [SerializeField] private float _hoverAcceleration = 2f;

    public float lookRateSpeed = 0.5f;
    private Vector3 lookInput, screenCenter, mouseDistance;

    private float rollInput;
    public float rollSpeed = 5f ;
    public float rollAcceleration = 0.5f;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        screenCenter = new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    void Update()
    {
        MouseSteeringUpdate();
        MovementUpdate();
    }

    private void MouseSteeringUpdate()
    {
        lookInput = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0f);

        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        mouseDistance = Vector3.ClampMagnitude(mouseDistance, 1f);

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
