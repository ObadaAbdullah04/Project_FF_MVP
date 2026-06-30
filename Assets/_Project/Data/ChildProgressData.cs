namespace Project.Data
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ChildProgressData", menuName = "Project/Data/Child Progress Data")]
    public class ChildProgressData : ScriptableObject
    {
        [SerializeField] private List<GameRecord> _gameHistory = new List<GameRecord>();
        [SerializeField] private List<SessionRecord> _sessionHistory = new List<SessionRecord>();

        public IReadOnlyList<GameRecord> GameHistory => _gameHistory;
        public IReadOnlyList<SessionRecord> SessionHistory => _sessionHistory;

        public event Action<GameRecord> OnGameRecorded;
        public event Action<SessionRecord> OnSessionRecorded;

        public int GetBestScore(string gameId)
        {
            int best = 0;
            for (int i = 0; i < _gameHistory.Count; i++)
            {
                if (_gameHistory[i].gameId == gameId && _gameHistory[i].score > best)
                    best = _gameHistory[i].score;
            }
            return best;
        }

        public void RecordGame(GameRecord record)
        {
            _gameHistory.Add(record);
            OnGameRecorded?.Invoke(record);
        }

        public void RecordSession(SessionRecord record)
        {
            _sessionHistory.Add(record);
            OnSessionRecorded?.Invoke(record);
        }

        public CategoryStatsReport GetCategoryStats()
        {
            var report = new CategoryStatsReport();

            for (int i = 0; i < _gameHistory.Count; i++)
            {
                GameRecord g = _gameHistory[i];

                if (g.educationalWeight > 0f)
                {
                    report.educational.totalGames++;
                    report.educational.totalScore += g.score;
                    if (g.avgDecisionTime > 0f)
                    {
                        report.educational.totalDecisionTime += g.avgDecisionTime;
                        report.educational.gamesWithDecisionTime++;
                    }
                }

                if (g.pedagogicalWeight > 0f)
                {
                    report.pedagogical.totalGames++;
                    report.pedagogical.totalScore += g.score;
                    if (g.avgDecisionTime > 0f)
                    {
                        report.pedagogical.totalDecisionTime += g.avgDecisionTime;
                        report.pedagogical.gamesWithDecisionTime++;
                    }
                }

                if (g.entertainmentWeight > 0f)
                {
                    report.entertainment.totalGames++;
                    report.entertainment.totalScore += g.score;
                    if (g.avgDecisionTime > 0f)
                    {
                        report.entertainment.totalDecisionTime += g.avgDecisionTime;
                        report.entertainment.gamesWithDecisionTime++;
                    }
                }
            }

            return report;
        }

        public void ResetData()
        {
            _gameHistory.Clear();
            _sessionHistory.Clear();
        }
    }
}
