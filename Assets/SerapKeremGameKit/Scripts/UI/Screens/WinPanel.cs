using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Game.Scoring;

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
        
        [Header("Score Breakdown")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _timeBonusText;
        [SerializeField] private TextMeshProUGUI _comboBonusText;
        [SerializeField] private TextMeshProUGUI _perfectBonusText;
        [SerializeField] private GameObject _perfectBanner;
        [SerializeField] private GameObject _newBestBadge;
        
        [Header("Buttons")]
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _levelSelectButton;
        
        [Header("References")]
        [SerializeField] private UIRootController _uiRoot;
        [SerializeField] private CoinFlyAnimator _coinFly;
        [SerializeField] private RectTransform _totalCoinTarget;

        private int _pendingReward;
        private Sequence _pendingSequence;
        private Sequence _starSequence;

        private void Awake()
        {
			if (_nextButton != null) _nextButton.BindOnClick(this, OnNextClicked);
			if (_levelSelectButton != null) _levelSelectButton.BindOnClick(this, OnLevelSelectClicked);
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
			if (_starSequence != null && _starSequence.IsActive())
			{
				_starSequence.Kill();
				_starSequence = null;
			}
		}

        public void Setup(int stars, int rewardedCoins, int totalCoins, UIRootController uiRoot)
        {
            SetStars(stars);
            if (_coinText != null) _coinText.text = rewardedCoins.ToString();
            if (_totalCoinText != null) _totalCoinText.text = Mathf.Max(0, totalCoins).ToString();
            _uiRoot = uiRoot;
            _pendingReward = rewardedCoins;
            
            // Hide optional elements by default
            if (_perfectBanner != null) _perfectBanner.SetActive(false);
            if (_newBestBadge != null) _newBestBadge.SetActive(false);
        }

        /// <summary>
        /// Extended setup with score breakdown from ScoreManager.
        /// </summary>
        public void SetupWithScore(int stars, int rewardedCoins, int totalCoins, 
            UIRootController uiRoot, LevelScoreResult scoreResult, bool isNewBest)
        {
            Setup(stars, rewardedCoins, totalCoins, uiRoot);
            
            // Score breakdown
            if (_scoreText != null)
                _scoreText.text = scoreResult.TotalScore.ToString("N0");
            
            if (_timeBonusText != null)
            {
                _timeBonusText.text = scoreResult.TimeBonus > 0 ? $"+{scoreResult.TimeBonus}" : "";
                _timeBonusText.gameObject.SetActive(scoreResult.TimeBonus > 0);
            }
            
            if (_comboBonusText != null)
            {
                string comboText = scoreResult.MaxCombo > 1 ? $"Max Combo: {scoreResult.MaxCombo}x" : "";
                _comboBonusText.text = comboText;
                _comboBonusText.gameObject.SetActive(scoreResult.MaxCombo > 1);
            }
            
            if (_perfectBonusText != null)
            {
                _perfectBonusText.text = scoreResult.IsPerfect ? $"+{scoreResult.PerfectBonus} PERFECT!" : "";
                _perfectBonusText.gameObject.SetActive(scoreResult.IsPerfect);
            }
            
            // Perfect banner
            if (_perfectBanner != null)
            {
                _perfectBanner.SetActive(scoreResult.IsPerfect);
            }
            
            // New best badge
            if (_newBestBadge != null)
            {
                _newBestBadge.SetActive(isNewBest);
            }
        }

        public override void Show(bool playSound = true)
        {
            base.Show(playSound);
            
            // Animate stars with sequential pop-in
            AnimateStars();
            
            // Animate next button with a play-arrow pulse effect
            if (_nextButton != null)
            {
                var btnTransform = _nextButton.transform;
                btnTransform.localScale = Vector3.zero;
                btnTransform.DOScale(Vector3.one, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(0.8f)
                    .SetUpdate(true)
                    .SetAutoKill(true)
                    .SetLink(_nextButton.gameObject, LinkBehaviour.KillOnDestroy);
            }
            
            // Animate new best badge
            if (_newBestBadge != null && _newBestBadge.activeSelf)
            {
                _newBestBadge.transform.localScale = Vector3.zero;
                _newBestBadge.transform.DOScale(1f, 0.4f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(1.2f)
                    .SetUpdate(true)
                    .SetAutoKill(true)
                    .SetLink(_newBestBadge, LinkBehaviour.KillOnDestroy);
                
                // Pulse gold
                _newBestBadge.transform.DOScale(1.1f, 0.6f)
                    .SetDelay(1.8f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetUpdate(true)
                    .SetLink(_newBestBadge, LinkBehaviour.KillOnDestroy);
            }
            
            // Animate perfect banner
            if (_perfectBanner != null && _perfectBanner.activeSelf)
            {
                _perfectBanner.transform.localScale = Vector3.zero;
                _perfectBanner.transform.DOScale(1.2f, 0.3f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(1f)
                    .SetUpdate(true)
                    .SetAutoKill(true)
                    .SetLink(_perfectBanner, LinkBehaviour.KillOnDestroy);
                _perfectBanner.transform.DOScale(1f, 0.15f)
                    .SetDelay(1.3f)
                    .SetUpdate(true)
                    .SetAutoKill(true)
                    .SetLink(_perfectBanner, LinkBehaviour.KillOnDestroy);
            }
        }

        /// <summary>
        /// Animate stars popping in sequentially.
        /// </summary>
        private void AnimateStars()
        {
            _starSequence?.Kill();
            _starSequence = DOTween.Sequence();

            float delay = 0.3f;
            float starDuration = 0.3f;

            GameObject[] stars = { _star1, _star2, _star3 };

            foreach (var star in stars)
            {
                if (star != null && star.activeSelf)
                {
                    star.transform.localScale = Vector3.zero;
                    _starSequence.Append(
                        star.transform.DOScale(1.3f, starDuration)
                            .SetEase(Ease.OutBack)
                    );
                    _starSequence.Append(
                        star.transform.DOScale(1f, starDuration * 0.5f)
                            .SetEase(Ease.InOutQuad)
                    );
                }
            }

            _starSequence.SetDelay(delay)
                .SetUpdate(true)
                .SetAutoKill(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
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
        
        private void OnLevelSelectClicked()
        {
            if (_uiRoot != null) _uiRoot.OnLevelSelectRequested();
        }

		public void SetUIRoot(UIRootController uiRoot)
		{
			_uiRoot = uiRoot;
		}
    }
}