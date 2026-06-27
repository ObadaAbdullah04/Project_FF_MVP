namespace Project.Data
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public struct LocalizedStringPair
    {
        public string key;
        public string value;
    }

    [CreateAssetMenu(fileName = "LocalizationData", menuName = "Project/Data/Localization Data")]
    public class LocalizationData : ScriptableObject
    {
        public List<LocalizedStringPair> entries;

        public void ResetData()
        {
            entries = new List<LocalizedStringPair>();
        }
    }
}
