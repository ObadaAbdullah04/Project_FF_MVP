namespace Project.Hub
{
    using Project.Core;
    using Project.Data;
    using Project.UI;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class HubWorldManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventoryData _inventoryData;
        [SerializeField] private ChildProgressData _childProgressData;
        [SerializeField] private ChunkController[] _chunks;
        [SerializeField] private GameObject _confirmationBubble;

        [Header("Systems")]
        [SerializeField] private HubCameraController _cameraController;
        [SerializeField] private HubInputHandler _inputHandler;

        private UnlockConfirmationBubble _activeBubble;

        private void Awake()
        {
            // Auto-discover camera controller
            if (_cameraController == null)
            {
                _cameraController = GetComponent<HubCameraController>();
                if (_cameraController == null)
                {
                    _cameraController = FindObjectOfType<HubCameraController>();
                }
            }

            // Auto-discover input handler
            if (_inputHandler == null)
            {
                _inputHandler = GetComponent<HubInputHandler>();
                if (_inputHandler == null)
                {
                    _inputHandler = FindObjectOfType<HubInputHandler>();
                }
            }
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            {
                ResetAndReload();
            }
        }

        private void ResetAndReload()
        {
            Debug.Log("[Dev] Clearing saved data via GameManager...");
            
            if (GameManager.Instance != null)
                GameManager.Instance.ResetAllData();
            
            if (SceneLoader.Instance != null)
                SceneLoader.Instance.TransitionToScene(SceneManager.GetActiveScene().name, null);
        }

        private void OnEnable()
        {
            if (_inventoryData != null)
            {
                _inventoryData.OnChunkUnlocked += HandleChunkUnlocked;
            }

            if (_inputHandler != null)
            {
                _inputHandler.OnInteractableTapped += HandleInteractableTapped;
            }
        }

        private void OnDisable()
        {
            if (_inventoryData != null)
            {
                _inventoryData.OnChunkUnlocked -= HandleChunkUnlocked;
            }

            if (_inputHandler != null)
            {
                _inputHandler.OnInteractableTapped -= HandleInteractableTapped;
            }

            DestroyActiveBubble();
        }

        private void Start()
        {
            InitializeChunks();
        }

        private void InitializeChunks()
        {
            if (_chunks == null) return;

            for (int i = 0; i < _chunks.Length; i++)
            {
                if (_chunks[i] != null)
                {
                    _chunks[i].Initialize(_inventoryData);
                }
            }
        }

        private void HandleInteractableTapped(Collider col)
        {
            ChunkController chunk = col.GetComponentInParent<ChunkController>();
            if (chunk == null) return;

            if (!chunk.IsUnlocked)
            {
                ShowConfirmationBubble(chunk);
                return;
            }

            BuildingController building = col.GetComponentInParent<BuildingController>();
            if (building != null)
            {
                if (string.IsNullOrEmpty(building.MiniGameScene))
                {
                    Debug.LogWarning($"Building '{building.name}' has no mini-game scene assigned. Tapping does nothing.", building);
                    return;
                }

                if (SceneLoader.Instance != null)
                    SceneLoader.Instance.TransitionToScene(building.MiniGameScene, null);
                else
                    Debug.LogError("SceneLoader is missing! Cannot transition.");
            }
        }

        private void ShowConfirmationBubble(ChunkController chunk)
        {
            DestroyActiveBubble();

            if (_confirmationBubble == null) return;

            _confirmationBubble.SetActive(true);
            _activeBubble = _confirmationBubble.GetComponent<UnlockConfirmationBubble>();

            if (_activeBubble != null)
            {
                _activeBubble.Setup(
                    chunk.UnlockCost,
                    () => OnConfirmUnlock(chunk),
                    OnCancelUnlock
                );
            }
        }

        private void OnConfirmUnlock(ChunkController chunk)
        {
            if (_inventoryData == null || chunk == null) return;

            // Attempt transaction using Encapsulated Business Logic in InventoryData
            if (_inventoryData.TrySpendCurrency(chunk.UnlockCost))
            {
                _inventoryData.UnlockChunk(chunk.ChunkId);
                DestroyActiveBubble();
            }
            else
            {
                // Failed transaction - show UI validation error
                if (_activeBubble != null)
                {
                    _activeBubble.PlayValidationError();
                }
            }
        }

        private void OnCancelUnlock()
        {
            DestroyActiveBubble();
        }

        private void HandleChunkUnlocked(string chunkId)
        {
            ChunkController chunk = FindChunkById(chunkId);
            if (chunk == null) return;
            
            if (_cameraController != null)
            {
                // Halt interaction systems during camera sequence by turning off the components
                if (_inputHandler != null) _inputHandler.enabled = false;
                _cameraController.enabled = false;

                _cameraController.PlayUnlockSequence(chunk.transform, () =>
                {
                    // Restore interaction systems when completed
                    if (_inputHandler != null) _inputHandler.enabled = true;
                    _cameraController.enabled = true;
                });
            }
        }

        private ChunkController FindChunkById(string chunkId)
        {
            if (_chunks == null || string.IsNullOrEmpty(chunkId)) return null;

            for (int i = 0; i < _chunks.Length; i++)
            {
                if (_chunks[i] != null && _chunks[i].ChunkId == chunkId)
                {
                    return _chunks[i];
                }
            }

            return null;
        }

        private void DestroyActiveBubble()
        {
            if (_activeBubble != null)
            {
                _activeBubble.gameObject.SetActive(false);
                _activeBubble = null;
            }
        }
    }
}
