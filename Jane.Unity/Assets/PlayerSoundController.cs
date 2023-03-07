using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    private AudioSource playerAudioSource;


    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GameObject.Find("Player").GetComponent<Rigidbody>();
        playerAudioSource = GameObject.Find("Player").GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (playerRigidbody.velocity.magnitude > 0.1f)
        {
            playerAudioSource.volume = playerRigidbody.velocity.magnitude / 391f;
        }
        else
        {
            playerAudioSource.volume = 0f;
        }
    }
}
