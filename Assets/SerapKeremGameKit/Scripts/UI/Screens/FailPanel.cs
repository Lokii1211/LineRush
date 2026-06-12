using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Game.UI;

namespace SerapKeremGameKit._UI
{
	public sealed class FailPanel : UIPanel
    {
        [SerializeField] private Image _failIcon;
        [SerializeField] private TextMeshProUGUI _coinText;
        
        [Header("Buttons")]
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _watchAdButton;
        
        [Header("Watch Ad UI")]
        [SerializeField] private TextMeshProUGUI _watchAdText;
        [SerializeField] private GameObject _watchAdContainer;
        
        [Header("References")]
        [SerializeField] private UIRootController _uiRoot;

		private void Awake()
		{
			if (_restartButton != null) _restartButton.BindOnClick(this, OnRestartClicked);
			if (_watchAdButton != null) _watchAdButton.BindOnClick(this, OnWatchAdClicked);
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
            
            // Setup watch ad button
            if (_watchAdContainer != null)
            {
                _watchAdContainer.SetActive(true);
            }
            if (_watchAdText != null)
            {
                _watchAdText.text = $"Watch Ad for +{AdConfig.REWARDED_EXTRA_LIVES} Lives";
            }
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
            
            // Animate watch ad button
            if (_watchAdButton != null)
            {
                var adBtnTransform = _watchAdButton.transform;
                adBtnTransform.localScale = Vector3.zero;
                adBtnTransform.DOScale(Vector3.one, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(0.5f)
                    .SetUpdate(true)
                    .SetAutoKill(true)
                    .SetLink(_watchAdButton.gameObject, LinkBehaviour.KillOnDestroy);
                
                // Pulse the ad button to draw attention
                adBtnTransform.DOScale(1.05f, 0.8f)
                    .SetDelay(1f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetUpdate(true)
                    .SetLink(_watchAdButton.gameObject, LinkBehaviour.KillOnDestroy);
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
        
        /// <summary>
        /// Handle watch ad button — shows rewarded ad for extra lives.
        /// </summary>
        private void OnWatchAdClicked()
        {
            if (AdManager.Instance == null) return;
            
            // Punch animation
            if (_watchAdButton != null)
            {
                _watchAdButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 4, 0.5f)
                    .SetUpdate(true)
                    .SetAutoKill(true)
                    .SetLink(_watchAdButton.gameObject, LinkBehaviour.KillOnDestroy);
            }
            
            bool adShown = AdManager.Instance.ShowRewardedAdForExtraLives(() =>
            {
                // Reward earned — restore lives and continue
                if (LivesManager.IsInitialized && LivesManager.Instance != null)
                {
                    LivesManager.Instance.ContinueFromFail(AdConfig.REWARDED_EXTRA_LIVES);
                }
                
                // Hide fail panel and resume
                if (_uiRoot != null)
                {
                    _uiRoot.OnContinueAfterAd();
                }
            });
            
            if (!adShown)
            {
                // No ad available — disable button
                if (_watchAdButton != null) _watchAdButton.interactable = false;
                if (_watchAdText != null) _watchAdText.text = "Ad Not Available";
            }
        }

		public void SetUIRoot(UIRootController uiRoot)
		{
			_uiRoot = uiRoot;
		}
    }
}
