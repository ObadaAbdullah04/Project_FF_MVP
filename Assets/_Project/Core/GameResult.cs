namespace Project.Core
{
    using System;

    [Serializable]
    public struct GameResult
    {
        public int score;
        public int bestScore;
        public int currencyEarned;
        public float avgDecisionTime;
        public float duration;
        public float[] decisionTimes;

    }
}
