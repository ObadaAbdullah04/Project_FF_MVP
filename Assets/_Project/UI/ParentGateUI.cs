namespace Project.UI
{
    using Project.Core;
    using UnityEngine;

    public class ParentGateUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _pinEntryPanel;

        [Header("Validation")]
        [SerializeField] private PinValidationView _pinValidation;

        private void OnEnable()
        {
            if (_pinValidation != null)
                _pinValidation.OnPinValidated += HandlePinValidated;
        }

        private void OnDisable()
        {
            if (_pinValidation != null)
                _pinValidation.OnPinValidated -= HandlePinValidated;
        }

        private void Start()
        {
            if (DeviceRoleManager.Instance == null) return;

            // Always show PIN entry
            if (_pinEntryPanel != null)
                _pinEntryPanel.SetActive(true);

            if (_pinValidation != null)
            {
                _pinValidation.ResetInput();
                _pinValidation.HideError();
            }
        }

        private void HandlePinValidated()
        {
            if (DeviceRoleManager.Instance == null) return;

            // First-time parent setup via RoleSelection -> ParentGate
            if (DeviceRoleManager.Instance.GetRole() == DeviceRole.Unassigned)
                DeviceRoleManager.Instance.SetRole(DeviceRole.Parent);

            MenuManager.Instance.ShowParentDashboard();
        }

        public void OnCancelPressed()
        {
            if (DeviceRoleManager.Instance != null && DeviceRoleManager.Instance.GetRole() == DeviceRole.Unassigned)
            {
                // First-time flow: go back to role selection
                DeviceRoleManager.Instance.ResetAll();
                MenuManager.Instance.ShowRoleSelection();
            }
            else
            {
                // Came from Hub or SessionLock — just go back by hiding UI
                MenuManager.Instance.HideAll();
            }
        }
    }
}
