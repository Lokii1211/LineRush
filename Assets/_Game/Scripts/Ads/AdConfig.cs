using UnityEngine;

/// <summary>
/// Centralized ad configuration for LineRush.
/// Contains all ad unit IDs and settings.
/// Replace these with your real AdMob IDs before publishing.
/// </summary>
public static class AdConfig
{
    // ============================================
    // AdMob App ID (set in Unity: Edit > Project Settings > Google Mobile Ads)
    // ============================================
    public const string APP_ID = "YOUR_APP_ID_HERE";

    // ============================================
    // Ad Unit IDs - LineRush
    // ============================================
    public const string BANNER_AD_UNIT_ID = "YOUR_BANNER_AD_UNIT_ID_HERE";
    public const string INTERSTITIAL_AD_UNIT_ID = "YOUR_INTERSTITIAL_AD_UNIT_ID_HERE";
    public const string REWARDED_AD_UNIT_ID = "YOUR_REWARDED_AD_UNIT_ID_HERE";

    // ============================================
    // Ad Behavior Settings
    // ============================================
    
    /// <summary>How often to show interstitial ads (every N levels)</summary>
    public const int INTERSTITIAL_FREQUENCY = 2;
    
    /// <summary>Show interstitial on retry/level fail</summary>
    public const bool SHOW_AD_ON_RETRY = true;
    
    /// <summary>Banner ad position (Top or Bottom)</summary>
    public const string BANNER_POSITION = "Bottom";
}
