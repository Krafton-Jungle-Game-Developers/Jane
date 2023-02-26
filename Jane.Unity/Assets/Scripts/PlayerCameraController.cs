using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

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

    //TEMP: Inspector See
    //[SerializeField] private Quaternion cameraRotationReal;
    //[SerializeField] private Vector3 cameraPositionReal;
    //[SerializeField] private Vector3 playerPositionNow;
    //[SerializeField] private Vector3 cameraPositionNow;

    private float _cameraX;
    private float _cameraY;
    private float _cameraZ;


    [SerializeField] private float cameraDistance;

    //Camera Booster Movement Variable
    private Booster booster;
    private float _shakeX = 0f;
    private float _shakeY = 0f;
    [SerializeField] private float _shakeMagnitude = 0.1f;

    //Camera Jimbal Movement Variable
    [SerializeField] private Vector3 mouseInput;
    [SerializeField] private float _gimbalX = 0f;
    [SerializeField] private float _gimbalY = 0f;

    void Start()
    {
        //Common

        //CameraControlMovement
        m_Camera = GetComponent<Camera>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        //CameraBoosterMovement
        booster = GameObject.FindGameObjectWithTag("Player").GetComponent<Booster>();
    }

    void FixedUpdate()
    {
        CameraControlMovement();
        CameraBoosterMovement();
        CameraGimbalMovement();

        m_Camera.transform.position = new Vector3(_cameraX + _shakeX + _gimbalX, _cameraY + _shakeY + _gimbalY, _cameraZ);

        //Tracking Value
        mouseInput = Input.mousePosition;
        //Screen.


        //TODO: Delete
        //cameraRotationReal.x = m_Camera.transform.rotation.x - playerTransform.rotation.x;
        //cameraRotationReal.y = m_Camera.transform.rotation.y - playerTransform.rotation.y;
        //cameraRotationReal.z = m_Camera.transform.rotation.z - playerTransform.rotation.z;
        //cameraRotationReal.w = m_Camera.transform.rotation.w - playerTransform.rotation.w;

        //cameraPositionReal = m_Camera.transform.position - playerTransform.position;
        //playerPositionNow = playerTransform.position;
        //cameraPositionNow = m_Camera.transform.position;
    }

    private void CameraControlMovement()
    {
        //TEMP: Check Camera Distance
        cameraDistance = (m_Camera.transform.position - playerTransform.position).magnitude;

        //Camera Rotation Smooth(1)
        //_rotationX = Mathf.Lerp(m_Camera.transform.rotation.x, playerTransform.rotation.x, Time.deltaTime * cameraRotationTension);
        //_rotationY = Mathf.Lerp(m_Camera.transform.rotation.y, playerTransform.rotation.y, Time.deltaTime * cameraRotationTension);
        //_rotationZ = Mathf.Lerp(m_Camera.transform.rotation.z, playerTransform.rotation.z, Time.deltaTime * cameraRotationTension);
        //_rotationW = Mathf.Lerp(m_Camera.transform.rotation.w, playerTransform.rotation.w, Time.deltaTime * cameraRotationTension);

        //Camera Rotation Smooth(2)
        m_Camera.transform.rotation = Quaternion.Slerp(m_Camera.transform.rotation, playerTransform.rotation, Time.deltaTime * cameraRotationTension);

        //Camera Rotation Hard(1)
        //playerQuaternion = playerTransform.rotation;
        //m_Camera.transform.rotation = playerQuaternion;

        //Camera Position Smooth
        _playerUp = playerTransform.up;
        _playerRight = playerTransform.right;
        _playerForward = playerTransform.forward;

        _cameraTargetPos = playerTransform.position - (_playerForward * (cameraOffset.x + _gimbalX)) - (_playerUp * (cameraOffset.y + _gimbalY) - (_playerRight * cameraOffset.z));
        _cameraX = Mathf.Lerp(m_Camera.transform.position.x, _cameraTargetPos.x, Time.deltaTime * cameraPositionTension);
        _cameraY = Mathf.Lerp(m_Camera.transform.position.y, _cameraTargetPos.y, Time.deltaTime * cameraPositionTension);
        _cameraZ = Mathf.Lerp(m_Camera.transform.position.z, _cameraTargetPos.z, Time.deltaTime * cameraPositionTension);
    }

    private void CameraBoosterMovement()
    {

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
    }

    private void CameraGimbalMovement()
    {
        Vector3 eulerAngles = playerTransform.rotation.eulerAngles;
        Debug.Log("transform.rotation angles x: " + eulerAngles.x + " y: " + eulerAngles.y + " z: " + eulerAngles.z);

    }
}
