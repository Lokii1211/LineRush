using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using _Game.UI;
using _Game.Scoring;

namespace _Game.UI
{
    /// <summary>
    /// Represents a single level node in the level select grid.
    /// Shows level number, star rating, and lock/unlock status.
    /// </summary>
    public class LevelSelectNode : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _levelNumberText;
        [SerializeField] private GameObject _star1;
        [SerializeField] private GameObject _star2;
        [SerializeField] private GameObject _star3;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private GameObject _lockIcon;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Visual Settings")]
        [SerializeField] private Color _unlockedColor = new Color(0.15f, 0.12f, 0.25f, 0.9f);
        [SerializeField] private Color _lockedColor = new Color(0.1f, 0.1f, 0.12f, 0.6f);
        [SerializeField] private Color _currentLevelColor = new Color(0f, 0.898f, 1f, 0.3f);
        [SerializeField] private Color _completedColor = new Color(0.15f, 0.2f, 0.15f, 0.9f);

        private int _levelNumber;
        private bool _isUnlocked;
        private bool _isCurrentLevel;
        private System.Action<int> _onLevelSelected;

        /// <summary>
        /// Configure this node for a specific level.
        /// </summary>
        public void Setup(int levelNumber, int bestStars, bool isCompleted, bool isUnlocked, bool isCurrentLevel, System.Action<int> onSelected)
        {
            _levelNumber = levelNumber;
            _isUnlocked = isUnlocked;
            _isCurrentLevel = isCurrentLevel;
            _onLevelSelected = onSelected;

            // Set level number
            if (_levelNumberText != null)
            {
                _levelNumberText.text = levelNumber.ToString();
                _levelNumberText.color = isUnlocked ? ThemeConfig.TextPrimary : ThemeConfig.InactiveColor;
            }

            // Set stars
            SetStars(isCompleted ? bestStars : 0);

            // Set lock icon
            if (_lockIcon != null)
            {
                _lockIcon.SetActive(!isUnlocked);
            }

            // Set background color
            if (_backgroundImage != null)
            {
                if (isCurrentLevel)
                    _backgroundImage.color = _currentLevelColor;
                else if (isCompleted)
                    _backgroundImage.color = _completedColor;
                else if (isUnlocked)
                    _backgroundImage.color = _unlockedColor;
                else
                    _backgroundImage.color = _lockedColor;
            }

            // Set interactability
            if (_button != null)
            {
                _button.interactable = isUnlocked;
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(OnClicked);
            }

            // Set alpha for locked
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = isUnlocked ? 1f : 0.5f;
            }

            // Pulse animation for current level
            if (isCurrentLevel)
            {
                AnimateCurrentLevel();
            }
        }

        private void SetStars(int starCount)
        {
            if (_star1 != null) _star1.SetActive(starCount >= 1);
            if (_star2 != null) _star2.SetActive(starCount >= 2);
            if (_star3 != null) _star3.SetActive(starCount >= 3);
        }

        private void OnClicked()
        {
            if (!_isUnlocked) return;

            // Punch animation
            if (_button != null)
            {
                _button.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 6, 0.5f)
                    .SetUpdate(true)
                    .SetAutoKill(true)
                    .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            }

            _onLevelSelected?.Invoke(_levelNumber);
        }

        private void AnimateCurrentLevel()
        {
            if (_backgroundImage == null) return;

            Color baseColor = _currentLevelColor;
            _backgroundImage.DOFade(0.6f, 0.8f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        /// <summary>
        /// Play unlock animation when this node becomes accessible.
        /// </summary>
        public void PlayUnlockAnimation()
        {
            if (_lockIcon != null)
            {
                _lockIcon.transform.DOScale(0f, 0.3f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => _lockIcon.SetActive(false))
                    .SetUpdate(true)
                    .SetAutoKill(true)
                    .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            }

            transform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 6, 0.5f)
                .SetDelay(0.3f)
                .SetUpdate(true)
                .SetAutoKill(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }
    }
}
