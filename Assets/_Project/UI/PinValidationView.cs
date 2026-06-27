namespace Project.UI
{
    using System;
    using Project.Core;
    using RTLTMPro;
    using UnityEngine;

    public class PinValidationView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private PinKeypadUI _keypad;
        [SerializeField] private RTLTextMeshPro _errorText;

        public event Action OnPinValidated;
        public event Action OnPinRejected;

        private void OnEnable()
        {
            if (_keypad != null)
            {
                _keypad.OnPinSubmitted += HandlePinSubmitted;
                _keypad.OnInputChanged += HandleInputChanged;
            }
        }

        private void OnDisable()
        {
            if (_keypad != null)
            {
                _keypad.OnPinSubmitted -= HandlePinSubmitted;
                _keypad.OnInputChanged -= HandleInputChanged;
            }
        }

        public void ResetInput()
        {
            if (_keypad != null) _keypad.ResetInput();
        }

        public void HideError()
        {
            if (_errorText != null)
                _errorText.gameObject.SetActive(false);
        }

        private void HandleInputChanged()
        {
            if (_errorText != null)
                _errorText.gameObject.SetActive(false);
        }

        private void HandlePinSubmitted(string enteredPin)
        {
            if (DeviceRoleManager.Instance != null && DeviceRoleManager.Instance.ValidatePIN(enteredPin))
            {
                HideError();
                OnPinValidated?.Invoke();
            }
            else
            {
                if (_errorText != null)
                {
                    _errorText.gameObject.SetActive(true);
                    _errorText.text = LocalizationManager.Instance != null
                        ? LocalizationManager.Instance.GetText("parent_gate_wrong_pin")
                        : "الرمز غير صحيح";
                }
                if (_keypad != null) _keypad.ResetInput();
                OnPinRejected?.Invoke();
            }
        }
    }
}
