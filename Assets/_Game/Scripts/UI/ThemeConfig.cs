using UnityEngine;

namespace _Game.UI
{
    /// <summary>
    /// Centralized theme configuration for LineRush.
    /// All UI colors and visual constants are defined here.
    /// </summary>
    public static class ThemeConfig
    {
        // ============================================
        // Primary Colors
        // ============================================
        
        /// <summary>Primary accent color — Neon Cyan</summary>
        public static readonly Color PrimaryColor = new Color(0f, 0.898f, 1f, 1f); // #00E5FF
        
        /// <summary>Secondary accent color — Neon Magenta</summary>
        public static readonly Color SecondaryColor = new Color(1f, 0f, 0.898f, 1f); // #FF00E5
        
        /// <summary>Background color — Deep Purple</summary>
        public static readonly Color BackgroundColor = new Color(0.051f, 0.008f, 0.129f, 1f); // #0D0221
        
        // ============================================
        // Feedback Colors
        // ============================================
        
        /// <summary>Success feedback — Neon Green</summary>
        public static readonly Color SuccessColor = new Color(0.224f, 1f, 0.078f, 1f); // #39FF14
        
        /// <summary>Failure feedback — Orange-Red</summary>
        public static readonly Color FailureColor = new Color(1f, 0.420f, 0.208f, 1f); // #FF6B35
        
        /// <summary>Warning/caution — Amber</summary>
        public static readonly Color WarningColor = new Color(1f, 0.835f, 0f, 1f); // #FFD500
        
        // ============================================
        // UI Colors
        // ============================================
        
        /// <summary>Primary text color — White</summary>
        public static readonly Color TextPrimary = Color.white;
        
        /// <summary>Secondary text color — Light Lavender</summary>
        public static readonly Color TextSecondary = new Color(0.788f, 0.749f, 0.882f, 1f); // #C9BFE1
        
        /// <summary>Disabled/inactive element color</summary>
        public static readonly Color InactiveColor = new Color(0.3f, 0.3f, 0.35f, 1f);
        
        /// <summary>Active energy orb color (replaces red heart)</summary>
        public static readonly Color EnergyActiveColor = PrimaryColor;
        
        /// <summary>Depleted energy orb color (replaces gray heart)</summary>
        public static readonly Color EnergyInactiveColor = InactiveColor;
        
        // ============================================
        // Line Colors
        // ============================================
        
        /// <summary>Default line color</summary>
        public static readonly Color LineDefaultColor = PrimaryColor;
        
        /// <summary>Line color on collision (failure)</summary>
        public static readonly Color LineFailureColor = FailureColor;
        
        /// <summary>Line color on successful completion</summary>
        public static readonly Color LineSuccessColor = SuccessColor;
        
        // ============================================
        // Button Colors
        // ============================================
        
        /// <summary>Primary button background</summary>
        public static readonly Color ButtonPrimaryBg = PrimaryColor;
        
        /// <summary>Secondary button background</summary>
        public static readonly Color ButtonSecondaryBg = new Color(0.15f, 0.12f, 0.25f, 0.9f);
        
        /// <summary>Danger/restart button background</summary>
        public static readonly Color ButtonDangerBg = FailureColor;
        
        /// <summary>Button text color</summary>
        public static readonly Color ButtonTextColor = Color.white;
    }
}
