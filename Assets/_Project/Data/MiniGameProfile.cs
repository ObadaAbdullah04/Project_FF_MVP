namespace Project.Data
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "MiniGameProfile", menuName = "Project/Data/Mini Game Profile")]
    public class MiniGameProfile : ScriptableObject
    {
        [SerializeField] private string _displayNameKey;
        [SerializeField] private Sprite _icon;
        [SerializeField] [Range(0f, 100f)] private float _educationalWeight;
        [SerializeField] [Range(0f, 100f)] private float _pedagogicalWeight;
        [SerializeField] [Range(0f, 100f)] private float _entertainmentWeight;

        public string DisplayNameKey => _displayNameKey;
        public Sprite Icon => _icon;
        public float EducationalWeight => _educationalWeight;
        public float PedagogicalWeight => _pedagogicalWeight;
        public float EntertainmentWeight => _entertainmentWeight;

        public bool IsValid => Mathf.Approximately(_educationalWeight + _pedagogicalWeight + _entertainmentWeight, 100f);

        private void OnValidate()
        {
            float sum = _educationalWeight + _pedagogicalWeight + _entertainmentWeight;
            if (sum > 100f)
            {
                float excess = sum - 100f;
                _educationalWeight = Mathf.Max(0f, _educationalWeight - excess * (_educationalWeight / sum));
                _pedagogicalWeight = Mathf.Max(0f, _pedagogicalWeight - excess * (_pedagogicalWeight / sum));
                _entertainmentWeight = Mathf.Max(0f, _entertainmentWeight - excess * (_entertainmentWeight / sum));
            }
        }
    }
}
