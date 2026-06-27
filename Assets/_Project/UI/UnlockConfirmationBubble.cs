namespace Project.UI
{
    using System;
    using DG.Tweening;
    using Project.Architecture;
    using RTLTMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UnlockConfirmationBubble : TweenableMonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image _coinIcon;
        [SerializeField] private RTLTextMeshPro _costText;
        [SerializeField] private RTLTextMeshPro _rejectionText;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private RectTransform _panel;

        private Action _onConfirm;
        private Action _onCancel;

        protected override void OnDestroy()
        {
            if (_panel != null)
                _panel.DOKill();
            base.OnDestroy();
        }

        public void Setup(int cost, Action onConfirm, Action onCancel)
        {
            _onConfirm = onConfirm;
            _onCancel = onCancel;

            _costText.text = cost.ToString();

            _confirmButton.onClick.RemoveListener(OnConfirmClicked);
            _cancelButton.onClick.RemoveListener(OnCancelClicked);
            _confirmButton.onClick.AddListener(OnConfirmClicked);
            _cancelButton.onClick.AddListener(OnCancelClicked);

            if (_rejectionText != null)
            {
                _rejectionText.gameObject.SetActive(false);
            }

            AnimateIn();
        }

        public void PlayValidationError()
        {
            if (_panel != null)
            {
                _panel.DOShakePosition(0.4f, new Vector3(8f, 0f, 0f), 10, 90f, false, true);
            }

            if (_rejectionText != null)
            {
                _rejectionText.gameObject.SetActive(true);
                DOVirtual.DelayedCall(2f, () =>
                {
                    if (_rejectionText != null)
                        _rejectionText.gameObject.SetActive(false);
                });
            }
        }

        private void AnimateIn()
        {
            if (_panel == null) return;
            _panel.localScale = Vector3.zero;
            _panel.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }

        private void OnConfirmClicked()
        {
            _onConfirm?.Invoke();
        }

        private void OnCancelClicked()
        {
            _onCancel?.Invoke();
        }
    }
}
