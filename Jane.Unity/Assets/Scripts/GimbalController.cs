using UnityEngine;

public class GimbalController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private bool gimbalEnabled = true;
    [SerializeField] private Transform horizontalPivot;
    public Transform HorizontalPivot => horizontalPivot;
    [SerializeField] private float IdleHorizontalPivotAngle = 0f;
    [SerializeField] private float minHorizontalPivotAngle = -180f;
    [SerializeField] private float maxHorizontalPivotAngle = 180f;

    [Space]
    [SerializeField] private Transform verticalPivot;
    public Transform VerticalPivot => verticalPivot;
    [SerializeField] private float IdleVerticalPivotAngle = 0f;
    [SerializeField] private float minVerticalPivotAngle = -90f;
    [SerializeField] private float maxVerticalPivotAngle = 90f;
    [SerializeField] private Transform gimbalChild;

    [Header("Gimbal Motors")]
    [SerializeField] private float proportionalCoefficient = 0.5f;
    [SerializeField] private float integralCoefficient = 0f;
    [SerializeField] private float derivativeCoefficient = 0.1f;
    [SerializeField] private float maxHorizontalAngularVelocity = 3f;
    [SerializeField] private float maxVerticalAngularVelocity = 3f;

    private float calculatedProportionalH = 0f;
    private float calculatedIntegralH = 0f;
    private float calculatedDerivativeH = 0f;
    private float calculatedProportionalV = 0f;
    private float calculatedIntegralV = 0f;
    private float calculatedDerivativeV = 0f;

    public void TrackPosition(Vector3 target, out float angleToTarget, bool snapToTarget)
    {
        // For aim assist
        angleToTarget = 180f;

        if (gimbalEnabled is false) { return; }

        // ****************************** Rotate Horizontally to Target ******************************************

        // Get the local target position wrt the horizontally rotating body
        Vector3 targetLocalPos = transform.InverseTransformPoint(target);

        // Get the angle from the base to the target on the local horizontal plane
        float desiredTargetAngleHPlane = Vector3.Angle(Vector3.forward, new Vector3(targetLocalPos.x, 0f, targetLocalPos.z));

        // Correct the sign 
        if (targetLocalPos.x < 0) { desiredTargetAngleHPlane *= -1; }

        // Get the desired angle for the horizontal gimbal on the horizontal plane
        float desiredLocalHorizontalPivotAngle = desiredTargetAngleHPlane;
        
        if (snapToTarget) { horizontalPivot.localRotation = Quaternion.Euler(0f, desiredLocalHorizontalPivotAngle, 0f); }
        else
        {
            // Get the current angle of the horizontal gimbal on the horizontal plane (wrt the gimbal parent forward vector)
            float currentHorizontalPivotAngle = WrapAngleTo180Range(horizontalPivot.localRotation.eulerAngles.y);
            float horizontalPivotAngle = desiredLocalHorizontalPivotAngle - currentHorizontalPivotAngle;
            
            // If the horizontal constraints allow it, allow the horizontal gimbal to cross the 180/-180 threshold
            if (Mathf.Abs(horizontalPivotAngle) > 180 && (minHorizontalPivotAngle <= -180 || maxHorizontalPivotAngle >= 180))
            {
                horizontalPivotAngle = Mathf.Sign(horizontalPivotAngle) * -1 * (360 - Mathf.Abs(horizontalPivotAngle));
            }
            
            calculatedProportionalH = horizontalPivotAngle * proportionalCoefficient;
            calculatedDerivativeH = -horizontalPivotAngle * derivativeCoefficient;
            calculatedIntegralH += horizontalPivotAngle * integralCoefficient;

            // Calculate and constrain the rotation speed
            float rotationSpeedHorizontal = calculatedProportionalH + calculatedIntegralH + calculatedDerivativeH;

            rotationSpeedHorizontal = Mathf.Clamp(rotationSpeedHorizontal, -maxHorizontalAngularVelocity, maxHorizontalAngularVelocity);
            
            horizontalPivot.Rotate(new Vector3(0f, rotationSpeedHorizontal, 0f));
        }
        
        // ****************************** Rotate Vertically to Target ******************************************

        Vector3 offset = Vector3.Scale(new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z),
                                                    transform.InverseTransformDirection(verticalPivot.position - transform.position));

        Vector3 targetLocalPosV = targetLocalPos - offset;

        angleToTarget = Vector3.Angle(verticalPivot.forward, target - verticalPivot.position);

        // Get the angle from the local target position vector to the local horizontal plane
        float desiredLocalVerticalPivotAngle = Vector3.Angle(targetLocalPosV, new Vector3(targetLocalPosV.x, 0f, targetLocalPosV.z));

        // Correct the sign
        if (targetLocalPosV.y > 0) { desiredLocalVerticalPivotAngle *= -1; }

        // Constrain the desired vertical pivot angle
        desiredLocalVerticalPivotAngle = Mathf.Clamp(desiredLocalVerticalPivotAngle, -maxVerticalPivotAngle, -minVerticalPivotAngle);
        
        if (snapToTarget) { verticalPivot.localRotation = Quaternion.Euler(desiredLocalVerticalPivotAngle, 0f, 0f); }
        else
        {
            float currentVerticalPivotAngle = WrapAngleTo180Range(verticalPivot.localRotation.eulerAngles.x);
            float verticalPivotAngle = desiredLocalVerticalPivotAngle - currentVerticalPivotAngle;
            
            calculatedProportionalV = verticalPivotAngle * proportionalCoefficient;
            calculatedDerivativeV = -verticalPivotAngle * derivativeCoefficient;
            calculatedIntegralV += verticalPivotAngle * integralCoefficient;

            // Calculate and constrain the rotation speed
            float rotationSpeedVertical = calculatedProportionalV + calculatedIntegralV + calculatedDerivativeV;
            rotationSpeedVertical = Mathf.Clamp(rotationSpeedVertical, -maxVerticalAngularVelocity, maxVerticalAngularVelocity);
            
            verticalPivot.Rotate(new Vector3(rotationSpeedVertical, 0f, 0f));
        }
    }

    private float WrapAngleTo180Range(float angle)
    {
        const float HalfCircle = 180f;
        const float FullCircle = 360f;

        if (angle is HalfCircle || angle is -HalfCircle) { return angle; }
        
        float offset = (Mathf.Floor((Mathf.Abs(angle) - HalfCircle) / FullCircle) + 1f) * FullCircle;

        return angle < -HalfCircle ? angle + offset : angle - offset;
    }
    
    public void Rotate(float horizontalRotation, float verticalRotation)
    {
        if (gimbalEnabled is false) { return; }
        
        float currentHAngle = WrapAngleTo180Range(horizontalPivot.localRotation.eulerAngles.y);
        float desiredHAngle = currentHAngle + horizontalRotation;
        
        horizontalPivot.localRotation = Quaternion.Euler(0f, desiredHAngle, 0f);
        
        float currentVAngle = WrapAngleTo180Range(verticalPivot.localRotation.eulerAngles.x);
        float desiredVAngle = currentVAngle + verticalRotation;
        
        desiredVAngle = Mathf.Clamp(desiredVAngle, minVerticalPivotAngle, maxVerticalPivotAngle);
        
        verticalPivot.localRotation = Quaternion.Euler(desiredVAngle, 0f, 0f);
    }
    
    public void ResetGimbal(bool snapToCenter)
    {
        if (snapToCenter)
        {
            horizontalPivot.localRotation = Quaternion.identity;
            verticalPivot.localRotation = Quaternion.identity;
        }
        else
        {
            TrackPosition(transform.TransformPoint(Vector3.forward * 10) + (verticalPivot.transform.position - transform.position), out float angleToTarget, false);
        }
    }
}
