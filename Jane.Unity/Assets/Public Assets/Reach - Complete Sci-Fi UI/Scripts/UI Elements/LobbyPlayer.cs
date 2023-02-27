using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Michsky.UI.Reach
{
    public class LobbyPlayer : MonoBehaviour
    {
        // Content
        public Sprite playerPicture;
        public string playerName = "Player";
        public string additionalText = "LVL. 50";
        public ItemState currentState = ItemState.Empty;

        // Resources
        [SerializeField] private GameObject emptyParent;
        [SerializeField] private GameObject readyParent;
        [SerializeField] private GameObject notReadyParent;
        [SerializeField] private GameObject playerIndicatorReady;
        [SerializeField] private GameObject playerIndicatorNotReady;
        [SerializeField] private Image pictureReadyImg;
        [SerializeField] private Image pictureNotReadyImg;
        [SerializeField] private TextMeshProUGUI nameReadyTMP;
        [SerializeField] private TextMeshProUGUI nameNotReadyTMP;
        [SerializeField] private TextMeshProUGUI adtReadyTMP;
        [SerializeField] private TextMeshProUGUI adtNotReadyTMP;

        // Events
        public UnityEvent onEmpty;
        public UnityEvent onReady;
        public UnityEvent onUnready;

        public enum ItemState { Empty, Ready, NotReady }

        void OnEnable()
        {
            SetState(currentState);

            if (currentState == ItemState.Empty) { return; }
            if (nameReadyTMP.text != playerName) { SetPlayerName(playerName); }
            if (pictureReadyImg.sprite != playerPicture) { SetPlayerPicture(playerPicture); }
            if (adtReadyTMP.text != additionalText) { SetAdditionalText(additionalText); }
        }

        public void SetPlayerName(string name)
        {
            playerName = name;

            if (nameReadyTMP != null) { nameReadyTMP.text = playerName; }
            if (nameNotReadyTMP != null) { nameNotReadyTMP.text = playerName; }
        }

        public void SetPlayerPicture(Sprite pic)
        {
            playerPicture = pic;

            if (pictureReadyImg != null) { pictureReadyImg.sprite = playerPicture; }
            if (pictureNotReadyImg != null) { pictureNotReadyImg.sprite = playerPicture; }
        }

        public void SetAdditionalText(string text)
        {
            additionalText = text;

            if (string.IsNullOrEmpty(additionalText)) { return; }
            if (adtReadyTMP != null) { adtReadyTMP.text = additionalText; }
            if (adtNotReadyTMP != null) { adtNotReadyTMP.text = additionalText; }
        }

        public void SetState(ItemState state)
        {
            currentState = state;

            if (state == ItemState.Empty)
            {
                emptyParent.SetActive(true);
                readyParent.SetActive(false);
                notReadyParent.SetActive(false);

                onEmpty.Invoke();
            }

            else if (state == ItemState.Ready)
            {
                emptyParent.SetActive(false);
                readyParent.SetActive(true);
                notReadyParent.SetActive(false);

                onReady.Invoke();
            }

            else if (state == ItemState.NotReady)
            {
                emptyParent.SetActive(false);
                readyParent.SetActive(false);
                notReadyParent.SetActive(true);

                onUnready.Invoke();
            }
        }

        public void SetEmpty() { SetState(ItemState.Empty); }
        public void SetReady() { SetState(ItemState.Ready); }
        public void SetNotReady() { SetState(ItemState.NotReady); }
    }
}