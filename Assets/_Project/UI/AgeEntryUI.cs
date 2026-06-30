namespace Project.UI
{
    using Project.Core;
    using Project.Data;
    using UnityEngine;
    using UnityEngine.UI;

    public class AgeEntryUI : BaseMenuPanel
    {
        [Header("Configuration")]
        [SerializeField] private GameSceneConfig _sceneConfig;

        [Header("Age Buttons (3 to 8)")]
        [SerializeField] private Button _btnAge3;
        [SerializeField] private Button _btnAge4;
        [SerializeField] private Button _btnAge5;
        [SerializeField] private Button _btnAge6;
        [SerializeField] private Button _btnAge7;
        [SerializeField] private Button _btnAge8;

        private void Start()
        {
            SetTitle("age_entry_title", "How old are you?");

            WireButton(_btnAge3, 3);
            WireButton(_btnAge4, 4);
            WireButton(_btnAge5, 5);
            WireButton(_btnAge6, 6);
            WireButton(_btnAge7, 7);
            WireButton(_btnAge8, 8);
        }

        private void WireButton(Button btn, int age)
        {
            if (btn == null) return;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnAgeSelected(age));
        }

        private void OnAgeSelected(int age)
        {
            if (DeviceRoleManager.Instance == null) return;

            AgeTier tier = age <= 5 ? AgeTier.Under6 : AgeTier.Ages6Plus;
            DeviceRoleManager.Instance.SetAgeTier(tier);

            MenuManager.Instance.HideAll();

            if (_sceneConfig == null) return;

            SceneLoader.Instance.TransitionToScene(_sceneConfig.HubSceneName, null);
        }
    }
}
