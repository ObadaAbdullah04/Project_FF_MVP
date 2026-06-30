namespace Project.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using Project.Core;
    using Project.Data;
    using RTLTMPro;

    public class ParentDashboardUI : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameSceneConfig _sceneConfig;

        [Header("UI Components")]
        [SerializeField] private RTLTextMeshPro _titleText;
        [SerializeField] private GameObject _backButton;

        [Header("Controls")]
        [SerializeField] private GameObject _playModeButton;

        [Header("Widgets")]
        [SerializeField] private CategoryStatsWidget _categoryStatsWidget;
        [SerializeField] private SessionHistoryWidget _sessionHistoryWidget;
        [SerializeField] private StrengthWeaknessIndicator _strengthWeaknessIndicator;
        [SerializeField] private ParentControlsPanel _parentControlsPanel;

        [Header("Insights")]
        [SerializeField] private RTLTextMeshPro _insightText;

        [Header("Data References")]
        [SerializeField] private ChildProgressData _childProgressData;

        private void Start()
        {
            if (_playModeButton != null)
            {
                Button playBtn = _playModeButton.GetComponent<Button>();
                if (playBtn != null)
                {
                    playBtn.onClick.RemoveAllListeners();
                    playBtn.onClick.AddListener(SwitchToPlayerMode);
                }
            }

            if (_backButton != null)
            {
                Button backBtn = _backButton.GetComponent<Button>();
                if (backBtn != null)
                {
                    backBtn.onClick.RemoveAllListeners();
                    backBtn.onClick.AddListener(OnBackToGamePressed);
                }
            }
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void SetButtonText(GameObject btnObj, string text)
        {
            if (btnObj == null) return;
            RTLTextMeshPro tmp = btnObj.GetComponentInChildren<RTLTextMeshPro>(true);
            if (tmp != null) tmp.text = text;
        }

        private string T(string key, string fallback)
        {
            return LocalizationManager.Instance != null ? LocalizationManager.Instance.GetText(key) : fallback;
        }

        public void Refresh()
        {
            if (DeviceRoleManager.Instance == null) return;

            DeviceRole role = DeviceRoleManager.Instance.GetRole();
            bool isParentMode = role == DeviceRole.Parent;

            if (_titleText != null)
                _titleText.text = isParentMode
                    ? T("dashboard_title_parent", "Parent Dashboard")
                    : T("dashboard_title_settings", "Settings");

            if (_backButton != null)
            {
                _backButton.SetActive(!isParentMode);
                SetButtonText(_backButton, T("dashboard_btn_back", "< Back"));
            }

            if (_playModeButton != null)
            {
                _playModeButton.SetActive(isParentMode);
                SetButtonText(_playModeButton, T("dashboard_btn_play_mode", "Play Mode"));
            }

            if (_categoryStatsWidget != null)
                _categoryStatsWidget.Refresh();

            if (_sessionHistoryWidget != null)
                _sessionHistoryWidget.Refresh();

            if (_strengthWeaknessIndicator != null)
                _strengthWeaknessIndicator.Refresh();

            if (_parentControlsPanel != null)
                _parentControlsPanel.Refresh();

            RefreshInsights();
        }

        private void RefreshInsights()
        {
            if (_insightText == null || _childProgressData == null) return;

            LocalizeSectionHeader();

            ChildInsightReport report = ChildInsightAnalyzer.Analyze(_childProgressData);

            if (!report.hasData)
            {
                _insightText.text = T("insight_no_data", "Play more games to see insights!");
                return;
            }

            _insightText.text = string.Format("{0}\n{1}\n{2}\n{3}",
                FormatInsight("insight_label_engagement", report.engagementKey),
                FormatInsight("insight_label_style", report.dominantStyleKey),
                FormatInsight("insight_label_independence", report.independenceKey),
                FormatInsight("insight_label_confidence", report.confidenceKey));
        }

        private void LocalizeSectionHeader()
        {
            if (_insightText == null) return;
            Transform section = _insightText.transform.parent;
            if (section == null) return;
            Transform header = section.Find("SectionHeader");
            if (header == null) return;
            RTLTextMeshPro tmp = header.GetComponent<RTLTextMeshPro>();
            if (tmp == null) return;
            tmp.text = T("insight_section_header", "Progress Overview");
        }

        private string FormatInsight(string labelKey, string valueKey)
        {
            string label = T(labelKey, labelKey);
            string value = T(valueKey, valueKey);
            return string.Format("{0}: {1}", label, value);
        }

        public void SwitchToPlayerMode()
        {
            if (DeviceRoleManager.Instance == null || _sceneConfig == null) return;

            DeviceRoleManager.Instance.SetRole(DeviceRole.Child);

            if (SessionTimer.Instance != null)
            {
                SessionTimer.Instance.StartSession();
            }

            MenuManager.Instance.HideAll();

            // Make sure Hub is loaded, otherwise transition to it
            bool hubLoaded = false;
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                if (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name == _sceneConfig.HubSceneName)
                {
                    hubLoaded = true;
                    break;
                }
            }

            if (!hubLoaded)
            {
                SceneLoader.Instance.TransitionToScene(_sceneConfig.HubSceneName, null);
            }
        }

        public void OnBackToGamePressed()
        {
            MenuManager.Instance.HideAll();
        }

        public void ResetChildData()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetAllData();
            }
            else
            {
                Debug.LogWarning("Cannot reset data: GameManager is missing.");
            }
            Refresh();
        }
    }
}
