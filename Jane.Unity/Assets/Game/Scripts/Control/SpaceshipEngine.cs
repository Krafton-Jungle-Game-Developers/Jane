using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpaceshipEngine : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private bool controlsDisabled = false;
    public bool ControlsDisabled
    {
        get => controlsDisabled;
        set => controlsDisabled = value;
    }

    [SerializeField] private Vector3 movementInputs;
    public Vector3 MovementInputs => movementInputs;
    [SerializeField] public Vector3 steeringInputs;
    [SerializeField] public Vector3 boostInputs;
    public Vector3 BoostInputs => boostInputs;

    [SerializeField] private Vector3 minMovementInputs = new(-1f, -1f, -0.3f);
    [SerializeField] private Vector3 maxMovementInputs = new(1f, 1f, 1f);

    [Header("Movement & Steering Forces")]
    [SerializeField] private bool enginesActivated = false;
    public bool EnginesActivated
    {
        get => enginesActivated;
        set => enginesActivated = value;
    }
    [SerializeField] private Rigidbody spaceShipRigidbody;

    [SerializeField] private Vector3 maxMovementForces = new(500f, 500f, 1000f);
    [SerializeField] private Vector3 maxSteeringForces = new(8f, 8f, 10f);
    [SerializeField] private Vector3 maxBoostForces = new(800f, 800f, 2500f);

    [SerializeField] private float movementInputResponseSpeed = 5f;
    private Vector3 currentMovementForcesByAxis = Vector3.zero;

    [Header("Speed-Steering Relationship")]
    [SerializeField] private AnimationCurve steeringBySpeedCurve = AnimationCurve.Linear(0, 1, 1, 1);
    [SerializeField] private float boostSteeringCoefficient = 1f;
    
    private void Awake() => TryGetComponent(out spaceShipRigidbody);

    public void ClearInputs()
    {
        steeringInputs = Vector3.zero;
        movementInputs = Vector3.zero;
        boostInputs = Vector3.zero;
    }

    public void DisableMovement()
    {
        minMovementInputs = Vector3.zero;
        maxMovementInputs = Vector3.zero;
        maxBoostForces = Vector3.zero;
    }

    public void EnableMovement()
    {
        minMovementInputs = new(-1f, -1f, -0.5f);
        maxMovementInputs = new(1f, 1f, 1f);
        maxBoostForces = new(800f, 800f, 2500f);
    }

    public Vector3 GetDefaultMaxSpeedByAxis(bool withBoost)
    {
        Vector3 maxForces = maxMovementForces + (withBoost ? maxBoostForces : Vector3.zero);

        return (new Vector3(GetSpeedFromForce(maxForces.x, spaceShipRigidbody), GetSpeedFromForce(maxForces.y, spaceShipRigidbody), GetSpeedFromForce(maxForces.z, spaceShipRigidbody)));
    }
    
    public Vector3 GetCurrentMaxSpeedByAxis(bool withBoost)
    {
        Vector3 maxForces = maxMovementForces + (withBoost ? maxBoostForces : Vector3.zero);

        return (new Vector3(GetSpeedFromForce(maxForces.x, spaceShipRigidbody), GetSpeedFromForce(maxForces.y, spaceShipRigidbody), GetSpeedFromForce(maxForces.z, spaceShipRigidbody)));
    }
    
    public static float GetSpeedFromForce(float force, Rigidbody rBody)
    {
        return (force / rBody.drag - force * Time.fixedDeltaTime) / rBody.mass;
    }
    
    public static float GetForceForSpeed(float speed, Rigidbody rBody)
    {
        return speed / ((1 / rBody.drag - Time.fixedDeltaTime) / rBody.mass);
    }

    public static float GetAngularSpeedFromTorque(float torque, Rigidbody rBody)
    {
        return Mathf.Clamp(torque / Mathf.Max(rBody.angularDrag, 1) - torque * Time.fixedDeltaTime, -rBody.maxAngularVelocity, rBody.maxAngularVelocity);
    }

    public static float GetTorqueForAngularSpeed(float angularSpeed, Rigidbody rBody)
    {
        return angularSpeed / ((1 / Mathf.Max(rBody.angularDrag, 1) - Time.fixedDeltaTime));
    }

    public void SetMovementInputs(Vector3 newValuesByAxis)
    {
        if (controlsDisabled) return;
        
        movementInputs.x = Mathf.Clamp(newValuesByAxis.x, minMovementInputs.x, maxMovementInputs.x);
        movementInputs.y = Mathf.Clamp(newValuesByAxis.y, minMovementInputs.y, maxMovementInputs.y);
        movementInputs.z = Mathf.Clamp(newValuesByAxis.z, minMovementInputs.z, maxMovementInputs.z);
    }

    public void SetSteeringInputs(Vector3 inputValuesByAxis)
    {
        if (controlsDisabled) return;
        
        steeringInputs.x = Mathf.Clamp(inputValuesByAxis.x, -1f, 1f);
        steeringInputs.y = Mathf.Clamp(inputValuesByAxis.y, -1f, 1f);
        steeringInputs.z = Mathf.Clamp(inputValuesByAxis.z, -1f, 1f);
    }
    
    public void SetSpeed(Vector3 speedsByAxis)
    {
        SetMovementInputs(new(GetForceForSpeed(speedsByAxis.x, spaceShipRigidbody) / maxMovementForces.x,
                                                  GetForceForSpeed(speedsByAxis.y, spaceShipRigidbody) / maxMovementForces.y,
                                                  GetForceForSpeed(speedsByAxis.z, spaceShipRigidbody) / maxMovementForces.z));
    }

    public void SetSpeedXAxis(float speed)
    {
        SetMovementInputs(new((speed * spaceShipRigidbody.mass * spaceShipRigidbody.drag) / maxMovementForces.x, movementInputs.y, movementInputs.z));
    }

    public void SetSpeedYAxis(float speed)
    {
        SetMovementInputs(new(movementInputs.x, (speed * spaceShipRigidbody.mass * spaceShipRigidbody.drag) / maxMovementForces.y, movementInputs.z));
    }

    public void SetSpeedZAxis(float speed)
    {
        SetMovementInputs(new(movementInputs.x, movementInputs.y, (speed * spaceShipRigidbody.mass * spaceShipRigidbody.drag) / maxMovementForces.z));
    }

    public void SetBoostInputs(Vector3 newValuesByAxis)
    {
        if (controlsDisabled) return;

        boostInputs.x = Mathf.Clamp(newValuesByAxis.x, -1f, 1f);
        boostInputs.y = Mathf.Clamp(newValuesByAxis.y, -1f, 1f);
        boostInputs.z = Mathf.Clamp(newValuesByAxis.z, -1f, 1f);
    }
    
    private void FixedUpdate()
    {
        if (enginesActivated is false) { return; }

        // Implement steering torques
        float steeringSpeedMultiplier = 1f;
        if (boostInputs.z > 0.5f)
        {
            steeringSpeedMultiplier = boostSteeringCoefficient;
        }
        else
        {
            float topSpeed = GetCurrentMaxSpeedByAxis(false).z;
            if (!Mathf.Approximately(topSpeed, 0f))
            {
                float topSpeedAmount = Mathf.Clamp(Mathf.Abs(spaceShipRigidbody.velocity.z / topSpeed), 0f, 1f);
                steeringSpeedMultiplier = steeringBySpeedCurve.Evaluate(topSpeedAmount);
            }
        }

        spaceShipRigidbody.AddRelativeTorque(steeringSpeedMultiplier * Vector3.Scale(steeringInputs, maxSteeringForces), ForceMode.Acceleration);

        Vector3 nextMovementForces = Vector3.Scale(movementInputs, maxMovementForces);

        if (boostInputs.x > 0.5f) { nextMovementForces.x = maxBoostForces.x; }
        if (boostInputs.y > 0.5f) { nextMovementForces.y = maxBoostForces.y; }
        if (boostInputs.z > 0.5f) { nextMovementForces.z = maxBoostForces.z; }

        nextMovementForces = Vector3.Lerp(currentMovementForcesByAxis, nextMovementForces, movementInputResponseSpeed * Time.deltaTime);
        currentMovementForcesByAxis = nextMovementForces;

        spaceShipRigidbody.AddRelativeForce(nextMovementForces);
    }
}
