using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI
{
    /// <summary>
    /// Custom styled button component for LineRush.
    /// Applies visual styles and micro-animations to UI buttons.
    /// Attach to any Button GameObject to apply styling.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class StyledButton : MonoBehaviour
    {
        public enum ButtonStyle
        {
            Pill,       // Rounded ends, wide body
            Rounded,    // Standard rounded corners
            PlayArrow,  // Triangle/play shape indicator
            Circle      // Circular button
        }

        public enum ButtonRole
        {
            Primary,    // Main action (large, accent color)
            Secondary,  // Secondary action (medium)
            Danger,     // Restart/retry (orange-red)
            Ghost       // Subtle/transparent
        }

        [Header("Style")]
        [SerializeField] private ButtonStyle _style = ButtonStyle.Rounded;
        [SerializeField] private ButtonRole _role = ButtonRole.Primary;

        [Header("Animation")]
        [SerializeField] private float _pressScale = 0.9f;
        [SerializeField] private float _hoverScale = 1.05f;
        [SerializeField] private float _animDuration = 0.12f;
        [SerializeField] private bool _enablePunchOnClick = true;
        [SerializeField] private float _punchStrength = 0.15f;

        [Header("Size Override")]
        [SerializeField] private bool _overrideSize = false;
        [SerializeField] private Vector2 _customSize = new Vector2(200f, 60f);

        private Button _button;
        private RectTransform _rect;
        private Image _image;
        private Vector3 _originalScale;
        private Tween _currentTween;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _rect = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
            _originalScale = transform.localScale;

            ApplyStyle();
            SetupAnimations();
        }

        private void ApplyStyle()
        {
            if (_image == null) return;

            // Apply role-based colors
            switch (_role)
            {
                case ButtonRole.Primary:
                    _image.color = ThemeConfig.ButtonPrimaryBg;
                    break;
                case ButtonRole.Secondary:
                    _image.color = ThemeConfig.ButtonSecondaryBg;
                    break;
                case ButtonRole.Danger:
                    _image.color = ThemeConfig.ButtonDangerBg;
                    break;
                case ButtonRole.Ghost:
                    _image.color = new Color(1f, 1f, 1f, 0.1f);
                    break;
            }

            // Apply size override
            if (_overrideSize && _rect != null)
            {
                _rect.sizeDelta = _customSize;
            }
        }

        private void SetupAnimations()
        {
            if (_button == null) return;

            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            if (!_enablePunchOnClick) return;

            _currentTween?.Kill();
            transform.localScale = _originalScale;
            _currentTween = transform
                .DOPunchScale(Vector3.one * _punchStrength, _animDuration * 2f, 6, 0.5f)
                .SetUpdate(true)
                .SetAutoKill(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        /// <summary>
        /// Called by Unity EventTrigger for pointer down.
        /// Can be wired in the Inspector.
        /// </summary>
        public void OnPointerDownAnim()
        {
            _currentTween?.Kill();
            _currentTween = transform
                .DOScale(_originalScale * _pressScale, _animDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true)
                .SetAutoKill(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        /// <summary>
        /// Called by Unity EventTrigger for pointer up.
        /// Can be wired in the Inspector.
        /// </summary>
        public void OnPointerUpAnim()
        {
            _currentTween?.Kill();
            _currentTween = transform
                .DOScale(_originalScale, _animDuration)
                .SetEase(Ease.OutBack)
                .SetUpdate(true)
                .SetAutoKill(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        /// <summary>
        /// Called by Unity EventTrigger for pointer enter (hover).
        /// </summary>
        public void OnPointerEnterAnim()
        {
            _currentTween?.Kill();
            _currentTween = transform
                .DOScale(_originalScale * _hoverScale, _animDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true)
                .SetAutoKill(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        /// <summary>
        /// Called by Unity EventTrigger for pointer exit.
        /// </summary>
        public void OnPointerExitAnim()
        {
            _currentTween?.Kill();
            _currentTween = transform
                .DOScale(_originalScale, _animDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true)
                .SetAutoKill(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        private void OnDestroy()
        {
            _currentTween?.Kill();
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnButtonClicked);
            }
        }
    }
}
