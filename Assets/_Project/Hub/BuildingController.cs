namespace Project.Hub
{
    using DG.Tweening;
    using UnityEngine;

    public class BuildingController : MonoBehaviour
    {
        [SerializeField] private string _miniGameScene;

        public string MiniGameScene => _miniGameScene;

        private void OnDestroy()
        {
            transform.DOKill();
        }

        public void PlayUnlockAnimation(float delay = 0f)
        {
            transform.DOPunchScale(Vector3.one * 0.3f, 0.5f, 5, 0.5f).SetDelay(delay);
        }

        public void SetToUnlockedState()
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
        }
    }
}
