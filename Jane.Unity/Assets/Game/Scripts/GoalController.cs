using Jane.Unity;
using TMPro;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RankManager.instance.SetStandings(RankManager.instance.resultsGenerator);
            RankManager.instance.SetResult();
            other.gameObject.GetComponent<SpaceshipEngine>().DisableMovement();
            other.gameObject.GetComponent<SpaceshipEngine>().ClearInputs();
        }
        if (other.gameObject.GetComponent<NetworkPlayer>() != null && !other.gameObject.GetComponent<NetworkPlayer>().isFinished)
        {
            Debug.Log("finished");
            other.gameObject.GetComponent<NetworkPlayer>().isFinished = true;
            RankManager.instance.finishCount++;
        }
    }
}
