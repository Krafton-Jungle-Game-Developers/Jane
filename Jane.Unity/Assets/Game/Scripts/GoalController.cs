using TMPro;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    private GameController _gameController;

    void Start()
    {
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

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
            other.gameObject.GetComponent<SpaceshipEngine>().DisableMovement();
            other.gameObject.GetComponent<SpaceshipEngine>().ClearInputs();
            _gameController.endGameText.GetComponent<TMP_Text>().enabled = true;
            Canvas hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<Canvas>();
            hud.enabled = false;
        }
    }
}
