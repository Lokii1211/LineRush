using UnityEngine;
using System;

/// <summary>
/// Ad Manager for Line Rush by Viya Nexus.
/// Manages Banner, Interstitial, and Rewarded ads with smart loading.
/// 
/// SETUP INSTRUCTIONS:
/// ===================
/// 1. Import Google Mobile Ads Unity plugin from:
///    https://github.com/googleads/googleads-mobile-unity/releases
///    Download: GoogleMobileAds-v9.x.x.unitypackage
///
/// 2. In Unity, go to Assets > Import Package > Custom Package
///    and select the downloaded .unitypackage
///
/// 3. Go to Assets > Google Mobile Ads > Settings
///    - Enable "Google Mobile Ads"
///    - Set your Android App ID: ca-app-pub-2857128148429490~6176198550
///
/// 4. Attach this script to a GameObject in your first scene
///    (or use the AdManager prefab)
///
/// 5. Build your project (File > Build Settings > Android > Build)
///
/// NOTE: The actual Google Mobile Ads SDK must be imported first.
/// The code below shows the complete integration pattern.
/// Uncomment the Google Ads imports after importing the SDK.
/// </summary>
public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    [Header("Ad Settings")]
    [SerializeField] private bool showBannerOnStart = true;
    [SerializeField] private bool enableInterstitial = true;
    [SerializeField] private bool enableRewarded = true;

    private int levelCompleteCount = 0;
    private bool isInitialized = false;
    private int failedLoadRetries = 0;
    private const int MAX_RETRY_ATTEMPTS = 3;
    private const float RETRY_DELAY = 5f;

    // -------------------------------------------------------
    // Uncomment these after importing Google Mobile Ads SDK:
    // -------------------------------------------------------
    // using GoogleMobileAds.Api;
    // private BannerView bannerView;
    // private InterstitialAd interstitialAd;
    // private RewardedAd rewardedAd;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeAds();
    }

    /// <summary>
    /// Initialize the Mobile Ads SDK.
    /// Call this once at app startup.
    /// </summary>
    public void InitializeAds()
    {
        if (isInitialized) return;

        Debug.Log("[AdManager] Initializing Google Mobile Ads SDK...");
        
        // -------------------------------------------------------
        // Uncomment after importing Google Mobile Ads SDK:
        // -------------------------------------------------------
        // MobileAds.Initialize(initStatus =>
        // {
        //     Debug.Log("[AdManager] SDK Initialized successfully");
        //     isInitialized = true;
        //     
        //     if (showBannerOnStart) LoadBannerAd();
        //     if (enableInterstitial) LoadInterstitialAd();
        //     if (enableRewarded) LoadRewardedAd();
        // });

        isInitialized = true;
        Debug.Log("[AdManager] Ready (import Google Mobile Ads SDK to enable real ads)");
    }

    // =========================================================
    // BANNER AD - Always visible at bottom of screen
    // =========================================================

    /// <summary>
    /// Load and show a banner ad at the bottom of the screen.
    /// </summary>
    public void LoadBannerAd()
    {
        string adUnitId = AdConfig.GetBannerID();
        Debug.Log($"[AdManager] Loading Banner Ad: {adUnitId}");

        // -------------------------------------------------------
        // Uncomment after importing Google Mobile Ads SDK:
        // -------------------------------------------------------
        // // Destroy existing banner if any
        // if (bannerView != null)
        // {
        //     bannerView.Destroy();
        //     bannerView = null;
        // }
        // 
        // // Create a banner at the bottom of the screen
        // bannerView = new BannerView(
        //     adUnitId,
        //     AdSize.Banner,
        //     AdPosition.Bottom
        // );
        // 
        // // Load the ad
        // var adRequest = new AdRequest();
        // bannerView.LoadAd(adRequest);
    }

    /// <summary>Hide the banner ad</summary>
    public void HideBanner()
    {
        // if (bannerView != null) bannerView.Hide();
    }

    /// <summary>Show the banner ad</summary>
    public void ShowBanner()
    {
        // if (bannerView != null) bannerView.Show();
    }

    // =========================================================
    // INTERSTITIAL AD - Shows on level complete / retry
    // =========================================================

    /// <summary>
    /// Load an interstitial ad in the background.
    /// </summary>
    public void LoadInterstitialAd()
    {
        string adUnitId = AdConfig.GetInterstitialID();
        Debug.Log($"[AdManager] Loading Interstitial Ad: {adUnitId}");

        // -------------------------------------------------------
        // Uncomment after importing Google Mobile Ads SDK:
        // -------------------------------------------------------
        // var adRequest = new AdRequest();
        // InterstitialAd.Load(
        //     adUnitId,
        //     adRequest,
        //     (InterstitialAd ad, LoadAdError error) =>
        //     {
        //         if (error != null || ad == null)
        //         {
        //             Debug.LogError($"[AdManager] Interstitial ad failed to load: {error}");
        //             RetryLoadWithDelay(() => LoadInterstitialAd());
        //             return;
        //         }
        //         Debug.Log("[AdManager] Interstitial ad loaded");
        //         interstitialAd = ad;
        //         failedLoadRetries = 0;
        //         
        //         // Set up close callback to reload next ad
        //         interstitialAd.OnAdFullScreenContentClosed += () =>
        //         {
        //             Debug.Log("[AdManager] Interstitial ad closed");
        //             interstitialAd = null;
        //             LoadInterstitialAd(); // Pre-load next ad
        //         };
        //     }
        // );
    }

    /// <summary>
    /// Show interstitial ad on level complete.
    /// Shows every N levels (configured in AdConfig).
    /// </summary>
    public void ShowInterstitialOnLevelComplete()
    {
        if (!enableInterstitial) return;
        
        levelCompleteCount++;
        if (levelCompleteCount % AdConfig.INTERSTITIAL_FREQUENCY != 0) return;

        Debug.Log($"[AdManager] Showing interstitial (level complete count: {levelCompleteCount})");
        
        // if (interstitialAd != null && interstitialAd.CanShowAd())
        // {
        //     interstitialAd.Show();
        // }
        // else
        // {
        //     LoadInterstitialAd();
        // }
    }

    /// <summary>
    /// Force show interstitial ad on retry/loss.
    /// </summary>
    public void ShowInterstitialOnRetry()
    {
        if (!enableInterstitial || !AdConfig.SHOW_AD_ON_RETRY) return;

        Debug.Log("[AdManager] Showing interstitial (retry)");
        
        // if (interstitialAd != null && interstitialAd.CanShowAd())
        // {
        //     interstitialAd.Show();
        // }
        // else
        // {
        //     LoadInterstitialAd();
        // }
    }

    /// <summary>
    /// Show interstitial after daily challenge completion.
    /// </summary>
    public void ShowInterstitialOnDailyComplete()
    {
        if (!enableInterstitial || !AdConfig.SHOW_AD_ON_DAILY_COMPLETE) return;

        Debug.Log("[AdManager] Showing interstitial (daily challenge complete)");
        
        // if (interstitialAd != null && interstitialAd.CanShowAd())
        // {
        //     interstitialAd.Show();
        // }
        // else
        // {
        //     LoadInterstitialAd();
        // }
    }

    // =========================================================
    // REWARDED AD - Watch ad for extra lives/hints/rewards
    // =========================================================

    /// <summary>
    /// Load a rewarded ad in the background.
    /// </summary>
    public void LoadRewardedAd()
    {
        string adUnitId = AdConfig.GetRewardedID();
        Debug.Log($"[AdManager] Loading Rewarded Ad: {adUnitId}");

        // -------------------------------------------------------
        // Uncomment after importing Google Mobile Ads SDK:
        // -------------------------------------------------------
        // var adRequest = new AdRequest();
        // RewardedAd.Load(
        //     adUnitId,
        //     adRequest,
        //     (RewardedAd ad, LoadAdError error) =>
        //     {
        //         if (error != null || ad == null)
        //         {
        //             Debug.LogError($"[AdManager] Rewarded ad failed to load: {error}");
        //             RetryLoadWithDelay(() => LoadRewardedAd());
        //             return;
        //         }
        //         Debug.Log("[AdManager] Rewarded ad loaded");
        //         rewardedAd = ad;
        //         failedLoadRetries = 0;
        //         
        //         rewardedAd.OnAdFullScreenContentClosed += () =>
        //         {
        //             rewardedAd = null;
        //             LoadRewardedAd(); // Pre-load next ad
        //         };
        //     }
        // );
    }

    /// <summary>
    /// Show rewarded ad. Returns true if ad was available.
    /// Call the onRewardEarned callback when user watches full ad.
    /// </summary>
    public bool ShowRewardedAd(Action onRewardEarned, Action onAdClosed = null)
    {
        Debug.Log("[AdManager] Attempting to show rewarded ad");

        // if (rewardedAd != null && rewardedAd.CanShowAd())
        // {
        //     rewardedAd.Show((Reward reward) =>
        //     {
        //         Debug.Log($"[AdManager] User earned reward: {reward.Amount} {reward.Type}");
        //         onRewardEarned?.Invoke();
        //     });
        //     return true;
        // }
        
        Debug.Log("[AdManager] No rewarded ad available");
        // LoadRewardedAd();
        
        // In development (no SDK), simulate reward for testing
#if UNITY_EDITOR
        Debug.Log("[AdManager] EDITOR: Simulating reward for testing");
        onRewardEarned?.Invoke();
        return true;
#else
        return false;
#endif
    }

    /// <summary>
    /// Show rewarded ad specifically for extra lives on fail screen.
    /// </summary>
    public bool ShowRewardedAdForExtraLives(Action onLivesGranted)
    {
        return ShowRewardedAd(() =>
        {
            Debug.Log($"[AdManager] Granting {AdConfig.REWARDED_EXTRA_LIVES} extra lives");
            onLivesGranted?.Invoke();
        });
    }

    /// <summary>
    /// Show rewarded ad specifically for hint reveal.
    /// </summary>
    public bool ShowRewardedAdForHint(Action onHintGranted)
    {
        return ShowRewardedAd(() =>
        {
            Debug.Log("[AdManager] Granting hint from rewarded ad");
            onHintGranted?.Invoke();
        });
    }

    // =========================================================
    // Retry Logic
    // =========================================================
    
    private void RetryLoadWithDelay(Action loadAction)
    {
        failedLoadRetries++;
        if (failedLoadRetries >= MAX_RETRY_ATTEMPTS)
        {
            Debug.LogWarning("[AdManager] Max retry attempts reached. Will try again on next opportunity.");
            failedLoadRetries = 0;
            return;
        }
        
        StartCoroutine(RetryCoroutine(loadAction));
    }

    private System.Collections.IEnumerator RetryCoroutine(Action loadAction)
    {
        yield return new WaitForSeconds(RETRY_DELAY * failedLoadRetries);
        loadAction?.Invoke();
    }

    // =========================================================
    // Cleanup
    // =========================================================

    private void OnDestroy()
    {
        // if (bannerView != null) bannerView.Destroy();
    }
}
