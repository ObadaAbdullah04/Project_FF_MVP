namespace Project.Core
{
    using Project.Architecture;
    using Project.Data;
    using UnityEngine;

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private InventoryData _inventoryData;

        private ISaveSystem _saveSystem;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            _saveSystem = GetComponent<ISaveSystem>();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveGame();
            }
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        public void SaveGame()
        {
            if (_saveSystem == null) return;
            if (_inventoryData == null) return;
            _saveSystem.SaveInventory(_inventoryData, () => { });
        }
    }
}
