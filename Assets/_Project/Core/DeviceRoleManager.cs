namespace Project.Core
{
    using UnityEngine;
    using System;
    using Project.Architecture;

    public enum DeviceRole
    {
        Unassigned,
        Parent,
        Child
    }

    public enum AgeTier
    {
        None,
        Under6,
        Ages6Plus
    }

    public class DeviceRoleManager : MonoBehaviour
    {
        public static DeviceRoleManager Instance => ServiceLocator.Get<DeviceRoleManager>();

        public const string RoleKey = "DeviceRole";
        public const string PINKey = "ParentPIN";
        public const string AgeTierKey = "AgeTier";
        public const string TimeLimitKey = "DailyTimeLimitMinutes";
        private const string DefaultPIN = "1234";

        /// <summary>Fired on role change. Currently unused — reserved for future reactive systems.</summary>
        public event Action<DeviceRole> OnRoleChanged;

        // Default time limits
        public const int DefaultUnder6Limit = 30;
        public const int DefaultAges6PlusLimit = 60;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            ServiceLocator.Register<DeviceRoleManager>(this);
        }

        // --- Role ---

        public DeviceRole GetRole()
        {
            string savedRole = PlayerPrefs.GetString(RoleKey, DeviceRole.Unassigned.ToString());
            if (Enum.TryParse(savedRole, out DeviceRole role))
                return role;
            return DeviceRole.Unassigned;
        }

        public void SetRole(DeviceRole role)
        {
            PlayerPrefs.SetString(RoleKey, role.ToString());
            PlayerPrefs.Save();
            OnRoleChanged?.Invoke(role);
        }

        // --- PIN ---

        public string GetPIN()
        {
            return PlayerPrefs.GetString(PINKey, DefaultPIN);
        }

        public void SetPIN(string newPIN)
        {
            if (string.IsNullOrEmpty(newPIN) || newPIN.Length != 4) return;
            PlayerPrefs.SetString(PINKey, newPIN);
            PlayerPrefs.Save();
        }

        public bool ValidatePIN(string inputPIN)
        {
            return GetPIN() == inputPIN;
        }

        // --- Age Tier ---

        public AgeTier GetAgeTier()
        {
            string saved = PlayerPrefs.GetString(AgeTierKey, AgeTier.None.ToString());
            if (Enum.TryParse(saved, out AgeTier tier))
                return tier;
            return AgeTier.None;
        }

        public void SetAgeTier(AgeTier tier)
        {
            PlayerPrefs.SetString(AgeTierKey, tier.ToString());
            PlayerPrefs.Save();
        }

        // --- Daily Time Limit ---

        public int GetDailyTimeLimitMinutes()
        {
            return PlayerPrefs.GetInt(TimeLimitKey, GetDefaultTimeLimit());
        }

        public void SetDailyTimeLimitMinutes(int minutes)
        {
            PlayerPrefs.SetInt(TimeLimitKey, Mathf.Max(5, minutes));
            PlayerPrefs.Save();
        }

        public int GetDefaultTimeLimit()
        {
            return GetAgeTier() == AgeTier.Under6 ? DefaultUnder6Limit : DefaultAges6PlusLimit;
        }

        // --- Reset ---

        public void ResetAll()
        {
            PlayerPrefs.DeleteKey(RoleKey);
            PlayerPrefs.DeleteKey(PINKey);
            PlayerPrefs.DeleteKey(AgeTierKey);
            PlayerPrefs.DeleteKey(TimeLimitKey);
            PlayerPrefs.Save();
        }
    }
}
