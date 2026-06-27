namespace Project.Hub
{
    using DG.Tweening;
    using Project.Data;
    using Project.UI;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class HubWorldManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InventoryData _inventoryData;
        [SerializeField] private ChunkController[] _chunks;
        [SerializeField] private GameObject _confirmationBubble;
        [SerializeField] private Camera _hubCamera;
        [SerializeField] private Transform _cameraRestPoint;

        [Header("Settings")]
        [SerializeField] private LayerMask _chunkLayer = -1;
        [SerializeField] private float _cameraPanDuration = 0.8f;
        [SerializeField] private float _cameraHoldDuration = 1.2f;
        [SerializeField] private float _dragSensitivity = 0.02f;

        private UnlockConfirmationBubble _activeBubble;
        private bool _isInputEnabled = true;
        private Vector3 _initialCameraPosition;
        private Quaternion _initialCameraRotation;

        private void OnEnable()
        {
            if (_inventoryData != null)
            {
                _inventoryData.OnChunkUnlocked += HandleChunkUnlocked;
            }
        }

        private void OnDisable()
        {
            if (_inventoryData != null)
            {
                _inventoryData.OnChunkUnlocked -= HandleChunkUnlocked;
            }

            DestroyActiveBubble();
        }

        private void Start()
        {
            if (_hubCamera != null)
            {
                _initialCameraPosition = _hubCamera.transform.position;
                _initialCameraRotation = _hubCamera.transform.rotation;
            }

            InitializeChunks();
        }

        private void Update()
        {
            if (!_isInputEnabled) return;
            HandleTap();
            HandleCameraDrag();

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetCamera();
            }
        }

        public void ResetCamera()
        {
            if (_hubCamera == null) return;

            _hubCamera.transform.DOKill();
            _hubCamera.transform.position = _initialCameraPosition;
            _hubCamera.transform.rotation = _initialCameraRotation;
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

        private void HandleTap()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase != TouchPhase.Began) return;
                ProcessTap(touch.position);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                ProcessTap(Input.mousePosition);
            }
        }

        private void ProcessTap(Vector2 screenPosition)
        {
            if (_hubCamera == null) return;

            if (_activeBubble != null && _activeBubble.gameObject.activeInHierarchy) return;

            Ray ray = _hubCamera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 100f, _chunkLayer)) return;

            ChunkController chunk = hit.collider.GetComponentInParent<ChunkController>();
            if (chunk == null) return;

            if (!chunk.IsUnlocked)
            {
                ShowConfirmationBubble(chunk);
                return;
            }

            BuildingController building = hit.collider.GetComponentInParent<BuildingController>();
            if (building != null && !string.IsNullOrEmpty(building.MiniGameScene))
            {
                SceneManager.LoadScene(building.MiniGameScene);
            }
        }

        private void HandleCameraDrag()
        {
            if (_hubCamera == null) return;
            if (_activeBubble != null && _activeBubble.gameObject.activeInHierarchy) return;

            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    PanCamera(touch.deltaPosition * _dragSensitivity);
                }
            }
            else if (Input.GetMouseButton(0))
            {
                float mx = Input.GetAxis("Mouse X");
                float my = Input.GetAxis("Mouse Y");
                if (Mathf.Abs(mx) > 0.01f || Mathf.Abs(my) > 0.01f)
                {
                    PanCamera(new Vector2(mx, my) * _dragSensitivity * 50f);
                }
            }
        }

        private void PanCamera(Vector2 delta)
        {
            Vector3 right = _hubCamera.transform.right;
            Vector3 forward = _hubCamera.transform.forward;
            right.y = 0f;
            forward.y = 0f;
            right.Normalize();
            forward.Normalize();

            Vector3 movement = (right * -delta.x + forward * -delta.y);
            _hubCamera.transform.position += movement;
        }

        private void ShowConfirmationBubble(ChunkController chunk)
        {
            DestroyActiveBubble();

            if (_confirmationBubble == null) return;

            _confirmationBubble.SetActive(true);
            _activeBubble = _confirmationBubble.GetComponent<UnlockConfirmationBubble>();

            if (_activeBubble != null)
            {
                _activeBubble.Setup(chunk, _inventoryData, OnConfirmUnlock, OnCancelUnlock);
            }
        }

        private void OnConfirmUnlock(ChunkController chunk)
        {
            if (_inventoryData == null) return;
            if (chunk == null) return;

            if (_inventoryData.SoftCurrency < chunk.UnlockCost)
            {
                if (_activeBubble != null)
                {
                    _activeBubble.PlayValidationError();
                }
                return;
            }

            if (_inventoryData.TrySpendCurrency(chunk.UnlockCost))
            {
                _inventoryData.UnlockChunk(chunk.ChunkId);
            }

            DestroyActiveBubble();
        }

        private void OnCancelUnlock()
        {
            DestroyActiveBubble();
        }

        private void HandleChunkUnlocked(string chunkId)
        {
            ChunkController chunk = FindChunkById(chunkId);
            if (chunk == null) return;

            chunk.PlayUnlockAnimation();
            PlayUnlockCameraSequence(chunk);
        }

        private void PlayUnlockCameraSequence(ChunkController chunk)
        {
            if (_hubCamera == null) return;

            _isInputEnabled = false;

            Vector3 targetPosition = chunk.transform.position + Vector3.back * 5f + Vector3.up * 3f;

            Sequence cameraSequence = DOTween.Sequence();
            cameraSequence.Append(_hubCamera.transform.DOMove(targetPosition, _cameraPanDuration).SetEase(Ease.InOutSine));
            cameraSequence.Join(_hubCamera.transform.DOLookAt(chunk.transform.position, _cameraPanDuration));
            cameraSequence.AppendInterval(_cameraHoldDuration);

            if (_cameraRestPoint != null)
            {
                cameraSequence.Append(_hubCamera.transform.DOMove(_cameraRestPoint.position, _cameraPanDuration).SetEase(Ease.InOutSine));
                cameraSequence.Join(_hubCamera.transform.DOLookAt(_cameraRestPoint.position + _cameraRestPoint.forward * 10f, _cameraPanDuration));
            }

            cameraSequence.OnComplete(() =>
            {
                _isInputEnabled = true;
            });
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
