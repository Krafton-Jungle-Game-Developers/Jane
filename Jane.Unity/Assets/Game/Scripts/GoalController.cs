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
            HUDManager hud = GameObject.FindGameObjectWithTag("HUD").GetComponentInParent<HUDManager>();
            hud.hudCanvas.enabled = false;
            hud.standingsCanvas.enabled = false;
            hud.targetCanvas.enabled = false;
            hud.resultCanvas.enabled = true;
            RankManager.instance.SetResult();
            other.gameObject.GetComponent<SpaceshipEngine>().DisableMovement();
            other.gameObject.GetComponent<SpaceshipEngine>().ClearInputs();
        }
    }
}
