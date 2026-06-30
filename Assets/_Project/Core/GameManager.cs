namespace Project.Core
{
    using Project.Architecture;
    using Project.Data;
    using UnityEngine;

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance => ServiceLocator.Get<GameManager>();

        [SerializeField] private InventoryData _inventoryData;
        [SerializeField] private ChildProgressData _childProgressData;
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

            if (_childProgressData != null)
            {
                _childProgressData.ResetData();
            }

            if (_saveSystem != null)
            {
                _saveSystem.Initialize(() =>
                {
                    if (_inventoryData != null)
                        _saveSystem.LoadInventory(_inventoryData, null);
                    if (_childProgressData != null)
                        _saveSystem.LoadProgress(_childProgressData, null);
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
            if (_inventoryData != null)
                _saveSystem.SaveInventory(_inventoryData, () => { });
            if (_childProgressData != null)
                _saveSystem.SaveProgress(_childProgressData, () => { });
        }

        public void ResetAllData()
        {
            if (_inventoryData != null)
                _inventoryData.ResetData();
            if (_childProgressData != null)
                _childProgressData.ResetData();

            PlayerPrefs.DeleteKey(LocalSaveSystem.InventorySaveKey);
            PlayerPrefs.DeleteKey(LocalSaveSystem.ProgressSaveKey);
            PlayerPrefs.Save();

            SaveGame();
        }
    }
}
