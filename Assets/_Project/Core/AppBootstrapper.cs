namespace Project.Core
{
    using UnityEngine;
    using Project.Data;

    public class AppBootstrapper : MonoBehaviour
    {
        [Header("System Dependencies")]
        [SerializeField] private LocalizationData _localizationData;
        [SerializeField] private GameSceneConfig _sceneConfig;

        private void Start()
        {
            RunBootstrapSequence();
        }

        private void RunBootstrapSequence()
        {
            if (_sceneConfig == null)
            {
                Debug.LogError("AppBootstrapper: GameSceneConfig dependency is missing!");
                return;
            }

            SceneLoader.Instance.LoadSceneAdditively(_sceneConfig.CoreSceneName, () =>
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
                SceneLoader.Instance.LoadSceneAdditively(_sceneConfig.HubSceneName, null);
                return;
            }

            DeviceRole role = DeviceRoleManager.Instance.GetRole();
            switch (role)
            {
                case DeviceRole.Parent:
                    SceneLoader.Instance.LoadSceneAdditively(_sceneConfig.ParentDashboardSceneName, null);
                    break;
                case DeviceRole.Child:
                    SceneLoader.Instance.LoadSceneAdditively(_sceneConfig.HubSceneName, null);
                    break;
                case DeviceRole.Unassigned:
                default:
                    SceneLoader.Instance.LoadSceneAdditively(_sceneConfig.ParentGateSceneName, null);
                    break;
            }
        }
    }
}
