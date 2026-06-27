namespace Project.MiniGames.Rules
{
    using System;
    using UnityEngine;

    [Serializable]
    public class StepRule : IGameRule
    {
        private int _fixedRewardAmount;
        private float _stageMultiplier;
        private bool _isGameOver;

        public StepRule(int fixedRewardAmount, float stageMultiplier)
        {
            _fixedRewardAmount = fixedRewardAmount;
            _stageMultiplier = stageMultiplier;
        }

        public bool IsGameOver => _isGameOver;

        public void OnGameStarted()
        {
            _isGameOver = false;
        }

        public void OnScoreChanged(int amount) { }

        public void OnPlayerDeath()
        {
            _isGameOver = true;
        }

        public void OnDistanceChanged(float distance) { }

        public void OnGameComplete()
        {
            _isGameOver = true;
        }

        public int CalculateReward()
        {
            return Mathf.FloorToInt(_fixedRewardAmount * _stageMultiplier);
        }
    }
}