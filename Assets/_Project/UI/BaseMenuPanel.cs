namespace Project.UI
{
    using UnityEngine;
    using Project.Core;
    using RTLTMPro;

    public abstract class BaseMenuPanel : MonoBehaviour
    {
        [Header("Base Panel")]
        [SerializeField] protected RTLTextMeshPro _titleText;

        protected string T(string key, string fallback)
        {
            return LocalizationManager.Instance != null ? LocalizationManager.Instance.GetText(key) : fallback;
        }

        protected void SetTitle(string key, string fallback)
        {
            if (_titleText != null)
                _titleText.text = T(key, fallback);
        }
    }
}