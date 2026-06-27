namespace Project.Architecture
{
    using System;
    using System.Collections.Concurrent;
    using UnityEngine;

    public class NetworkDispatcher : MonoBehaviour
    {
        private ConcurrentQueue<Action> _executionQueue;

        public static NetworkDispatcher Instance => ServiceLocator.Get<NetworkDispatcher>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            ServiceLocator.Register<NetworkDispatcher>(this);
            _executionQueue = new ConcurrentQueue<Action>();
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
