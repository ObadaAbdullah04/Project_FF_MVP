namespace Project.Core
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void LoadSceneAdditively(string sceneName, Action onComplete)
        {
            StartCoroutine(LoadSceneAsyncRoutine(sceneName, onComplete));
        }

        public void UnloadScene(string sceneName, Action onComplete)
        {
            StartCoroutine(UnloadSceneAsyncRoutine(sceneName, onComplete));
        }

        private IEnumerator LoadSceneAsyncRoutine(string sceneName, Action onComplete)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

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
