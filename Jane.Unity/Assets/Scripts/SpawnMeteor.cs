using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpawnMeteor : MonoBehaviour
{
    public GameObject meteorVFX;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        var _startPos = startPoint.position;
        GameObject objVFX = Instantiate(meteorVFX, _startPos, Quaternion.identity) as GameObject;

        var _endPos = endPoint.position;

        
        RotateTo (objVFX, _endPos);
    }

    // Rotate Meteor Toward Destination
    private void RotateTo(GameObject obj, Vector3 destination)
    {
        var _direction = destination - obj.transform.position;
        var _rotation = Quaternion.LookRotation(_direction);

        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, _rotation, 1);
    }
}
