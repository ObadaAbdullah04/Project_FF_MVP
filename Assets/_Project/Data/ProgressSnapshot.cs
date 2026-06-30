namespace Project.Data
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class ProgressSnapshot
    {
        public List<GameRecord> gameHistory = new List<GameRecord>();
        public List<SessionRecord> sessionHistory = new List<SessionRecord>();

        public static ProgressSnapshot FromProgress(ChildProgressData data)
        {
            var snapshot = new ProgressSnapshot();
            snapshot.gameHistory.AddRange(data.GameHistory);
            snapshot.sessionHistory.AddRange(data.SessionHistory);
            return snapshot;
        }

        public void ApplyTo(ChildProgressData data)
        {
            data.ResetData();
            for (int i = 0; i < gameHistory.Count; i++)
                data.RecordGame(gameHistory[i]);
            for (int i = 0; i < sessionHistory.Count; i++)
                data.RecordSession(sessionHistory[i]);
        }
    }
}
