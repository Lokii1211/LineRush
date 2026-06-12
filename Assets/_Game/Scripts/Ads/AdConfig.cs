using UnityEngine;

/// <summary>
/// Centralized ad configuration for Line Rush.
/// Contains all ad unit IDs and settings.
/// Published by Viya Nexus.
/// </summary>
public static class AdConfig
{
    // ============================================
    // AdMob App ID (set in Unity: Edit > Project Settings > Google Mobile Ads)
    // ============================================
    public const string APP_ID = "ca-app-pub-2857128148429490~6176198550";

    // ============================================
    // Ad Unit IDs — Line Rush by Viya Nexus
    // ============================================
    
    /// <summary>
    /// Banner ad unit ID.
    /// NOTE: If this shows the same value as APP_ID, you may need to create 
    /// a separate Banner ad unit in your AdMob dashboard (admob.google.com).
    /// Banner unit IDs use '/' separator, not '~'.
    /// </summary>
    public const string BANNER_AD_UNIT_ID = "ca-app-pub-2857128148429490/8599181917";

    /// <summary>Interstitial ad unit ID — shows between levels.</summary>
    public const string INTERSTITIAL_AD_UNIT_ID = "ca-app-pub-2857128148429490/3671614281";

    /// <summary>Rewarded ad unit ID — watch for extra lives or hints.</summary>
    public const string REWARDED_AD_UNIT_ID = "ca-app-pub-2857128148429490/7934831924";

    // ============================================
    // Test Ad Unit IDs (for development builds)
    // ============================================
    public const string TEST_BANNER_ID = "ca-app-pub-3940256099942544/6300978111";
    public const string TEST_INTERSTITIAL_ID = "ca-app-pub-3940256099942544/1033173712";
    public const string TEST_REWARDED_ID = "ca-app-pub-3940256099942544/5224354917";

    // ============================================
    // Ad Behavior Settings
    // ============================================
    
    /// <summary>How often to show interstitial ads (every N levels)</summary>
    public const int INTERSTITIAL_FREQUENCY = 3;
    
    /// <summary>Show interstitial on retry/level fail</summary>
    public const bool SHOW_AD_ON_RETRY = true;
    
    /// <summary>Show interstitial after daily challenge completion</summary>
    public const bool SHOW_AD_ON_DAILY_COMPLETE = true;
    
    /// <summary>Banner ad position (Top or Bottom)</summary>
    public const string BANNER_POSITION = "Bottom";

    /// <summary>Number of extra lives granted from rewarded ad</summary>
    public const int REWARDED_EXTRA_LIVES = 2;

    /// <summary>Number of free hints per level (before needing ads)</summary>
    public const int FREE_HINTS_PER_LEVEL = 1;

    // ============================================
    // Helper: Use test IDs in debug builds
    // ============================================
    
    /// <summary>Get the appropriate Banner ID based on build type.</summary>
    public static string GetBannerID()
    {
#if UNITY_EDITOR
        return TEST_BANNER_ID;
#else
        return BANNER_AD_UNIT_ID;
#endif
    }

    /// <summary>Get the appropriate Interstitial ID based on build type.</summary>
    public static string GetInterstitialID()
    {
#if UNITY_EDITOR
        return TEST_INTERSTITIAL_ID;
#else
        return INTERSTITIAL_AD_UNIT_ID;
#endif
    }

    /// <summary>Get the appropriate Rewarded ID based on build type.</summary>
    public static string GetRewardedID()
    {
#if UNITY_EDITOR
        return TEST_REWARDED_ID;
#else
        return REWARDED_AD_UNIT_ID;
#endif
    }
}
