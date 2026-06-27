namespace Project.Architecture
{
    using System;
    using System.Collections.Concurrent;
    using UnityEngine;

    public class NetworkDispatcher : MonoBehaviour
    {
        private ConcurrentQueue<Action> _executionQueue;

        public static NetworkDispatcher Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                _executionQueue = new ConcurrentQueue<Action>();
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

        private void Update()
        {
            if (_executionQueue == null) return;

            while (_executionQueue.TryDequeue(out Action action))
            {
                action?.Invoke();
            }
        }

        public void Enqueue(Action action)
        {
            if (action == null) return;
            if (_executionQueue == null)
            {
                _executionQueue = new ConcurrentQueue<Action>();
            }
            _executionQueue.Enqueue(action);
        }
    }
}
