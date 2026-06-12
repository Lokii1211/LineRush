using UnityEngine;
using DG.Tweening;
using TMPro;
using _Game.UI;

namespace _Game.Scoring
{
    /// <summary>
    /// Displays combo text and perfect clear notifications in-game.
    /// Shows floating animated text for combo multipliers.
    /// </summary>
    public class ComboDisplay : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _comboText;
        [SerializeField] private TextMeshProUGUI _perfectText;
        [SerializeField] private CanvasGroup _comboCanvasGroup;
        [SerializeField] private CanvasGroup _perfectCanvasGroup;

        [Header("Animation Settings")]
        [SerializeField] private float _showDuration = 0.3f;
        [SerializeField] private float _holdDuration = 1.0f;
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private float _scaleOvershoot = 1.3f;

        private Sequence _comboSequence;
        private Sequence _perfectSequence;

        private void Awake()
        {
            if (_comboCanvasGroup != null) _comboCanvasGroup.alpha = 0;
            if (_perfectCanvasGroup != null) _perfectCanvasGroup.alpha = 0;
        }

        private void OnEnable()
        {
            if (ScoreManager.IsInitialized)
            {
                ScoreManager.Instance.OnComboChanged += HandleComboChanged;
                ScoreManager.Instance.OnComboReset += HandleComboReset;
            }
        }

        private void OnDisable()
        {
            if (ScoreManager.IsInitialized)
            {
                ScoreManager.Instance.OnComboChanged -= HandleComboChanged;
                ScoreManager.Instance.OnComboReset -= HandleComboReset;
            }
        }

        private void HandleComboChanged(int combo, float multiplier)
        {
            if (combo < 2) return; // Don't show for first clear

            ShowCombo(combo, multiplier);
        }

        private void HandleComboReset()
        {
            HideCombo();
        }

        /// <summary>
        /// Display the combo count with animated text.
        /// </summary>
        public void ShowCombo(int combo, float multiplier)
        {
            if (_comboText == null) return;

            // Set text
            string comboLabel = combo >= 5 ? "UNSTOPPABLE!" :
                               combo >= 4 ? "AMAZING!" :
                               combo >= 3 ? "GREAT!" :
                               "COMBO!";

            _comboText.text = $"{multiplier:F1}x {comboLabel}";

            // Set color based on combo tier
            Color comboColor = GetComboColor(combo);
            _comboText.color = comboColor;

            // Animate
            _comboSequence?.Kill();
            _comboSequence = DOTween.Sequence();

            if (_comboCanvasGroup != null)
            {
                _comboCanvasGroup.alpha = 0;
                _comboSequence.Append(_comboCanvasGroup.DOFade(1f, _showDuration));
            }

            if (_comboText != null)
            {
                _comboText.transform.localScale = Vector3.one * _scaleOvershoot;
                _comboSequence.Join(_comboText.transform.DOScale(1f, _showDuration).SetEase(Ease.OutBack));
            }

            _comboSequence.AppendInterval(_holdDuration);

            if (_comboCanvasGroup != null)
            {
                _comboSequence.Append(_comboCanvasGroup.DOFade(0f, _fadeDuration));
            }

            _comboSequence.SetUpdate(true)
                .SetAutoKill(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        /// <summary>
        /// Hide the combo display immediately.
        /// </summary>
        public void HideCombo()
        {
            _comboSequence?.Kill();
            if (_comboCanvasGroup != null) _comboCanvasGroup.alpha = 0;
        }

        /// <summary>
        /// Show perfect clear text animation.
        /// </summary>
        public void ShowPerfectClear()
        {
            if (_perfectText == null) return;

            _perfectText.text = "★ PERFECT CLEAR ★";
            _perfectText.color = ThemeConfig.WarningColor; // Gold

            _perfectSequence?.Kill();
            _perfectSequence = DOTween.Sequence();

            if (_perfectCanvasGroup != null)
            {
                _perfectCanvasGroup.alpha = 0;
                _perfectSequence.Append(_perfectCanvasGroup.DOFade(1f, 0.3f));
            }

            if (_perfectText != null)
            {
                _perfectText.transform.localScale = Vector3.zero;
                _perfectSequence.Join(_perfectText.transform.DOScale(1.2f, 0.4f).SetEase(Ease.OutBack));
                _perfectSequence.Append(_perfectText.transform.DOScale(1f, 0.2f));
            }

            _perfectSequence.AppendInterval(2f);

            if (_perfectCanvasGroup != null)
            {
                _perfectSequence.Append(_perfectCanvasGroup.DOFade(0f, 0.5f));
            }

            _perfectSequence.SetUpdate(true)
                .SetAutoKill(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        private Color GetComboColor(int combo)
        {
            if (combo >= 5) return ThemeConfig.WarningColor;    // Gold
            if (combo >= 4) return ThemeConfig.SecondaryColor;   // Magenta
            if (combo >= 3) return ThemeConfig.SuccessColor;     // Green
            return ThemeConfig.PrimaryColor;                      // Cyan
        }

        private void OnDestroy()
        {
            _comboSequence?.Kill();
            _perfectSequence?.Kill();
        }
    }
}
