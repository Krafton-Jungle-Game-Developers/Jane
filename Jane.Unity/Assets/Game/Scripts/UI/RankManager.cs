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
    public TargetBoxGenerator targetBoxGenerator;

    private Dictionary<string, NetworkPlayer> players;
    private Ulid playerID;
    private List<KeyValuePair<string, NetworkPlayer>> sortedList = new List<KeyValuePair<string, NetworkPlayer>>();
    public CheckPoints checkPoints;
    public float switchTime = 0.5f;

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

    public void GetLocalPlayer(Ulid currentLocalID)
    {
        playerID = currentLocalID;
    }

    public void GetPlayers(NetworkPlayer id)
    {
        // Get players through playerID and add them to List
        players.Add(id.UserId, id);
        standingsGenerator.AddPlayerStanding();
        if (id.UniqueId != playerID)
        {
            targetBoxGenerator.AddPlayerTargetBox(id.gameObject);
        }
    }

    void SetPlayers()
    {
        IOrderedEnumerable<KeyValuePair<string, NetworkPlayer>> sortedPlayer = players.OrderByDescending(x => x.Value.activeCheckpointIndex)
                                                                                      .ThenBy(x => x.Value.distanceToCheckpoint);
/*        List<KeyValuePair<string, NetworkPlayer>> tempList = sortedPlayer.ToList();
        if (!sortedList.Any() && !sortedList.SequenceEqual(tempList))
        {
            List<int?> differentPositions = sortedList.Zip(tempList, (x, y) => x.Equals(y) ? (int?)null : Array.IndexOf(tempList.ToArray(), x)).ToList();
            differentPositions = differentPositions.Where(x => x != null).ToList();

            int change1 = differentPositions.First() ?? 0;
            int change2 = differentPositions.Last() ?? 0;
*/            int i = 0;

            foreach (KeyValuePair<string, NetworkPlayer> item in sortedPlayer)
            {
                if (item.Value.UniqueId == playerID)
                {
                    standingsGenerator.standingsBox[i].GetComponent<Image>().color = new Color(1.0f, 0.5f, 0f, 1.0f);
                }
                else
                {
                    standingsGenerator.standingsBox[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 1.0f);
                }
/*            if (i != change1 || i != change2)
            {
*/                standingsGenerator.standingsBox[i].GetComponentInChildren<TMP_Text>().text = " " + (i + 1) + "   " + item.Key;
/*            }
*//*            else if (i == change2)
                {
                    RectTransform first = standingsGenerator.standingsBox[change1].GetComponent<RectTransform>();
                    RectTransform second = standingsGenerator.standingsBox[change2].GetComponent<RectTransform>();
                    StartCoroutine(MoveStandings(first, second));
                }
*/                i++;
            }
        }
/*    sortedList = tempList;
  }
*/
public float GetDistance(GameObject playerObj, GameObject checkpointObj)
    {
        Vector3 playerLocation = playerObj.transform.position;
        Vector3 checkpointLocation = checkpointObj.transform.position;
        float distance = Vector3.Distance(playerLocation, checkpointLocation);
        return distance;
    }

    IEnumerator MoveStandings(RectTransform rectTransformA, RectTransform rectTransformB)
    {
        Vector2 startPosA = rectTransformA.anchoredPosition;
        Vector2 endPosA = rectTransformB.anchoredPosition;
        Vector2 startPosB = rectTransformB.anchoredPosition;
        Vector2 endPosB = rectTransformA.anchoredPosition;

        float elapsedTime = 0f;
        while (elapsedTime < switchTime)
        {
            float t = Mathf.Clamp01(elapsedTime / switchTime);
            rectTransformA.anchoredPosition = Vector2.Lerp(startPosA, endPosA, t);
            rectTransformB.anchoredPosition = Vector2.Lerp(startPosB, endPosB, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransformA.anchoredPosition = endPosA;
        rectTransformB.anchoredPosition = endPosB;
    }
}
