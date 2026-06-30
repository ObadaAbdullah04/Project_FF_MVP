namespace Project.Core
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Project.Data;
    using System.Collections;

    public class AppBootstrapper : MonoBehaviour
    {
        [Header("System Dependencies")]
        [SerializeField] private LocalizationData _localizationData;
        [SerializeField] private GameSceneConfig _sceneConfig;

        private void Awake()
        {
            if (_sceneConfig != null)
                Project.Architecture.ServiceLocator.Register<GameSceneConfig>(_sceneConfig);
        }

        private void Start()
        {
            StartCoroutine(RunBootstrapSequence());
        }

        private IEnumerator RunBootstrapSequence()
        {
            if (_sceneConfig == null)
            {
                Debug.LogError("AppBootstrapper: GameSceneConfig dependency is missing!");
                yield break;
            }

            // FIX: Load 1_Core using raw SceneManager because SceneLoader lives INSIDE 1_Core.
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneConfig.CoreSceneName, LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // At this point, 1_Core is fully loaded and Singletons are awake.
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.Initialize(_localizationData);
            }
            else
            {
                Debug.LogWarning("AppBootstrapper: LocalizationManager missing in Core.");
            }

            DetermineNextScene();
        }

        private void DetermineNextScene()
        {
            if (DeviceRoleManager.Instance == null)
            {
                SceneLoader.Instance.TransitionToScene(_sceneConfig.HubSceneName, null);
                return;
            }

            DeviceRole role = DeviceRoleManager.Instance.GetRole();

            if (role == DeviceRole.Parent)
            {
                Project.UI.MenuManager.Instance.ShowParentGate();
                return;
            }

            if (role == DeviceRole.Unassigned)
            {
                Project.UI.MenuManager.Instance.ShowRoleSelection();
                return;
            }

            // role == DeviceRole.Child
            if (DeviceRoleManager.Instance.GetAgeTier() == AgeTier.None)
            {
                Project.UI.MenuManager.Instance.ShowAgeEntry();
                return;
            }

            if (SessionTimer.Instance != null && SessionTimer.Instance.IsSessionExpired())
            {
                Project.UI.MenuManager.Instance.ShowSessionLock();
                return;
            }

            if (SessionTimer.Instance != null)
                SessionTimer.Instance.StartSession();

            SceneLoader.Instance.TransitionToScene(_sceneConfig.HubSceneName, null);
        }
    }
}
