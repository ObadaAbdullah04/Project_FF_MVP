namespace Project.UI
{
    using RTLTMPro;
    using UnityEngine;

    [RequireComponent(typeof(RTLTextMeshPro))]
    public class LocalizedRTLText : MonoBehaviour
    {
        [SerializeField] private string _localizationKey;

        private RTLTextMeshPro _text;

        private void Awake()
        {
            _text = GetComponent<RTLTextMeshPro>();
        }

        private void Start()
        {
            UpdateText();
        }

        public void SetKey(string key)
        {
            _localizationKey = key;
            UpdateText();
        }

        public void UpdateText()
        {
            if (_text == null) return;
            if (Core.LocalizationManager.Instance == null) return;
            _text.text = Core.LocalizationManager.Instance.GetText(_localizationKey);
        }
    }
}
