using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace Michsky.UI.Reach
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CanvasGroup))]
    public class ModalWindowManager : MonoBehaviour
    {
        // Resources
        public Image windowIcon;
        public TextMeshProUGUI windowTitle;
        public TextMeshProUGUI windowDescription;
        public ButtonManager confirmButton;
        public ButtonManager cancelButton;
        [SerializeField] private Animator mwAnimator;

        // Content
        public Sprite icon;
        public string titleText = "Title";
        [TextArea] public string descriptionText = "Description here";

        // Localization
        public string titleKey;
        public string descriptionKey;

        // Settings
        public bool useCustomContent = false;
        public bool isOn = false;
        public bool closeOnCancel = true;
        public bool closeOnConfirm = true;
        public bool showCancelButton = true;
        public bool showConfirmButton = true;
        public bool useLocalization = true;
        [Range(0.5f, 2)] public float animationSpeed = 1;
        public StartBehaviour startBehaviour = StartBehaviour.Disable;
        public CloseBehaviour closeBehaviour = CloseBehaviour.Disable;
        public InputType inputType = InputType.Focused;

        // Events
        public UnityEvent onConfirm;
        public UnityEvent onCancel;
        public UnityEvent onOpen;
        public UnityEvent onClose;

        // Helpers
        string animIn = "In";
        string animOut = "Out";
        string animSpeedKey = "AnimSpeed";

        // Event System
        bool canProcessEventSystem;
        float openStateLength;
        float closeStateLength;
        GameObject latestEventSystemObject;
        ControllerManager controllerManager;

        public enum StartBehaviour { Enable, Disable }
        public enum CloseBehaviour { Disable, Destroy }
        public enum InputType { Focused, Free }

        void Awake()
        {
            InitModalWindow();
            InitEventSystem();
            UpdateUI();
        }

        void Update()
        {
            if (inputType == InputType.Free || isOn == false || canProcessEventSystem == false || controllerManager.gamepadEnabled == false)
                return;

            CheckForEventButtons();
        }

        void InitModalWindow()
        {
            if (mwAnimator == null) { mwAnimator = gameObject.GetComponent<Animator>(); }
            if (closeOnCancel == true) { onCancel.AddListener(CloseWindow); }
            if (closeOnConfirm == true) { onConfirm.AddListener(CloseWindow); }
            if (confirmButton != null) { confirmButton.onClick.AddListener(onConfirm.Invoke); }
            if (cancelButton != null) { cancelButton.onClick.AddListener(onCancel.Invoke); }

            if (useLocalization == true)
            {
                LocalizedObject mainLoc = GetComponent<LocalizedObject>();

                if (mainLoc == null || mainLoc.CheckLocalizationStatus() == false) { useLocalization = false; }
                else
                {
                    if (windowTitle != null && !string.IsNullOrEmpty(titleKey))
                    {
                        LocalizedObject titleLoc = windowTitle.gameObject.GetComponent<LocalizedObject>();
                        if (titleLoc != null) 
                        { 
                            titleLoc.tableIndex = mainLoc.tableIndex; 
                            titleLoc.localizationKey = titleKey;
                            titleLoc.UpdateItem();
                        }
                    }

                    if (windowDescription != null && !string.IsNullOrEmpty(descriptionKey))
                    {
                        LocalizedObject descLoc = windowDescription.gameObject.GetComponent<LocalizedObject>();
                        if (descLoc != null) 
                        { 
                            descLoc.tableIndex = mainLoc.tableIndex; 
                            descLoc.localizationKey = descriptionKey;
                            descLoc.UpdateItem();
                        }
                    }
                }
            }

            if (startBehaviour == StartBehaviour.Disable) { isOn = false; gameObject.SetActive(false); }
            else if (startBehaviour == StartBehaviour.Enable) { isOn = false; OpenWindow(); }

            openStateLength = ReachInternalTools.GetAnimatorClipLength(mwAnimator, "ModalWindow_In");
            closeStateLength = ReachInternalTools.GetAnimatorClipLength(mwAnimator, "ModalWindow_Out");
        }

        void InitEventSystem()
        {
            if (FindObjectsOfType(typeof(ControllerManager)).Length > 0) { controllerManager = (ControllerManager)FindObjectsOfType(typeof(ControllerManager))[0]; }
            if (controllerManager == null) { canProcessEventSystem = false; }
            else if (cancelButton == null && confirmButton == null) { canProcessEventSystem = false; }
            else { canProcessEventSystem = true; }
        }

        void CheckForEventButtons()
        {
            if (cancelButton != null && EventSystem.current.currentSelectedGameObject != cancelButton.gameObject && EventSystem.current.currentSelectedGameObject != confirmButton.gameObject) { controllerManager.SelectUIObject(cancelButton.gameObject); }
            else if (confirmButton != null && EventSystem.current.currentSelectedGameObject != cancelButton.gameObject && EventSystem.current.currentSelectedGameObject != confirmButton.gameObject) { controllerManager.SelectUIObject(confirmButton.gameObject); }
        }

        public void UpdateUI()
        {
            if (useCustomContent == false)
            {
                if (windowIcon != null) { windowIcon.sprite = icon; }
                if (windowTitle != null && useLocalization == false) { windowTitle.text = titleText; }
                if (windowDescription != null && useLocalization == false) { windowDescription.text = descriptionText; }
            }

            if (showCancelButton == true && cancelButton != null) { cancelButton.gameObject.SetActive(true); }
            else if (cancelButton != null) { cancelButton.gameObject.SetActive(false); }

            if (showConfirmButton == true && confirmButton != null) { confirmButton.gameObject.SetActive(true); }
            else if (confirmButton != null) { confirmButton.gameObject.SetActive(false); }
        }

        public void OpenWindow()
        {
            if (isOn == true) { return; }
            if (EventSystem.current.currentSelectedGameObject != null) { latestEventSystemObject = EventSystem.current.currentSelectedGameObject; }

            gameObject.SetActive(true);
            isOn = true;

            StopCoroutine("DisableObject");
            StopCoroutine("DisableAnimator");
            StartCoroutine("DisableAnimator");

            mwAnimator.enabled = true;
            mwAnimator.SetFloat(animSpeedKey, animationSpeed);
            mwAnimator.Play(animIn);
            onOpen.Invoke();
        }

        public void CloseWindow()
        {
            if (isOn == false)
                return;

            if (gameObject.activeSelf == true)
            {
                StopCoroutine("DisableObject");
                StopCoroutine("DisableAnimator");
                StartCoroutine("DisableObject");
            }

            isOn = false;
            mwAnimator.enabled = true;
            mwAnimator.SetFloat(animSpeedKey, animationSpeed);
            mwAnimator.Play(animOut);
            onClose.Invoke();

            if (latestEventSystemObject != null && latestEventSystemObject.activeInHierarchy) { controllerManager.SelectUIObject(latestEventSystemObject); }
        }

        public void AnimateWindow()
        {
            if (isOn == false) { OpenWindow(); }
            else { CloseWindow(); }
        }

        IEnumerator DisableObject()
        {
            yield return new WaitForSecondsRealtime(closeStateLength);

            if (closeBehaviour == CloseBehaviour.Disable) { gameObject.SetActive(false); }
            else if (closeBehaviour == CloseBehaviour.Destroy) { Destroy(gameObject); }

            mwAnimator.enabled = false;
        }

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSecondsRealtime(openStateLength + 0.1f);
            mwAnimator.enabled = false;
        }
    }
}