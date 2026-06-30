namespace Templates.ColorCube
{
    using Project.Core;
    using Project.MiniGames;
    using UnityEngine;

    public class ColorCubeAdapter : MonoBehaviour
    {
        [Header("Architecture Bindings")]
        public UniversalGameController controller;

        [Header("Game References (auto-found if empty)")]
        public GameObject[] obstacles;
        public GameObject player;

        public static ColorCubeAdapter Instance { get; private set; }

        private int _score;
        private float _lastEventTime;
        private float _gameStartTime;
        private bool _autoStartPending;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (MenuSelect.Instance == null)
                _autoStartPending = true;
        }

        private void Update()
        {
            if (!_autoStartPending) return;
            if (player == null) player = GameObject.Find("player");
            if (player != null)
            {
                _autoStartPending = false;
                BeginGame();
            }
        }

        public void BeginGame()
        {
            _autoStartPending = false;
            _score = 0;
            _gameStartTime = Time.time;
            _lastEventTime = 0f;

            if (MenuSelect.Instance != null)
            {
                MenuSelect.Instance.GameStart();
                return;
            }

            if (obstacles == null || obstacles.Length == 0)
            {
                Obstacle[] found = FindObjectsByType<Obstacle>(FindObjectsSortMode.None);
                obstacles = new GameObject[found.Length];
                for (int i = 0; i < found.Length; i++)
                    obstacles[i] = found[i].gameObject;
            }
            if (player == null)
                player = GameObject.Find("player");

            PlayerPrefs.SetInt("gamesPlayed", PlayerPrefs.GetInt("gamesPlayed") + 1);
            PlayerPrefs.SetInt("lastScore", 0);
            PlayerLogic.collision = 0;

            foreach (GameObject obstacle in obstacles)
            {
                if (obstacle != null)
                    obstacle.GetComponent<Obstacle>().enabled = true;
            }

            if (player != null)
            {
                if (player.GetComponent<PlayerLogic>() != null)
                    player.GetComponent<PlayerLogic>().enabled = true;
                if (player.GetComponent<ColorSwap>() != null)
                    player.GetComponent<ColorSwap>().enabled = true;
            }
        }

        public void OnScoreChanged()
        {
            _score++;

            float now = Time.time;
            if (_lastEventTime > 0f && controller != null)
            {
                float decisionTime = now - _lastEventTime;
                controller.ReportDecisionTime(decisionTime);
            }
            _lastEventTime = now;

            if (controller != null)
                controller.UpdateHUDScore(_score);
        }

        public void OnPlayerDeath()
        {
            if (controller == null) return;

            if (_score > 0 && _lastEventTime > 0f)
            {
                float now = Time.time;
                controller.ReportDecisionTime(now - _lastEventTime);
            }

            float totalTime = Time.time - _gameStartTime;
            float avgDecision = _score > 0 ? totalTime / _score : 0f;

            var result = new GameResult
            {
                score = _score,
                currencyEarned = _score,
                avgDecisionTime = avgDecision,
                duration = totalTime
            };

            controller.ShowGameResult(result);
        }
    }
}
