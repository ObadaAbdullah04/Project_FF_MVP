#if UNITY_EDITOR
namespace Project.Editor
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Simple editor utility to reset device roles during local testing.
    /// Access this from the top Unity menu bar: Tools > Parent Gate Setup > Reset Saved Device Role
    /// </summary>
    public static class ParentGateSetupTool
    {
        [MenuItem("Tools/Parent Gate Setup/Reset Saved Device Role")]
        public static void ResetDeviceRole()
        {
            // Delete player preference keys that store the device role and parent PIN
            PlayerPrefs.DeleteKey("DeviceRole");
            PlayerPrefs.DeleteKey("ParentPIN");
            PlayerPrefs.Save();

            Debug.Log("Saved Device Role and PIN have been successfully reset!");
            EditorUtility.DisplayDialog("Reset Completed", "Device role and PIN have been reset to Unassigned.", "OK");
        }
    }
}
#endif
