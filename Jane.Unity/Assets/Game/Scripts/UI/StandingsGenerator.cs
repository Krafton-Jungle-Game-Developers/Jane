using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingsGenerator : MonoBehaviour
{
    public List<GameObject> standingsBox = new List<GameObject>();
    public GameObject standingPrefab;

    public void AddPlayerStanding()
    {
        standingsBox.Add(Instantiate(standingPrefab));
        for (int i = 0; i < standingsBox.Count; i++)
        {
            standingsBox[i].transform.parent = transform;
        }
    }
}
