namespace Project.Data
{
    using System;

    [Serializable]
    public struct ChildInsightReport
    {
        public string engagementKey;
        public string dominantStyleKey;
        public string independenceKey;
        public string confidenceKey;
        public bool hasData;
    }
}
