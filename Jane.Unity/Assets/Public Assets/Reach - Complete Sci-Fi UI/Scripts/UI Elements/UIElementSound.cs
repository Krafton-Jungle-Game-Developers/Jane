using UnityEngine;
using UnityEngine.EventSystems;

namespace Michsky.UI.Reach
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Reach UI/Audio/UI Element Sound")]
    public class UIElementSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [Header("Resources")]
        public UIManagerAudio audioManager;
        public AudioSource audioSource;

        [Header("Custom SFX")]
        public AudioClip hoverSFX;
        public AudioClip clickSFX;

        [Header("Settings")]
        public bool enableHoverSound = true;
        public bool enableClickSound = true;

        void OnEnable()
        {
            if (audioManager == null) { audioManager = (UIManagerAudio)GameObject.FindObjectsOfType(typeof(UIManagerAudio))[0]; }
            if (audioManager != null && audioSource == null) { audioSource = audioManager.audioSource; }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enableHoverSound == true)
            {
                if (hoverSFX == null) { audioSource.PlayOneShot(audioManager.UIManagerAsset.hoverSound); }
                else { audioSource.PlayOneShot(hoverSFX); }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (enableClickSound == true)
            {
                if (clickSFX == null) { audioSource.PlayOneShot(audioManager.UIManagerAsset.clickSound); }
                else { audioSource.PlayOneShot(clickSFX); }
            }
        }
    }
}