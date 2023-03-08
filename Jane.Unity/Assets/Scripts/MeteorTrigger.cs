using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorTrigger : MonoBehaviour
{
    private SpawnMeteor _spawnMeteor;

    private void Start()
    {
        _spawnMeteor = GameObject.FindGameObjectWithTag("Spawner").GetComponent<SpawnMeteor>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Player Enter");
            _spawnMeteor._isPlayerInside = true;
            StartCoroutine(_spawnMeteor.Spawn());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Player Exit");
            _spawnMeteor._isPlayerInside = false;
            StopCoroutine(_spawnMeteor.Spawn());
        }
    }
}
