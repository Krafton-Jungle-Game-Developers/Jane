using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameController : MonoBehaviour
{
    [SerializeField] private TMP_Text connectingText;

    // BGM Player
    [SerializeField] AudioSource bgmPlayer;
    [SerializeField] AudioClip waitingClip;
    [SerializeField] AudioClip gogoClip;

    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI endGameText;
    public GameObject hud;
    private int _countdownTime = 3;
    private SpaceshipController _spaceshipController;

    private void Start()
    {
        // BGM Player
        bgmPlayer = GetComponent<AudioSource>();
        bgmPlayer.clip = waitingClip;
        bgmPlayer.Play();

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


        // Change bgm to flying clip
        // bgmPlayer.Stop();
        bgmPlayer.clip = gogoClip;
        bgmPlayer.Play();
    }

    private void ChangeBGM()
    {
        
    }
}
