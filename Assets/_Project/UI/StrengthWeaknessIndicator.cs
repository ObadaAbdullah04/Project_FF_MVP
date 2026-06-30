namespace Project.UI
{
    using Project.Core;
    using Project.Data;
    using RTLTMPro;
    using UnityEngine;

    public class StrengthWeaknessIndicator : MonoBehaviour
    {
        [SerializeField] private ChildProgressData _childProgressData;
        [SerializeField] private RTLTextMeshPro _indicatorText;

        private void Awake()
        {
            if (_indicatorText == null)
                _indicatorText = GetComponentInChildren<RTLTextMeshPro>(true);
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

        public void Refresh()
        {
            if (_childProgressData == null || _indicatorText == null) return;

            CategoryStatsReport report = _childProgressData.GetCategoryStats();
            var entries = new (string name, string nameKey, float avg)[]
            {
                (T("cat_edu_name", "Educational"), "cat_edu_name", report.educational.AverageScore),
                (T("cat_ped_name", "Pedagogical"), "cat_ped_name", report.pedagogical.AverageScore),
                (T("cat_ent_name", "Entertainment"), "cat_ent_name", report.entertainment.AverageScore),
            };

            System.Array.Sort(entries, (a, b) => b.avg.CompareTo(a.avg));

            if (entries[0].avg <= 0f)
            {
                _indicatorText.text = T("sw_no_data", "Play some games to see your progress!");
                return;
            }

            string strengthName = entries[0].name;
            string weaknessName = entries[2].name;
            float strengthAvg = entries[0].avg;
            float weaknessAvg = entries[2].avg;

            string format = T("sw_format", "{0} (avg {1:F0}) — {2} needs practice (avg {3:F0})");
            _indicatorText.text = string.Format(format, strengthName, strengthAvg, weaknessName, weaknessAvg);
        }

        private string T(string key, string fallback)
        {
            return LocalizationManager.Instance != null ? LocalizationManager.Instance.GetText(key) : fallback;
        }
    }
}
