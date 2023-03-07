using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankManager : MonoBehaviour
{
    public static RankManager instance;
    public StandingsGenerator standingsGenerator;

    private Dictionary<string, NetworkPlayer> players;
    private Ulid playerID;
    public CheckPoints checkPoints;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        players = new Dictionary<string, NetworkPlayer>();
    }

    void Update()
    {
        SetPlayers();
    }

    public void GetPlayers(NetworkPlayer playerID)
    {
        // Get players through playerID and add them to List
        players.Add(playerID.UserId, playerID);
        standingsGenerator.AddPlayerStanding();
    }

    public void GetLocalPlayer(Ulid currentLocalID)
    {
        playerID = currentLocalID;
    }

    void SetPlayers()
    {
        IOrderedEnumerable<KeyValuePair<string, NetworkPlayer>> sortedPlayer = players.OrderBy(x => x.Value.activeCheckpointIndex)
                                                                                      .OrderByDescending(x => x.Value.activeCheckpointIndex)
                                                                                      .ThenBy(x => x.Value.distanceToCheckpoint)
                                                                                      .ThenByDescending(x => x.Value.distanceToCheckpoint);
        int i = 0;
        foreach (KeyValuePair<string, NetworkPlayer> item in sortedPlayer)
        {
            if (item.Value.UniqueId == playerID)
            {
                standingsGenerator.standingsBox[i].GetComponent<TMP_Text>().color = new Color(0.3f, 1.0f, 0f, 1.0f);
            }
            else
            {
                standingsGenerator.standingsBox[i].GetComponent<TMP_Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            standingsGenerator.standingsBox[i].GetComponent<TMP_Text>().text = (i + 1) + " . " + item.Key;
            i++;
        }
    }

    public float GetDistance(GameObject playerObj, GameObject checkpointObj)
    {
        Vector3 playerLocation = playerObj.transform.position;
        Vector3 checkpointLocation = checkpointObj.transform.position;
        float distance = Vector3.Distance(playerLocation, checkpointLocation);
        return distance;
    }
}
