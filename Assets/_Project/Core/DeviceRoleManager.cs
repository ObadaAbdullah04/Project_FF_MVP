namespace Project.Core
{
    using UnityEngine;
    using System;

    // Defines the possible operational states of the device.
    public enum DeviceRole
    {
        Unassigned, // Device is clean and needs role selection (onboarding phase)
        Parent,     // Device is in Parent Tracking/Companion Mode
        Child       // Device is in Child Gameplay Mode
    }

    /// <summary>
    /// Singleton manager that handles storing and loading the Device Role and the parent PIN code.
    /// Persists data locally on the device using Unity's PlayerPrefs.
    /// </summary>
    public class DeviceRoleManager : MonoBehaviour
    {
        // Singleton instance reference
        public static DeviceRoleManager Instance { get; private set; }

        // PlayerPrefs storage keys
        private const string RoleKey = "DeviceRole";
        private const string PINKey = "ParentPIN";
        private const string DefaultPIN = "1234";

        // Event triggered when the device role changes (e.g. from parent to child)
        public event Action<DeviceRole> OnRoleChanged;

        private void Awake()
        {
            // Enforce singleton pattern: ensure only one manager survives transitions between scenes
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

        /// <summary>
        /// Retrieves the currently saved device role. Defaults to Unassigned.
        /// </summary>
        public DeviceRole GetRole()
        {
            string savedRole = PlayerPrefs.GetString(RoleKey, DeviceRole.Unassigned.ToString());
            if (Enum.TryParse(savedRole, out DeviceRole role))
            {
                return role;
            }
            return DeviceRole.Unassigned;
        }

        /// <summary>
        /// Saves a new device role to the local PlayerPrefs and alerts listeners.
        /// </summary>
        public void SetRole(DeviceRole role)
        {
            PlayerPrefs.SetString(RoleKey, role.ToString());
            PlayerPrefs.Save();
            OnRoleChanged?.Invoke(role);
        }

        /// <summary>
        /// Retrieves the parent PIN code. Defaults to "1234".
        /// </summary>
        public string GetPIN()
        {
            return PlayerPrefs.GetString(PINKey, DefaultPIN);
        }

        /// <summary>
        /// Saves a new 4-digit PIN. Validates that the input is exactly 4 digits.
        /// </summary>
        public void SetPIN(string newPIN)
        {
            if (string.IsNullOrEmpty(newPIN) || newPIN.Length != 4) return;
            PlayerPrefs.SetString(PINKey, newPIN);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Validates an entered PIN against the saved one.
        /// </summary>
        public bool ValidatePIN(string inputPIN)
        {
            return GetPIN() == inputPIN;
        }

        /// <summary>
        /// Clears saved role and PIN data (reverts device to unassigned).
        /// </summary>
        public void ResetAll()
        {
            PlayerPrefs.DeleteKey(RoleKey);
            PlayerPrefs.DeleteKey(PINKey);
            PlayerPrefs.Save();
        }
    }
}
