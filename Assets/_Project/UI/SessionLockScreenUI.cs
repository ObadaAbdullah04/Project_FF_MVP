namespace Project.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class SessionLockScreenUI : BaseMenuPanel
    {
        [Header("Buttons")]
        [SerializeField] private Button _askParentButton;

        private void Start()
        {
            SetTitle("session_lock_message", "Play time is up for today!\nAsk your parent to unlock more time.");

            if (_askParentButton != null)
            {
                _askParentButton.onClick.RemoveAllListeners();
                _askParentButton.onClick.AddListener(OnAskParentPressed);
            }
        }

        public void OnAskParentPressed()
        {
            MenuManager.Instance.ShowParentGate();
        }
    }
}
