using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private int _damage;
    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private AudioSource birthAudio;
    private Rigidbody _rigidbody;
    private bool _hasCollided = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        birthAudio.Play();
    }

    void Update()
    {
        _rigidbody.position += transform.forward * (_speed * Time.deltaTime);
        Destroy(gameObject, 1f);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_hasCollided && other.gameObject.CompareTag("Destroyable"))
        {
            if (impactPrefab != null)
            {
                var impactVFX = Instantiate(impactPrefab, transform.position, transform.rotation);
                Destroy(impactVFX, 3);
            }
            _hasCollided = true;
            other.gameObject.GetComponent<Destroyable>().durability -= _damage;
            Destroy(gameObject);
        }
    }
}
