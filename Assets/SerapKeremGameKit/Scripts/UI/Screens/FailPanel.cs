using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SerapKeremGameKit._UI
{
	public sealed class FailPanel : UIPanel
    {
        [SerializeField] private Image _failIcon;
        [SerializeField] private TextMeshProUGUI _coinText;
        
        [Header("Buttons")]
        [SerializeField] private Button _restartButton;
        
        [Header("References")]
        [SerializeField] private UIRootController _uiRoot;

		private void Awake()
		{
			if (_restartButton != null) _restartButton.BindOnClick(this, OnRestartClicked);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			// Auto-unsubscribe handled by ButtonExtensions
		}

        public void Setup(int rewardedCoins, UIRootController uiRoot)
        {
            if (_coinText != null) _coinText.text = rewardedCoins.ToString();
            _uiRoot = uiRoot;
        }

        public override void Show(bool playSound = true)
        {
            base.Show(playSound);
            
            // Animate restart button with circular retry entrance
            if (_restartButton != null)
            {
                var btnTransform = _restartButton.transform;
                btnTransform.localScale = Vector3.zero;
                btnTransform.localRotation = Quaternion.Euler(0, 0, -180f);
                
                Sequence seq = DOTween.Sequence();
                seq.Append(btnTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));
                seq.Join(btnTransform.DORotate(Vector3.zero, 0.4f).SetEase(Ease.OutBack));
                seq.SetDelay(0.2f)
                   .SetUpdate(true)
                   .SetAutoKill(true)
                   .SetLink(_restartButton.gameObject, LinkBehaviour.KillOnDestroy);
            }
        }

        private void OnRestartClicked()
        {
            // Punch animation on restart click
            if (_restartButton != null)
            {
                _restartButton.transform.DOPunchScale(Vector3.one * 0.15f, 0.2f, 6, 0.5f)
                    .SetUpdate(true)
                    .SetAutoKill(true)
                    .SetLink(_restartButton.gameObject, LinkBehaviour.KillOnDestroy);
            }
            
			if (_uiRoot != null) _uiRoot.OnRestartConfirmed();
        }

		public void SetUIRoot(UIRootController uiRoot)
		{
			_uiRoot = uiRoot;
		}
    }
}
