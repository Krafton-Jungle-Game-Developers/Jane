using MilkShake;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{

    [SerializeField] private Camera m_Camera;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Quaternion playerQuaternion;

    private float _rotationX;
    private float _rotationY;
    private float _rotationZ;
    private float _rotationW;

    //Position Variables
    private Vector3 _cameraTargetPos;
    private Vector3 _playerForward;
    private Vector3 _playerUp;
    private Vector3 _playerRight;

    [Space]
    //Inspector Edit
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private Quaternion cameraRotationOffset = new Quaternion(0f, 0f, 0f, 0f);
    [SerializeField] private float cameraPositionTension = 10;
    [SerializeField] private float cameraRotationTension = 10;

    private float _cameraX;
    private float _cameraY;
    private float _cameraZ;


    [SerializeField] private float cameraDistance;

    // Booster Camera Shake options 
    private Booster booster;
    private float _shakeX = 0f;
    private float _shakeY = 0f;
    [SerializeField] private float _shakeMagnitude = 0.1f;
    

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponentInParent<Camera>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        booster = GameObject.FindGameObjectWithTag("Player").GetComponent<Booster>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check Camera Distance
        cameraDistance = (m_Camera.transform.position - playerTransform.position).magnitude;

        //Camera Rotation
        playerQuaternion = playerTransform.rotation;
        

        //Camera Rotation Smooth(1)
        _rotationX = Mathf.Lerp(m_Camera.transform.rotation.x, playerTransform.rotation.x, Time.deltaTime * cameraRotationTension);
        _rotationY = Mathf.Lerp(m_Camera.transform.rotation.y, playerTransform.rotation.y, Time.deltaTime * cameraRotationTension);
        _rotationZ = Mathf.Lerp(m_Camera.transform.rotation.z, playerTransform.rotation.z, Time.deltaTime * cameraRotationTension);
        _rotationW = Mathf.Lerp(m_Camera.transform.rotation.w, playerTransform.rotation.w, Time.deltaTime * cameraRotationTension);

        m_Camera.transform.rotation = new Quaternion(_rotationX, _rotationY, _rotationZ, _rotationW);

        //Camera Rotation Hard(2)
        //m_Camera.transform.rotation = playerQuaternion;

        //Camera Position Smooth
        _playerUp = playerTransform.up;
        _playerRight = playerTransform.right;
        _playerForward = playerTransform.forward;

        _cameraTargetPos = playerTransform.position - _playerForward * cameraOffset.x - _playerUp * cameraOffset.y - _playerRight * cameraOffset.z;

        _cameraX = Mathf.Lerp(m_Camera.transform.position.x, _cameraTargetPos.x, Time.deltaTime * cameraPositionTension);
        _cameraY = Mathf.Lerp(m_Camera.transform.position.y, _cameraTargetPos.y, Time.deltaTime * cameraPositionTension);
        _cameraZ = Mathf.Lerp(m_Camera.transform.position.z, _cameraTargetPos.z, Time.deltaTime * cameraPositionTension);

        if (booster._isBoosterActive)
        {
            _shakeX = UnityEngine.Random.Range(1f, -1f) * _shakeMagnitude;
            _shakeY = UnityEngine.Random.Range(1f, -1f) * _shakeMagnitude;
        } 
        else
        {
            _shakeX = 0;
            _shakeY = 0;
        }

        m_Camera.transform.position = new Vector3(_cameraX + _shakeX, _cameraY + _shakeY, _cameraZ);
    }

}
