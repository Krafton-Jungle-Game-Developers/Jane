using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpawnMeteor : MonoBehaviour
{
    // Spawn Control 
    [SerializeField] private float _spawnRate = 1f;
    public GameObject[] objectPrefabs;
    public float destroyTime = 10.0f;
    
    //public GameObject meteorVFX;
    public Transform startPoint;

    public GameObject[] middlePoints;
    
    // Start is called before the first frame update
    void Start()
    {
        // Spawn objects at spawnrate 
        InvokeRepeating("SpawnObject", _spawnRate, _spawnRate);
        
    }


    private void SpawnObject()
    {
    
        // Random Spawn from List 
        int idxPrefabList = UnityEngine.Random.Range (0, objectPrefabs.Length);

        Instantiate(objectPrefabs[idxPrefabList], startPoint.position, Quaternion.identity);

        
    }


    //// Rotate Meteor Toward Destination
    //private void RotateTo(GameObject obj, Vector3 destination)
    //{
    //    var _direction = destination - obj.transform.position;
    //    var _rotation = Quaternion.LookRotation(_direction);

    //    obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, _rotation, 1);
    //}


}
