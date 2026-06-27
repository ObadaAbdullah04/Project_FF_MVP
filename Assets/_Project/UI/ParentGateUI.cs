namespace Project.UI
{
    using Project.Core;
    using Project.Data;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class ParentGateUI : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameSceneConfig _sceneConfig;

        [Header("Panels")]
        [SerializeField] private GameObject _roleSelectionPanel;
        [SerializeField] private GameObject _pinEntryPanel;

        [Header("Validation")]
        [SerializeField] private PinValidationView _pinValidation;

        private bool _isConfiguringParentOnboarding;

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
            InitializeFlow();
        }

        private void InitializeFlow()
        {
            if (DeviceRoleManager.Instance == null || _sceneConfig == null) return;

            if (IsHubWorldLoaded())
            {
                _roleSelectionPanel.SetActive(false);
                _pinEntryPanel.SetActive(true);
                if (_pinValidation != null) _pinValidation.ResetInput();
            }
            else
            {
                DeviceRole role = DeviceRoleManager.Instance.GetRole();
                switch (role)
                {
                    case DeviceRole.Parent:
                        SceneLoader.Instance.LoadSceneAdditively(_sceneConfig.ParentDashboardSceneName, () =>
                        {
                            SceneLoader.Instance.UnloadScene(_sceneConfig.ParentGateSceneName, null);
                        });
                        break;

                    case DeviceRole.Child:
                        _roleSelectionPanel.SetActive(false);
                        _pinEntryPanel.SetActive(true);
                        if (_pinValidation != null) _pinValidation.ResetInput();
                        break;

                    case DeviceRole.Unassigned:
                    default:
                        _roleSelectionPanel.SetActive(true);
                        _pinEntryPanel.SetActive(false);
                        break;
                }
            }
        }

        public void SelectParentRole()
        {
            _isConfiguringParentOnboarding = true;
            _roleSelectionPanel.SetActive(false);
            _pinEntryPanel.SetActive(true);
            if (_pinValidation != null)
            {
                _pinValidation.ResetInput();
                _pinValidation.HideError();
            }
        }

        public void SelectChildRole()
        {
            _isConfiguringParentOnboarding = false;
            if (DeviceRoleManager.Instance == null || _sceneConfig == null) return;
            DeviceRoleManager.Instance.SetRole(DeviceRole.Child);

            SceneLoader.Instance.LoadSceneAdditively(_sceneConfig.HubSceneName, () =>
            {
                SceneLoader.Instance.UnloadScene(_sceneConfig.ParentGateSceneName, null);
            });
        }

        private void HandlePinValidated()
        {
            if (DeviceRoleManager.Instance == null || _sceneConfig == null) return;

            if (IsHubWorldLoaded())
            {
                SceneLoader.Instance.LoadSceneAdditively(_sceneConfig.ParentDashboardSceneName, () =>
                {
                    SceneLoader.Instance.UnloadScene(_sceneConfig.ParentGateSceneName, null);
                });
            }
            else if (_isConfiguringParentOnboarding)
            {
                DeviceRoleManager.Instance.SetRole(DeviceRole.Parent);
                SceneLoader.Instance.LoadSceneAdditively(_sceneConfig.ParentDashboardSceneName, () =>
                {
                    SceneLoader.Instance.UnloadScene(_sceneConfig.ParentGateSceneName, null);
                });
            }
            else
            {
                SceneLoader.Instance.LoadSceneAdditively(_sceneConfig.HubSceneName, () =>
                {
                    SceneLoader.Instance.UnloadScene(_sceneConfig.ParentGateSceneName, null);
                });
            }
        }

        public void OnCancelPressed()
        {
            _isConfiguringParentOnboarding = false;

            if (IsHubWorldLoaded())
            {
                SceneLoader.Instance.UnloadScene(_sceneConfig.ParentGateSceneName, null);
            }
            else
            {
                DeviceRole savedRole = DeviceRoleManager.Instance != null
                    ? DeviceRoleManager.Instance.GetRole()
                    : DeviceRole.Unassigned;

                if (savedRole == DeviceRole.Unassigned)
                {
                    _roleSelectionPanel.SetActive(true);
                    _pinEntryPanel.SetActive(false);
                }
                else
                {
                    if (_pinValidation != null) _pinValidation.ResetInput();
                }
            }
        }

        private bool IsHubWorldLoaded()
        {
            if (_sceneConfig == null) return false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == _sceneConfig.HubSceneName)
                    return true;
            }
            return false;
        }
    }
}
