using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

namespace Michsky.UI.Reach
{
    public class ControllerManager : MonoBehaviour
    {
        // Resources
        public ControllerPresetManager presetManager;
        public GameObject firstSelected;
        public List<PanelManager> panels = new List<PanelManager>();
        public List<ButtonManager> buttons = new List<ButtonManager>();
        public List<SettingsElement> settingsElements = new List<SettingsElement>();
        public List<ModeSelector> modeSelectors = new List<ModeSelector>();
        [Tooltip("Objects in this list will be enabled when the gamepad is un-plugged.")]
        public List<GameObject> keyboardObjects = new List<GameObject>();
        [Tooltip("Objects in this list will be enabled when the gamepad is plugged.")]
        public List<GameObject> gamepadObjects = new List<GameObject>();
        public List<HotkeyEvent> hotkeyObjects = new List<HotkeyEvent>();

        // Settings
        [Tooltip("Checks for input changes each frame.")]
        public bool alwaysUpdate = true;
        public bool affectCursor = true;
        public InputAction gamepadHotkey;

        // Helpers
        Vector3 cursorPos;
        Vector3 lastCursorPos;
        Navigation customNav = new Navigation();
        [HideInInspector] public int currentManagerIndex;

        [HideInInspector] public bool gamepadConnected;
        [HideInInspector] public bool gamepadEnabled;
        [HideInInspector] public bool keyboardEnabled;

        [HideInInspector] public float hAxis;
        [HideInInspector] public float vAxis;

        [HideInInspector] public string currentController;
        [HideInInspector] public ControllerPreset currentControllerPreset;

        void Start()
        {
            InitObjects();
            InitInput();
        }

        void Update()
        {
            if (alwaysUpdate == false)
                return;

            CheckForController();
            CheckForEmptyObject();
        }

        void InitObjects()
        {
            foreach (ButtonManager bm in Resources.FindObjectsOfTypeAll(typeof(ButtonManager)) as ButtonManager[]) { if (bm.gameObject.scene.name != null) { buttons.Add(bm); } }
            foreach (SettingsElement se in Resources.FindObjectsOfTypeAll(typeof(SettingsElement)) as SettingsElement[]) { if (se.gameObject.scene.name != null) { settingsElements.Add(se); } }
            foreach (ModeSelector ms in Resources.FindObjectsOfTypeAll(typeof(ModeSelector)) as ModeSelector[]) { if (ms.gameObject.scene.name != null) { modeSelectors.Add(ms); } }
            foreach (PanelManager pm in Resources.FindObjectsOfTypeAll(typeof(PanelManager)) as PanelManager[])
            {
                if (pm.gameObject.scene.name == null)
                    continue;

                pm.controllerManager = this;
                pm.managerIndex = panels.Count;
                panels.Add(pm);
            }
        }

        void InitInput()
        {
            gamepadHotkey.Enable();

            if (Gamepad.current == null) { gamepadConnected = false; SwitchToKeyboard(); }
            else { gamepadConnected = true; SwitchToGamepad(); }
        }

        void CheckForEmptyObject()
        {
            if (gamepadEnabled == false) { return; }
            else if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.gameObject.activeInHierarchy == true) { return; }

            if (gamepadHotkey.triggered && panels.Count != 0)
            {
                SelectUIObject(panels[currentManagerIndex].panels[panels[currentManagerIndex].currentPanelIndex].firstSelected);
            }
        }

        public void CheckForController()
        {
            if (Gamepad.current == null) { gamepadConnected = false; }
            else
            {
                gamepadConnected = true;
                hAxis = Gamepad.current.rightStick.x.ReadValue();
                vAxis = Gamepad.current.rightStick.y.ReadValue();
            }

            cursorPos = Mouse.current.position.ReadValue();

            if (gamepadConnected == true && gamepadEnabled == true && keyboardEnabled == false && cursorPos != lastCursorPos) { SwitchToKeyboard(); }
            else if (gamepadConnected == true && gamepadEnabled == false && keyboardEnabled == true && gamepadHotkey.triggered) { SwitchToGamepad(); }
            else if (gamepadConnected == false && keyboardEnabled == false) { SwitchToKeyboard(); }
        }

        void CheckForCurrentObject()
        {
            if ((EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.gameObject.activeInHierarchy == false) && panels.Count != 0)
            {
                SelectUIObject(panels[currentManagerIndex].panels[panels[currentManagerIndex].currentPanelIndex].firstSelected);
            }
        }

