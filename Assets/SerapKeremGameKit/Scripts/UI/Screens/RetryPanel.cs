using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SerapKeremGameKit._UI
{
	public sealed class RetryPanel : UIPanel
    {
        [Header("Buttons")]
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        
        [Header("References")]
        [SerializeField] private UIRootController _uiRoot;

		private void Awake()
		{
			if (_yesButton != null) _yesButton.BindOnClick(this, OnYes);
			if (_noButton != null) _noButton.BindOnClick(this, OnNo);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			// Auto-unsubscribe handled by ButtonExtensions
		}

        public override void Show(bool playSound = true)
        {
            base.Show(playSound);
            
            // Animate buttons with staggered entrance
            AnimateButtonEntrance(_yesButton, 0.15f);
            AnimateButtonEntrance(_noButton, 0.25f);
        }

        private void AnimateButtonEntrance(Button button, float delay)
        {
            if (button == null) return;
            
            var btnTransform = button.transform;
            btnTransform.localScale = Vector3.zero;
            btnTransform.DOScale(Vector3.one, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay)
                .SetUpdate(true)
                .SetAutoKill(true)
                .SetLink(button.gameObject, LinkBehaviour.KillOnDestroy);
        }

        private void OnYes()
        {
            // Confirmation punch animation
            if (_yesButton != null)
            {
                _yesButton.transform.DOPunchScale(Vector3.one * 0.15f, 0.2f, 6, 0.5f)
                    .SetUpdate(true)
                    .SetAutoKill(true)
                    .SetLink(_yesButton.gameObject, LinkBehaviour.KillOnDestroy);
            }
            
			if (_uiRoot != null) _uiRoot.OnRestartConfirmed();
        }

        private void OnNo()
        {
            Hide();
        }

		public void SetUIRoot(UIRootController uiRoot)
		{
			_uiRoot = uiRoot;
		}
    }
}