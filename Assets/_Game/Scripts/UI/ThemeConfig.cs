using UnityEngine;

namespace _Game.UI
{
    /// <summary>
    /// Centralized theme configuration for Line Rush by Viya Nexus.
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
        
        /// <summary>Warning/caution — Amber/Gold</summary>
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

        // ============================================
        // Combo Colors (NEW)
        // ============================================
        
        /// <summary>Combo tier 1 color (2x) — Cyan</summary>
        public static readonly Color ComboTier1Color = PrimaryColor;
        
        /// <summary>Combo tier 2 color (3x) — Green</summary>
        public static readonly Color ComboTier2Color = SuccessColor;
        
        /// <summary>Combo tier 3 color (4x) — Magenta</summary>
        public static readonly Color ComboTier3Color = SecondaryColor;
        
        /// <summary>Combo tier 4 color (5x+) — Gold</summary>
        public static readonly Color ComboTier4Color = WarningColor;

        // ============================================
        // Achievement Colors (NEW)
        // ============================================
        
        /// <summary>Achievement popup background</summary>
        public static readonly Color AchievementBgColor = new Color(0.1f, 0.08f, 0.18f, 0.95f);
        
        /// <summary>Achievement title color — Gold</summary>
        public static readonly Color AchievementTitleColor = WarningColor;
        
        /// <summary>Achievement description color</summary>
        public static readonly Color AchievementDescColor = TextSecondary;

        // ============================================
        // Daily Challenge Colors (NEW)
        // ============================================
        
        /// <summary>Daily challenge accent color — Electric Purple</summary>
        public static readonly Color DailyChallengeColor = new Color(0.694f, 0.282f, 1f, 1f); // #B148FF
        
        /// <summary>Daily streak color — Flame Orange</summary>
        public static readonly Color DailyStreakColor = new Color(1f, 0.584f, 0.118f, 1f); // #FF951E

        // ============================================
        // Hint Colors (NEW)
        // ============================================
        
        /// <summary>Hint highlight color — Pulsing White-Cyan</summary>
        public static readonly Color HintHighlightColor = new Color(0.7f, 1f, 1f, 1f);
        
        /// <summary>Hint button active color</summary>
        public static readonly Color HintButtonActiveColor = PrimaryColor;
        
        /// <summary>Hint button inactive color (no hints remaining)</summary>
        public static readonly Color HintButtonInactiveColor = InactiveColor;

        // ============================================
        // Level Select Colors (NEW)
        // ============================================
        
        /// <summary>Level node — unlocked but not completed</summary>
        public static readonly Color LevelNodeUnlocked = new Color(0.15f, 0.12f, 0.25f, 0.9f);
        
        /// <summary>Level node — completed</summary>
        public static readonly Color LevelNodeCompleted = new Color(0.1f, 0.25f, 0.15f, 0.9f);
        
        /// <summary>Level node — current level</summary>
        public static readonly Color LevelNodeCurrent = new Color(0f, 0.898f, 1f, 0.3f);
        
        /// <summary>Level node — locked</summary>
        public static readonly Color LevelNodeLocked = new Color(0.1f, 0.1f, 0.12f, 0.6f);

        // ============================================
        // Score Display Colors (NEW)
        // ============================================
        
        /// <summary>New best score badge color</summary>
        public static readonly Color NewBestColor = WarningColor;
        
        /// <summary>Score text color</summary>
        public static readonly Color ScoreTextColor = TextPrimary;
        
        /// <summary>Bonus text color (time bonus, perfect bonus)</summary>
        public static readonly Color BonusTextColor = SuccessColor;

        // ============================================
        // Rewarded Ad Button Colors (NEW)
        // ============================================
        
        /// <summary>Watch ad button gradient start</summary>
        public static readonly Color AdButtonColorStart = new Color(0.467f, 0.114f, 0.882f, 1f); // #771CDD — Purple
        
        /// <summary>Watch ad button gradient end</summary>
        public static readonly Color AdButtonColorEnd = new Color(0.204f, 0.580f, 0.961f, 1f); // #3494F5 — Blue
    }
}
