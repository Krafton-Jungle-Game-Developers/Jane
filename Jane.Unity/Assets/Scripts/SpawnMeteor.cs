using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpawnMeteor : MonoBehaviour
{
    // Spawn Control 
    [SerializeField] private float _spawnRate = 5f;
    [SerializeField] private int _maxObjInScene = 7;

    // Array of Meteors to Control 


    
    // Meteor Prefabs Control
    public GameObject[] objectPrefabs;
    private int idxPrefabList = 0;
    public float destroyTime = 10.0f;
    [SerializeField] private float _minSpeed = 10f;
    [SerializeField] private float _maxSpeed = 20f;
    
    //public GameObject meteorVFX;
    public GameObject startPoint;

    public GameObject[] middlePoints;
    
    void Start()
    {
        // Spawn objects at spawnrate 
        InvokeRepeating("SpawnObject", 10f, _spawnRate);
    }


    private void SpawnObject()
    {
        
        // Random Spawn from List 
        //int idxPrefabList = UnityEngine.Random.Range (0, objectPrefabs.Length);
        if (idxPrefabList == objectPrefabs.Length)
        {
            idxPrefabList = 0;
        }


        // Instantiate Meteor
        MeteorMove newMeteorCs = Instantiate(objectPrefabs[idxPrefabList], startPoint.transform.position, Quaternion.identity).GetComponent<MeteorMove>(); ;
        newMeteorCs._speed = UnityEngine.Random.Range(_minSpeed, _maxSpeed);


        idxPrefabList++;
    }


    // Rotate Meteor Toward Destination
    public void RotateTo(GameObject obj, Vector3 destination)
    {
        var _direction = destination - obj.transform.position;
        var _rotation = Quaternion.LookRotation(_direction);

        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, _rotation, 1);
    }


}

/// TODO : ������ �� �̰� �ϼ��ؾ��� 
/// rb�� �����̴°� �����ϰ� üũ����Ʈ ������ ���� meteor controller�� �����ϴ� ��� �˾Ƴ��߰ڴ� 
