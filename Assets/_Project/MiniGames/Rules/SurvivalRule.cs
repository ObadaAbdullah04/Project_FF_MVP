namespace Project.MiniGames.Rules
{
    using System;
    using UnityEngine;

    [Serializable]
    public class SurvivalRule : IGameRule
    {
        private float _distanceMultiplier;
        private float _distance;
        private float _stageMultiplier;
        private bool _isGameOver;

        public SurvivalRule(float distanceMultiplier, float stageMultiplier)
        {
            _distanceMultiplier = distanceMultiplier;
            _stageMultiplier = stageMultiplier;
        }

        public bool IsGameOver => _isGameOver;

        public void OnGameStarted()
        {
            _distance = 0f;
            _isGameOver = false;
        }

        public void OnScoreChanged(int amount) { }

        public void OnPlayerDeath()
        {
            _isGameOver = true;
        }

        public void OnDistanceChanged(float distance)
        {
            if (!_isGameOver)
            {
                _distance = distance;
            }
        }

        public void OnGameComplete() { }

        public int CalculateReward()
        {
            return Mathf.FloorToInt(_distance * _distanceMultiplier * _stageMultiplier);
        }
    }
}