using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Jane.Unity.ServerShared.Enums;

public class GameController : MonoBehaviour
{
    [SerializeField] private TMP_Text connectingText;

    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI endGameText;
    private GameObject hud;
    public GameState gameState;
    private int _countdownTime = 3;
    private SpaceshipEngine _spaceshipEngine;

    // BGM Player
    [SerializeField] AudioSource bgmPlayer;
    [SerializeField] AudioClip bgmClip;
    [SerializeField] float volumeMax = 0.2f;

    private float fadeRate = 0.05f;
    

    private void Start()
    {
        // BGM Player
        bgmPlayer = GetComponent<AudioSource>();
        bgmPlayer.clip = bgmClip;
        bgmPlayer.volume = volumeMax;
        bgmPlayer.Play();

        //for local test
        _spaceshipEngine = GameObject.FindGameObjectWithTag("Player").GetComponent<SpaceshipEngine>();
        hud = GameObject.FindGameObjectWithTag("HUD");
        hud.GetComponent<Canvas>().enabled = false;
        gameState = GameState.Waiting;
        StartGame();
    }

    private void StartGame()
    {
        _spaceshipEngine.DisableMovement();
        StartCoroutine(CountdownStart());
    }

    public void EndGame()
    {
        hud.GetComponent<Canvas>().enabled = false;
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

        _spaceshipEngine.EnableMovement();
        hud.GetComponent<Canvas>().enabled = true;
        countdownText.gameObject.SetActive(false);
        gameState = GameState.Playing;
    }

    public void HideMouse()
    {
        Cursor.visible = false;
    }
}
