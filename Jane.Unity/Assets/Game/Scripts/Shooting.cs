using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] private float cooldown = 0.1f;
    private bool _cooldown;
    public GameObject lazerPrefab;
    public GameObject[] lazerPoint;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(shootKey) && !_cooldown)
        {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        // make shoot target to cursor
        foreach (GameObject points in lazerPoint)
        {
            Instantiate(lazerPrefab, points.transform.position, points.transform.rotation);
        }
        _cooldown = true;
        yield return new WaitForSeconds(cooldown);
        _cooldown = false;
    }
}
