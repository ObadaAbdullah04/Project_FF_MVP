namespace Project.UI
{
    using UnityEngine;
    using Project.Core;
    using RTLTMPro;
    using UnityEngine.SceneManagement;
    using System.Text;

    /// <summary>
    /// Handles the user interface logic for the Parental Gate, including role selection
    /// (first startup onboarding) and PIN keypad validation.
    /// </summary>
    public class ParentGateUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _roleSelectionPanel; // Onboarding choice panel (Parent vs Child)
        [SerializeField] private GameObject _pinEntryPanel;      // 4-digit PIN keypad entry panel

        [Header("PIN UI Components")]
        [SerializeField] private RTLTextMeshPro _pinDisplayText; // Text showing masked password entries (e.g. ••••)
        [SerializeField] private RTLTextMeshPro _errorText;      // Text showing verification errors (e.g. "Incorrect Code")

        [Header("Scene Names")]
        [SerializeField] private string _hubSceneName = "2_HubWorld";
        [SerializeField] private string _parentDashboardSceneName = "ParentDashboard";
        [SerializeField] private string _parentGateSceneName = "Parent Gate";

        private StringBuilder _currentInput = new StringBuilder();
        private const int MaxPinLength = 4;
        
        // Tracks if the parent is currently entering their PIN during first-time startup onboarding
        private bool _isConfiguringParentOnboarding = false;

        private void Start()
        {
            InitializeFlow();
        }

        /// <summary>
        /// Evaluates the starting scene configuration and device state to show the correct panel.
        /// </summary>
        private void InitializeFlow()
        {
            if (DeviceRoleManager.Instance == null) return;

            bool isHubLoaded = IsHubWorldLoaded();

            if (isHubLoaded)
            {
                // In-game Settings entry: Show PIN keypad immediately (user clicked Settings gear in Hub)
                _roleSelectionPanel.SetActive(false);
                _pinEntryPanel.SetActive(true);
                ResetPinInput();
            }
            else
            {
                // Startup entry: Check saved device role
                DeviceRole role = DeviceRoleManager.Instance.GetRole();
                switch (role)
                {
                    case DeviceRole.Parent:
                        // Dedicated Parent Companion Mode: Load dashboard directly
                        SceneLoader.Instance.LoadSceneAdditively(_parentDashboardSceneName, () =>
                        {
                            SceneLoader.Instance.UnloadScene(_parentGateSceneName, null);
                        });
                        break;

                    case DeviceRole.Child:
                        // Dedicated Child Mode: Show PIN pad immediately
                        _roleSelectionPanel.SetActive(false);
                        _pinEntryPanel.SetActive(true);
                        ResetPinInput();
                        break;

                    case DeviceRole.Unassigned:
                    default:
                        // Onboarding: Show Role Selection Panel
                        _roleSelectionPanel.SetActive(true);
                        _pinEntryPanel.SetActive(false);
                        break;
                }
            }
        }

        // --- Role Selection View ---

        /// <summary>
        /// Called when the "Parent" button is clicked on the onboarding panel.
        /// Initiates the parent PIN validation flow before setting the role.
        /// </summary>
        public void SelectParentRole()
        {
            _isConfiguringParentOnboarding = true;
            _roleSelectionPanel.SetActive(false);
            _pinEntryPanel.SetActive(true);
            ResetPinInput();
        }

        /// <summary>
        /// Called when the "Child" button is clicked on the onboarding panel.
        /// Saves Child role and loads the Hub World immediately.
        /// </summary>
        public void SelectChildRole()
        {
            _isConfiguringParentOnboarding = false;
            if (DeviceRoleManager.Instance == null) return;
            DeviceRoleManager.Instance.SetRole(DeviceRole.Child);
            
            // Go straight to the game
            SceneLoader.Instance.LoadSceneAdditively(_hubSceneName, () =>
            {
                SceneLoader.Instance.UnloadScene(_parentGateSceneName, null);
            });
        }

        // --- PIN Entry View ---

        /// <summary>
        /// Called by keypad digit buttons (0-9) to input numbers.
        /// </summary>
        public void OnDigitPressed(int digit)
        {
            if (_currentInput.Length >= MaxPinLength) return;

            _currentInput.Append(digit);
            UpdatePinDisplay();
            _errorText.gameObject.SetActive(false); // Hide previous error text

            // Automatically validate once the PIN reaches 4 digits
            if (_currentInput.Length == MaxPinLength)
            {
                SubmitPIN();
            }
        }

        /// <summary>
        /// Clears all entered digits (Btn_Clear / 'C').
        /// </summary>
        public void OnClearPressed()
        {
            ResetPinInput();
        }

        /// <summary>
        /// Deletes the last entered digit (Btn_Backspace / '<').
        /// </summary>
        public void OnBackspacePressed()
        {
            if (_currentInput.Length > 0)
            {
                _currentInput.Length--;
                UpdatePinDisplay();
                _errorText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Validates input code and routes the user to the correct scene.
        /// </summary>
        public void SubmitPIN()
        {
            if (DeviceRoleManager.Instance == null) return;

            string enteredPin = _currentInput.ToString();
            if (DeviceRoleManager.Instance.ValidatePIN(enteredPin))
            {
                bool isHubLoaded = IsHubWorldLoaded();

                if (isHubLoaded)
                {
                    // In-game Settings entry -> Open Parent Dashboard
                    SceneLoader.Instance.LoadSceneAdditively(_parentDashboardSceneName, () =>
                    {
                        SceneLoader.Instance.UnloadScene(_parentGateSceneName, null);
                    });
                }
                else
                {
                    if (_isConfiguringParentOnboarding)
                    {
                        // Startup onboarding entry -> Set role to Parent and open dashboard!
                        DeviceRoleManager.Instance.SetRole(DeviceRole.Parent);
                        SceneLoader.Instance.LoadSceneAdditively(_parentDashboardSceneName, () =>
                        {
                            SceneLoader.Instance.UnloadScene(_parentGateSceneName, null);
                        });
                    }
                    else
                    {
                        // Startup entry for saved Child role -> Just open child game hub and keep Child role!
                        SceneLoader.Instance.LoadSceneAdditively(_hubSceneName, () =>
                        {
                            SceneLoader.Instance.UnloadScene(_parentGateSceneName, null);
                        });
                    }
                }
            }
            else
            {
                // Invalid PIN entered: Show error feedback and reset entry buffer
                _errorText.gameObject.SetActive(true);
                if (LocalizationManager.Instance != null)
                {
                    _errorText.text = LocalizationManager.Instance.GetText("parent_gate_wrong_pin");
                }
                else
                {
                    _errorText.text = "الرمز غير صحيح";
                }
                ResetPinInput();
            }
        }

        private void UpdatePinDisplay()
        {
            if (_pinDisplayText != null)
            {
                // Masks the input digits with dot characters
                _pinDisplayText.text = new string('•', _currentInput.Length);
            }
        }

        private void ResetPinInput()
        {
            _currentInput.Clear();
            if (_pinDisplayText != null)
            {
                _pinDisplayText.text = "";
            }
        }

        /// <summary>
        /// Called when the "Cancel" button is clicked.
        /// </summary>
        public void OnCancelPressed()
        {
            _isConfiguringParentOnboarding = false;
            bool isHubLoaded = IsHubWorldLoaded();

            if (isHubLoaded)
            {
                // In-game settings cancel -> Just close parental gate to return to hub
                SceneLoader.Instance.UnloadScene(_parentGateSceneName, null);
            }
            else
            {
                // Startup cancel: only allow returning to role selection during initial unassigned onboarding
                DeviceRole savedRole = DeviceRoleManager.Instance != null ? DeviceRoleManager.Instance.GetRole() : DeviceRole.Unassigned;
                if (savedRole == DeviceRole.Unassigned)
                {
                    _roleSelectionPanel.SetActive(true);
                    _pinEntryPanel.SetActive(false);
                }
                else
                {
                    // Locked to child mode startup: Reset PIN input
                    ResetPinInput();
                }
            }
        }

        private bool IsHubWorldLoaded()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == _hubSceneName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
