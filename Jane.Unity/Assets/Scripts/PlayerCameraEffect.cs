using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using SCPE;

public class PlayerCameraEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Volume globalVolume;
    [Space]

    [SerializeField] private float lastingDuration = 1f;
    [Space]
    //Reactive FOV
    [SerializeField] private float playerVelocity;
    [SerializeField] private Vector3 playerPosition;
    [SerializeField] private Vector3 oldPlayerPosition;
    [SerializeField] private float playerAcceleration = 0f;
    [Space]

    [Header("FOV")]
    [SerializeField] public float baseFOV = 60f;
    [SerializeField] public float nowFOV = 60f;
    [SerializeField] public float maxFOV = 80f;
    [Space]

    [Header("Chromatic Aberration")]
    [SerializeField] private float baseCAIntensity = 0f;
    [SerializeField] private float nowCAIntensity = 0f;
    [SerializeField] private float maxCAIntensity = 0.8f;
    [Space]

    [Header("Motion Blur")]
    [SerializeField] private float baseMBIntensity = 0f;
    [SerializeField] private float nowMBIntensity = 0f;
    [SerializeField] private float maxMBIntensity = 0.5f;
    [Space]

    [Header("Bloom")]
    [SerializeField] public float baseBloomIntensity = 0f;
    [SerializeField] public float nowBloomIntensity = 0f;
    [SerializeField] public float maxBloomIntensity = 0.5f;
    [Space]

    [Header("RadialBlur")]
    [SerializeField] private float baseRadialBlurIntensity = 0f;
    [SerializeField] private float nowRadialBlurIntensity = 0f;
    [SerializeField] private float maxRadialBlurIntensity = 0.5f;
    [Space]

    [Header("SpeedLine")]
    [SerializeField] private ParticleSystem _speedParticleSystem;
    private ParticleSystem.EmissionModule _speedParticleEmission;
    [SerializeField] private ParticleSystem _verticalParticleSystem;
    private ParticleSystem.EmissionModule _verticalParticleEmission;
    [SerializeField] private ParticleSystem _cockpitParticleSystem;
    private ParticleSystem.EmissionModule _cockpitParticleEmission;
    [Space]

    [SerializeField] private float baseParticleIntensity = 0f;
    [SerializeField] private float nowParticleIntensity = 0f;
    [SerializeField] private float maxParticleIntensity = 50f;
    [Space]

    [SerializeField] private float baseVerticalIntensity = 0f;
    [SerializeField] private float nowVerticalIntensity = 0f;
    [SerializeField] private float maxVerticalIntensity = 100f;

    private Bloom _bloom;
    private ChromaticAberration _chromaticAberration;
    private MotionBlur _motionBlur;
    private RadialBlur _radialBlur;

    private void Awake()
    {
        globalVolume.profile.TryGet(out _motionBlur);
        globalVolume.profile.TryGet(out _chromaticAberration);
        globalVolume.profile.TryGet(out _bloom);
        globalVolume.profile.TryGet(out _radialBlur);
        _speedParticleEmission = _speedParticleSystem.emission;
        _verticalParticleEmission = _verticalParticleSystem.emission;
        _cockpitParticleEmission = _cockpitParticleSystem.emission;
    }

    private void Update()
    {
        CameraPropertiesControl();
        GlovalVolumeControl();
        ParticleSystemControl();
    }

    private void FixedUpdate()
    {
        //Get player's Velocity & acceleration
        playerPosition = new Vector3(playerRigidbody.position.x,
                                     0.0f,
                                     playerRigidbody.position.z);

        //NOTE: Don't use Delta Time (Jittering)
        playerVelocity = (Mathf.Abs(playerPosition.magnitude - oldPlayerPosition.magnitude)) / Time.deltaTime;
        playerAcceleration = Vector3.Distance(playerPosition, oldPlayerPosition);
        oldPlayerPosition = new Vector3(playerRigidbody.position.x,
                                        0.0f,
                                        playerRigidbody.position.z);
    }

    private void CameraPropertiesControl()
    {
        if (playerAcceleration > 1f)
        {
            //NOTE: Don't use Delta Time (Jittering)
            nowFOV = Mathf.Lerp(nowFOV, maxFOV, 0.001f * playerAcceleration);
        }
        else
        {
            //NOTE: Don't use Delta Time (Jittering)
            if (nowFOV > baseFOV)
            {
                nowFOV -= 0.05f * lastingDuration;
            }
        }
        playerCamera.fieldOfView = nowFOV;
    }

    private void GlovalVolumeControl()
    {
        if (playerAcceleration > 1f)
        {
            //NOTE: Don't use Delta Time (Jittering)
            nowCAIntensity = Mathf.Lerp(nowCAIntensity, maxCAIntensity, 0.001f * playerAcceleration);
            nowMBIntensity = Mathf.Lerp(nowMBIntensity, maxMBIntensity, 0.001f * playerAcceleration);
            nowBloomIntensity = Mathf.Lerp(nowBloomIntensity, maxBloomIntensity, 0.001f * playerAcceleration);
            nowRadialBlurIntensity = Mathf.Lerp(nowRadialBlurIntensity, maxRadialBlurIntensity, 0.001f * playerAcceleration);
            nowParticleIntensity = Mathf.Lerp(nowParticleIntensity, maxParticleIntensity, 0.1f * playerAcceleration);
        }
        else
        {
            //NOTE: Don't use Delta Time (Jittering)
            if (nowCAIntensity > baseCAIntensity)
            {
                nowCAIntensity -= 0.05f * lastingDuration;
            }
            if (nowMBIntensity > baseMBIntensity)
            {
                nowMBIntensity -= 0.01f * lastingDuration;
            }
            if (nowBloomIntensity > baseBloomIntensity)
            {
                nowBloomIntensity -= 0.1f * lastingDuration;
            }
            if (nowRadialBlurIntensity > baseRadialBlurIntensity)
            {
                nowRadialBlurIntensity -= 0.1f * lastingDuration;
            }
        }
        playerCamera.fieldOfView = nowFOV;
        _chromaticAberration.intensity.value = nowCAIntensity;
        _motionBlur.intensity.value = nowMBIntensity;
        _radialBlur.amount.value = nowRadialBlurIntensity;
        _bloom.intensity.value = nowBloomIntensity;
    }

    private void ParticleSystemControl()
    {
        if (playerAcceleration > 1f)
        {
            //NOTE: Don't use Delta Time (Jittering)
            nowParticleIntensity = Mathf.Lerp(nowParticleIntensity, maxParticleIntensity, 0.1f * playerAcceleration);

        }
        else
        {
            //NOTE: Don't use Delta Time (Jittering)
            if (nowParticleIntensity > baseParticleIntensity)
            {
                nowParticleIntensity -= 0.1f * lastingDuration;
            }
        }
        _speedParticleEmission.rateOverTime = nowParticleIntensity;
        _verticalParticleEmission.rateOverTime = nowParticleIntensity / 10f;
        _cockpitParticleEmission.rateOverTime = nowParticleIntensity;
    }
}
