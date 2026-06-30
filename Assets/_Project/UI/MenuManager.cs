namespace Project.UI
{
    using UnityEngine;
    using Project.Architecture;

    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance => ServiceLocator.Get<MenuManager>();

        [Header("Panels")]
        [SerializeField] private GameObject _roleSelectionPanel;
        [SerializeField] private GameObject _ageEntryPanel;
        [SerializeField] private GameObject _parentGatePanel;
        [SerializeField] private GameObject _parentDashboardPanel;
        [SerializeField] private GameObject _sessionLockPanel;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            // This object lives in 1_Core, so it persists forever
            ServiceLocator.Register<MenuManager>(this);
            HideAll();
        }

        public void HideAll()
        {
            if (_roleSelectionPanel != null) _roleSelectionPanel.SetActive(false);
            if (_ageEntryPanel != null) _ageEntryPanel.SetActive(false);
            if (_parentGatePanel != null) _parentGatePanel.SetActive(false);
            if (_parentDashboardPanel != null) _parentDashboardPanel.SetActive(false);
            if (_sessionLockPanel != null) _sessionLockPanel.SetActive(false);
            
            // Unfreeze the game world
            Time.timeScale = 1f;
        }

        private void PrepareMenu()
        {
            HideAll();
            // Freeze the 3D/2D game world to prevent ghost clicks behind the UI
            Time.timeScale = 0f;
        }

        public void ShowRoleSelection() { PrepareMenu(); if (_roleSelectionPanel != null) _roleSelectionPanel.SetActive(true); }
        public void ShowAgeEntry() { PrepareMenu(); if (_ageEntryPanel != null) _ageEntryPanel.SetActive(true); }
        public void ShowParentGate() { PrepareMenu(); if (_parentGatePanel != null) _parentGatePanel.SetActive(true); }
        public void ShowParentDashboard() { PrepareMenu(); if (_parentDashboardPanel != null) _parentDashboardPanel.SetActive(true); }
        public void ShowSessionLock() { PrepareMenu(); if (_sessionLockPanel != null) _sessionLockPanel.SetActive(true); }
    }
}