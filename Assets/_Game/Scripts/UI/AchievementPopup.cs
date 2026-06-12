using UnityEngine;
using DG.Tweening;
using TMPro;
using _Game.UI;

namespace _Game.Gameplay
{
    /// <summary>
    /// Achievement unlock notification popup for Line Rush.
    /// Slides in from top when an achievement is unlocked.
    /// </summary>
    public class AchievementPopup : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RectTransform _popupPanel;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _iconText;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Animation Settings")]
        [SerializeField] private float _slideInDuration = 0.5f;
        [SerializeField] private float _holdDuration = 3f;
        [SerializeField] private float _slideOutDuration = 0.4f;
        [SerializeField] private float _slideDistance = 200f;

        private Sequence _currentSequence;
        private System.Collections.Generic.Queue<Achievement> _queue = new System.Collections.Generic.Queue<Achievement>();
        private bool _isShowing;

        private void Awake()
        {
            if (_canvasGroup != null) _canvasGroup.alpha = 0;
            if (_popupPanel != null) _popupPanel.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnAchievementUnlocked += QueueAchievement;
            }
        }

        private void OnDisable()
        {
            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.OnAchievementUnlocked -= QueueAchievement;
            }
        }

        /// <summary>
        /// Queue an achievement for display.
        /// </summary>
        public void QueueAchievement(Achievement achievement)
        {
            _queue.Enqueue(achievement);
            if (!_isShowing)
            {
                ShowNext();
            }
        }

        private void ShowNext()
        {
            if (_queue.Count == 0)
            {
                _isShowing = false;
                return;
            }

            _isShowing = true;
            var achievement = _queue.Dequeue();
            ShowPopup(achievement);
        }

        /// <summary>
        /// Display the achievement popup with slide animation.
        /// </summary>
        private void ShowPopup(Achievement achievement)
        {
            if (_popupPanel == null) return;

            // Set content
            if (_titleText != null)
            {
                _titleText.text = achievement.Name;
                _titleText.color = ThemeConfig.WarningColor; // Gold for achievements
            }
            if (_descriptionText != null)
            {
                _descriptionText.text = achievement.Description;
                _descriptionText.color = ThemeConfig.TextSecondary;
            }
            if (_iconText != null)
            {
                _iconText.text = achievement.Icon;
            }

            // Setup initial state
            _popupPanel.gameObject.SetActive(true);
            Vector2 startPos = _popupPanel.anchoredPosition;
            Vector2 hidePos = startPos + Vector2.up * _slideDistance;
            _popupPanel.anchoredPosition = hidePos;

            // Animate
            _currentSequence?.Kill();
            _currentSequence = DOTween.Sequence();

            // Slide in
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0;
                _currentSequence.Append(_canvasGroup.DOFade(1f, _slideInDuration));
            }
            _currentSequence.Join(_popupPanel.DOAnchorPos(startPos, _slideInDuration).SetEase(Ease.OutBack));

            // Hold
            _currentSequence.AppendInterval(_holdDuration);

            // Slide out
            if (_canvasGroup != null)
            {
                _currentSequence.Append(_canvasGroup.DOFade(0f, _slideOutDuration));
            }
            _currentSequence.Join(_popupPanel.DOAnchorPos(hidePos, _slideOutDuration).SetEase(Ease.InBack));

            // Cleanup and show next
            _currentSequence.OnComplete(() =>
            {
                if (_popupPanel != null) _popupPanel.gameObject.SetActive(false);
                ShowNext();
            });

            _currentSequence.SetUpdate(true)
                .SetAutoKill(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        private void OnDestroy()
        {
            _currentSequence?.Kill();
        }
    }
}
