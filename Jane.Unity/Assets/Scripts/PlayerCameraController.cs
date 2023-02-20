using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float cameraRotationTension = 10;

    //Position Variables
    [SerializeField] private Vector3 cameraTargetPos;
    [SerializeField] private Vector3 playerForward;
    [SerializeField] private Vector3 playerUp;
    [SerializeField] private Vector3 playerRight;

    //Inspector Edit
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private Quaternion cameraRotationOffset = new Quaternion(0f, 0f, 0f, 0f);
    [SerializeField] private float cameraTension = 10;

    private float _cameraX;
    private float _cameraY;
    private float _cameraZ;

    [SerializeField] private float cameraDistance;


    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponentInParent<Camera>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
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
        playerUp = playerTransform.up;
        playerRight = playerTransform.right;
        playerForward = playerTransform.forward;

        cameraTargetPos = playerTransform.position - playerForward * cameraOffset.x - playerUp * cameraOffset.y - playerRight * cameraOffset.z;

        _cameraX = Mathf.Lerp(m_Camera.transform.position.x, cameraTargetPos.x, Time.deltaTime * cameraTension);
        _cameraY = Mathf.Lerp(m_Camera.transform.position.y, cameraTargetPos.y, Time.deltaTime * cameraTension);
        _cameraZ = Mathf.Lerp(m_Camera.transform.position.z, cameraTargetPos.z, Time.deltaTime * cameraTension);

        m_Camera.transform.position = new Vector3(_cameraX, _cameraY, _cameraZ);
    }
}
