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
        [SerializeField] protected string _hubSceneName = "2_HubWorld";
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

            if (_inventoryData == null)
            {
                Debug.LogWarning("CompleteGame: _inventoryData is not assigned — coins not added");
                SceneManager.LoadScene(_hubSceneName);
                return;
            }

            _inventoryData.AddCurrency(currencyEarned);

            if (GameManager.Instance != null)
                GameManager.Instance.SaveGame();

            SceneManager.LoadScene(_hubSceneName);
        }
    }
}
