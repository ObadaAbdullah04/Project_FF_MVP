namespace Project.Hub
{
    using DG.Tweening;
    using Project.Data;
    using UnityEngine;

    [SelectionBase]
    public class ChunkController : MonoBehaviour
    {
        [SerializeField] private string _chunkId;
        [SerializeField] private int _unlockCost;
        [SerializeField] private bool _isUnlockedByDefault;
        [SerializeField] private GameObject[] _childBuildings;
        [SerializeField] private GameObject _lockedOverlay;
        [SerializeField] private Transform _targetLandingPosition;

        private bool _isUnlocked;

        public string ChunkId => _chunkId;
        public int UnlockCost => _unlockCost;
        public bool IsUnlocked => _isUnlocked;

        private void OnDestroy()
        {
            transform.DOKill();
        }

        public void Initialize(InventoryData inventory)
        {
            if (inventory == null) return;

            if (inventory.HasUnlockedChunk(_chunkId) || _isUnlockedByDefault)
            {
                SetUnlockedImmediate();
            }
            else
            {
                SetLockedImmediate();
            }
        }

        public void PlayUnlockAnimation()
        {
            if (_isUnlocked) return;
            _isUnlocked = true;

            if (_lockedOverlay != null)
            {
                _lockedOverlay.SetActive(false);
            }

            if (_childBuildings == null) return;

            for (int i = 0; i < _childBuildings.Length; i++)
            {
                if (_childBuildings[i] == null) continue;

                BuildingController building = _childBuildings[i].GetComponent<BuildingController>();
                if (building != null)
                {
                    building.PlayUnlockAnimation(0.15f + i * 0.12f);
                }
            }
        }

        private void SetUnlockedImmediate()
        {
            _isUnlocked = true;

            if (_lockedOverlay != null)
            {
                _lockedOverlay.SetActive(false);
            }

            if (_targetLandingPosition != null)
            {
                transform.position = _targetLandingPosition.position;
            }

            transform.localScale = Vector3.one;

            if (_childBuildings == null) return;

            for (int i = 0; i < _childBuildings.Length; i++)
            {
                if (_childBuildings[i] == null) continue;
                _childBuildings[i].SetActive(true);

                BuildingController building = _childBuildings[i].GetComponent<BuildingController>();
                if (building != null)
                {
                    building.SetToUnlockedState();
                }
            }
        }

        private void SetLockedImmediate()
        {
            _isUnlocked = false;

            if (_targetLandingPosition != null)
            {
                transform.position = _targetLandingPosition.position;
            }

            transform.localScale = Vector3.one;

            if (_lockedOverlay != null)
            {
                _lockedOverlay.SetActive(true);
            }

            if (_childBuildings == null) return;

            for (int i = 0; i < _childBuildings.Length; i++)
            {
                if (_childBuildings[i] != null)
                {
                    _childBuildings[i].SetActive(true);
                    _childBuildings[i].transform.localScale = Vector3.one;
                }
            }
        }
    }
}
