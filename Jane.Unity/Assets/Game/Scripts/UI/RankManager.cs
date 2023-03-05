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

    private Dictionary<NetworkPlayer, string> players;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        players = new Dictionary<NetworkPlayer, string>();
    }

    void Update()
    {
        SetPlayers();
    }

    public void GetPlayers(NetworkPlayer playerID)
    {
        // Get players through playerID and add them to List
        players.Add(playerID, playerID.UserId);
        standingsGenerator.AddPlayerStanding();
        Debug.Log($"current player dictionary: {players}");
    }

    void SetPlayers()
    {
        IOrderedEnumerable<KeyValuePair<NetworkPlayer, string>> sortedPlayer = players.OrderBy(x => x.Key.activeCheckpointIndex)
                                                                                      .OrderByDescending(x => x.Key.activeCheckpointIndex)
                                                                                      .ThenBy(x => x.Key.distanceToCheckpoint)
                                                                                      .ThenByDescending(x => x.Key.distanceToCheckpoint);
        int i = 0;
        foreach (KeyValuePair<NetworkPlayer, string> item in sortedPlayer)
        {
            standingsGenerator.standingsBox[i].GetComponent<TMP_Text>().text = (i + 1) + " . " + item.Value;
            i++;
        }
    }
}
