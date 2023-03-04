using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RankManager : MonoBehaviour
{
    public Text[] txtStandings;
    Dictionary<string, int> players;

    void Start()
    {
        players = new Dictionary<string, int>();
    }

    void Update()
    {
        
    }

    void GetPlayers(int playerID)
    {
        // Get players through playerID and add them to List
        players.Add("playerName", playerID);
    }

    void SetPlayers()
    {
        IOrderedEnumerable<KeyValuePair<string, int>> sortedPlayer = players.OrderBy(x => x.Value).OrderByDescending(x => x.Value);
        int i = 0;
        foreach (KeyValuePair<string, int> item in sortedPlayer)
        {
            txtStandings[i].text = (i + 1) + " . " + item.Value;
            i++;
        }
    }
}
