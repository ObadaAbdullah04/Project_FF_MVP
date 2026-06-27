namespace Project.Core
{
    using System;
    using System.Threading.Tasks;
    using Firebase;
    using Firebase.Database;
    using Project.Architecture;
    using Project.Data;
    using UnityEngine;

    public class FirebaseManager : MonoBehaviour, ISaveSystem
    {
        public static FirebaseManager Instance => ServiceLocator.Get<FirebaseManager>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            ServiceLocator.Register<FirebaseManager>(this);
        }

        public void Initialize(Action onComplete)
        {
            Task.Run(async () =>
            {
                await Task.Delay(100);

                if (NetworkDispatcher.Instance != null)
                {
                    NetworkDispatcher.Instance.Enqueue(() =>
                    {
                        onComplete?.Invoke();
                    });
                }
            });
        }

        public void SaveInventory(InventoryData inventory, Action onComplete)
        {
            Task.Run(async () =>
            {
                string json = JsonUtility.ToJson(InventorySnapshot.FromInventory(inventory));

                await Task.Delay(100);

                if (NetworkDispatcher.Instance != null)
                {
                    NetworkDispatcher.Instance.Enqueue(() =>
                    {
                        onComplete?.Invoke();
                    });
                }
            });
        }

        public void LoadInventory(InventoryData inventory, Action onComplete)
        {
            Task.Run(async () =>
            {
                await Task.Delay(100);

                string json = JsonUtility.ToJson(InventorySnapshot.FromInventory(inventory));

                if (NetworkDispatcher.Instance != null)
                {
                    NetworkDispatcher.Instance.Enqueue(() =>
                    {
                        var snapshot = JsonUtility.FromJson<InventorySnapshot>(json);
                        snapshot?.ApplyTo(inventory);
                        onComplete?.Invoke();
                    });
                }
            });
        }
    }
}
