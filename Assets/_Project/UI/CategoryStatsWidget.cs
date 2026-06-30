namespace Project.UI
{
    using Project.Core;
    using Project.Data;
    using RTLTMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class CategoryStatsWidget : MonoBehaviour
    {
        [SerializeField] private ChildProgressData _childProgressData;

        private Image _eduBar;
        private Image _pedBar;
        private Image _entBar;
        private RTLTextMeshPro _eduLabel;
        private RTLTextMeshPro _pedLabel;
        private RTLTextMeshPro _entLabel;

        private void Awake()
        {
            FindComponents();
        }

        private void OnEnable()
        {
            Refresh();
            if (_childProgressData != null)
                _childProgressData.OnGameRecorded += OnGameRecorded;
        }

        private void OnDisable()
        {
            if (_childProgressData != null)
                _childProgressData.OnGameRecorded -= OnGameRecorded;
        }

        private void OnGameRecorded(GameRecord _) { Refresh(); }

        private void FindComponents()
        {
            Image[] images = GetComponentsInChildren<Image>(true);
            RTLTextMeshPro[] texts = GetComponentsInChildren<RTLTextMeshPro>(true);

            for (int i = 0; i < images.Length; i++)
            {
                switch (images[i].name)
                {
                    case "EduBar": _eduBar = images[i]; break;
                    case "PedBar": _pedBar = images[i]; break;
                    case "EntBar": _entBar = images[i]; break;
                }
            }

            for (int i = 0; i < texts.Length; i++)
            {
                switch (texts[i].name)
                {
                    case "EduLabel": _eduLabel = texts[i]; break;
                    case "PedLabel": _pedLabel = texts[i]; break;
                    case "EntLabel": _entLabel = texts[i]; break;
                }
            }
        }

        public void Refresh()
        {
            if (_childProgressData == null) return;

            CategoryStatsReport report = _childProgressData.GetCategoryStats();
            float maxScore = Mathf.Max(report.educational.totalScore, report.pedagogical.totalScore, report.entertainment.totalScore, 1f);

            SetBar(_eduBar, _eduLabel, "Edu", report.educational, maxScore);
            SetBar(_pedBar, _pedLabel, "Ped", report.pedagogical, maxScore);
            SetBar(_entBar, _entLabel, "Ent", report.entertainment, maxScore);
        }

        private void SetBar(Image bar, RTLTextMeshPro label, string prefix, CategoryStatsReport.CategoryEntry entry, float maxScore)
        {
            if (bar != null)
                bar.fillAmount = maxScore > 0f ? entry.totalScore / maxScore : 0f;

            if (label != null)
            {
                label.text = $"{T($"cat_{prefix.ToLowerInvariant()}_name", prefix)}\n" +
                             $"{T("cat_games", "Games")}: {entry.totalGames}  " +
                             $"{T("cat_avg", "Avg")}: {entry.AverageScore:F1}";
            }
        }

        private string T(string key, string fallback)
        {
            return LocalizationManager.Instance != null ? LocalizationManager.Instance.GetText(key) : fallback;
        }
    }
}