        public void SwitchToGamepad()
        {
            if (affectCursor == true) { Cursor.visible = false; }
          
            for (int i = 0; i < keyboardObjects.Count; i++) 
            {
                if (keyboardObjects[i] == null)
                    continue;

                keyboardObjects[i].SetActive(false);
            }

            for (int i = 0; i < gamepadObjects.Count; i++)
            {
                if (gamepadObjects[i] == null)
                    continue;

                gamepadObjects[i].SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(gamepadObjects[i].GetComponentInParent<RectTransform>());
            }

            customNav.mode = Navigation.Mode.Automatic;

            for (int i = 0; i < buttons.Count; i++) { if (buttons[i] != null && buttons[i].useUINavigation == false) { buttons[i].AddUINavigation(); } }
            for (int i = 0; i < settingsElements.Count; i++) { if (settingsElements[i] != null && settingsElements[i].useUINavigation == false) { settingsElements[i].AddUINavigation(); } }
            for (int i = 0; i < modeSelectors.Count; i++) { if (modeSelectors[i] != null && modeSelectors[i].useUINavigation == false) { modeSelectors[i].AddUINavigation(); } }

            gamepadEnabled = true;
            keyboardEnabled = false;
            lastCursorPos = Mouse.current.position.ReadValue();

            CheckForGamepadType();
            CheckForCurrentObject();
        }

        void CheckForGamepadType()
        {
            currentController = Gamepad.current.displayName;

            // Search for main and custom gameapds
            if (Gamepad.current is XInputController && presetManager != null && presetManager.xboxPreset != null) { currentControllerPreset = presetManager.xboxPreset; }
#if !UNITY_WEBGL
            else if (Gamepad.current is DualSenseGamepadHID && presetManager != null && presetManager.dualsensePreset != null) { currentControllerPreset = presetManager.dualsensePreset; }
#endif
            else
            {
                for (int i = 0; i < presetManager.customPresets.Count; i++)
                {
                    if (currentController == presetManager.customPresets[i].controllerName)
                    {
                        currentControllerPreset = presetManager.customPresets[i];
                        break;
                    }
                }
            }

            foreach (HotkeyEvent he in hotkeyObjects) 
            { 
                he.controllerPreset = currentControllerPreset;
                he.UpdateUI();
            }
        }

        public void SwitchToKeyboard()
        {
            if (affectCursor == true) { Cursor.visible = true; }
            if (presetManager != null && presetManager.keyboardPreset != null) 
            {
                currentControllerPreset = presetManager.keyboardPreset;
                foreach (HotkeyEvent he in hotkeyObjects)
                {
                    he.controllerPreset = currentControllerPreset;
                    he.UpdateUI();
                }
            }

            for (int i = 0; i < gamepadObjects.Count; i++) 
            {
                if (gamepadObjects[i] == null)
                    continue;

                gamepadObjects[i].SetActive(false);
            }

            for (int i = 0; i < keyboardObjects.Count; i++)
            {
                if (keyboardObjects[i] == null)
                    continue;

                keyboardObjects[i].SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(keyboardObjects[i].GetComponentInParent<RectTransform>());
            }

            customNav.mode = Navigation.Mode.None;

            for (int i = 0; i < buttons.Count; i++) { if (buttons[i] != null && buttons[i].useUINavigation == false) { buttons[i].DisableUINavigation(); } }
            for (int i = 0; i < settingsElements.Count; i++) { if (settingsElements[i] != null && settingsElements[i].useUINavigation == false) { settingsElements[i].DisableUINavigation(); } }
            for (int i = 0; i < modeSelectors.Count; i++) { if (modeSelectors[i] != null && modeSelectors[i].useUINavigation == false) { modeSelectors[i].DisableUINavigation(); } }

            gamepadEnabled = false;
            keyboardEnabled = true;
        }

        public void SelectUIObject(GameObject tempObj)
        {
            if (gamepadEnabled == false)
                return;

            EventSystem.current.SetSelectedGameObject(tempObj.gameObject);
        }

        public void AddButton(ButtonManager btn)
        {
            buttons.Add(btn);

            if (gamepadEnabled == true && btn.useUINavigation == false)
            {
                btn.AddUINavigation();
            }
        }

        public void AddSettingsElement(SettingsElement se)
        {
            settingsElements.Add(se);

            if (gamepadEnabled == true && se.useUINavigation == false)
            {
                se.AddUINavigation();
            }
        }

        public void AddModeSelector(ModeSelector ms)
        {
            modeSelectors.Add(ms);

            if (gamepadEnabled == true && ms.useUINavigation == false)
            {
                ms.AddUINavigation();
            }
        }

        public void AddPanelManager(PanelManager pm)
        {
            panels.Add(pm);
        }
    }
}