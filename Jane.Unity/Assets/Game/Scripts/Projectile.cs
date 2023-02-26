using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
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
        _rigidbody.position += transform.forward * (_speed * Time.deltaTime);
        Destroy(gameObject, 1f);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Destroyable")
        {
            // destroy collided object
        }
        if (impactPrefab != null)
        {
            var impactVFX = Instantiate(impactPrefab, transform.position, transform.rotation);
            Destroy(impactVFX, 3);
        }
        Destroy(gameObject);
    }
}
