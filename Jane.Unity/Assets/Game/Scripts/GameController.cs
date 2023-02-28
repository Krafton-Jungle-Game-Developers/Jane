using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameController : MonoBehaviour
{
    [SerializeField] private TMP_Text connectingText;

    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI endGameText;
    public GameObject hud;
    private int _countdownTime = 3;
    private SpaceshipController _spaceshipController;

    // BGM Player
    [SerializeField] AudioSource bgmPlayer;
    [SerializeField] AudioClip waitingClip;
    [SerializeField] AudioClip gogoClip;
    [SerializeField] float volumeMax = 0.2f;

    private float fadeRate = 0.05f;
    

    private void Start()
    {
        // BGM Player
        bgmPlayer = GetComponent<AudioSource>();
        bgmPlayer.clip = waitingClip;
        bgmPlayer.volume = volumeMax;
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

            bgmPlayer.volume -= fadeRate;
        }

        countdownText.text = "GO!";

        yield return new WaitForSeconds(1f);

        _spaceshipController.canControl = true;

        hud.GetComponentInChildren<Canvas>().enabled = true;

        countdownText.gameObject.SetActive(false);

        bgmPlayer.clip = gogoClip;
        bgmPlayer.Play();

        while (bgmPlayer.volume < volumeMax)
        {
            bgmPlayer.volume += fadeRate;
            yield return new WaitForSeconds(0.1f);
        }
        
    }

    
}
