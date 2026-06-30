namespace Project.MiniGames
{
    using DG.Tweening;
    using UnityEngine;

    public class UniversalGameController : BaseMiniGameManager
    {
        [SerializeField] private GameObject _templateRoot;

        private void Start()
        {
            StartGame();
        }

        protected override void OnGameStart()
        {
            if (_templateRoot != null)
            {
                _templateRoot.SetActive(true);
            }
        }

        protected void OnDisable()
        {
            DOTween.Kill(this);
            if (_templateRoot != null)
            {
                _templateRoot.transform.DOKill();
            }
        }
    }
}
