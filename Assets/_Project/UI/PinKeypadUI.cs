namespace Project.UI
{
    using System;
    using System.Text;
    using RTLTMPro;
    using UnityEngine;

    public class PinKeypadUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private RTLTextMeshPro _pinDisplayText;

        private StringBuilder _currentInput = new StringBuilder();
        private const int MaxPinLength = 4;

        public event Action<string> OnPinSubmitted;
        public event Action OnInputChanged;

        private void OnEnable()
        {
            ResetInput();
        }

        public void OnDigitPressed(int digit)
        {
            if (_currentInput.Length >= MaxPinLength) return;

            _currentInput.Append(digit);
            UpdatePinDisplay();
            OnInputChanged?.Invoke();

            if (_currentInput.Length == MaxPinLength)
            {
                OnPinSubmitted?.Invoke(_currentInput.ToString());
            }
        }

        public void OnClearPressed()
        {
            ResetInput();
            OnInputChanged?.Invoke();
        }

        public void OnBackspacePressed()
        {
            if (_currentInput.Length > 0)
            {
                _currentInput.Length--;
                UpdatePinDisplay();
                OnInputChanged?.Invoke();
            }
        }

        public void ResetInput()
        {
            _currentInput.Clear();
            UpdatePinDisplay();
        }

        private void UpdatePinDisplay()
        {
            if (_pinDisplayText != null)
            {
                _pinDisplayText.text = new string('•', _currentInput.Length);
            }
        }
    }
}
