using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Michsky.UI.Reach
{
    [RequireComponent(typeof(Animator))]
    public class FeedNotification : MonoBehaviour
    {
        // Content
        public Sprite icon;
        [TextArea] public string notificationText = "Quest text here";
        public string localizationKey;

        // Resources
        [SerializeField] private Animator itemAnimator;
        [SerializeField] private Image iconObj;
        [SerializeField] private TextMeshProUGUI textObj;

        // Settings
        public bool useLocalization = true;
        [SerializeField] private bool updateOnAnimate = true;
        [Range(0, 10)] public float minimizeAfter = 3;
        public DefaultState defaultState = DefaultState.Minimized;
        public AfterMinimize afterMinimize = AfterMinimize.Disable;

        // Events
        public UnityEvent onDestroy;

        // Helpers
        bool isOn;
        LocalizedObject localizedObject;

        public enum DefaultState { Minimized, Expanded }
        public enum AfterMinimize { Disable, Destroy }

        void Start()
        {
            if (itemAnimator == null) { itemAnimator = GetComponent<Animator>(); }
            if (useLocalization == true)
            {
                localizedObject = textObj.GetComponent<LocalizedObject>();

                if (localizedObject == null || localizedObject.CheckLocalizationStatus() == false) { useLocalization = false; }
                else if (localizedObject != null && !string.IsNullOrEmpty(localizationKey))
                {
                    // Forcing component to take the localized output on awake
                    notificationText = localizedObject.GetKeyOutput(localizationKey);

                    // Change text on language change
                    localizedObject.onLanguageChanged.AddListener(delegate
                    {
                        notificationText = localizedObject.GetKeyOutput(localizationKey);
                        UpdateUI();
                    });
                }
            }

            if (defaultState == DefaultState.Minimized) { gameObject.SetActive(false); }
            else if (defaultState == DefaultState.Expanded) { ExpandNotification(); }

            UpdateUI();
        }

        public void UpdateUI()
        {
            iconObj.sprite = icon;
            textObj.text = notificationText;
        }

        public void AnimateNotification()
        {
            ExpandNotification();
        }

        public void ExpandNotification()
        {
            if (isOn == true)
            {
                StopCoroutine("DisableAnimator");
                StartCoroutine("DisableAnimator");

                if (minimizeAfter != 0)
                {
                    StopCoroutine("MinimizeItem");
                    StartCoroutine("MinimizeItem");
                }

                return;
            }

            isOn = true;
            gameObject.SetActive(true);
            itemAnimator.enabled = true;
            itemAnimator.Play("In");

            if (updateOnAnimate == true) { UpdateUI(); }
            if (minimizeAfter != 0) { StopCoroutine("MinimizeItem"); StartCoroutine("MinimizeItem"); }

            StopCoroutine("DisableAnimator");
            StartCoroutine("DisableAnimator");

            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        public void MinimizeNotification()
        {
            if (isOn == false)
                return;

            StopCoroutine("DisableAnimator");

            itemAnimator.enabled = true;
            itemAnimator.Play("Out");

            StopCoroutine("DisableItem");
            StartCoroutine("DisableItem");
        }

        public void DestroyNotification()
        {
            onDestroy.Invoke();
            Destroy(gameObject);
        }

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSeconds(ReachInternalTools.GetAnimatorClipLength(itemAnimator, "FeedNotification_In"));
            itemAnimator.enabled = false;
        }

        IEnumerator DisableItem()
        {
            yield return new WaitForSeconds(ReachInternalTools.GetAnimatorClipLength(itemAnimator, "FeedNotification_Out"));

            isOn = false;

            if (afterMinimize == AfterMinimize.Disable) { gameObject.SetActive(false); }
            else if (afterMinimize == AfterMinimize.Destroy) { DestroyNotification(); }
        }

        IEnumerator MinimizeItem()
        {
            yield return new WaitForSeconds(minimizeAfter);
            MinimizeNotification();
        }
    }
}