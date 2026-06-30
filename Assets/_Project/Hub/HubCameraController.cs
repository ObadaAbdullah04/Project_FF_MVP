namespace Project.Hub
{
    using System;
    using DG.Tweening;
    using Project.Architecture;
    using UnityEngine;
    using Project.UI;

    public class HubCameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _hubCamera;

        [Header("Settings")]
        [SerializeField] private float _cameraPanDuration = 0.8f;
        [SerializeField] private float _cameraHoldDuration = 1.2f;
        [SerializeField] private float _dragSensitivity = 0.02f;
        [SerializeField] private float _dragFriction = 5f;
        [SerializeField] private float _smoothFocusTime = 0.3f;

        private Vector3 _initialCameraPosition;
        private Quaternion _initialCameraRotation;
        private Vector3 _resetFocusPosition;
        private bool _hasResetFocus;
        private bool _isSequenceRunning;

        private Vector3 _lastMousePosition;
        private bool _isDraggingMouse;

        // Inertia & Focus variables
        private Vector3 _velocity;
        private Vector3 _smoothDampVelocity;
        private bool _isFocusing;
        private Vector3 _focusTargetPosition;
        private Vector3 _focusLookTarget;

        public bool IsSequenceRunning => _isSequenceRunning;

        private void Awake()
        {
            _hubCamera = CameraUtility.ResolveCamera(_hubCamera, this);
        }

        private void Start()
        {
            if (_hubCamera != null)
            {
                _initialCameraPosition = _hubCamera.transform.position;
                _initialCameraRotation = _hubCamera.transform.rotation;
            }
        }

        private void Update()
        {
            // 1. Handle Reset Hotkey (Duty of the Camera system)
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetCamera();
            }

            // 2. Handle Camera Dragging if not blocked by UI pointer focus
            if (!_isSequenceRunning && !UIHelper.IsPointerOverUI())
            {
                HandleCameraDrag();
            }
        }

        private void LateUpdate()
        {
            if (_isSequenceRunning || _hubCamera == null) return;

            if (_isFocusing)
            {
                // Smooth Focus Pan
                _hubCamera.transform.position = Vector3.SmoothDamp(
                    _hubCamera.transform.position, 
                    _focusTargetPosition, 
                    ref _smoothDampVelocity, 
                    _smoothFocusTime);

                // Smooth LookAt
                Quaternion targetRot = Quaternion.LookRotation(_focusLookTarget - _hubCamera.transform.position);
                _hubCamera.transform.rotation = Quaternion.Slerp(_hubCamera.transform.rotation, targetRot, Time.deltaTime * 5f);

                if (Vector3.Distance(_hubCamera.transform.position, _focusTargetPosition) < 0.05f)
                {
                    _isFocusing = false;
                }
            }
            else
            {
                // Apply Inertia
                if (!_isDraggingMouse && Input.touchCount == 0 && _velocity.sqrMagnitude > 0.001f)
                {
                    _hubCamera.transform.position += _velocity * Time.deltaTime;
                    _velocity = Vector3.Lerp(_velocity, Vector3.zero, Time.deltaTime * _dragFriction);
                }
            }
        }

        public void ResetCamera()
        {
            if (_hubCamera == null) return;

            _hubCamera.transform.DOKill();
            _isFocusing = false;
            _velocity = Vector3.zero;

            if (_hasResetFocus)
            {
                Vector3 pos = _resetFocusPosition + Vector3.back * 5f + Vector3.up * 3f;
                _hubCamera.transform.position = pos;
                _hubCamera.transform.LookAt(_resetFocusPosition);
            }
            else
            {
                _hubCamera.transform.position = _initialCameraPosition;
                _hubCamera.transform.rotation = _initialCameraRotation;
            }
        }

        public void FocusOnTarget(Vector3 targetPosition)
        {
            if (_hubCamera == null) return;
            
            _hubCamera.transform.DOKill();
            _isFocusing = true;
            _focusLookTarget = targetPosition;
            _focusTargetPosition = targetPosition + Vector3.back * 5f + Vector3.up * 3f;
            _velocity = Vector3.zero; // Stop inertia
        }

        private void HandleCameraDrag()
        {
            // Touch Panning (Mobile)
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    PanCamera(touch.deltaPosition * _dragSensitivity);
                }
                _isDraggingMouse = false;
            }
            // Mouse Panning (Editor/PC) - Uses delta position tracking to prevent click-frame jumping
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _lastMousePosition = Input.mousePosition;
                    _isDraggingMouse = true;
                    _isFocusing = false; // Stop focus on manual drag
                }
                else if (Input.GetMouseButton(0) && _isDraggingMouse)
                {
                    Vector3 currentMousePos = Input.mousePosition;
                    Vector3 mouseDelta = currentMousePos - _lastMousePosition;
                    _lastMousePosition = currentMousePos;

                    if (mouseDelta.magnitude > 0.01f)
                    {
                        PanCamera(new Vector2(mouseDelta.x, mouseDelta.y) * _dragSensitivity);
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    _isDraggingMouse = false;
                }
            }
        }

        private void PanCamera(Vector2 delta)
        {
            _isFocusing = false; // Override focus

            Vector3 right = _hubCamera.transform.right;
            Vector3 forward = _hubCamera.transform.forward;
            right.y = 0f;
            forward.y = 0f;
            right.Normalize();
            forward.Normalize();

            Vector3 movement = (right * -delta.x + forward * -delta.y);
            _hubCamera.transform.position += movement;

            // Track velocity for inertia
            _velocity = movement / Time.deltaTime;
            
            // Cap max velocity to prevent wild flinging
            if (_velocity.magnitude > 20f)
            {
                _velocity = _velocity.normalized * 20f;
            }
        }

        public void PlayUnlockSequence(Transform targetChunk, Action onComplete)
        {
            if (_hubCamera == null || targetChunk == null)
            {
                onComplete?.Invoke();
                return;
            }

            _isSequenceRunning = true;
            _isFocusing = false;
            _velocity = Vector3.zero;
            _hubCamera.transform.DOKill();

            _hasResetFocus = true;
            _resetFocusPosition = targetChunk.position;

            Vector3 focusPosition = _resetFocusPosition + Vector3.back * 5f + Vector3.up * 3f;

            Sequence cameraSequence = DOTween.Sequence();
            cameraSequence.Append(_hubCamera.transform.DOMove(focusPosition, _cameraPanDuration).SetEase(Ease.InOutSine));
            cameraSequence.Join(_hubCamera.transform.DOLookAt(_resetFocusPosition, _cameraPanDuration));
            cameraSequence.AppendInterval(_cameraHoldDuration);

            cameraSequence.OnComplete(() =>
            {
                _isSequenceRunning = false;
                onComplete?.Invoke();
            });
        }

    }
}
