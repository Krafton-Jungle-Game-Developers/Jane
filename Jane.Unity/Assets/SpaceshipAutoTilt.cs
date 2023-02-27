using UnityEngine;

public class SpaceshipAutoTilt : MonoBehaviour
{
    [SerializeField] private float animationLerpSpeed = 0.5f;
    [SerializeField] private bool isEnabled = true;

    [Header("Roll")]
    [SerializeField] private float sideRotationToRoll = 20;
    [SerializeField] private float sideMovementToRoll = -0.15f;

    [Header("Turn Following")]
    [SerializeField] private float verticalTurnFollowing = 8f;
    [SerializeField] private float sideTurnFollowing = 5f;
    [SerializeField] private SpaceshipCameraController cameraController;
    [SerializeField] private Rigidbody spaceShipRigidbody;

    private void FixedUpdate()
    {
        if (isEnabled is false) { return; }

        Vector3 localAngularVelocity = spaceShipRigidbody.transform.InverseTransformDirection(spaceShipRigidbody.angularVelocity);
        Vector3 localVelocity = spaceShipRigidbody.transform.InverseTransformDirection(spaceShipRigidbody.velocity);

        float targetRoll = sideRotationToRoll * -localAngularVelocity.y + sideMovementToRoll * localVelocity.x;
        float targetPitch = verticalTurnFollowing * localAngularVelocity.x;
        float targetYaw = sideTurnFollowing * localAngularVelocity.y;

        // Yaw to roll
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(targetPitch, targetYaw, targetRoll), animationLerpSpeed);
    }
}
