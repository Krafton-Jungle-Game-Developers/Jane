using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsGenerator : MonoBehaviour
{
    public List<GameObject> resultsBox = new List<GameObject>();
    public GameObject resultPrefab;

    public void AddPlayerResult()
    {
        resultsBox.Add(Instantiate(resultPrefab));
        for (int i = 0; i < resultsBox.Count; i++)
        {
            resultsBox[i].transform.parent = transform;
        }
    }
}
