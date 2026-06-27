namespace Project.Architecture
{
    using DG.Tweening;
    using UnityEngine;

    public abstract class TweenableMonoBehaviour : MonoBehaviour
    {
        protected virtual void OnDestroy()
        {
            transform.DOKill();
        }
    }
}
