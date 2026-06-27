namespace Project.MiniGames.Rules
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameRuleConfig", menuName = "Project/Game Rule Config")]
    public class GameRuleConfig : ScriptableObject
    {
        public RuleType ruleType = RuleType.Survival;

        [Header("Score Target")]
        public int scoreTarget = 10;
        public int baseRewardPerScore = 1;

        [Header("Survival")]
        public float distanceMultiplier = 1f;

        [Header("Step")]
        public int fixedRewardAmount = 100;

        [Header("Common")]
        public float stageMultiplier = 1f;

        public IGameRule CreateRule()
        {
            switch (ruleType)
            {
                case RuleType.ScoreTarget:
                    return new ScoreTargetRule(scoreTarget, baseRewardPerScore, stageMultiplier);
                case RuleType.Survival:
                    return new SurvivalRule(distanceMultiplier, stageMultiplier);
                case RuleType.Step:
                    return new StepRule(fixedRewardAmount, stageMultiplier);
                default:
                    return new SurvivalRule(distanceMultiplier, stageMultiplier);
            }
        }
    }

    public enum RuleType
    {
        Survival,
        ScoreTarget,
        Step
    }
}
