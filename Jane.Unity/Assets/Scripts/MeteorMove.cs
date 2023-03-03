using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// This Script Only Moves Meteors from start to end. 
/// Start and End should be controlled by outside Scripts. 
/// Speed is also controlled by outside components.
/// </summary>
public class MeteorMove : MonoBehaviour
{
    private GameObject SpawnController;
    private SpawnMeteor _spawnMeteor;

    public float _speed;
    [SerializeField] GameObject _impactPrefab;

    private Rigidbody _rb;

    void Start()
    {
        SpawnController = GameObject.FindWithTag("Spawner");
        _spawnMeteor = SpawnController.GetComponent<SpawnMeteor>();
        _rb = GetComponent<Rigidbody>();    
    }
    
    void Update()
    { 
        Destroy(gameObject, _spawnMeteor.destroyTime);
    }

    private void FixedUpdate()
    {   
        if (_speed != 0 && _rb != null)
        {
            _rb.position += transform.forward * (_speed * Time.deltaTime);
        } 

    }

    private void OnCollisionEnter(Collision other)
    {
        // _speed = 0;

        if (other.gameObject.CompareTag("MeteorPoint"))
        {
            // Reach Checkpoint
            
        } 
        else
        {
            ContactPoint contact = other.contacts[0];
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 position = contact.point;

            if (_impactPrefab != null)
            {
                var impactVFX = Instantiate(_impactPrefab, position, rotation) as GameObject;
                Destroy(impactVFX, 3);
            }

        }


    }
}
