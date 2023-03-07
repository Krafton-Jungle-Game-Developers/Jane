using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class ProjectileMove : MonoBehaviour
{
    [SerializeField] private float _speedMin;
    [SerializeField] private float _speedMax;
    private float _speed;
    [SerializeField] private GameObject impactPrefab;

    //private Rigidbody _rigidbody;
    private Vector3 _startPoint;
    private Vector3 _destination;
    //private Vector3 _direction;
    private int _midIdx = 0;
    private SpawnMeteor _spawnMeteor;
    private float _timer;
    [SerializeField] private GameObject SpawnController;
    
    void Start()
    {
        SpawnController = GameObject.FindWithTag("Spawner");
        _spawnMeteor = SpawnController.GetComponent<SpawnMeteor>();
        _startPoint = _spawnMeteor.startPoint.position;
        _destination = _spawnMeteor.middlePoints[0].position;
        //_direction = (_destination.transform.position - gameObject.transform.position).normalized;
        _timer = 0;
        _speed = UnityEngine.Random.Range(_speedMin, _speedMax);

        //_rigidbody = GetComponent<Rigidbody>();

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

        // rotate 50degree per second 
        transform.Rotate (120 * Time.deltaTime, 0, 360 * Time.deltaTime);

        
    }

    private void Update()
    {
        //
        //transform.position += _direction * _speed * Time.deltaTime;
        _timer += 0.1f * _speed * Time.deltaTime;
        transform.position = Vector3.Lerp(_startPoint, _destination, _timer);

        if (Vector3.Distance(transform.position, _destination) < 1f)
        {
            // if Last Checkpoint
            if (_midIdx == _spawnMeteor.middlePoints.Length - 1)
            {
                Destroy(gameObject);
            } 
            else
            {
                //Debug.Log("check : " + _spawnMeteor.middlePoints[_midIdx]);
                _timer = 0;
                _startPoint = _spawnMeteor.middlePoints[_midIdx].position;
                _midIdx++;
                _destination = _spawnMeteor.middlePoints[_midIdx].position;
                //_direction = (_destination.position - gameObject.transform.position).normalized;

            }
        }

        //
        Destroy(gameObject, _spawnMeteor.destroyTime);
    }

    

    // Play VFX & Play SFX 
    private void OnCollisionEnter(Collision collision)
    {
        //_speed = 0;
        //Debug.Log("collision" + collision.gameObject.name);

        ContactPoint contact = collision.contacts[0];
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 position = contact.point;

        if (impactPrefab != null)
        {
            var impactVFX = Instantiate(impactPrefab, position, rotation) as GameObject;
            Destroy(impactVFX, 1.2f);
        }

    }
}
