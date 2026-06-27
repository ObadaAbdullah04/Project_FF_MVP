namespace Project.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Firebase;
    using Firebase.Database;
    using Project.Architecture;
    using Project.Data;
    using UnityEngine;

    public class FirebaseManager : MonoBehaviour, ISaveSystem
    {
        public static FirebaseManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void Initialize(Action onComplete)
        {
            Task.Run(async () =>
            {
                await Task.Delay(100);

                if (NetworkDispatcher.Instance != null)
                {
                    NetworkDispatcher.Instance.Enqueue(() =>
                    {
                        onComplete?.Invoke();
                    });
                }
            });
        }

        public void SaveInventory(InventoryData inventory, Action onComplete)
        {
            Task.Run(async () =>
            {
                string json = SerializeInventory(inventory);

                await Task.Delay(100);

                if (NetworkDispatcher.Instance != null)
                {
                    NetworkDispatcher.Instance.Enqueue(() =>
                    {
                        onComplete?.Invoke();
                    });
                }
            });
        }

        public void LoadInventory(InventoryData inventory, Action onComplete)
        {
            Task.Run(async () =>
            {
                await Task.Delay(100);

                string json = SerializeInventory(inventory);

                if (NetworkDispatcher.Instance != null)
                {
                    NetworkDispatcher.Instance.Enqueue(() =>
                    {
                        DeserializeInventory(json, inventory);
                        onComplete?.Invoke();
                    });
                }
            });
        }

        private string SerializeInventory(InventoryData inventory)
        {
            var snapshot = new InventorySnapshot
            {
                softCurrency = inventory.SoftCurrency,
                unlockedBuildingIds = inventory.UnlockedBuildingIds != null
                    ? new List<string>(inventory.UnlockedBuildingIds)
                    : new List<string>(),
                unlockedChunkIds = inventory.UnlockedChunkIds != null
                    ? new List<string>(inventory.UnlockedChunkIds)
                    : new List<string>()
            };

            return JsonUtility.ToJson(snapshot);
        }

        private void DeserializeInventory(string json, InventoryData inventory)
        {
            InventorySnapshot snapshot = JsonUtility.FromJson<InventorySnapshot>(json);
            if (snapshot == null) return;

            inventory.ResetData();

            if (snapshot.softCurrency > 0)
            {
                inventory.AddCurrency(snapshot.softCurrency);
            }

            if (snapshot.unlockedBuildingIds != null)
            {
                foreach (string buildingId in snapshot.unlockedBuildingIds)
                {
                    inventory.UnlockBuilding(buildingId);
                }
            }

            if (snapshot.unlockedChunkIds != null)
            {
                foreach (string chunkId in snapshot.unlockedChunkIds)
                {
                    inventory.UnlockChunk(chunkId);
                }
            }

        }

        [Serializable]
        private class InventorySnapshot
        {
            public int softCurrency;
            public List<string> unlockedBuildingIds;
            public List<string> unlockedChunkIds;
        }
    }
}
