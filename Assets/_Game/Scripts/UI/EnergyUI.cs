using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI
{
    /// <summary>
    /// Represents a single energy orb in the HUD.
    /// Replaces the original heart-based lives display with a themed energy system.
    /// </summary>
    public class EnergyUI : MonoBehaviour
    {
        [Header("Energy Sprites")]
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _inactiveSprite;

        [Header("Color Tinting")]
        [SerializeField] private bool _useColorTint = true;

        private Image _image;
        private bool _isActive = true;
        private bool _isInitialized = false;

        public void SetActive(bool active)
        {
            _isActive = active;

            if (_image == null) return;
            
            if (active)
            {
                if (_activeSprite != null)
                {
                    _image.sprite = _activeSprite;
                }
                if (_useColorTint)
                {
                    _image.color = ThemeConfig.EnergyActiveColor;
                }
            }
            else
            {
                if (_inactiveSprite != null)
                {
                    _image.sprite = _inactiveSprite;
                }
                if (_useColorTint)
                {
                    _image.color = ThemeConfig.EnergyInactiveColor;
                }
            }
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            _image = gameObject.GetComponent<Image>();

            if (_image == null)
            {
                Debug.LogWarning($"{name}: Image component is not found. Please assign it in Inspector.", this);
            }

            if (_activeSprite == null || _inactiveSprite == null)
            {
                Debug.LogWarning($"{name}: Active or Inactive energy sprite is not assigned in Inspector.", this);
            }

            SetActive(true);
            _isInitialized = true;
        }
    }
}
