namespace Project.MiniGames
{
    using DG.Tweening;
    using Project.MiniGames.Rules;
    using Project.UI;
    using UnityEngine;
    using UnityEngine.Events;

    public class UniversalGameController : BaseMiniGameManager
    {
        [SerializeField] private GameRuleConfig _ruleConfig;
        [SerializeField] private GameObject _templateRoot;

        private IGameRule _gameRule;

        [Header("Events")]
        public UnityEvent<int> OnScoreUpdated;

        private bool _isGameActive;
        private int _currentScore;

        public bool IsGameActive => _isGameActive;

        private void Start()
        {
            StartGame();
        }

        protected override void OnGameStart()
        {
            if (_ruleConfig != null)
            {
                _gameRule = _ruleConfig.CreateRule();
            }

            _isGameActive = true;
            _currentScore = 0;
            OnScoreUpdated?.Invoke(_currentScore);

            if (_templateRoot != null)
            {
                _templateRoot.SetActive(true);
            }

            _gameRule?.OnGameStarted();
        }

        public void ReportPlayerDeath()
        {
            if (!_isGameActive) return;

            _gameRule?.OnPlayerDeath();
            CompleteCurrentGame();
        }

        public void ReportScore(int amount)
        {
            if (!_isGameActive) return;

            _currentScore += amount;
            OnScoreUpdated?.Invoke(_currentScore);

            if (_hud != null)
            {
                _hud.UpdateScore(_currentScore);
            }

            _gameRule?.OnScoreChanged(amount);
            CheckGameOver();
        }

        public void ReportDistance(float distance)
        {
            if (!_isGameActive) return;

            _gameRule?.OnDistanceChanged(distance);
            CheckGameOver();
        }

        public void ReportGameComplete()
        {
            if (!_isGameActive) return;

            _gameRule?.OnGameComplete();
            CheckGameOver();
        }

        private void CheckGameOver()
        {
            if (_gameRule != null && _gameRule.IsGameOver)
            {
                CompleteCurrentGame();
            }
        }

        private void CompleteCurrentGame()
        {
            if (!_isGameActive) return;
            _isGameActive = false;

            int reward = _gameRule?.CalculateReward() ?? 0;
            if (reward <= 0)
                reward = _currentScore;

            if (_hud != null)
            {
                _hud.ShowSummary(_currentScore, reward);
            }
            else
            {
                CompleteGame(reward);
            }
        }

        protected void OnDisable()
        {
            DOTween.Kill(this);
            if (_templateRoot != null)
            {
                _templateRoot.transform.DOKill();
            }
        }
    }
}