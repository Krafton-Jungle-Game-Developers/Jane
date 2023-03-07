using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DistanceText : MonoBehaviour
{
    [SerializeField] private TMP_Text distanceText;
    private float distance;
    private Transform player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    private void UpdateText(Vector3 objectPos)
    {
        distance = Vector3.Distance(player.position, objectPos);
        distance = Mathf.RoundToInt(distance);
        distanceText.text = $"{distance}m";
    }
}
