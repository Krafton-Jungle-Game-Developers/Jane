using System;
using System.Buffers;
using UnityEngine;

// TODO: Responsible to find MainCamera and connect reference
// Vehicle Camera + SpaceFighterCameraController(VehicleCameraController(CameraController))
public class SpaceshipCameraController : MonoBehaviour
{
    [SerializeField] private bool isEnabled = true;

    public bool IsEnabled => isEnabled;

    [SerializeField] private Camera mainCamera;
    public Camera MainCamera => mainCamera;
    [SerializeField] private bool canControlCamera = true;
    public bool CanControlCamera => canControlCamera;
    [SerializeField] private float defaultFOV = 75f;
    public float DefaultFOV => defaultFOV;
    [SerializeField] private bool cameraCollisionEnabled = true;
    public bool CameraCollisionEnabled => cameraCollisionEnabled;
    [SerializeField] private LayerMask collisionMask = ~0;
    [SerializeField] private Transform currentViewTarget;
    [SerializeField] private Rigidbody spaceShipRigidbody;
    [SerializeField] private SpaceshipEngine spaceEngine;

    [SerializeField] private float positionFollowStrength = 0.4f;
    public float PositionFollowStrength => positionFollowStrength;
    [SerializeField] private float rotationFollowStrength = 0.1f;
    public float RotationFollowStrength => rotationFollowStrength;
    [SerializeField] private float spinOffsetCoefficient = 1f;
    [SerializeField] private float yawLateralOffset = 2f;
    public float YawLateralOffset => yawLateralOffset;

    private RaycastHitComparer comparer = new();

    private void FixedUpdate() => UpdateCameraFollow();

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(currentViewTarget.forward, transform.up);
        UpdateCameraCollision();
    }

    private void UpdateCameraFollow()
    {
        if (currentViewTarget == null) { return; }

        float spinLateralOffset = 0f, yawLateralOffset = 0f;
        if (spaceShipRigidbody != null)
        {
            // Calculate the lateral offset from the camera view target based on the spin.
            spinLateralOffset = spinOffsetCoefficient *
                                -currentViewTarget.InverseTransformDirection(spaceShipRigidbody.angularVelocity).z;

            yawLateralOffset = YawLateralOffset *
                               currentViewTarget.InverseTransformDirection(spaceShipRigidbody.angularVelocity).y;
        }

        // Calculate the target position for the camera 
        Vector3 targetPosition = currentViewTarget.TransformPoint(new Vector3(spinLateralOffset + yawLateralOffset, 0f, 0f));

        // Lerp toward the target position
        transform.position = (1 - PositionFollowStrength) * transform.position + PositionFollowStrength * targetPosition;

        // Spherically interpolate between the current rotation and the target rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, currentViewTarget.rotation, rotationFollowStrength);
    }

    private void UpdateCameraCollision()
    {
        if (cameraCollisionEnabled is false || currentViewTarget is null) { return; }
        
        Vector3 targetPosition = transform.position;
        
        Vector3 sphereCastStart = currentViewTarget.position;
        Vector3 sphereCastEnd = transform.position;
        Vector3 sphereCastDir = (sphereCastEnd - sphereCastStart).normalized;
        float dist = (sphereCastEnd - sphereCastStart).magnitude;
        
        RaycastHit[] hitsPool = ArrayPool<RaycastHit>.Shared.Rent(32);
        int hitCount = Physics.SphereCastNonAlloc(sphereCastStart, 0.1f, sphereCastDir, hitsPool, dist, collisionMask, QueryTriggerInteraction.Ignore);
        
        Array.Sort(hitsPool, null, hitsPool.GetLowerBound(0), hitsPool.Length, comparer);

        for (int i = 0; i < hitCount; i++)
        {
            if (Mathf.Approximately(hitsPool[i].distance, 0f)) continue;
            
            Rigidbody hitRigidbody = hitsPool[i].collider.attachedRigidbody;
            if (hitRigidbody != null && currentViewTarget != null && hitRigidbody.transform == currentViewTarget) { continue; }
            if (hitsPool[i].collider.transform == currentViewTarget) { continue; }
            
            targetPosition = hitsPool[i].point;

            break;
        }
        
        transform.position = targetPosition;
        ArrayPool<RaycastHit>.Shared.Return(hitsPool);
    }
}
