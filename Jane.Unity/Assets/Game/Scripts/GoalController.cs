using Jane.Unity;
using TMPro;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<NetworkPlayer>() != null && !other.gameObject.GetComponent<NetworkPlayer>().isFinished)
        {
            Debug.Log("finished");
            other.gameObject.GetComponent<NetworkPlayer>().isFinished = true;
            RankManager.instance.finishCount++;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            RankManager.instance.SetResult();
            other.gameObject.GetComponent<SpaceshipEngine>().DisableMovement();
            other.gameObject.GetComponent<SpaceshipEngine>().ClearInputs();
        }
    }
}
