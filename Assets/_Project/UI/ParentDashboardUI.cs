namespace Project.UI
{
    using UnityEngine;
    using Project.Core;
    using Project.Data;
    using RTLTMPro;

    /// <summary>
    /// Handles the user interface logic for the Parent Dashboard, which displays child
    /// gameplay progress statistics and allows configuring parent parameters (like changing PINs).
    /// </summary>
    public class ParentDashboardUI : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameSceneConfig _sceneConfig;

        [Header("UI Components")]
        [SerializeField] private RTLTextMeshPro _titleText;      // Dynamic screen title (changes based on Parent vs In-Game Settings)
        [SerializeField] private RTLTextMeshPro _pinStatusText;  // Text displaying the current active parent PIN code
        [SerializeField] private GameObject _backButton;         // Back to game button (only active when accessed from in-game settings)

        [Header("Child Progress Data")]
        [SerializeField] private InventoryData _inventoryData;  // Reference to ScriptableObject holding currency and unlocks
        [SerializeField] private RTLTextMeshPro _coinsText;      // Text showing soft currency balance (Arabic formatted)
        [SerializeField] private RTLTextMeshPro _buildingsText;  // Text showing number of unlocked buildings (Arabic formatted)

        [Header("Controls")]
        [SerializeField] private GameObject _playModeButton;     // Button that transitions the device to Child Mode

        private void OnEnable()
        {
            if (_inventoryData != null)
            {
                _inventoryData.OnCurrencyChanged += OnProgressChanged;
                _inventoryData.OnBuildingUnlocked += OnProgressChanged;
            }
            UpdateView();
        }

        private void OnDisable()
        {
            if (_inventoryData != null)
            {
                _inventoryData.OnCurrencyChanged -= OnProgressChanged;
                _inventoryData.OnBuildingUnlocked -= OnProgressChanged;
            }
        }

        private void OnProgressChanged(int _) { UpdateView(); }
        private void OnProgressChanged(string _) { UpdateView(); }

        private string T(string key, string fallback)
        {
            return LocalizationManager.Instance != null ? LocalizationManager.Instance.GetText(key) : fallback;
        }

        /// <summary>
        /// Populates progress data and sets button visibility depending on the active device role.
        /// </summary>
        private void UpdateView()
        {
            if (DeviceRoleManager.Instance == null) return;

            DeviceRole role = DeviceRoleManager.Instance.GetRole();
            if (role == DeviceRole.Parent)
            {
                // Dedicated Parent Companion Mode:
                // Disable back-to-game button, but enable Play Mode switch button
                if (_titleText != null)
                {
                    _titleText.text = T("dashboard_title_parent", "لوحة تحكم ولي الأمر");
                }
                if (_backButton != null)
                {
                    _backButton.SetActive(false);
                }
                if (_playModeButton != null)
                {
                    _playModeButton.SetActive(true);
                }
            }
            else
            {
                // In-Game Settings Mode:
                // Enable back-to-game button, but disable Play Mode switch button (already playing!)
                if (_titleText != null)
                {
                    _titleText.text = T("dashboard_title_settings", "إعدادات اللعبة");
                }
                if (_backButton != null)
                {
                    _backButton.SetActive(true);
                }
                if (_playModeButton != null)
                {
                    _playModeButton.SetActive(false);
                }
            }

            // Display current active PIN
            if (_pinStatusText != null)
            {
                _pinStatusText.text = $"{T("dashboard_pin_status", "الرمز الحالي")}: {DeviceRoleManager.Instance.GetPIN()}";
            }

            // Display Child Inventory progress from the ScriptableObject
            if (_inventoryData != null)
            {
                if (_coinsText != null)
                {
                    _coinsText.text = $"{T("dashboard_coins_status", "النقود الحالية")}: {_inventoryData.SoftCurrency}";
                }
                if (_buildingsText != null)
                {
                    _buildingsText.text = $"{T("dashboard_buildings_status", "المباني المفتوحة")}: {_inventoryData.UnlockedBuildingIds.Count}";
                }
            }
        }

        /// <summary>
        /// Updates the parent PIN code and refreshes the display.
        /// </summary>
        public void ChangePIN(string newPIN)
        {
            if (DeviceRoleManager.Instance == null) return;
            DeviceRoleManager.Instance.SetPIN(newPIN);
            UpdateView();
        }

        /// <summary>
        /// Transitions the device role to Child mode and launches the Hub World.
        /// Called by the "Switch to Player Mode" button on the Dashboard.
        /// </summary>
        public void SwitchToPlayerMode()
        {
            if (DeviceRoleManager.Instance == null || _sceneConfig == null) return;
            
            // Set role to Child so that future boots go straight to the game
            DeviceRoleManager.Instance.SetRole(DeviceRole.Child);

            // Load Hub World additively and unload dashboard
            SceneLoader.Instance.LoadSceneAdditively(_sceneConfig.HubSceneName, () =>
            {
                SceneLoader.Instance.UnloadScene(_sceneConfig.ParentDashboardSceneName, null);
            });
        }

        /// <summary>
        /// Returns to the gameplay Hub without changing device roles.
        /// Called by the "Back to Game" button.
        /// </summary>
        public void OnBackToGamePressed()
        {
            if (_sceneConfig == null) return;
            SceneLoader.Instance.UnloadScene(_sceneConfig.ParentDashboardSceneName, null);
        }
    }
}
