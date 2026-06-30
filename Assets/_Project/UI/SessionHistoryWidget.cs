namespace Project.UI
{
    using System.Collections.Generic;
    using Project.Core;
    using Project.Data;
    using RTLTMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SessionHistoryWidget : MonoBehaviour
    {
        [SerializeField] private ChildProgressData _childProgressData;
        [SerializeField] private GameObject _entryPrefab;
        [SerializeField] private int _maxEntries = 20;

        private ScrollRect _scrollRect;
        private Transform _contentRoot;
        private readonly List<GameObject> _entries = new List<GameObject>();

        private void Awake()
        {
            FindComponents();
        }

        private void OnEnable()
        {
            Refresh();
            if (_childProgressData != null)
                _childProgressData.OnSessionRecorded += OnSessionRecorded;
        }

        private void OnDisable()
        {
            if (_childProgressData != null)
                _childProgressData.OnSessionRecorded -= OnSessionRecorded;
        }

        private void OnSessionRecorded(SessionRecord _) { Refresh(); }

        private void FindComponents()
        {
            _scrollRect = GetComponentInChildren<ScrollRect>(true);
            if (_scrollRect != null)
                _contentRoot = _scrollRect.content;
        }

        public void Refresh()
        {
            ClearEntries();
            if (_childProgressData == null || _contentRoot == null) return;

            IReadOnlyList<SessionRecord> history = _childProgressData.SessionHistory;
            int total = history.Count;
            int count = Mathf.Min(total, _maxEntries);

            for (int i = 0; i < count; i++)
            {
                SessionRecord session = history[total - 1 - i];
                GameObject entry;

                if (_entryPrefab != null)
                {
                    entry = Instantiate(_entryPrefab, _contentRoot);
                    entry.SetActive(true);
                }
                else
                {
                    entry = CreateEntryFallback();
                    entry.transform.SetParent(_contentRoot, false);
                }

                RTLTextMeshPro[] texts = entry.GetComponentsInChildren<RTLTextMeshPro>(true);
                for (int t = 0; t < texts.Length; t++)
                {
                    switch (texts[t].name)
                    {
                        case "DateLabel":
                            texts[t].text = session.date;
                            break;
                        case "DurationLabel":
                            texts[t].text = $"{T("session_duration", "Duration")}: {session.totalDuration / 60f:F0}{T("minutes_abbr", "m")}";
                            break;
                        case "GamesLabel":
                            texts[t].text = $"{T("session_games", "Games")}: {session.gamesPlayed}";
                            break;
                    }
                }

                _entries.Add(entry);
            }
        }

        private GameObject CreateEntryFallback()
        {
            var go = new GameObject("SessionEntry", typeof(RectTransform));
            return go;
        }

        private void ClearEntries()
        {
            for (int i = 0; i < _entries.Count; i++)
                Destroy(_entries[i]);
            _entries.Clear();
        }

        private string T(string key, string fallback)
        {
            return LocalizationManager.Instance != null ? LocalizationManager.Instance.GetText(key) : fallback;
        }
    }
}
