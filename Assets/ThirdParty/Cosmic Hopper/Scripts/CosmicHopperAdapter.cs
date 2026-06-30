namespace FlappyJump
{
    using Project.Core;
    using Project.MiniGames;
    using UnityEngine;

    public class CosmicHopperAdapter : MonoBehaviour
    {
        [Header("Architecture Bindings")]
        public UniversalGameController controller;

        [Header("Template Prefabs")]
        public GameObject playerPrefab;
        public GameObject obstaclePrefab;

        public static CosmicHopperAdapter Instance { get; private set; }

        public float playerSpeed { get; set; }

        private int _score;
        public int Score => _score;
        private float _lastEventTime;
        private float _gameStartTime;
        private PlayerMovement _spawnedPlayer;
        private bool _isWaitingForTap;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (playerPrefab == null || obstaclePrefab == null)
            {
                return;
            }

            _score = 0;
            playerSpeed = 0f;
            _gameStartTime = Time.time;
            _lastEventTime = 0f;

            // 1. Instantiate Player FIRST at (-5, 3)
            GameObject playerObj = Instantiate(playerPrefab, new Vector3(-5f, 3f, 0f), Quaternion.identity);
            playerObj.name = "Player";
            playerObj.transform.SetParent(this.transform);

            _spawnedPlayer = playerObj.GetComponent<PlayerMovement>();
            if (_spawnedPlayer != null)
            {
                _spawnedPlayer.enabled = false;
            }

            Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
            if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

            // 2. CameraFollow.FindThePlayer() must be called AFTER Player is named
            CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
            if (camFollow != null) camFollow.FindThePlayer();

            // 3. Instantiate AreaObstacles SECOND at (-2.76, 0)
            GameObject firstArea = Instantiate(obstaclePrefab, new Vector3(-2.76f, 0f, 0f), Quaternion.identity);
            firstArea.name = "AreaObstacles";
            firstArea.transform.SetParent(this.transform);

            // 4. Configure First Obstacle Lanes
            Transform c3 = firstArea.transform.Find("ColorAreas3");
            Transform c4 = firstArea.transform.Find("ColorAreas4");
            Transform c5 = firstArea.transform.Find("ColorAreas5");

            if (c3 != null) c3.name = "ColorAreas";
            if (c4 != null) c4.gameObject.SetActive(false);
            if (c5 != null) c5.gameObject.SetActive(false);

            // 5. Setup complete. Await user tap.
            _isWaitingForTap = true;
        }

        private void Update()
        {
            if (_isWaitingForTap && Input.GetMouseButtonDown(0))
            {
                _isWaitingForTap = false;

                Rigidbody2D rb = _spawnedPlayer.GetComponent<Rigidbody2D>();
                if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;

                _spawnedPlayer.enabled = true;
                _spawnedPlayer.Jump();

                this.enabled = false;
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
