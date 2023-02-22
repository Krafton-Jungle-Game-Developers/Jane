using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI endGameText;
    public GameObject hud;
    private int _countdownTime = 3;
    private SpaceshipController _spaceshipController;

    void Start()
    {
        _spaceshipController = GameObject.FindGameObjectWithTag("Player").GetComponent<SpaceshipController>();
        StartGame();
    }

    private void StartGame()
    {
        StartCoroutine(CountdownStart());
        _spaceshipController.canControl = false;
    }

    public void EndGame()
    {
        _spaceshipController.canControl = false;
        hud.GetComponentInChildren<Canvas>().enabled = false;
        endGameText.gameObject.SetActive(true);
    }

    IEnumerator CountdownStart()
    {
        while (_countdownTime > 0)
        {
            countdownText.text = _countdownTime.ToString();

            yield return new WaitForSeconds(1f);

            _countdownTime--;
        }

        countdownText.text = "GO!";

        yield return new WaitForSeconds(1f);

        _spaceshipController.canControl = true;

        hud.GetComponentInChildren<Canvas>().enabled = true;

        countdownText.gameObject.SetActive(false);
    }

    IEnumerator SpawnMeteor()
    {
        yield return null;
    }
}
