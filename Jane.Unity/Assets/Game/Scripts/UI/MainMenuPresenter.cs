using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using CancellationToken = System.Threading.CancellationToken;

namespace Jane.Unity.UI
{
    public class MainMenuPresenter : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private Image titleImage;
        private readonly int _titleAnimationFactorID = Shader.PropertyToID("_Animation_Factor");

        private void Awake()
        {
            titleImage.material.SetFloat(_titleAnimationFactorID, 0f);
            ActivateTitleAnimation(this.GetCancellationTokenOnDestroy()).Forget();

            playButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromSeconds(1)).Subscribe(async _ =>
            {
                await fadeCanvasGroup.DOFade(1f, 0.5f);
                sceneLoader.LoadSceneAsync("Main").Forget();
            }).AddTo(this);

            optionsButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                Debug.Log("Options Button Pressed.");
            }).AddTo(this);

            quitButton.OnClickAsObservable().ThrottleFirst(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                Application.Quit();
            }).AddTo(this);
        }

        private async UniTaskVoid ActivateTitleAnimation(CancellationToken token)
        {
            float current = 0f;
            while (!token.IsCancellationRequested && titleImage.materialForRendering.GetFloat(_titleAnimationFactorID) < 1f)
            {
                current = titleImage.materialForRendering.GetFloat(_titleAnimationFactorID);
                titleImage.materialForRendering.SetFloat(_titleAnimationFactorID, Mathf.Min(current + Time.deltaTime, 1f));
                await UniTask.Yield();
            }
        }
    }    
}
