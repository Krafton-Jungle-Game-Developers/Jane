using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Cysharp.Threading.Tasks;

namespace Jane.Unity
{
    public class CheckPoint : MonoBehaviour
    {
        [SerializeField] private int region = 1;
        public int Region => region;
        [SerializeField] private int checkPointNumber = 1;
        public int CheckPointNumber => checkPointNumber;
        [SerializeField] private VisualEffect VFX;

        [Header("Warp")]
        [SerializeField] private bool isWarpGate;
        public bool IsWarpGate => isWarpGate;
        [SerializeField] private float maxFOV;
        [SerializeField] private float maxBloomIntensity;
        [SerializeField] private float duration;
        [SerializeField] private PlayerCameraEffect playerCameraEffect;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Transform warpDestination;

        private void Awake() => VFX.Play();

        public void Interact()
        {
            if (isWarpGate is false) { VFX.Stop(); }
            audioSource.Play();
        }

        public async UniTask Warp(Transform player)
        {
            float timeElapsed = 0f;
            float startingFOV = playerCameraEffect.nowFOV;
            float startingBloom = playerCameraEffect.nowBloomIntensity;
            
            while (timeElapsed < duration)
            {
                float newFOV = Mathf.Lerp(startingFOV, maxFOV, Mathf.Pow(timeElapsed / duration, 5f));
                float newBloomIntensity = Mathf.Lerp(startingBloom, maxBloomIntensity, Mathf.Pow(timeElapsed / duration, 2f));

                playerCameraEffect.nowFOV = newFOV;
                playerCameraEffect.nowBloomIntensity = newBloomIntensity;

                timeElapsed += Time.deltaTime;
                await UniTask.Yield();
            }

            player.position = warpDestination.transform.position;
            player.rotation = warpDestination.transform.rotation;

            timeElapsed = 0f;

            playerCameraEffect.nowFOV = maxFOV;
            playerCameraEffect.nowBloomIntensity = maxBloomIntensity;

            while (timeElapsed < duration)
            {
                float newFOV = Mathf.Lerp(maxFOV, playerCameraEffect.baseFOV, Mathf.Pow(timeElapsed / duration, 4f));
                float newBloomIntensity = Mathf.Lerp(maxBloomIntensity, playerCameraEffect.baseBloomIntensity, Mathf.Pow(timeElapsed / duration, 0.5f));

                playerCameraEffect.nowFOV = newFOV;
                playerCameraEffect.nowBloomIntensity = newBloomIntensity;

                timeElapsed += Time.deltaTime;
                await UniTask.Yield();
            }
        }
    }
}
