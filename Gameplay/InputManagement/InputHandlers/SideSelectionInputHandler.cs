using System;
using UltimateFootballSystem.Gameplay.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UltimateFootballSystem.Gameplay.InputManagement
{
    public class SideSelectionInputHandler : IInputHandler
    {
        private MainInputActions.SideSelectionActions _sideSelectionActions;
        // private Gamer _gamer;
        
        // Public events for external subscription
        public event Action OnMoveLeft;
        public event Action OnMoveRight;
        public event Action OnSubmitSelection;
        public event Action OnCancelSelection;
        // public event Action<InputDevice> OnChangeInputDevice;
        public event Action OnChangeInputDevice;
        public event Action OnConfigure;

        public SideSelectionInputHandler(MainInputActions.SideSelectionActions actions)
        {
            _sideSelectionActions = actions;
            // _gamer = gamer;
            RegisterInputActions();
        }

        public void RegisterInputActions()
        {
            // Register SwitchSide action listeners
            if (_sideSelectionActions.SwitchSide != null)
            {
                // Register SwitchSide action listeners
                _sideSelectionActions.SwitchSide.started += OnSwitchSideStarted;
                _sideSelectionActions.SwitchSide.performed += OnSwitchSidePerformed;
                _sideSelectionActions.SwitchSide.canceled += OnSwitchSideCanceled;
                
                // Register ChangeInputDevice action listeners
                _sideSelectionActions.ChangeInputDevice.started += (ctx) => Debug.Log("ChangeInputDevice STARTED");
                _sideSelectionActions.ChangeInputDevice.performed += OnChangeInputDevicePerformed;
                _sideSelectionActions.ChangeInputDevice.canceled += (ctx) => Debug.Log("ChangeInputDevice CANCELED");
                    
                // Register Configure action listeners
                _sideSelectionActions.Configure.started += (ctx) => Debug.Log("Configure STARTED");
                _sideSelectionActions.Configure.performed += OnConfigurePerformed;
                _sideSelectionActions.Configure.canceled += (ctx) => Debug.Log("Configure CANCELED");
                
                // Register Submit action listeners
                _sideSelectionActions.SubmitSelection.started += (ctx) => Debug.Log("SubmitSelection STARTED");
                _sideSelectionActions.SubmitSelection.performed += OnSubmitSelectionPerformed;
                _sideSelectionActions.SubmitSelection.canceled += (ctx) => Debug.Log("SubmitSelection CANCELED");
                
                // Register Cancel action listeners
                _sideSelectionActions.CancelSelection.started += (ctx) => Debug.Log("Cancel STARTED");
                _sideSelectionActions.CancelSelection.performed += OnCancelSelectionPerformed;
                _sideSelectionActions.CancelSelection.canceled += (ctx) => Debug.Log("Cancel CANCELED");

                Debug.Log("[SideSelectionInputHandler] Input actions registered successfully");
            }
            else
            {
                Debug.LogWarning("[SideSelectionInputHandler] SwitchSide action is null");
            }

            // Register other actions if they exist
            // TODO: Add ChangeInputDevice, Configure, Select/Cancel actions when they're configured in MainInputActions
        }

        private void OnSwitchSideStarted(InputAction.CallbackContext context)
        {
            Debug.Log($"[SideSelectionInputHandler] SwitchSide STARTED - Control: {context.control.displayName}, Device: {context.control.device.displayName}");
        }

        #region Actions
        private void OnSwitchSidePerformed(InputAction.CallbackContext context)
        {
            // Read as Vector2 (action should be configured with Vector2 control type)
            Vector2 inputVector = context.ReadValue<Vector2>();
            float horizontalValue = inputVector.x;

            Debug.Log($"[SideSelectionInputHandler] Vector2 input - X: {horizontalValue}, Y: {inputVector.y}");

            // Process horizontal input with deadzone
            if (horizontalValue < -0.1f)
            {
                Debug.Log("[SideSelectionInputHandler] LEFT movement");
                OnMoveLeft?.Invoke();
            }
            else if (horizontalValue > 0.1f)
            {
                Debug.Log("[SideSelectionInputHandler] RIGHT movement");
                OnMoveRight?.Invoke();
            }
        }

        private void OnChangeInputDevicePerformed(InputAction.CallbackContext context)
        {
            // OnChangeInputDevice?.Invoke(context.control.device);
            OnChangeInputDevice?.Invoke();
            Debug.Log($"[SideSelectionInputHandler] ChangeInputDevice PERFORMED - Control: {context.control.displayName}, Device: {context.control.device.displayName}");
            // GamerManager.Instance.HandleChangeInputDevice(_gamer);
        }

        private void OnConfigurePerformed(InputAction.CallbackContext context) => OnConfigure?.Invoke();
        private void OnSubmitSelectionPerformed(InputAction.CallbackContext context) => OnSubmitSelection?.Invoke();
        private void OnCancelSelectionPerformed(InputAction.CallbackContext context) => OnCancelSelection?.Invoke();

        #endregion

        private void OnSwitchSideCanceled(InputAction.CallbackContext context)
        {
            Debug.Log($"[SideSelectionInputHandler] SwitchSide CANCELED - Control: {context.control.displayName}, Device: {context.control.device.displayName}");
        }

        public void Cleanup()
        {
            // Unregister actions to prevent memory leaks
            if (_sideSelectionActions.SwitchSide != null)
            {
                _sideSelectionActions.SwitchSide.started -= OnSwitchSideStarted;
                _sideSelectionActions.SwitchSide.performed -= OnSwitchSidePerformed;
                _sideSelectionActions.SwitchSide.canceled -= OnSwitchSideCanceled;
            }

            if (_sideSelectionActions.SubmitSelection != null)
            {
                // sideSelectionActions.SubmitSelection.started -= OnSubmitSelectionStarted;
                _sideSelectionActions.SubmitSelection.performed -= OnSubmitSelectionPerformed;
                // sideSelectionActions.SubmitSelection.canceled -= OnSubmitSelectionCanceled;
            }

            if (_sideSelectionActions.CancelSelection != null)
            {
                // sideSelectionActions.CancelSelection.started -= OnCancelSelectionStarted;
                _sideSelectionActions.CancelSelection.performed -= OnCancelSelectionPerformed;
                // sideSelectionActions.CancelSelection.canceled -= OnCancelSelectionCanceled;
            }

            if (_sideSelectionActions.ChangeInputDevice != null)
            {
                // sideSelectionActions.ChangeInputDevice.started -= OnChangeInputDeviceStarted;
                _sideSelectionActions.ChangeInputDevice.performed -= OnChangeInputDevicePerformed;
                // sideSelectionActions.ChangeInputDevice.canceled -= OnChangeInputDeviceCanceled;
            }

            if (_sideSelectionActions.Configure != null)
            {
                // sideSelectionActions.Configure.started -= OnConfigureStarted;
                _sideSelectionActions.Configure.performed -= OnConfigurePerformed;
                // sideSelectionActions.Configure.canceled -= OnConfigureCanceled;
            }

            Debug.Log("[SideSelectionInputHandler] Input actions unregistered");
        }
    }
}