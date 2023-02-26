using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private GameObject impactPrefab;
    

    
    private Rigidbody _rigidbody;

    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

    }


    void FixedUpdate()
    {
        if (_speed != 0 && _rigidbody != null)
        {
            _rigidbody.position += transform.forward * (_speed * Time.deltaTime);
        }
    }

    // Play VFX & Play SFX 
    private void OnCollisionEnter(Collision collision)
    {
        _speed = 0;

        ContactPoint contact = collision.contacts[0];
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 position = contact.point;

        if (impactPrefab != null)
        {
            var impactVFX = Instantiate(impactPrefab, position, rotation) as GameObject;
            Destroy(impactVFX, 3);
        }

        Destroy(gameObject);


    }


    
}
