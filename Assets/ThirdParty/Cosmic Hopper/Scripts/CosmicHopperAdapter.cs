namespace FlappyJump
{
    using UnityEngine;
    using Project.MiniGames;

    /// <summary>
    /// Replaces the old 'Menus.cs' and 'TapToStart.cs' to perfectly recreate the template's 
    /// original initialization sequence, but decoupled so it works with UniversalGameController.
    /// </summary>
    public class CosmicHopperAdapter : MonoBehaviour
    {
        [Header("Architecture Bindings")]
        public UniversalGameController controller;

        [Header("Template Prefabs")]
        public GameObject playerPrefab;
        public GameObject obstaclePrefab;

        private PlayerMovement _spawnedPlayer;
        private bool _isWaitingForTap;

        private void Start()
        {
            if (playerPrefab == null || obstaclePrefab == null)
            {
                return;
            }

            // EXACT CHRONOLOGICAL FLOW FROM ORIGINAL GAME:

            // 1. Instantiate Player FIRST at (-5, 3)
            GameObject playerObj = Instantiate(playerPrefab, new Vector3(-5f, 3f, 0f), Quaternion.identity);
            playerObj.name = "Player"; // Name must be exact for CameraFollow and ObstacleColorGenerator
            playerObj.transform.SetParent(this.transform); // FIX LEAK: Bind to TemplateRoot

            // Ensure PlayerMovement is disabled and Rigidbody is Kinematic BEFORE physics tick
            _spawnedPlayer = playerObj.GetComponent<PlayerMovement>();
            if (_spawnedPlayer != null)
            {
                Vars.playerSpeed = 0f; // Reset Vars.playerSpeed 
                Vars.score = 0; // Reset Vars.score
                _spawnedPlayer.enabled = false;
            }

            Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
            if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

            // We no longer inject 'controller' into the scripts. 
            // They will find the UniversalGameController themselves when needed.

            // 2. CameraFollow.FindThePlayer() must be called AFTER Player is named
            CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
            if (camFollow != null) camFollow.FindThePlayer();

            // 3. Instantiate AreaObstacles SECOND at (-2.76, 0)
            GameObject firstArea = Instantiate(obstaclePrefab, new Vector3(-2.76f, 0f, 0f), Quaternion.identity);
            firstArea.name = "AreaObstacles";
            firstArea.transform.SetParent(this.transform); // FIX LEAK: Bind to TemplateRoot

            // 4. Configure First Obstacle Lanes
            Transform c3 = firstArea.transform.Find("ColorAreas3");
            Transform c4 = firstArea.transform.Find("ColorAreas4");
            Transform c5 = firstArea.transform.Find("ColorAreas5");

            if (c3 != null) c3.name = "ColorAreas";
            if (c4 != null) c4.gameObject.SetActive(false);
            if (c5 != null) c5.gameObject.SetActive(false);

            // Note: We DO NOT call GenerateColor() explicitly here! 
            // The original game queued ObstacleColorGenerator.Start() to do it.

            // 5. Setup complete. Await user tap.
            _isWaitingForTap = true;
        }

        private void Update()
        {
            if (_isWaitingForTap && Input.GetMouseButtonDown(0))
            {
                _isWaitingForTap = false;
                
                // STEP 3: First Tap 
                // 1. rb.bodyType = Dynamic (enables physics)
                Rigidbody2D rb = _spawnedPlayer.GetComponent<Rigidbody2D>();
                if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
                
                // 2. PlayerMovement.enabled = true
                _spawnedPlayer.enabled = true;

                // 3. PlayerMovement.Jump() (immediate first jump)
                _spawnedPlayer.Jump();
                
                // PERFORMANCE: Turn off this script's Update loop now that the game has started
                this.enabled = false; 
            }
        }
    }
}
