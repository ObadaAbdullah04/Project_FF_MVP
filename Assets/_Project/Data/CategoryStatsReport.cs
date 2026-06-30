namespace Project.Data
{
    using System;

    [Serializable]
    public class CategoryStatsReport
    {
        public CategoryEntry educational = new CategoryEntry();
        public CategoryEntry pedagogical = new CategoryEntry();
        public CategoryEntry entertainment = new CategoryEntry();

        [Serializable]
        public class CategoryEntry
        {
            public int totalGames;
            public int totalScore;
            public float totalDecisionTime;
            public int gamesWithDecisionTime;

            public float AverageScore => totalGames > 0 ? (float)totalScore / totalGames : 0f;
            public float AverageDecisionTime => gamesWithDecisionTime > 0 ? totalDecisionTime / gamesWithDecisionTime : 0f;
        }
    }
}
