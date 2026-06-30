namespace Project.UI
{
    using Project.Core;
    using RTLTMPro;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ParentControlsPanel : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Events.UnityEvent _onDataReset;
        [SerializeField] private Slider _timeLimitSlider;
        [SerializeField] private RTLTextMeshPro _timeLimitLabel;
        [SerializeField] private Button _under6Button;
        [SerializeField] private Button _ages6PlusButton;
        [SerializeField] private Button _resetDataButton;
        [SerializeField] private Button _changePinButton;
        [SerializeField] private RTLTextMeshPro _currentPinLabel;
        [SerializeField] private TMP_InputField _pinInput;

        private void Awake()
        {
            if (_timeLimitSlider == null)
                _timeLimitSlider = GetComponentInChildren<Slider>(true);
            if (_timeLimitLabel == null)
                _timeLimitLabel = GetComponentInChildren<RTLTextMeshPro>(true);
            if (_currentPinLabel == null)
                _currentPinLabel = GetComponentInChildren<RTLTextMeshPro>(true);
            if (_pinInput == null)
                _pinInput = GetComponentInChildren<TMP_InputField>(true);
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void Start()
        {
            WireButtons();
            WireSlider();
            WireDataReset();
        }

        private void WireDataReset()
        {
            _onDataReset.RemoveAllListeners();
            ParentDashboardUI dashboard = GetComponentInParent<ParentDashboardUI>();
            if (dashboard != null)
                _onDataReset.AddListener(dashboard.ResetChildData);
        }

        private void WireButtons()
        {
            if (_under6Button != null)
            {
                _under6Button.onClick.RemoveAllListeners();
                _under6Button.onClick.AddListener(() => SetAgeTier(AgeTier.Under6));
            }

            if (_ages6PlusButton != null)
            {
                _ages6PlusButton.onClick.RemoveAllListeners();
                _ages6PlusButton.onClick.AddListener(() => SetAgeTier(AgeTier.Ages6Plus));
            }

            if (_changePinButton != null)
            {
                _changePinButton.onClick.RemoveAllListeners();
                _changePinButton.onClick.AddListener(OnChangePinPressed);
            }

            if (_resetDataButton != null)
            {
                _resetDataButton.onClick.RemoveAllListeners();
                _resetDataButton.onClick.AddListener(OnResetData);
            }
        }

        private void OnChangePinPressed()
        {
            if (DeviceRoleManager.Instance == null || _pinInput == null) return;
            string newPin = _pinInput.text;
            if (string.IsNullOrEmpty(newPin) || newPin.Length != 4) return;
            DeviceRoleManager.Instance.SetPIN(newPin);
            _pinInput.text = "";
            Refresh();
        }

        private void WireSlider()
        {
            if (_timeLimitSlider == null) return;

            _timeLimitSlider.onValueChanged.RemoveAllListeners();
            _timeLimitSlider.minValue = 5f;
            _timeLimitSlider.maxValue = 120f;
            _timeLimitSlider.wholeNumbers = true;

            _timeLimitSlider.onValueChanged.AddListener((val) =>
            {
                int minutes = Mathf.RoundToInt(val);
                if (DeviceRoleManager.Instance != null)
                    DeviceRoleManager.Instance.SetDailyTimeLimitMinutes(minutes);
                Refresh();
            });
        }

        public void Refresh()
        {
            if (DeviceRoleManager.Instance == null) return;

            int currentLimit = DeviceRoleManager.Instance.GetDailyTimeLimitMinutes();
            AgeTier currentTier = DeviceRoleManager.Instance.GetAgeTier();
            string currentPin = DeviceRoleManager.Instance.GetPIN();

            if (_timeLimitSlider != null)
                _timeLimitSlider.SetValueWithoutNotify(currentLimit);

            if (_timeLimitLabel != null)
                _timeLimitLabel.text = $"{T("controls_time_limit", "Daily Limit")}: {currentLimit}{T("minutes_abbr", "m")}";

            if (_currentPinLabel != null)
                _currentPinLabel.text = $"{T("controls_current_pin", "Current PIN")}: {currentPin}";

            if (_under6Button != null)
            {
                SetButtonSelected(_under6Button, currentTier == AgeTier.Under6);
                SetButtonText(_under6Button, T("controls_btn_under6", "Under 6 (30m)"));
            }

            if (_ages6PlusButton != null)
            {
                SetButtonSelected(_ages6PlusButton, currentTier == AgeTier.Ages6Plus);
                SetButtonText(_ages6PlusButton, T("controls_btn_ages6plus", "Ages 6+ (60m)"));
            }

            SetButtonText(_resetDataButton, T("controls_btn_reset_data", "Reset Data"));
        }

        private void SetButtonSelected(Button btn, bool selected)
        {
            if (btn == null) return;
            Color baseColor = selected ? new Color(0.18f, 0.65f, 0.43f) : new Color(0.24f, 0.24f, 0.28f);

            Image img = btn.GetComponent<Image>();
            if (img != null)
            {
                img.color = baseColor;
            }

            ColorBlock cb = btn.colors;
            cb.normalColor = baseColor;
            cb.highlightedColor = baseColor * 1.15f;
            cb.pressedColor = baseColor * 0.85f;
            cb.selectedColor = baseColor;
            btn.colors = cb;
        }

        private void SetButtonText(Button btn, string text)
        {
            if (btn == null) return;
            RTLTextMeshPro tmp = btn.GetComponentInChildren<RTLTextMeshPro>(true);
            if (tmp != null) tmp.text = text;
        }

        private void SetAgeTier(AgeTier tier)
        {
            if (DeviceRoleManager.Instance == null) return;
            DeviceRoleManager.Instance.SetAgeTier(tier);
            DeviceRoleManager.Instance.SetDailyTimeLimitMinutes(
                DeviceRoleManager.Instance.GetDefaultTimeLimit());
            Refresh();
        }

        private string T(string key, string fallback)
        {
            return LocalizationManager.Instance != null ? LocalizationManager.Instance.GetText(key) : fallback;
        }

        private void OnResetData()
        {
            _onDataReset?.Invoke();
        }
    }
}
