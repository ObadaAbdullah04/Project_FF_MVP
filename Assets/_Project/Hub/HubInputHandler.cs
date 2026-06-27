namespace Project.Hub
{
    using System;
    using Project.Architecture;
    using UnityEngine;
    using Project.UI;

    public class HubInputHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _hubCamera;

        [Header("Settings")]
        [SerializeField] private LayerMask _interactableLayer = -1;
        [SerializeField] private float _dragThresholdPixels = 15f;
        [SerializeField] private float _maxTapDuration = 0.4f;

        public event Action<Collider> OnInteractableTapped;

        private Vector2 _pressStartPosition;
        private float _pressStartTime;
        private bool _isPressActive;
        private bool _hasDraggedExceededThreshold;

        private void Awake()
        {
            _hubCamera = CameraUtility.ResolveCamera(_hubCamera, this);
        }

        private void Update()
        {
            // Execute tap detection autonomously as long as the pointer is not over UI
            if (!UIHelper.IsPointerOverUI())
            {
                HandleTapDetection();
            }
            else
            {
                // Reset press if we hovered or slid over UI to prevent stuck press states
                _isPressActive = false;
            }
        }

        private void HandleTapDetection()
        {
            if (_hubCamera == null)
            {
                _hubCamera = Camera.main;
            }
            if (_hubCamera == null) return;

            // Handle touch/mouse down
            if (Input.GetMouseButtonDown(0))
            {
                _pressStartPosition = Input.mousePosition;
                _pressStartTime = Time.time;
                _isPressActive = true;
                _hasDraggedExceededThreshold = false;
            }

            // Track movement while held down
            if (_isPressActive && Input.GetMouseButton(0))
            {
                float distance = Vector2.Distance(Input.mousePosition, _pressStartPosition);
                if (distance > _dragThresholdPixels)
                {
                    _hasDraggedExceededThreshold = true;
                }
            }

            // Handle touch/mouse up
            if (_isPressActive && Input.GetMouseButtonUp(0))
            {
                _isPressActive = false;

                float pressDuration = Time.time - _pressStartTime;
                // Only register as a tap if we didn't drag and it was a quick click/tap
                if (!_hasDraggedExceededThreshold && pressDuration <= _maxTapDuration)
                {
                    ProcessTap(Input.mousePosition);
                }
            }
        }

        private void ProcessTap(Vector2 screenPosition)
        {
            Ray ray = _hubCamera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _interactableLayer))
            {
                OnInteractableTapped?.Invoke(hit.collider);
            }
        }

    }
}
