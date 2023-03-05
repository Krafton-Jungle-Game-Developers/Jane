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

    void SetPlayers()
    {
        IOrderedEnumerable<KeyValuePair<string, NetworkPlayer>> sortedPlayer = players.OrderBy(x => x.Value.activeCheckpointIndex)
                                                                                      .OrderByDescending(x => x.Value.activeCheckpointIndex)
                                                                                      .ThenBy(x => x.Value.distanceToCheckpoint)
                                                                                      .ThenByDescending(x => x.Value.distanceToCheckpoint);
        int i = 0;
        foreach (KeyValuePair<string, NetworkPlayer> item in sortedPlayer)
        {
            Debug.Log(item.Value.activeCheckpointIndex);
            standingsGenerator.standingsBox[i].GetComponent<TMP_Text>().text = (i + 1) + " . " + item.Key;
            i++;
        }
    }

    public void UpdateInformation(NetworkPlayer playerID)
    {
        players[playerID.UserId] = playerID;
    }
}
