namespace Project.Data
{
    using System;

    [Serializable]
    public struct GameRecord
    {
        public string gameId;
        public string gameNameKey;
        public float educationalWeight;
        public float pedagogicalWeight;
        public float entertainmentWeight;
        public int score;
        public int currencyEarned;
        public float avgDecisionTime;
        public float duration;
        public string timestamp;
    }
}
