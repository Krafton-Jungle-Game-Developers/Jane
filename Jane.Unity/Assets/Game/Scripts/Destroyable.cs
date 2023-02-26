using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public int durability = 10;
    [SerializeField] private GameObject destroyPrefab;

    void Update()
    {
        if(durability <= 0)
        {
            if(destroyPrefab!= null)
            {
                Instantiate(destroyPrefab, transform.position, Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
            }
            Destroy(gameObject);
        }
    }
}
