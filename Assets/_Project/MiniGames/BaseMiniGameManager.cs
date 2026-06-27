namespace Project.MiniGames
{
    using Project.Core;
    using Project.Data;
    using Project.UI;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public abstract class BaseMiniGameManager : MonoBehaviour
    {
        [SerializeField] protected InventoryData _inventoryData;
        [SerializeField] protected GameSceneConfig _sceneConfig;
        [SerializeField] protected UniversalMiniGameHUD _hud;

        protected abstract void OnGameStart();

        public void StartGame()
        {
            if (_hud != null)
                _hud.Initialize(this);

            OnGameStart();
        }

        public void CompleteGame(int currencyEarned)
        {
            if (_hud != null)
                _hud.Cleanup();

            if (_sceneConfig == null)
            {
                Debug.LogError("CompleteGame: _sceneConfig is not assigned — cannot determine hub scene");
                return;
            }

            if (_inventoryData == null)
            {
                Debug.LogWarning("CompleteGame: _inventoryData is not assigned — coins not added");
                SceneManager.LoadScene(_sceneConfig.HubSceneName);
                return;
            }

            _inventoryData.AddCurrency(currencyEarned);

            if (GameManager.Instance != null)
                GameManager.Instance.SaveGame();

            SceneManager.LoadScene(_sceneConfig.HubSceneName);
        }
    }
}
