using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SerapKeremGameKit._UI
{
	public sealed class WinPanel : UIPanel
    {
        [Header("Stars")]
        [SerializeField] private GameObject _star1;
        [SerializeField] private GameObject _star2;
        [SerializeField] private GameObject _star3;
        [SerializeField] private GameObject _starZeroIcon; // optional icon for 0 stars

        [Header("Reward")]
        [SerializeField] private TextMeshProUGUI _coinText;
        [SerializeField] private TextMeshProUGUI _totalCoinText;
        
        [Header("Buttons")]
        [SerializeField] private Button _nextButton;
        
        [Header("References")]
        [SerializeField] private UIRootController _uiRoot;
        [SerializeField] private CoinFlyAnimator _coinFly;
        [SerializeField] private RectTransform _totalCoinTarget;

        private int _pendingReward;
        private Sequence _pendingSequence;

        private void Awake()
        {
			if (_nextButton != null) _nextButton.BindOnClick(this, OnNextClicked);
        }

		protected override void OnDestroy()
		{
			base.OnDestroy();
			// Listener auto-unsubscribed by ButtonExtensions binding component
			if (_pendingSequence != null && _pendingSequence.IsActive())
			{
				_pendingSequence.Kill();
				_pendingSequence = null;
			}
		}

        public void Setup(int stars, int rewardedCoins, int totalCoins, UIRootController uiRoot)
        {
            SetStars(stars);
            if (_coinText != null) _coinText.text = rewardedCoins.ToString();
            if (_totalCoinText != null) _totalCoinText.text = Mathf.Max(0, totalCoins).ToString();
            _uiRoot = uiRoot;
            _pendingReward = rewardedCoins;
        }

        public override void Show(bool playSound = true)
        {
            base.Show(playSound);
            
            // Animate next button with a play-arrow pulse effect
            if (_nextButton != null)
            {
                var btnTransform = _nextButton.transform;
                btnTransform.localScale = Vector3.zero;
                btnTransform.DOScale(Vector3.one, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(0.3f)
                    .SetUpdate(true)
                    .SetAutoKill(true)
                    .SetLink(_nextButton.gameObject, LinkBehaviour.KillOnDestroy);
            }
        }

        private void OnTotalCoinStep(long value) { }

        private void SetStars(int count)
        {
            if (_star1 != null) _star1.SetActive(count >= 1);
            if (_star2 != null) _star2.SetActive(count >= 2);
            if (_star3 != null) _star3.SetActive(count >= 3);
            if (_starZeroIcon != null) _starZeroIcon.SetActive(count == 0);
        }

        private void OnNextClicked()
        {
            if (_pendingSequence != null && _pendingSequence.IsActive()) return;
            
            // Punch animation on click
            if (_nextButton != null)
            {
                _nextButton.transform.DOPunchScale(Vector3.one * 0.15f, 0.2f, 6, 0.5f)
                    .SetUpdate(true)
                    .SetAutoKill(true)
                    .SetLink(_nextButton.gameObject, LinkBehaviour.KillOnDestroy);
            }
            
            if (_coinFly != null && _totalCoinTarget != null && _coinText != null && _totalCoinText != null)
            {
                long startAmount = 0;
                long.TryParse(_totalCoinText.text, out startAmount);
				_pendingSequence = _coinFly.AnimateAdd(_totalCoinText, _coinText.rectTransform, _totalCoinTarget, startAmount, _pendingReward);
                if (_pendingSequence != null)
                {
					_pendingSequence.SetAutoKill(true).SetLink(gameObject, LinkBehaviour.KillOnDestroy).OnComplete(() =>
                    {
						if (_uiRoot != null) _uiRoot.ProceedNextLevelAfterReward(_pendingReward);
                    });
                    return;
                }
            }
            if (_uiRoot != null) _uiRoot.ProceedNextLevelAfterReward(_pendingReward);
        }

		public void SetUIRoot(UIRootController uiRoot)
		{
			_uiRoot = uiRoot;
		}
    }
}