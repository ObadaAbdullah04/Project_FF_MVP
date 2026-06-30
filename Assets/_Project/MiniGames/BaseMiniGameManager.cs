namespace Project.MiniGames
{
    using System;
    using System.Collections.Generic;
    using Project.Core;
    using Project.Data;
    using Project.UI;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public abstract class BaseMiniGameManager : MonoBehaviour
    {
        [SerializeField] protected InventoryData _inventoryData;
        [SerializeField] protected ChildProgressData _childProgressData;
        [SerializeField] protected MiniGameProfile _miniGameProfile;
        [SerializeField] protected GameSceneConfig _sceneConfig;
        [SerializeField] protected UniversalMiniGameHUD _hud;

        private List<float> _decisionTimes = new List<float>();
        private float _gameStartTime;
        private GameResult _lastResult;

        protected abstract void OnGameStart();

        public void StartGame()
        {
            _decisionTimes.Clear();
            _gameStartTime = Time.time;

            if (_hud != null)
                _hud.Initialize(this);

            OnGameStart();
        }

        public void ReportDecisionTime(float seconds)
        {
            _decisionTimes.Add(seconds);
        }

        public void UpdateHUDScore(int score)
        {
            if (_hud != null)
                _hud.UpdateScore(score);
        }

        public void ShowGameResult(GameResult result)
        {
            _lastResult = result;

            if (_miniGameProfile != null && _childProgressData != null)
            {
                int prevBest = _childProgressData.GetBestScore(_miniGameProfile.DisplayNameKey);
                result.bestScore = Mathf.Max(result.score, prevBest);
            }
            else
            {
                result.bestScore = result.score;
            }

            if (_hud != null)
                _hud.ShowSummary(result);
        }

        public void ReplayGame()
        {
            RecordResult(_lastResult);

            if (_hud != null)
                _hud.Cleanup();

            Time.timeScale = 1f;
            
            if (SceneLoader.Instance != null)
                SceneLoader.Instance.TransitionToScene(SceneManager.GetActiveScene().name, null);
        }

        public void CompleteGame(GameResult result)
        {
            RecordResult(result);

            if (_sceneConfig != null && SceneLoader.Instance != null)
                SceneLoader.Instance.TransitionToScene(_sceneConfig.HubSceneName, null);
        }

        private void RecordResult(GameResult result)
        {
            if (result.score == 0 && result.currencyEarned == 0)
                return;

            if (_hud != null)
                _hud.Cleanup();

            if (_inventoryData != null)
                _inventoryData.AddCurrency(result.currencyEarned);

            if (_childProgressData != null && _miniGameProfile != null)
            {
                var record = new GameRecord
                {
                    gameId = _miniGameProfile.DisplayNameKey,
                    gameNameKey = _miniGameProfile.DisplayNameKey,
                    educationalWeight = _miniGameProfile.EducationalWeight,
                    pedagogicalWeight = _miniGameProfile.PedagogicalWeight,
                    entertainmentWeight = _miniGameProfile.EntertainmentWeight,
                    score = result.score,
                    currencyEarned = result.currencyEarned,
                    avgDecisionTime = result.avgDecisionTime,
                    duration = result.duration > 0f ? result.duration : Time.time - _gameStartTime,
                    timestamp = DateTime.UtcNow.ToString("o")
                };
                _childProgressData.RecordGame(record);
            }

            if (GameManager.Instance != null)
                GameManager.Instance.SaveGame();
        }

    }
}
