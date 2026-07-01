namespace Project.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Firebase;
    using Firebase.Auth;
    using Firebase.Firestore;
    using Project.Architecture;
    using Project.Data;
    using UnityEngine;

    public class FirebaseManager : MonoBehaviour, ISaveSystem
    {
        public static FirebaseManager Instance => ServiceLocator.Get<FirebaseManager>();

        private FirebaseAuth _auth;
        public string UserId => _auth?.CurrentUser?.UserId;

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
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError($"Firebase Init Failed: {task.Exception}");
                    NetworkDispatcher.Instance?.Enqueue(() => onComplete?.Invoke());
                    return;
                }

                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _auth = FirebaseAuth.DefaultInstance;
                    
                    if (_auth.CurrentUser != null)
                    {
                        // User session was restored from the device!
                        NetworkDispatcher.Instance?.Enqueue(() => 
                        {
                            // Explicitly enable offline persistence (enabled by default, but explicitly set for clarity)
                            // In newer SDKs, PersistenceEnabled is on by default and Settings might be read-only
                            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
                            // FirebaseFirestoreSettings settings = db.Settings;
                            // settings.PersistenceEnabled = true;
                            // db.Settings = settings;

                            // Check if a wipe was requested by the Editor script
                            if (PlayerPrefs.GetInt("PendingCloudWipe", 0) == 1)
                            {
                                db.Collection("users").Document(UserId).Collection("data").Document("inventory").DeleteAsync();
                                db.Collection("users").Document(UserId).Collection("data").Document("progress").DeleteAsync();
                                PlayerPrefs.SetInt("PendingCloudWipe", 0);
                                PlayerPrefs.Save();
                                Debug.Log("[FirebaseManager] Performed requested Cloud Data Wipe for this user.");
                            }

                            Debug.Log($"[FirebaseManager] Restored existing Auth UID: {UserId}. Offline caching enabled.");
                            onComplete?.Invoke();
                        });
                    }
                    else
                    {
                        // No user found, create a new Anonymous user
                        _auth.SignInAnonymouslyAsync().ContinueWith(authTask =>
                        {
                            if (authTask.IsCanceled || authTask.IsFaulted)
                            {
                                Debug.LogError($"Firebase Auth Failed: {authTask.Exception}");
                            }
                            
                            NetworkDispatcher.Instance?.Enqueue(() => 
                            {
                                // Explicitly enable offline persistence (enabled by default)
                                FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
                                // FirebaseFirestoreSettings settings = db.Settings;
                                // settings.PersistenceEnabled = true;
                                // db.Settings = settings;

                                Debug.Log($"[FirebaseManager] Created NEW Auth UID: {UserId}. Offline caching enabled.");
                                onComplete?.Invoke();
                            });
                        });
                    }
                }
                else
                {
                    Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
                    NetworkDispatcher.Instance?.Enqueue(() => onComplete?.Invoke());
                }
            });
        }

        public void SaveInventory(InventoryData inventory, Action onComplete)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                Debug.LogWarning("[FirebaseManager] Cannot save inventory, user not logged in.");
                onComplete?.Invoke();
                return;
            }

            var snapshot = InventorySnapshot.FromInventory(inventory);
            var docData = new Dictionary<string, object>
            {
                { "softCurrency", snapshot.softCurrency },
                { "unlockedBuildingIds", snapshot.unlockedBuildingIds },
                { "unlockedChunkIds", snapshot.unlockedChunkIds }
            };

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference docRef = db.Collection("users").Document(UserId).Collection("data").Document("inventory");

            docRef.SetAsync(docData, SetOptions.MergeAll).ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError($"[FirebaseManager] Failed to save inventory: {task.Exception}");
                }
                else
                {
                    Debug.Log("[FirebaseManager] Inventory saved to Firestore.");
                }

                NetworkDispatcher.Instance?.Enqueue(() => onComplete?.Invoke());
            });
        }

        public void LoadInventory(InventoryData inventory, Action onComplete)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                onComplete?.Invoke();
                return;
            }

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference docRef = db.Collection("users").Document(UserId).Collection("data").Document("inventory");

            docRef.GetSnapshotAsync().ContinueWith(task =>
            {
                try
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogError($"[FirebaseManager] Failed to load inventory: {task.Exception}");
                    }
                    else if (task.Result.Exists)
                    {
                        DocumentSnapshot doc = task.Result;
                        
                        var bIds = new List<string>();
                        if (doc.ContainsField("unlockedBuildingIds"))
                        {
                            var rawList = doc.GetValue<IEnumerable<object>>("unlockedBuildingIds");
                            if (rawList != null) foreach (var o in rawList) bIds.Add(o.ToString());
                        }

                        var cIds = new List<string>();
                        if (doc.ContainsField("unlockedChunkIds"))
                        {
                            var rawList = doc.GetValue<IEnumerable<object>>("unlockedChunkIds");
                            if (rawList != null) foreach (var o in rawList) cIds.Add(o.ToString());
                        }

                        var snapshot = new InventorySnapshot
                        {
                            softCurrency = doc.ContainsField("softCurrency") ? doc.GetValue<int>("softCurrency") : 0,
                            unlockedBuildingIds = bIds,
                            unlockedChunkIds = cIds
                        };
                        
                        NetworkDispatcher.Instance?.Enqueue(() => 
                        {
                            snapshot.ApplyTo(inventory);
                            Debug.Log("[FirebaseManager] Inventory loaded from Firestore.");
                        });
                    }
                    else
                    {
                        Debug.Log("[FirebaseManager] No inventory data found for this user in Firestore.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[FirebaseManager] Error parsing inventory data: {e.Message}");
                }
                finally
                {
                    NetworkDispatcher.Instance?.Enqueue(() => onComplete?.Invoke());
                }
            });
        }

        public void SaveProgress(ChildProgressData progress, Action onComplete)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                Debug.LogWarning("[FirebaseManager] Cannot save progress, user not logged in.");
                onComplete?.Invoke();
                return;
            }

            string json = JsonUtility.ToJson(ProgressSnapshot.FromProgress(progress));
            var docData = new Dictionary<string, object>
            {
                { "progressJson", json }
            };

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference docRef = db.Collection("users").Document(UserId).Collection("data").Document("progress");

            docRef.SetAsync(docData, SetOptions.MergeAll).ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError($"[FirebaseManager] Failed to save progress: {task.Exception}");
                }
                else
                {
                    Debug.Log("[FirebaseManager] Progress saved to Firestore.");
                }

                NetworkDispatcher.Instance?.Enqueue(() => onComplete?.Invoke());
            });
        }

        public void LoadProgress(ChildProgressData progress, Action onComplete)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                onComplete?.Invoke();
                return;
            }

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference docRef = db.Collection("users").Document(UserId).Collection("data").Document("progress");

            docRef.GetSnapshotAsync().ContinueWith(task =>
            {
                try
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogError($"[FirebaseManager] Failed to load progress: {task.Exception}");
                    }
                    else if (task.Result.Exists)
                    {
                        DocumentSnapshot doc = task.Result;
                        if (doc.ContainsField("progressJson"))
                        {
                            string json = doc.GetValue<string>("progressJson");
                            var snapshot = JsonUtility.FromJson<ProgressSnapshot>(json);
                            
                            NetworkDispatcher.Instance?.Enqueue(() => 
                            {
                                snapshot?.ApplyTo(progress);
                                Debug.Log("[FirebaseManager] Progress loaded from Firestore.");
                            });
                        }
                    }
                    else
                    {
                        Debug.Log("[FirebaseManager] No progress data found for this user in Firestore.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[FirebaseManager] Error parsing progress data: {e.Message}");
                }
                finally
                {
                    NetworkDispatcher.Instance?.Enqueue(() => onComplete?.Invoke());
                }
            });
        }
    }
}
