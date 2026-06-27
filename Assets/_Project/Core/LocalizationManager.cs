namespace Project.Core
{
    using System.Collections.Generic;
    using Project.Architecture;
    using Project.Data;
    using UnityEngine;

    public class LocalizationManager : MonoBehaviour
    {
        [SerializeField] private LocalizationData _localizationData;

        private Dictionary<string, string> _localizedTextMap;

        public static LocalizationManager Instance => ServiceLocator.Get<LocalizationManager>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            ServiceLocator.Register<LocalizationManager>(this);
        }

        public void Initialize(LocalizationData data)
        {
            _localizationData = data;
            _localizedTextMap = new Dictionary<string, string>();

            if (data == null)
            {
                return;
            }

            foreach (LocalizedStringPair pair in data.entries)
            {
                if (!_localizedTextMap.ContainsKey(pair.key))
                {
                    _localizedTextMap.Add(pair.key, pair.value);
                }
                else
                {
                }
            }
        }

        public string GetText(string key)
        {
            if (_localizedTextMap != null && _localizedTextMap.TryGetValue(key, out string value))
            {
                return value;
            }

            return key;
        }
    }
}
