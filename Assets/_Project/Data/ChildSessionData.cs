namespace Project.Data
{
    using UnityEngine;
    using System;

    [CreateAssetMenu(fileName = "ChildSessionData", menuName = "Project/Data/Child Session Data")]
    public class ChildSessionData : ScriptableObject
    {
        [SerializeField] private string _currentMiniGameVariationId;

        public string CurrentMiniGameVariationId => _currentMiniGameVariationId;

        public event Action<string> OnVariationChanged;

        public void SetMiniGameVariationId(string variationId)
        {
            variationId = variationId ?? string.Empty;
            if (_currentMiniGameVariationId == variationId) return;
            _currentMiniGameVariationId = variationId;
            OnVariationChanged?.Invoke(_currentMiniGameVariationId);
        }

        public void ResetData()
        {
            _currentMiniGameVariationId = string.Empty;
        }
    }
}
