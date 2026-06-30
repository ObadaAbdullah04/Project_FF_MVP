#if UNITY_EDITOR
namespace Project.Editor
{
    using Project.Core;
    using Project.Data;
    using UnityEditor;
    using UnityEngine;

    public static class WipeSaveDataTool
    {
        private const string InventoryPath = "Assets/_Project/Data/InventoryData_SO.asset";
        private const string ProgressPath = "Assets/_Project/Data/ChildProgressData_SO.asset";

        [MenuItem("Tools/Dev/Wipe All Save Data", priority = 1000)]
        public static void WipeAllData()
        {
            if (!EditorUtility.DisplayDialog("Wipe All Save Data",
                    "This will reset EVERYTHING to factory fresh:\n" +
                    "- Chunks: all locked\n" +
                    "- Coins: set to 0\n" +
                    "- Game history: cleared\n" +
                    "- Session history: cleared\n" +
                    "- Device role: unassigned (you'll pick again)\n" +
                    "- Parent PIN: cleared\n" +
                    "- Age tier: cleared\n" +
                    "- Daily session timer: reset\n" +
                    "- PlayerPrefs saves: deleted\n\n" +
                    "This CANNOT be undone. Proceed?",
                    "Wipe Everything", "Cancel"))
                return;

            InventoryData inventory = AssetDatabase.LoadAssetAtPath<InventoryData>(InventoryPath);
            if (inventory != null)
            {
                SerializedObject so = new SerializedObject(inventory);
                so.FindProperty("_softCurrency").intValue = 0;
                SerializedProperty chunks = so.FindProperty("_unlockedChunkIds");
                chunks.ClearArray();
                SerializedProperty buildings = so.FindProperty("_unlockedBuildingIds");
                buildings.ClearArray();
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(inventory);
                Debug.Log("Inventory: reset (0 coins, no unlocked chunks/buildings)");
            }
            else
            {
                Debug.LogError($"InventoryData asset not found at {InventoryPath}");
            }

            ChildProgressData progress = AssetDatabase.LoadAssetAtPath<ChildProgressData>(ProgressPath);
            if (progress != null)
            {
                SerializedObject so = new SerializedObject(progress);
                SerializedProperty history = so.FindProperty("_gameHistory");
                history.ClearArray();
                SerializedProperty sessions = so.FindProperty("_sessionHistory");
                sessions.ClearArray();
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(progress);
                Debug.Log("ChildProgress: reset (no game/session history)");
            }
            else
            {
                Debug.LogError($"ChildProgressData asset not found at {ProgressPath}");
            }

            PlayerPrefs.DeleteKey(LocalSaveSystem.InventorySaveKey);
            PlayerPrefs.DeleteKey(LocalSaveSystem.ProgressSaveKey);
            PlayerPrefs.DeleteKey(DeviceRoleManager.RoleKey);
            PlayerPrefs.DeleteKey(DeviceRoleManager.PINKey);
            PlayerPrefs.DeleteKey(DeviceRoleManager.AgeTierKey);
            PlayerPrefs.DeleteKey(DeviceRoleManager.TimeLimitKey);
            PlayerPrefs.DeleteKey(SessionTimer.TodayDateKey);
            PlayerPrefs.DeleteKey(SessionTimer.TodayDurationKey);
            PlayerPrefs.Save();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("All save data wiped. Assets saved.");
            EditorUtility.DisplayDialog("Done",
                "Full factory reset complete.\n" +
                "- Chunks locked, coins 0\n" +
                "- Role/PIN/age tier cleared\n" +
                "- Session timer reset\n\n" +
                "Next Play: you'll see the role selection screen.", "OK");
        }
    }
}
#endif
