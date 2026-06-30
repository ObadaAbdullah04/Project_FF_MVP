namespace Project.Core
{
    using System;
    using Project.Architecture;
    using Project.Data;
    using UnityEngine;

    public class LocalSaveSystem : MonoBehaviour, ISaveSystem
    {
        public const string InventorySaveKey = "OfflineInventorySave";
        public const string ProgressSaveKey = "OfflineProgressSave";

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
            PlayerPrefs.SetString(InventorySaveKey, json);
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

            if (PlayerPrefs.HasKey(InventorySaveKey))
            {
                string json = PlayerPrefs.GetString(InventorySaveKey);
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

        public void SaveProgress(ChildProgressData progress, Action onComplete)
        {
            if (progress == null)
            {
                onComplete?.Invoke();
                return;
            }

            string json = JsonUtility.ToJson(ProgressSnapshot.FromProgress(progress));
            PlayerPrefs.SetString(ProgressSaveKey, json);
            PlayerPrefs.Save();

            onComplete?.Invoke();
        }

        public void LoadProgress(ChildProgressData progress, Action onComplete)
        {
            if (progress == null)
            {
                onComplete?.Invoke();
                return;
            }

            if (PlayerPrefs.HasKey(ProgressSaveKey))
            {
                string json = PlayerPrefs.GetString(ProgressSaveKey);
                try
                {
                    var snapshot = JsonUtility.FromJson<ProgressSnapshot>(json);
                    snapshot?.ApplyTo(progress);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"LocalSaveSystem: Failed to deserialize progress. {ex.Message}");
                }
            }

            onComplete?.Invoke();
        }
    }
}
