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

        public void LoadSceneAdditively(string sceneName, Action onComplete)
        {
            StartCoroutine(LoadSceneAsyncRoutine(sceneName, LoadSceneMode.Additive, onComplete));
        }

        public void LoadSceneSingle(string sceneName, Action onComplete)
        {
            StartCoroutine(LoadSceneAsyncRoutine(sceneName, LoadSceneMode.Single, onComplete));
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
    }
}
