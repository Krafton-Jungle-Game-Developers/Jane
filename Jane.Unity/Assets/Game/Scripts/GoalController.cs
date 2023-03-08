using UnityEngine;

public class GoalController : MonoBehaviour
{
    //private GameController _gameController;

    //void Start()
    //{
    //    _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<NetworkPlayer>() != null)
        {
            Debug.Log("finished");
            other.gameObject.GetComponent<NetworkPlayer>().isFinished = true;
        }
    }
}
