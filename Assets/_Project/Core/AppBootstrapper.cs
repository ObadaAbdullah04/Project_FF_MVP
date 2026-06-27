namespace Project.Core
{
    using UnityEngine;
    using Project.Data;

    public class AppBootstrapper : MonoBehaviour
    {
        [Header("System Dependencies")]
        [SerializeField] private LocalizationData _localizationData;

        [Header("Scene Configuration")]
        [SerializeField] private string _coreSceneName = "1_Core";
        [SerializeField] private string _hubSceneName = "2_HubWorld";
        [SerializeField] private string _parentGateSceneName = "Parent Gate";
        [SerializeField] private string _parentDashboardSceneName = "ParentDashboard";

        private void Start()
        {
            RunBootstrapSequence();
        }

        private void RunBootstrapSequence()
        {
            SceneLoader.Instance.LoadSceneAdditively(_coreSceneName, () =>
            {
                if (LocalizationManager.Instance != null)
                {
                    LocalizationManager.Instance.Initialize(_localizationData);
                }
                else
                {
                    return;
                }

                DetermineNextScene();
            });
        }

        private void DetermineNextScene()
        {
            if (DeviceRoleManager.Instance == null)
            {
                SceneLoader.Instance.LoadSceneAdditively(_hubSceneName, null);
                return;
            }

            DeviceRole role = DeviceRoleManager.Instance.GetRole();
            switch (role)
            {
                case DeviceRole.Parent:
                    SceneLoader.Instance.LoadSceneAdditively(_parentDashboardSceneName, null);
                    break;
                case DeviceRole.Child:
                    SceneLoader.Instance.LoadSceneAdditively(_hubSceneName, null);
                    break;
                case DeviceRole.Unassigned:
                default:
                    SceneLoader.Instance.LoadSceneAdditively(_parentGateSceneName, null);
                    break;
            }
        }
    }
}
