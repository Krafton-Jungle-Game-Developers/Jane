using System;
using Cysharp.Threading.Tasks;
using Jane.Unity;
using Jane.Unity.ServerShared.Enums;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject countDownHolder;
    public GameObject CountDownHolder => countDownHolder;
    [SerializeField] private GameObject playingHolder;
    public GameObject PlayingHolder => playingHolder;
    [SerializeField] private TMP_Text countDownText;
    public TMP_Text CountDownText => countDownText;
    [SerializeField] private TMP_Text timerText;

    [SerializeField] private SpaceshipInputManager inputManager;
    [SerializeField] private SpaceshipCameraController cameraController;
    [SerializeField] private GameState CurrentGameState;

    private void Update()
    {
        CurrentGameState = GameInfo.GameState;
    }

    public async UniTask CountDownAsync(int seconds)
    {
        countDownHolder.SetActive(true);
        while (seconds > 0)
        {
            countDownText.text = $"{seconds}";
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            seconds--;
        }
    }

    public void StartGame()
    {
        countDownText.text = "GO!";
        playingHolder.SetActive(true);

        inputManager.EnableInput();
        inputManager.EnableMovement();
        inputManager.EnableSteering();
    }
}
