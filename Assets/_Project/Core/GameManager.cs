namespace Project.Core
{
    using Project.Architecture;
    using Project.Data;
    using UnityEngine;

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance => ServiceLocator.Get<GameManager>();

        [SerializeField] private InventoryData _inventoryData;
        [SerializeField] private MonoBehaviour _saveSystemComponent;

        private ISaveSystem _saveSystem;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            ServiceLocator.Register<GameManager>(this);
            _saveSystem = _saveSystemComponent as ISaveSystem;
            if (_saveSystem != null)
                ServiceLocator.Register<ISaveSystem>(_saveSystem);
        }

        private void Start()
        {
            if (_inventoryData != null)
            {
                _inventoryData.ResetData();
            }

            if (_saveSystem != null && _inventoryData != null)
            {
                _saveSystem.Initialize(() =>
                {
                    _saveSystem.LoadInventory(_inventoryData, null);
                });
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
