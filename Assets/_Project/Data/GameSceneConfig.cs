namespace Project.Data
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameSceneConfig", menuName = "Project/Data/Game Scene Config")]
    public class GameSceneConfig : ScriptableObject
    {
        [SerializeField] private string _coreSceneName = "1_Core";
        [SerializeField] private string _hubSceneName = "2_HubWorld";
        [SerializeField] private string _parentGateSceneName = "Parent Gate";
        [SerializeField] private string _parentDashboardSceneName = "ParentDashboard";

        public string CoreSceneName => _coreSceneName;
        public string HubSceneName => _hubSceneName;
        public string ParentGateSceneName => _parentGateSceneName;
        public string ParentDashboardSceneName => _parentDashboardSceneName;
    }
}
