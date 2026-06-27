namespace Project.Data
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "InventoryData", menuName = "Project/Data/Inventory Data")]
    public class InventoryData : ScriptableObject
    {
        [SerializeField] private int _softCurrency;
        [SerializeField] private List<string> _unlockedBuildingIds = new List<string>();
        [SerializeField] private List<string> _unlockedChunkIds = new List<string>();
        public int SoftCurrency => _softCurrency;
        public List<string> UnlockedBuildingIds => _unlockedBuildingIds;
        public List<string> UnlockedChunkIds => _unlockedChunkIds;

        public event Action<int> OnCurrencyChanged;
        public event Action<string> OnBuildingUnlocked;
        public event Action<string> OnChunkUnlocked;

        public void AddCurrency(int amount)
        {
            if (amount <= 0) return;
            _softCurrency += amount;
            OnCurrencyChanged?.Invoke(_softCurrency);
        }

        public bool TrySpendCurrency(int amount)
        {
            if (amount <= 0) return false;
            if (_softCurrency < amount) return false;
            _softCurrency -= amount;
            OnCurrencyChanged?.Invoke(_softCurrency);
            return true;
        }

        public void UnlockBuilding(string buildingId)
        {
            if (string.IsNullOrEmpty(buildingId)) return;
            if (_unlockedBuildingIds == null)
            {
                _unlockedBuildingIds = new List<string>();
            }
            if (_unlockedBuildingIds.Contains(buildingId)) return;
            _unlockedBuildingIds.Add(buildingId);
            OnBuildingUnlocked?.Invoke(buildingId);
        }

        public bool HasUnlockedBuilding(string buildingId)
        {
            if (string.IsNullOrEmpty(buildingId)) return false;
            if (_unlockedBuildingIds == null) return false;
            return _unlockedBuildingIds.Contains(buildingId);
        }

        public void UnlockChunk(string chunkId)
        {
            if (string.IsNullOrEmpty(chunkId)) return;
            if (_unlockedChunkIds == null)
            {
                _unlockedChunkIds = new List<string>();
            }
            if (_unlockedChunkIds.Contains(chunkId)) return;
            _unlockedChunkIds.Add(chunkId);
            OnChunkUnlocked?.Invoke(chunkId);
        }

        public bool HasUnlockedChunk(string chunkId)
        {
            if (string.IsNullOrEmpty(chunkId)) return false;
            if (_unlockedChunkIds == null) return false;
            return _unlockedChunkIds.Contains(chunkId);
        }

        public void ResetData()
        {
            _softCurrency = 0;
            _unlockedBuildingIds.Clear();
            _unlockedChunkIds.Clear();
        }
    }
}
