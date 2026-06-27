namespace Project.UI
{
    using DG.Tweening;
    using Project.Core;
    using Project.MiniGames;
    using RTLTMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UniversalMiniGameHUD : MonoBehaviour
    {
        [Header("Score Display")]
        [SerializeField] private RTLTextMeshPro _scoreText;

        [Header("Timer Display")]
        [SerializeField] private RTLTextMeshPro _timerText;

        [Header("Pause")]
        [SerializeField] private Button _pauseButton;
        [SerializeField] private GameObject _pauseOverlay;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private RTLTextMeshPro _resumeButtonText;

        [Header("Summary")]
        [SerializeField] private GameObject _summaryOverlay;
        [SerializeField] private RTLTextMeshPro _summaryTitleText;
        [SerializeField] private RTLTextMeshPro _summaryCoinsText;
        [SerializeField] private Button _backToCityButton;
        [SerializeField] private RTLTextMeshPro _backToCityButtonText;

        [Header("Animation")]
        [SerializeField] private RectTransform _summaryPanel;
        [SerializeField] private float _animateInDuration = 0.3f;

        public void SetScoreText(RTLTextMeshPro text) => _scoreText = text;
        public void SetTimerText(RTLTextMeshPro text) => _timerText = text;
        public void SetPauseButton(Button button) => _pauseButton = button;
        public void SetPauseOverlay(GameObject overlay, Button resumeButton)
        {
            _pauseOverlay = overlay;
            _resumeButton = resumeButton;
        }
        public void SetResumeButtonText(RTLTextMeshPro text) => _resumeButtonText = text;
        public void SetSummaryTitleText(RTLTextMeshPro text) => _summaryTitleText = text;
        public void SetSummaryOverlay(GameObject overlay) => _summaryOverlay = overlay;
        public void SetSummaryCoinsText(RTLTextMeshPro text) => _summaryCoinsText = text;
        public void SetBackToCityButton(Button button) => _backToCityButton = button;
        public void SetBackToCityButtonText(RTLTextMeshPro text) => _backToCityButtonText = text;
        public void SetSummaryPanel(RectTransform panel) => _summaryPanel = panel;

        private BaseMiniGameManager _gameManager;
        private int _coinsEarned;
        private bool _isPaused;

        private string T(string key)
        {
            return LocalizationManager.Instance != null ? LocalizationManager.Instance.GetText(key) : key;
        }

        public void Initialize(BaseMiniGameManager manager)
        {
            _gameManager = manager;

            if (_pauseButton != null)
                _pauseButton.onClick.AddListener(TogglePause);
            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(TogglePause);
            if (_backToCityButton != null)
                _backToCityButton.onClick.AddListener(BackToCity);

            if (_resumeButtonText != null)
                _resumeButtonText.text = T("hud_pause");
            if (_backToCityButtonText != null)
                _backToCityButtonText.text = T("hud_back_to_city");

            if (_pauseOverlay != null)
                _pauseOverlay.SetActive(false);
            if (_summaryOverlay != null)
                _summaryOverlay.SetActive(false);
            if (_summaryPanel != null)
                _summaryPanel.localScale = Vector3.zero;
        }

        public void Cleanup()
        {
            if (_pauseButton != null)
                _pauseButton.onClick.RemoveListener(TogglePause);
            if (_resumeButton != null)
                _resumeButton.onClick.RemoveListener(TogglePause);
            if (_backToCityButton != null)
                _backToCityButton.onClick.RemoveListener(BackToCity);
        }

        public void UpdateScore(int score)
        {
            if (_scoreText != null)
                _scoreText.text = $"{T("hud_score")} {score}";
        }

        public void UpdateTimer(float time)
        {
            if (_timerText != null)
                _timerText.text = $"{T("hud_time")} {Mathf.CeilToInt(time)}";
        }

        public void ShowSummary(int score, int coinsEarned)
        {
            _coinsEarned = coinsEarned;

            if (_summaryOverlay != null)
                _summaryOverlay.SetActive(true);

            if (_summaryTitleText != null)
                _summaryTitleText.text = T("hud_game_over");
            if (_summaryCoinsText != null)
                _summaryCoinsText.text = $"{T("hud_coins")} {coinsEarned}";

            if (_pauseButton != null)
                _pauseButton.gameObject.SetActive(false);

            if (_summaryPanel != null)
            {
                _summaryPanel.localScale = Vector3.zero;
                _summaryPanel.DOScale(Vector3.one, _animateInDuration).SetEase(Ease.OutBack);
            }
        }

        public void TogglePause()
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;

            if (_pauseOverlay != null)
                _pauseOverlay.SetActive(_isPaused);
        }

        public void BackToCity()
        {
            Time.timeScale = 1f;
            if (_gameManager != null)
                _gameManager.CompleteGame(_coinsEarned);
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}
