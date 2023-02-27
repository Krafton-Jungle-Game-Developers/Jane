using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Michsky.UI.Reach
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Reach UI/Localization/Localized Object")]
    public class LocalizedObject : MonoBehaviour
    {
        // Resources
        public LocalizationManager localizationManager;
        public LocalizationSettings localizationSettings;
        public TextMeshProUGUI textObj;

        // Settings
        public int tableIndex = -1;
        public string localizationKey;
        public ObjectType objectType = ObjectType.TextMeshPro;
        public UpdateMode updateMode = UpdateMode.OnEnable;
        public bool rebuildLayoutOnUpdate;
        [SerializeField] private bool forceAddToManager = false;
#if UNITY_EDITOR
        public bool showOutputOnEditor = true;
#endif

        // Events
        public LanguageChangedEvent onLanguageChanged = new LanguageChangedEvent();

        [System.Serializable]
        public class LanguageChangedEvent : UnityEvent<string> { }

        // Helpers
        public bool isInitialized = false;

        public enum UpdateMode { OnEnable, OnDemand }
        public enum ObjectType { TextMeshPro, Custom, ComponentDriven }

        void Awake()
        {
            if (localizationManager != null && localizationManager.UIManagerAsset.enableLocalization == false) { Destroy(this); return; }
            InitializeItem();
        }

        void OnEnable()
        {
            if (localizationManager == null) { Destroy(this); return; }
            if (isInitialized == false || localizationManager == null || localizationManager.currentLanguageAsset == null) { return; }
            if (updateMode == UpdateMode.OnEnable) { UpdateItem(); }
        }

        public void InitializeItem()
        {
            if (isInitialized == true)
                return;

            if (localizationManager == null) 
            {
                bool locManagerFound = false;

                foreach (LocalizationManager lm in Resources.FindObjectsOfTypeAll(typeof(LocalizationManager)) as LocalizationManager[])
                {
                    if (lm.gameObject.scene.name != null)
                    {
                        localizationManager = lm;
                        locManagerFound = true;
                        break;
                    }
                }

                if (locManagerFound == false)
                {
                    UIManager tempUIM = (UIManager)Resources.FindObjectsOfTypeAll(typeof(UIManager))[0];
                    if (tempUIM == null || tempUIM.enableLocalization == false) { return; }

                    GameObject newLM = new GameObject("Localization Manager [Auto Generated]");
                    localizationManager = newLM.AddComponent<LocalizationManager>();
                }
            }
            if (localizationManager != null && localizationManager.UIManagerAsset.enableLocalization == false) { Destroy(this); return; }
            if (localizationManager == null || localizationManager.UIManagerAsset == null || localizationManager.UIManagerAsset.enableLocalization == false) { return; }
            if (forceAddToManager == true && !localizationManager.localizedItems.Contains(this)) { localizationManager.localizedItems.Add(this); }
            if (objectType == ObjectType.TextMeshPro && textObj == null) { textObj = gameObject.GetComponent<TextMeshProUGUI>(); }

            isInitialized = true;
        }

        public void ReInitializeItem()
        {
            isInitialized = false;
            InitializeItem();
        }

        public void UpdateItem()
        {
            if (isInitialized == false || localizationManager == null || localizationManager.currentLanguageAsset == null || localizationManager.currentLanguageAsset.tableList.Count == 0)
                return;

            if (objectType == ObjectType.TextMeshPro && textObj != null)
            {
                for (int i = 0; i < localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent.Count; i++)
                {
                    if (localizationKey == localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].key)
                    {
                        if (string.IsNullOrEmpty(localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].value))
                        {
                            if (LocalizationManager.enableLogs == true) { Debug.Log("<b>[Localized Object]</b> The specified key '" + localizationKey + "' could not be found or the output value is empty for " + localizationManager.currentLanguageAsset.languageName + ".", this); }           
                            break;
                        }

                        textObj.text = localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].value;
                        onLanguageChanged.Invoke(localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].value);
                        break;
                    }
                }
            }

            else if (objectType == ObjectType.Custom || objectType == ObjectType.ComponentDriven)
            {
                for (int i = 0; i < localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent.Count; i++)
                {
                    if (localizationKey == localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].key)
                    {
                        if (string.IsNullOrEmpty(localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].value))
                            break;

                        onLanguageChanged.Invoke(localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].value);
                        break;
                    }
                }
            }

            if (rebuildLayoutOnUpdate == true && gameObject.activeInHierarchy == true) { StartCoroutine("RebuildLayout"); }
        }

        public bool CheckLocalizationStatus()
        {
            if (isInitialized == false) { InitializeItem(); }
            if (localizationManager == null || localizationManager.UIManagerAsset == null || localizationManager.UIManagerAsset.enableLocalization == false) { return false; }
            else { return true; }
        }

        public string GetKeyOutput(string key)
        {
            string keyValue = null;
            bool keyFound = false;

            if (isInitialized == false || localizationManager == null || localizationManager.currentLanguageAsset == null || localizationManager.currentLanguageAsset.tableList.Count == 0)
                return LocalizationSettings.notInitializedText;

            for (int i = 0; i < localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent.Count; i++)
            {
                if (key == localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].key)
                {
                    keyValue = localizationManager.currentLanguageAsset.tableList[tableIndex].tableContent[i].value;
                    keyFound = true;
                    break;
                }
            }

            if (keyFound == true && string.IsNullOrEmpty(keyValue))
            {
                if (LocalizationManager.enableLogs == true) { Debug.Log("<b>[Localized Object]</b> The output value for '" + key + "' is empty in " + localizationManager.currentLanguageAsset.languageName + ".", this); }
                return "EMPTY_KEY_IN_" + localizationManager.currentLanguageAsset.languageID + ": " + key;
            }

            else if (keyFound == false)
            {
                if (LocalizationManager.enableLogs == true) { Debug.Log("<b>[Localized Object]</b> The specified key '" + key + "' could not be found in " + localizationManager.currentLanguageAsset.languageName + ".", this); }
                return "MISSING_KEY_IN_" + localizationManager.currentLanguageAsset.languageID + ": " + key;
            }

            return keyValue;
        }

        public static string GetKeyOutput(string tableID, string tableKey)
        {
            UIManager tempUIM = (UIManager)Resources.FindObjectsOfTypeAll(typeof(UIManager))[0];

            if (tempUIM == null || tempUIM.enableLocalization == false)
                return null;

            int tableIndex = -1;
            string keyValue = null;

            for (int i = 0; i < tempUIM.currentLanguage.tableList.Count; i++)
            {
                if (tempUIM.currentLanguage.tableList[i].table.tableID == tableID)
                {
                    tableIndex = i;
                    break;
                }
            }

            if (tableIndex == -1) { return null; }
            else
            {
                for (int i = 0; i < tempUIM.currentLanguage.tableList[tableIndex].tableContent.Count; i++)
                {
                    if (tempUIM.currentLanguage.tableList[tableIndex].tableContent[i].key == tableKey)
                    {
                        keyValue = tempUIM.currentLanguage.tableList[tableIndex].tableContent[i].value;
                        break;
                    }
                }
            }

            return keyValue;
        }

        IEnumerator RebuildLayout()
        {
            yield return new WaitForSecondsRealtime(0.025f);
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        }
    }
}