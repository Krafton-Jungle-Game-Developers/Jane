using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpaceshipMovementManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private bool controlsDisabled = false;
    [SerializeField] public Vector3 movementInputs;
    [SerializeField] public Vector3 steeringInputs;
    [SerializeField] public Vector3 boostInputs;

    [SerializeField] private Vector3 minMovementInputs = new(-1f, -1f, -0.1f);
    [SerializeField] private Vector3 maxMovementInputs = new(1f, 1f, 1f);

    [Header("Movement & Steering Forces")]
    [SerializeField] private bool enginesActivated = false;
    [SerializeField] private Rigidbody m_rigidbody;

    [SerializeField] private Vector3 maxMovementForces = new(400f, 400f, 400f);
    [SerializeField] private Vector3 maxSteeringForces = new(16f, 16f, 25f);
    [SerializeField] private Vector3 maxBoostForces = new(800f, 800f, 800f);

    [SerializeField] private float movementInputResponseSpeed = 5f;
    private Vector3 currentMovementForcesByAxis = Vector3.zero;

    [Header("Speed-Steering Relationship")]
    [Tooltip("A curve that represents how much the player can steer (Y axis) relative to the amount of top speed the ship is going (X axis). Works for forward movement only.")]
    [SerializeField] private AnimationCurve steeringBySpeedCurve = AnimationCurve.Linear(0, 1, 1, 1);

    [Tooltip("A coefficient that is multiplied by the steering during boost. Used instead of the Steering By Speed Curve when boost is activated.")]
    [SerializeField] private float boostSteeringCoefficient = 1;

    //[Header("Resource Handlers")]
    //[SerializeField] private List<ResourceHandler> boostResourceHandlers = new();

    private void Awake() => TryGetComponent(out m_rigidbody);

    /// <summary>
    /// Get the maximum speed on each axis, for example for loadout data.
    /// </summary>
    /// <param name="withBoost">Whether to include boost in the maximum speed.</param>
    /// <returns>The maximum speed on each axis.</returns>
    public Vector3 GetDefaultMaxSpeedByAxis(bool withBoost)
    {
        Vector3 maxForces = maxMovementForces + (withBoost ? maxBoostForces : Vector3.zero);

        return (new Vector3(GetSpeedFromForce(maxForces.x, m_rigidbody), GetSpeedFromForce(maxForces.y, m_rigidbody), GetSpeedFromForce(maxForces.z, m_rigidbody)));
    }

    /// <summary>
    /// Get the current maximum speed on each axis, for example for normalizing speed indicators.
    /// </summary>
    /// <param name="withBoost">Whether to include boost in the maximum speed.</param>
    /// <returns>The maximum speed on each axis.</returns>
    public Vector3 GetCurrentMaxSpeedByAxis(bool withBoost)
    {
        Vector3 maxForces = maxMovementForces + (withBoost ? maxBoostForces : Vector3.zero);

        return (new Vector3(GetSpeedFromForce(maxForces.x, m_rigidbody), GetSpeedFromForce(maxForces.y, m_rigidbody), GetSpeedFromForce(maxForces.z, m_rigidbody)));
    }

    /// <summary>
    /// Calculate the maximum speed of this Rigidbody for a given force.
    /// </summary>
    /// <param name="force">The linear force to be used in the calculation.</param>
    /// <returns>The maximum speed.</returns>
    public static float GetSpeedFromForce(float force, Rigidbody rBody)
    {
        /// Calculate the maximum speed of this Rigidbody for a given force
        return (force / rBody.drag - force * Time.fixedDeltaTime) / rBody.mass; // Subtracting (force * Time.fixedDeltaTime) / rBody.mass because drag is applied AFTER force is added
    }

    /// <summary>
    /// Get the force required for the vehicle to end up at a specific speed 
    /// </summary>
    /// <param name="speed">The desired speed.</param>
    /// <returns>The force required to achieve that speed.</returns>
    public static float GetForceForSpeed(float speed, Rigidbody rBody)
    {
        return speed / ((1 / rBody.drag - Time.fixedDeltaTime) / rBody.mass);
    }

    public static float GetAngularSpeedFromTorque(float torque, Rigidbody rBody)
    {
        // torque is applied using ForceMode.Acceleration, which ignores the mass distribution and treats the rBody as a point mass
        return Mathf.Clamp(torque / Mathf.Max(rBody.angularDrag, 1) - torque * Time.fixedDeltaTime, -rBody.maxAngularVelocity, rBody.maxAngularVelocity);
    }

    public static float GetTorqueForAngularSpeed(float angularSpeed, Rigidbody rBody)
    {
        return angularSpeed / ((1 / Mathf.Max(rBody.angularDrag, 1) - Time.fixedDeltaTime));
    }

    public void SetMovementInputs(Vector3 newValuesByAxis)
    {
        if (controlsDisabled) return;

        // Set and clamp the translation throttle values
        movementInputs.x = Mathf.Clamp(newValuesByAxis.x, minMovementInputs.x, maxMovementInputs.x);
        movementInputs.y = Mathf.Clamp(newValuesByAxis.y, minMovementInputs.y, maxMovementInputs.y);
        movementInputs.z = Mathf.Clamp(newValuesByAxis.z, minMovementInputs.z, maxMovementInputs.z);
    }

    public void SetSteeringInputs(Vector3 inputValuesByAxis)
    {
        if (controlsDisabled) return;

        // Set and clamp the rotation throttle values 
        steeringInputs.x = Mathf.Clamp(inputValuesByAxis.x, -1f, 1f);
        steeringInputs.y = Mathf.Clamp(inputValuesByAxis.y, -1f, 1f);
        steeringInputs.z = Mathf.Clamp(inputValuesByAxis.z, -1f, 1f);
    }

    /// <summary>
    /// Set the speed of the vehicle directly (will only work if the required force is within the max force limits.
    /// </summary>
    /// <param name="speedsByAxis">The speeds for the vehicle along each local axis.</param>
    public void SetSpeed(Vector3 speedsByAxis)
    {
        SetMovementInputs(new(GetForceForSpeed(speedsByAxis.x, m_rigidbody) / maxMovementForces.x,
                                                  GetForceForSpeed(speedsByAxis.y, m_rigidbody) / maxMovementForces.y,
                                                  GetForceForSpeed(speedsByAxis.z, m_rigidbody) / maxMovementForces.z));
    }

    public void SetSpeedXAxis(float speed)
    {
        SetMovementInputs(new((speed * m_rigidbody.mass * m_rigidbody.drag) / maxMovementForces.x, movementInputs.y, movementInputs.z));
    }

    public void SetSpeedYAxis(float speed)
    {
        SetMovementInputs(new(movementInputs.x, (speed * m_rigidbody.mass * m_rigidbody.drag) / maxMovementForces.y, movementInputs.z));
    }

    public void SetSpeedZAxis(float speed)
    {
        SetMovementInputs(new(movementInputs.x, movementInputs.y, (speed * m_rigidbody.mass * m_rigidbody.drag) / maxMovementForces.z));
    }

    public void SetBoostInputs(Vector3 newValuesByAxis)
    {
        //for (int i = 0; i < boostResourceHandlers.Count; ++i)
        //{
        //    if (!boostResourceHandlers[i].Ready())
        //    {
        //        newValuesByAxis = Vector3.zero;
        //        break;
        //    }
        //}

        if (controlsDisabled) return;

        boostInputs.x = Mathf.Clamp(newValuesByAxis.x, -1f, 1f);
        boostInputs.y = Mathf.Clamp(newValuesByAxis.y, -1f, 1f);
        boostInputs.z = Mathf.Clamp(newValuesByAxis.z, -1f, 1f);
    }

    private void Update()
    {
        // Use resources during boost
        //if (boostInputs.magnitude != 0f)
        //{
        //    for (int i = 0; i < boostResourceHandlers.Count; ++i)
        //    {
        //        if (boostResourceHandlers[i].Ready())
        //        {
        //            boostResourceHandlers[i].Implement();
        //        }
        //        else
        //        {
        //            if (controlsDisabled) return;

        //            boostInputs.x = Mathf.Clamp(0f, -1f, 1f);
        //            boostInputs.y = Mathf.Clamp(0f, -1f, 1f);
        //            boostInputs.z = Mathf.Clamp(0f, -1f, 1f);
        //        }
        //    }
        //}
    }

    private void FixedUpdate()
    {
        if (enginesActivated is false) return;

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
                float topSpeedAmount = Mathf.Clamp(Mathf.Abs(m_rigidbody.velocity.z / topSpeed), 0f, 1f);
                steeringSpeedMultiplier = steeringBySpeedCurve.Evaluate(topSpeedAmount);
            }
        }

        m_rigidbody.AddRelativeTorque(steeringSpeedMultiplier * Vector3.Scale(steeringInputs, maxSteeringForces), ForceMode.Acceleration);

        Vector3 nextMovementForces = Vector3.Scale(movementInputs, maxMovementForces);

        if (boostInputs.x > 0.5f) { nextMovementForces.x = maxBoostForces.x; }
        if (boostInputs.y > 0.5f) { nextMovementForces.y = maxBoostForces.y; }
        if (boostInputs.z > 0.5f) { nextMovementForces.z = maxBoostForces.z; }

        nextMovementForces = Vector3.Lerp(currentMovementForcesByAxis, nextMovementForces, movementInputResponseSpeed * Time.deltaTime);
        currentMovementForcesByAxis = nextMovementForces;

        m_rigidbody.AddRelativeForce(nextMovementForces);
    }
}
