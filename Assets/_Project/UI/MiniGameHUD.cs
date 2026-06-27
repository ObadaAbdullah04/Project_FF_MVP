namespace Project.UI
{
    using RTLTMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class MiniGameHUD : MonoBehaviour
    {
        [SerializeField] private RTLTextMeshPro _scoreText;
        [SerializeField] private RTLTextMeshPro _timerText;
        [SerializeField] private Button _pauseButton;

        public void UpdateScore(int score)
        {
            if (_scoreText != null)
            {
                string label = Core.LocalizationManager.Instance != null ? Core.LocalizationManager.Instance.GetText("hud_score") : "Score:";
                _scoreText.text = $"{label} {score}";
            }
        }

        public void UpdateTimer(float time)
        {
            if (_timerText != null)
            {
                string label = Core.LocalizationManager.Instance != null ? Core.LocalizationManager.Instance.GetText("hud_time") : "Time:";
                _timerText.text = $"{label} {Mathf.CeilToInt(time)}";
            }
        }

        private void OnEnable()
        {
            if (_pauseButton != null)
            {
                _pauseButton.onClick.AddListener(OnPauseClicked);
            }
        }

        private void OnDisable()
        {
            if (_pauseButton != null)
            {
                _pauseButton.onClick.RemoveListener(OnPauseClicked);
            }
        }

        private void OnPauseClicked()
        {
        }
    }
}
