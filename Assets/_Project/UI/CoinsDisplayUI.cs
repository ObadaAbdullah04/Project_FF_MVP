namespace Project.UI
{
    using UnityEngine;
    using Project.Data;
    using RTLTMPro;

    public class CoinsDisplayUI : MonoBehaviour
    {
        [SerializeField] private InventoryData _inventoryData;
        [SerializeField] private RTLTextMeshPro _coinsText;

        private void OnEnable()
        {
            if (_inventoryData != null)
            {
                _inventoryData.OnCurrencyChanged += UpdateCoinsDisplay;
            }
            UpdateCoinsDisplay(GetCoinsCount());
        }

        private void OnDisable()
        {
            if (_inventoryData != null)
            {
                _inventoryData.OnCurrencyChanged -= UpdateCoinsDisplay;
            }
        }

        private int GetCoinsCount()
        {
            return _inventoryData != null ? _inventoryData.SoftCurrency : 0;
        }

        private void UpdateCoinsDisplay(int coins)
        {
            if (_coinsText != null)
            {
                string label = Core.LocalizationManager.Instance != null 
                    ? Core.LocalizationManager.Instance.GetText("hud_coins_display") 
                    : "النقود";
                _coinsText.text = $"{label}: {coins}";
            }
        }
    }
}
