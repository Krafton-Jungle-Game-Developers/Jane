using Jane.Unity;
using TMPro;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    private GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NetworkPlayer player) 
            && player.IsFinished is false)
        {
            Debug.Log("finished");

            player.IsFinished = true;
            RankManager.instance.finishCount++;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            gameManager.RaceFinish();

            //other.gameObject.GetComponent<SpaceshipEngine>().DisableMovement();
            //other.gameObject.GetComponent<SpaceshipEngine>().ClearInputs();
            //_gameController.endGameText.GetComponent<TMP_Text>().enabled = true;
            //Canvas hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<Canvas>();
            //hud.enabled = false;
        }
    }
}
