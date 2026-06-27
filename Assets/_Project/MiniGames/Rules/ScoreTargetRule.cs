namespace Project.MiniGames.Rules
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ScoreTargetRule : IGameRule
    {
        private int _targetScore;
        private int _baseRewardPerScore;
        private int _score;
        private float _stageMultiplier;
        private bool _isGameOver;

        public ScoreTargetRule(int targetScore, int baseRewardPerScore, float stageMultiplier)
        {
            _targetScore = targetScore;
            _baseRewardPerScore = baseRewardPerScore;
            _stageMultiplier = stageMultiplier;
        }

        public bool IsGameOver => _isGameOver;

        public void OnGameStarted()
        {
            _score = 0;
            _isGameOver = false;
        }

        public void OnScoreChanged(int amount)
        {
            if (_isGameOver) return;

            _score += amount;

            if (_score >= _targetScore)
            {
                _isGameOver = true;
            }
        }

        public void OnPlayerDeath() { }

        public void OnDistanceChanged(float distance) { }

        public void OnGameComplete() { }

        public int CalculateReward()
        {
            return Mathf.FloorToInt(_score * _baseRewardPerScore * _stageMultiplier);
        }
    }
}