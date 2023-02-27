using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Michsky.UI.Reach
{
    [DisallowMultipleComponent]
    public class MenuManager : MonoBehaviour
    {
        // Resources
        public UIManager UIManagerAsset;
        public Animator splashScreen;
        [SerializeField] private GameObject mainContent;
        [SerializeField] private ImageFading initPanel;
        [SerializeField] private ControllerManager controllerManager;

        // Helpers
        float splashInTime;
        float splashOutTime;

        void Awake()
        {
            if (initPanel != null) { initPanel.gameObject.SetActive(true); }
            if (splashScreen != null) { splashScreen.gameObject.SetActive(false); }
        }

        void Start()
        {
            StartCoroutine("StartInitialize");
        }

        public void DisableSplashScreen() 
        {
            StopCoroutine("DisableSplashScreenAnimator");
            StartCoroutine("FinalizeSplashScreen");

            splashScreen.enabled = true;
            splashScreen.Play("Out");
        }

        void Initialize()
        {
            if (UIManagerAsset == null || mainContent == null)
            {
                Debug.LogError("<b>[Reach UI]</b> Cannot initialize the resources due to missing resources.", this);
                return;
            }

            mainContent.gameObject.SetActive(false);

            if (UIManagerAsset.enableSplashScreen == true)
            {
                if (splashScreen == null)
                {
                    Debug.LogError("<b>[Reach UI]</b> Splash Screen is enabled but its resource is missing. Please assign the correct variable for 'Splash Screen'.", this);
                    return;
                }

                // Getting in and out animation length
                AnimationClip[] clips = splashScreen.runtimeAnimatorController.animationClips;
                splashInTime = clips[0].length;
                splashOutTime = clips[1].length;

                splashScreen.enabled = true;
                splashScreen.gameObject.SetActive(true);
                StartCoroutine("DisableSplashScreenAnimator");
            }

            else
            {
                if (mainContent == null)
                {
                    Debug.LogError("<b>[Reach UI]</b> 'Main Panels' is missing. Please assign the correct variable for 'Main Panels'.", this);
                    return;
                }

                if (splashScreen != null) { splashScreen.gameObject.SetActive(false); }
                mainContent.gameObject.SetActive(false);
                StartCoroutine("FinalizeSplashScreen");
            }
        }

        IEnumerator StartInitialize()
        {
            yield return new WaitForSecondsRealtime(1);
            if (initPanel != null) { initPanel.FadeOut(); }
            Initialize();
        }

        IEnumerator DisableSplashScreenAnimator()
        {
            yield return new WaitForSecondsRealtime(splashInTime + 0.1f);
            splashScreen.enabled = false;
        }

        IEnumerator FinalizeSplashScreen()
        {
            yield return new WaitForSecondsRealtime(splashOutTime + 0.1f);
           
            if (UIManagerAsset != null && UIManagerAsset.enableSplashScreen == true) 
            {
                splashScreen.gameObject.SetActive(false); 
            }

            mainContent.gameObject.SetActive(true);

            if (controllerManager != null && controllerManager.gamepadEnabled == true && controllerManager.firstSelected.activeInHierarchy == true)
            { 
                EventSystem.current.SetSelectedGameObject(controllerManager.firstSelected); 
            }
        }
    }
}