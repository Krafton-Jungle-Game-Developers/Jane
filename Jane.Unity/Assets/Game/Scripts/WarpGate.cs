using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WarpGate : MonoBehaviour
{
    [SerializeField] private float maxFOV;
    [SerializeField] private float maxBloomIntensity;
    [SerializeField] private float duration;
    [SerializeField] private PlayerCameraEffect playerCameraEffect;
    [SerializeField] private AudioSource audioSource;
    public GameObject warpDestination;
    private GameObject _player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _player = other.gameObject;
            StartCoroutine(Warp());
        }
    }

    IEnumerator Warp()
    {
        float timeElapsed = 0f;
        float startingFOV = playerCameraEffect.nowFOV;
        float startingBloom = playerCameraEffect.nowBloomIntensity;

        if(audioSource != null)
        {
            // Play Warping SFX 
            audioSource.Play();
        }

        //some method in controller that stops player input & movement
        _player.GetComponent<SpaceshipController>().canControl = false;

        while(timeElapsed < duration)
        {
            float newFOV = Mathf.Lerp(startingFOV, maxFOV, Mathf.Pow(timeElapsed / duration, 5f));
            float newBloomIntensity = Mathf.Lerp(startingBloom, maxBloomIntensity, Mathf.Pow(timeElapsed / duration, 2f));

            playerCameraEffect.nowFOV = newFOV;
            playerCameraEffect.nowBloomIntensity = newBloomIntensity;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _player.transform.position = warpDestination.transform.position;
        _player.transform.rotation = warpDestination.transform.rotation;

        timeElapsed = 0f;

        playerCameraEffect.nowFOV = maxFOV;
        playerCameraEffect.nowBloomIntensity = maxBloomIntensity;

        while(timeElapsed < duration)
        {
            float newFOV = Mathf.Lerp(maxFOV, playerCameraEffect.baseFOV, Mathf.Pow(timeElapsed / duration, 4f));
            float newBloomIntensity = Mathf.Lerp(maxBloomIntensity, playerCameraEffect.baseBloomIntensity, Mathf.Pow(timeElapsed / duration, 0.5f));

            playerCameraEffect.nowFOV = newFOV;
            playerCameraEffect.nowBloomIntensity = newBloomIntensity;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _player.GetComponent<SpaceshipController>().canControl = true;
    }
}