namespace Project.Core
{
    using System.Collections.Generic;
    using Project.Data;
    using UnityEngine;

    public static class ChildInsightAnalyzer
    {
        private const int RecentGameCount = 10;

        public static ChildInsightReport Analyze(ChildProgressData data)
        {
            var report = new ChildInsightReport { hasData = false };

            if (data == null)
                return report;

            IReadOnlyList<GameRecord> games = data.GameHistory;
            IReadOnlyList<SessionRecord> sessions = data.SessionHistory;
            int gameCount = games.Count;

            if (gameCount == 0 && sessions.Count == 0)
                return report;

            report.hasData = true;
            report.engagementKey = GetEngagementKey(sessions);
            report.dominantStyleKey = GetDominantStyleKey(data.GetCategoryStats());
            report.independenceKey = GetIndependenceKey(games, gameCount);
            report.confidenceKey = GetConfidenceKey(games, gameCount);
            return report;
        }

        private static string GetEngagementKey(IReadOnlyList<SessionRecord> sessions)
        {
            float totalSeconds = 0f;
            for (int i = 0; i < sessions.Count; i++)
                totalSeconds += sessions[i].totalDuration;

            float totalMinutes = totalSeconds / 60f;

            if (totalMinutes < 30f)
                return "insight_engagement_low";
            if (totalMinutes < 120f)
                return "insight_engagement_medium";
            return "insight_engagement_high";
        }

        private static string GetDominantStyleKey(CategoryStatsReport report)
        {
            float eduScore = report.educational.totalScore;
            float pedScore = report.pedagogical.totalScore;
            float entScore = report.entertainment.totalScore;

            if (eduScore >= pedScore && eduScore >= entScore)
                return "style_educational";
            if (pedScore >= eduScore && pedScore >= entScore)
                return "style_pedagogical";
            return "style_entertainment";
        }

        private static string GetIndependenceKey(IReadOnlyList<GameRecord> games, int gameCount)
        {
            if (gameCount < 2)
                return "insight_developing";

            int start = Mathf.Max(0, gameCount - RecentGameCount);
            int windowCount = gameCount - start;
            int midpoint = start + windowCount / 2;

            float olderTime = 0f, newerTime = 0f;
            float olderScore = 0f, newerScore = 0f;
            int olderCount = 0, newerCount = 0;

            for (int i = start; i < gameCount; i++)
            {
                if (i < midpoint)
                {
                    olderTime += games[i].avgDecisionTime;
                    olderScore += games[i].score;
                    olderCount++;
                }
                else
                {
                    newerTime += games[i].avgDecisionTime;
                    newerScore += games[i].score;
                    newerCount++;
                }
            }

            olderTime /= olderCount;
            newerTime /= newerCount;
            olderScore /= olderCount;
            newerScore /= newerCount;

            bool improvingTime = newerTime < olderTime;
            bool improvingScore = newerScore >= olderScore;

            if (improvingTime && improvingScore)
                return "insight_independent";
            if (improvingTime || improvingScore)
                return "insight_guided";
            return "insight_assisted";
        }

        private static string GetConfidenceKey(IReadOnlyList<GameRecord> games, int gameCount)
        {
            int start = Mathf.Max(0, gameCount - RecentGameCount);

            float totalScore = 0f;
            float totalDecisionTime = 0f;
            int count = 0;

            for (int i = start; i < gameCount; i++)
            {
                totalScore += games[i].score;
                totalDecisionTime += games[i].avgDecisionTime;
                count++;
            }

            if (count == 0)
                return "insight_developing";

            float avgScore = totalScore / count;
            float avgDecisionTime = totalDecisionTime / count;

            float overallAvgScore = 0f;
            float overallAvgDecisionTime = 0f;
            for (int i = 0; i < gameCount; i++)
            {
                overallAvgScore += games[i].score;
                overallAvgDecisionTime += games[i].avgDecisionTime;
            }
            overallAvgScore /= gameCount;
            overallAvgDecisionTime /= gameCount;

            bool aboveAverageScore = avgScore >= overallAvgScore;
            bool belowAverageTime = avgDecisionTime <= overallAvgDecisionTime;

            if (aboveAverageScore && belowAverageTime)
                return "insight_fast_accurate";
            if (aboveAverageScore && !belowAverageTime)
                return "insight_methodical";
            if (!aboveAverageScore && belowAverageTime)
                return "insight_rushed";
            return "insight_struggling";
        }
    }
}
