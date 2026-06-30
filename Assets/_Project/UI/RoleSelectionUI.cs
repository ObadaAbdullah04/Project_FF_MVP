namespace Project.UI
{
    using Project.Core;
    using UnityEngine;
    using UnityEngine.UI;

    public class RoleSelectionUI : BaseMenuPanel
    {
        [Header("Buttons")]
        [SerializeField] private Button _parentButton;
        [SerializeField] private Button _childButton;

        private void Start()
        {
            SetTitle("role_selection_title", "Who are you?");

            if (_parentButton != null)
            {
                _parentButton.onClick.RemoveAllListeners();
                _parentButton.onClick.AddListener(OnParentSelected);
            }

            if (_childButton != null)
            {
                _childButton.onClick.RemoveAllListeners();
                _childButton.onClick.AddListener(OnChildSelected);
            }
        }

        private void OnParentSelected()
        {
            MenuManager.Instance.ShowParentGate();
        }

        private void OnChildSelected()
        {
            if (DeviceRoleManager.Instance == null) return;

            DeviceRoleManager.Instance.SetRole(DeviceRole.Child);
            MenuManager.Instance.ShowAgeEntry();
        }
    }
}
