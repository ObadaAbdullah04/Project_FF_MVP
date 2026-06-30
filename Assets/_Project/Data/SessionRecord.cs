namespace Project.Data
{
    using System;

    [Serializable]
    public struct SessionRecord
    {
        public string date;
        public float totalDuration;
        public int gamesPlayed;
    }
}
