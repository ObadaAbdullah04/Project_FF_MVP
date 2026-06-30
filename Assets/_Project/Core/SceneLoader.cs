namespace Project.Core
{
    using System;
    using System.Collections;
    using Project.Architecture;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance => ServiceLocator.Get<SceneLoader>();

        // Tracks the current environment/UI scene so we can unload it before loading the next one.
        private string _currentActiveScene = string.Empty;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            ServiceLocator.Register<SceneLoader>(this);
        }

        private void Start()
        {
            EnsureEventSystem();
        }

        private void EnsureEventSystem()
        {
            var es = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (es == null)
            {
                GameObject esGO = new GameObject("PersistentEventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
                DontDestroyOnLoad(esGO);
            }
            else
            {
                DontDestroyOnLoad(es.gameObject);
            }
        }

        private void CleanDuplicateEventSystems()
        {
            var eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>(true);
            if (eventSystems.Length > 1)
            {
                UnityEngine.EventSystems.EventSystem primary = null;
                for (int i = 0; i < eventSystems.Length; i++)
                {
                    var es = eventSystems[i];
                    if (es.gameObject.scene.name == "DontDestroyOnLoad")
                    {
                        primary = es;
                        break;
                    }
                }

                if (primary == null)
                {
                    primary = eventSystems[0];
                }

                for (int i = 0; i < eventSystems.Length; i++)
                {
                    var es = eventSystems[i];
                    if (es != primary)
                    {
                        Debug.Log($"[SceneLoader] Destroying duplicate EventSystem in scene: {es.gameObject.scene.name}");
                        Destroy(es.gameObject);
                    }
                }
            }
        }

        public void LoadSceneAdditively(string sceneName, Action onComplete)
        {
            StartCoroutine(LoadSceneAsyncRoutine(sceneName, LoadSceneMode.Additive, onComplete));
        }

        public void LoadSceneSingle(string sceneName, Action onComplete)
        {
            StartCoroutine(LoadSceneAsyncRoutine(sceneName, LoadSceneMode.Single, onComplete));
        }

        // The new State Machine transition method
        public void TransitionToScene(string sceneName, Action onComplete = null)
        {
            StartCoroutine(TransitionRoutine(sceneName, onComplete));
        }

        public void UnloadScene(string sceneName, Action onComplete)
        {
            StartCoroutine(UnloadSceneAsyncRoutine(sceneName, onComplete));
        }

        private IEnumerator LoadSceneAsyncRoutine(string sceneName, LoadSceneMode mode, Action onComplete)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, mode);

            while (!operation.isDone)
            {
                yield return null;
            }

            CleanDuplicateEventSystems();

            onComplete?.Invoke();
        }

        private IEnumerator UnloadSceneAsyncRoutine(string sceneName, Action onComplete)
        {
            AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);

            if (operation == null)
            {
                Debug.LogWarning($"Cannot unload '{sceneName}' — not loaded additively");
                onComplete?.Invoke();
                yield break;
            }

            while (!operation.isDone)
            {
                yield return null;
            }

            onComplete?.Invoke();
        }

        private IEnumerator TransitionRoutine(string nextSceneName, Action onComplete)
        {
            // 1. If we have a tracked active scene, unload it first
            if (!string.IsNullOrEmpty(_currentActiveScene))
            {
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(_currentActiveScene);
                if (unloadOp != null)
                {
                    while (!unloadOp.isDone)
                    {
                        yield return null;
                    }
                }
            }

            // 2. Load the new scene additively (on top of Bootstrap/Core)
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
            while (!loadOp.isDone)
            {
                yield return null;
            }

            // 3. Set as the active scene for Unity's default instantiations and lighting
            Scene loadedScene = SceneManager.GetSceneByName(nextSceneName);
            if (loadedScene.IsValid())
            {
                SceneManager.SetActiveScene(loadedScene);
            }

            // 4. Camera Management: Turn off the Core camera if the new scene brought its own
            ManageCameras(loadedScene);

            // 5. Track this scene as the one to unload next time
            _currentActiveScene = nextSceneName;

            // 6. Clean up any rogue event systems that were accidentally left in the environment scenes
            CleanDuplicateEventSystems();

            onComplete?.Invoke();
        }

        private void ManageCameras(Scene activeEnvironmentScene)
        {
            Camera[] allCameras = FindObjectsOfType<Camera>(true);
            Camera coreCam = null;
            Camera envCam = null;

            foreach (Camera cam in allCameras)
            {
                // Find the persistent Core camera
                if (cam.gameObject.scene.name == "1_Core" || cam.gameObject.scene.name == "DontDestroyOnLoad")
                {
                    coreCam = cam;
                }
                // Find the newly loaded environment camera
                else if (cam.gameObject.scene == activeEnvironmentScene)
                {
                    envCam = cam;
                }
            }

            // If the Core camera exists, yield rendering AND audio to the environment scene.
            if (coreCam != null)
            {
                bool shouldCoreBeActive = (envCam == null);
                
                // Toggle the Camera
                coreCam.enabled = shouldCoreBeActive;
                
                // Toggle the AudioListener (if one exists on the same object)
                AudioListener coreListener = coreCam.GetComponent<AudioListener>();
                if (coreListener != null)
                {
                    coreListener.enabled = shouldCoreBeActive;
                }
                
                if (shouldCoreBeActive)
                    Debug.Log("[SceneLoader] Enabling Core Camera & AudioListener (No environment camera found).");
                else
                    Debug.Log($"[SceneLoader] Disabling Core Camera & AudioListener (Yielding to {envCam.gameObject.name}).");
            }
        }
    }
}
