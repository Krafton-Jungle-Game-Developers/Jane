using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ProjectileMove : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private GameObject impactPrefab;
    private Rigidbody _rigidbody;
    private Transform _startPoint;
    private Transform _destination;
    private Vector3 _direction;
    private int _midIdx = 0;
    private SpawnMeteor _spawnMeteor;
    private float _timer;
    [SerializeField] private GameObject SpawnController;
    
    void Start()
    {
        _spawnMeteor = SpawnController.GetComponent<SpawnMeteor>();
        _startPoint = _spawnMeteor.startPoint;
        _destination = _spawnMeteor.middlePoints[0].transform;
        _direction = (_destination.position - gameObject.transform.position).normalized;
        _timer = 0;

        _rigidbody = GetComponent<Rigidbody>();

    }


    void FixedUpdate()
    {
        //_rigidbody.MovePosition(_destination + _direction * Time.deltaTime * _speed);

        //if (_speed != 0 && _rigidbody != null)
        //{
        //    // if Arrived at Destination
        //    if (Vector3.Distance(_rigidbody.position, _destination) < 0.01f)
        //    {
        //        // if Last Checkpoint
        //        if (_midIdx == _spawnMeteor.middlePoints.Length - 1)
        //        {
        //            Destroy(gameObject);
        //        }
              
        //        _midIdx++;
        //        _destination = _spawnMeteor.middlePoints[_midIdx].position;
              
        //        UpdateDestination();
        //    }
        //    _rigidbody.position += transform.forward * (_speed * Time.deltaTime);

        //}

        
    }

    private void Update()
    {
        //
        //transform.position += _direction * _speed * Time.deltaTime;
        _timer += 0.1f * _speed * Time.deltaTime;
        transform.position = Vector3.Lerp(_startPoint.position, _destination.position, _timer);

        if (Vector3.Distance(transform.position, _destination.position) < 0.0001f)
        {
            // if Last Checkpoint
            if (_midIdx == _spawnMeteor.middlePoints.Length - 1)
            {
                Destroy(gameObject);
            } 
            else
            {
                Debug.Log("check : " + (_midIdx).ToString());
                _timer = 0;
                _startPoint.position = transform.position;
                _midIdx++;
                _destination.position = _spawnMeteor.middlePoints[_midIdx].transform.position;
                _direction = (_destination.position - gameObject.transform.position).normalized;

            }
        }

        //
            Destroy(gameObject, _spawnMeteor.destroyTime);
    }

    // Rotate Meteor Toward Destination
    private void UpdateDestination()
    {
        _direction = (_destination.position - gameObject.transform.position).normalized;
        var _rotation = Quaternion.LookRotation(_direction);

        // gameObject.transform.localRotation = Quaternion.Lerp(gameObject.transform.rotation, _rotation, 1);
        
    }

    // Play VFX & Play SFX 
    private void OnCollisionEnter(Collision collision)
    {
        //_speed = 0;

        ContactPoint contact = collision.contacts[0];
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 position = contact.point;

        if (impactPrefab != null)
        {
            var impactVFX = Instantiate(impactPrefab, position, rotation) as GameObject;
            Destroy(impactVFX, 3);
        }

    }
}
