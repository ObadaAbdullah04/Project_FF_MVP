namespace Project.Core
{
    using System;
    using Project.Architecture;
    using Project.Data;
    using UnityEngine;

    public class SessionTimer : MonoBehaviour
    {
        public static SessionTimer Instance => ServiceLocator.Get<SessionTimer>();

        public const string TodayDateKey = "SessionTimer_Date";
        public const string TodayDurationKey = "SessionTimer_Duration";

        private float _currentSessionDuration;
        private bool _isSessionActive;

        public event Action OnSessionExpired;

        public float TodayRemainingMinutes
        {
            get
            {
                int limit = DeviceRoleManager.Instance != null
                    ? DeviceRoleManager.Instance.GetDailyTimeLimitMinutes()
                    : DeviceRoleManager.DefaultUnder6Limit;
                float used = GetTodayDuration();
                return Mathf.Max(0f, limit - used);
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            ServiceLocator.Register<SessionTimer>(this);
        }

        private void Start()
        {
            OnSessionExpired += HandleSessionExpired;
        }

        private void OnDestroy()
        {
            OnSessionExpired -= HandleSessionExpired;
        }

        private void HandleSessionExpired()
        {
            Project.UI.MenuManager.Instance.ShowSessionLock();
        }

        private void Update()
        {
            if (_isSessionActive)
            {
                _currentSessionDuration += Time.unscaledDeltaTime;

                float totalToday = GetTodayDuration() + _currentSessionDuration;
                int limit = DeviceRoleManager.Instance != null
                    ? DeviceRoleManager.Instance.GetDailyTimeLimitMinutes()
                    : DeviceRoleManager.DefaultUnder6Limit;

                if (totalToday >= limit * 60f)
                {
                    _isSessionActive = false;
                    FlushTodayDuration();
                    OnSessionExpired?.Invoke();
                }
            }
        }

        public void StartSession()
        {
            if (IsSessionExpired()) return;
            _currentSessionDuration = 0f;
            _isSessionActive = true;
        }

        public void PauseSession()
        {
            if (!_isSessionActive) return;
            _isSessionActive = false;
            FlushTodayDuration();
        }

        public void ResumeSession()
        {
            if (IsSessionExpired()) return;
            _currentSessionDuration = 0f;
            _isSessionActive = true;
        }

        public void EndSession()
        {
            _isSessionActive = false;
            FlushTodayDuration();
        }

        public bool IsSessionExpired()
        {
            string todayDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            string savedDate = PlayerPrefs.GetString(TodayDateKey, "");

            if (savedDate != todayDate)
            {
                PlayerPrefs.SetString(TodayDateKey, todayDate);
                PlayerPrefs.SetFloat(TodayDurationKey, 0f);
                PlayerPrefs.Save();
                return false;
            }

            float usedSeconds = PlayerPrefs.GetFloat(TodayDurationKey, 0f);
            int limit = DeviceRoleManager.Instance != null
                ? DeviceRoleManager.Instance.GetDailyTimeLimitMinutes()
                : DeviceRoleManager.DefaultUnder6Limit;

            return usedSeconds >= limit * 60f;
        }

        private float GetTodayDuration()
        {
            string todayDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            string savedDate = PlayerPrefs.GetString(TodayDateKey, "");

            if (savedDate != todayDate)
                return 0f;

            return PlayerPrefs.GetFloat(TodayDurationKey, 0f);
        }

        private void FlushTodayDuration()
        {
            float existing = GetTodayDuration();
            PlayerPrefs.SetFloat(TodayDurationKey, existing + _currentSessionDuration);
            PlayerPrefs.SetString(TodayDateKey, DateTime.UtcNow.ToString("yyyy-MM-dd"));
            PlayerPrefs.Save();
            _currentSessionDuration = 0f;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && _isSessionActive)
                PauseSession();
        }

        private void OnApplicationQuit()
        {
            if (_isSessionActive)
                PauseSession();
        }
    }
}
