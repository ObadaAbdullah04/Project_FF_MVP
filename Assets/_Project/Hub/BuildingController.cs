namespace Project.Hub
{
    using DG.Tweening;
    using Project.Architecture;
    using UnityEngine;

    public class BuildingController : TweenableMonoBehaviour
    {
        [SerializeField] private string _miniGameScene;

        public string MiniGameScene => _miniGameScene;

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
