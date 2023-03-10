//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Game/Scripts/Control/SpaceshipInputAction.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @SpaceshipInputAction : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @SpaceshipInputAction()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""SpaceshipInputAction"",
    ""maps"": [
        {
            ""name"": ""Spaceship Controls"",
            ""id"": ""e3aa8928-7929-4841-8446-6ac63e4df878"",
            ""actions"": [
                {
                    ""name"": ""Strafe"",
                    ""type"": ""PassThrough"",
                    ""id"": ""f6171447-765e-41cb-b212-da216c4e13e7"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Steer"",
                    ""type"": ""PassThrough"",
                    ""id"": ""60781e86-23fd-4c40-9f62-35403142d087"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Boost"",
                    ""type"": ""PassThrough"",
                    ""id"": ""6ea3ad93-3e6b-4cef-8ae8-e8ad29d43c9e"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Throttle"",
                    ""type"": ""PassThrough"",
                    ""id"": ""602a2e9e-7b49-41ce-81e1-bb280f107d62"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Roll"",
                    ""type"": ""PassThrough"",
                    ""id"": ""978e7bcf-6600-4166-8253-17305c3ce22b"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Strafe Keyboard"",
                    ""id"": ""aaabee2f-fd8b-492c-bcd0-21d71524b32b"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Strafe"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""16ad3586-698e-4276-b88a-125bfc4f8bb3"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Strafe"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""bc0ef2c5-eb93-4a39-af10-c319cfe44adc"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Strafe"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""29373260-f7c3-4652-bbb4-fd8ffffc2296"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Strafe"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9121af71-7a34-4bf4-a11c-58337df40f7c"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Strafe"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Steering Keyboard"",
                    ""id"": ""eeac00c3-4bb7-4462-b68d-91eb38dffb6a"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""9b7f223a-49e2-40ae-8b43-879b8d68891e"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""1df593ba-5481-4a11-8c6b-900dc294cee0"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""615d7085-c10e-406d-9d3c-2ee93fb397a5"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f36d1d54-c100-4729-8581-738443efde6d"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""0278519b-fafa-450d-9362-588f4002d9c1"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Boost"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""AccelerationKeyboard"",
                    ""id"": ""6459322c-34ed-480a-b5ac-910aea6139a2"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Throttle"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""44039359-3aa6-4f96-88a6-dfca5aa215ae"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Throttle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""8613c16f-0192-49f8-bbb1-57e015dd297a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Throttle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""RollKeyboard"",
                    ""id"": ""5d357d9e-1142-4845-a3cb-80b9e5910ec3"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Roll"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""b74622c9-1f95-425e-8099-da5a1b330669"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Roll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""34de5253-89cd-499a-97cc-4f412bf22695"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Roll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Spaceship Controls
        m_SpaceshipControls = asset.FindActionMap("Spaceship Controls", throwIfNotFound: true);
        m_SpaceshipControls_Strafe = m_SpaceshipControls.FindAction("Strafe", throwIfNotFound: true);
        m_SpaceshipControls_Steer = m_SpaceshipControls.FindAction("Steer", throwIfNotFound: true);
        m_SpaceshipControls_Boost = m_SpaceshipControls.FindAction("Boost", throwIfNotFound: true);
        m_SpaceshipControls_Throttle = m_SpaceshipControls.FindAction("Throttle", throwIfNotFound: true);
        m_SpaceshipControls_Roll = m_SpaceshipControls.FindAction("Roll", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Spaceship Controls
    private readonly InputActionMap m_SpaceshipControls;
    private ISpaceshipControlsActions m_SpaceshipControlsActionsCallbackInterface;
    private readonly InputAction m_SpaceshipControls_Strafe;
    private readonly InputAction m_SpaceshipControls_Steer;
    private readonly InputAction m_SpaceshipControls_Boost;
    private readonly InputAction m_SpaceshipControls_Throttle;
    private readonly InputAction m_SpaceshipControls_Roll;
    public struct SpaceshipControlsActions
    {
        private @SpaceshipInputAction m_Wrapper;
        public SpaceshipControlsActions(@SpaceshipInputAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @Strafe => m_Wrapper.m_SpaceshipControls_Strafe;
        public InputAction @Steer => m_Wrapper.m_SpaceshipControls_Steer;
        public InputAction @Boost => m_Wrapper.m_SpaceshipControls_Boost;
        public InputAction @Throttle => m_Wrapper.m_SpaceshipControls_Throttle;
        public InputAction @Roll => m_Wrapper.m_SpaceshipControls_Roll;
        public InputActionMap Get() { return m_Wrapper.m_SpaceshipControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SpaceshipControlsActions set) { return set.Get(); }
        public void SetCallbacks(ISpaceshipControlsActions instance)
        {
            if (m_Wrapper.m_SpaceshipControlsActionsCallbackInterface != null)
            {
                @Strafe.started -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnStrafe;
                @Strafe.performed -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnStrafe;
                @Strafe.canceled -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnStrafe;
                @Steer.started -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnSteer;
                @Steer.performed -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnSteer;
                @Steer.canceled -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnSteer;
                @Boost.started -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnBoost;
                @Boost.performed -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnBoost;
                @Boost.canceled -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnBoost;
                @Throttle.started -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnThrottle;
                @Throttle.performed -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnThrottle;
                @Throttle.canceled -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnThrottle;
                @Roll.started -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnRoll;
                @Roll.performed -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnRoll;
                @Roll.canceled -= m_Wrapper.m_SpaceshipControlsActionsCallbackInterface.OnRoll;
            }
            m_Wrapper.m_SpaceshipControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Strafe.started += instance.OnStrafe;
                @Strafe.performed += instance.OnStrafe;
                @Strafe.canceled += instance.OnStrafe;
                @Steer.started += instance.OnSteer;
                @Steer.performed += instance.OnSteer;
                @Steer.canceled += instance.OnSteer;
                @Boost.started += instance.OnBoost;
                @Boost.performed += instance.OnBoost;
                @Boost.canceled += instance.OnBoost;
                @Throttle.started += instance.OnThrottle;
                @Throttle.performed += instance.OnThrottle;
                @Throttle.canceled += instance.OnThrottle;
                @Roll.started += instance.OnRoll;
                @Roll.performed += instance.OnRoll;
                @Roll.canceled += instance.OnRoll;
            }
        }
    }
    public SpaceshipControlsActions @SpaceshipControls => new SpaceshipControlsActions(this);
    public interface ISpaceshipControlsActions
    {
        void OnStrafe(InputAction.CallbackContext context);
        void OnSteer(InputAction.CallbackContext context);
        void OnBoost(InputAction.CallbackContext context);
        void OnThrottle(InputAction.CallbackContext context);
        void OnRoll(InputAction.CallbackContext context);
    }
}
