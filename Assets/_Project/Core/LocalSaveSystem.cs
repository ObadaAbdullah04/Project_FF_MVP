namespace Project.Core
{
    using System;
    using Project.Architecture;
    using Project.Data;
    using UnityEngine;

    public class LocalSaveSystem : MonoBehaviour, ISaveSystem
    {
        private const string SaveKey = "OfflineInventorySave";

        public void Initialize(Action onComplete)
        {
            onComplete?.Invoke();
        }

        public void SaveInventory(InventoryData inventory, Action onComplete)
        {
            if (inventory == null)
            {
                onComplete?.Invoke();
                return;
            }

            string json = JsonUtility.ToJson(InventorySnapshot.FromInventory(inventory));
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();

            onComplete?.Invoke();
        }

        public void LoadInventory(InventoryData inventory, Action onComplete)
        {
            if (inventory == null)
            {
                onComplete?.Invoke();
                return;
            }

            if (PlayerPrefs.HasKey(SaveKey))
            {
                string json = PlayerPrefs.GetString(SaveKey);
                try
                {
                    var snapshot = JsonUtility.FromJson<InventorySnapshot>(json);
                    snapshot?.ApplyTo(inventory);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"LocalSaveSystem: Failed to deserialize inventory. {ex.Message}");
                }
            }

            onComplete?.Invoke();
        }
    }
}
