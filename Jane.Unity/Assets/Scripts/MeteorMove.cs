using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// This Script Only Moves Meteors from start to end. 
/// Start and End should be controlled by outside Scripts. 
/// Speed is also controlled by outside components.
/// </summary>
//public class MeteorMove : MonoBehaviour
//{
//    private GameObject SpawnController;
//    private SpawnMeteor _spawnMeteor;

//    public float _speed;
//    [SerializeField] GameObject _impactPrefab;

//    private Rigidbody _rb;

//    void Start()
//    {
//        SpawnController = GameObject.FindWithTag("Spawner");
//        _spawnMeteor = SpawnController.GetComponent<SpawnMeteor>();
//        _rb = GetComponent<Rigidbody>();    
//    }
    
//    void Update()
//    { 
//        Destroy(gameObject, _spawnMeteor.destroyTime);
//    }

//    private void FixedUpdate()
//    {   
//        if (_speed != 0 && _rb != null)
//        {
//            _rb.position += transform.forward * (_speed * Time.deltaTime);
//        } 

//    }


//    private void OnTriggerEnter(Collider other)
//    {
//        Debug.Log($"OnTriggerEnter Called. other's tag was {other.tag}");
//        if (other.gameObject.CompareTag("MeteorPoint"))
//        {
//            Debug.Log("Triggered");
//            // Reach Checkpoint
//            int midIdx = System.Array.IndexOf(_spawnMeteor.middlePoints, other);
//            if (midIdx >= _spawnMeteor.middlePoints.Length - 1)
//            {
//                // Last CheckPoint
//                Destroy(gameObject);
//            }
//            else
//            {
//                _spawnMeteor.RotateTo(gameObject, _spawnMeteor.middlePoints[midIdx + 1].transform.position);
//            }


//        }
//    }

//    private void OnCollisionEnter(Collision other)
//    {
//        // _speed = 0;


//        ContactPoint contact = other.contacts[0];
//        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
//        Vector3 position = contact.point;

//        if (_impactPrefab != null)
//        {
//            var impactVFX = Instantiate(_impactPrefab, position, rotation) as GameObject;
//            Destroy(impactVFX, 2);
//        }

        


//    }
//}
