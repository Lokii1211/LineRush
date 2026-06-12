using System;
using UnityEngine;
using _Game.Level;

namespace _Game.Gameplay
{
    /// <summary>
    /// Daily Challenge system for Line Rush.
    /// Generates a unique daily level using date-based seed.
    /// Players can complete one daily challenge per day for bonus rewards.
    /// </summary>
    public class DailyChallengeManager : MonoBehaviour
    {
        [Header("Daily Challenge Settings")]
        [SerializeField] private int _dailyLives = 3;
        [SerializeField] private float _dailySpeedMultiplier = 1.3f;
        [SerializeField] private int _dailyBonusCoins = 50;
        [SerializeField] private int _dailyBonusStars = 3;

        private const string DAILY_COMPLETED_PREFIX = "LR_Daily_";
        private const string DAILY_STREAK_KEY = "LR_DailyStreak";
        private const string DAILY_LAST_DATE_KEY = "LR_DailyLastDate";

        public static DailyChallengeManager Instance { get; private set; }

        // Events
        public event Action OnDailyChallengeAvailable;
        public event Action<int> OnDailyChallengeCompleted; // bonus coins earned

        // Public accessors
        public int DailyLives => _dailyLives;
        public float DailySpeedMultiplier => _dailySpeedMultiplier;
        public int BonusCoins => _dailyBonusCoins;
        public int CurrentStreak => PlayerPrefs.GetInt(DAILY_STREAK_KEY, 0);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Check if today's daily challenge is available (not yet completed).
        /// </summary>
        public bool IsDailyAvailable()
        {
            string dateKey = GetTodayKey();
            return PlayerPrefs.GetInt(dateKey, 0) == 0;
        }

        /// <summary>
        /// Get today's seed for deterministic daily level generation.
        /// </summary>
        public int GetDailySeed()
        {
            DateTime today = DateTime.Now;
            return today.Year * 10000 + today.DayOfYear * 7919 + 42;
        }

        /// <summary>
        /// Get the daily challenge level parameters.
        /// Returns modified LevelParameters for the daily challenge.
        /// </summary>
        public LevelParameters GetDailyParameters()
        {
            // Daily challenge difficulty based on day of year (cycles)
            int dayOfYear = DateTime.Now.DayOfYear;
            float t = (dayOfYear % 30) / 30f; // 30-day difficulty cycle

            return new LevelParameters
            {
                LevelNumber = 9000 + dayOfYear, // Special level number range
                LineCount = Mathf.RoundToInt(Mathf.Lerp(4, 10, t)),
                GridSize = Mathf.Lerp(5f, 9f, t),
                LineLength = Mathf.RoundToInt(Mathf.Lerp(3, 6, t)),
                DirectionComplexity = Mathf.Lerp(0.3f, 0.9f, t),
                IntersectionDensity = Mathf.Lerp(0.3f, 0.7f, t),
                SpeedMultiplier = _dailySpeedMultiplier
            };
        }

        /// <summary>
        /// Mark today's daily challenge as completed.
        /// </summary>
        public void CompleteDailyChallenge()
        {
            string dateKey = GetTodayKey();
            PlayerPrefs.SetInt(dateKey, 1);

            // Update streak
            string lastDate = PlayerPrefs.GetString(DAILY_LAST_DATE_KEY, "");
            string yesterday = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            string today = DateTime.Now.ToString("yyyyMMdd");

            int streak = CurrentStreak;
            if (lastDate == yesterday)
            {
                streak++;
            }
            else if (lastDate != today)
            {
                streak = 1;
            }

            PlayerPrefs.SetInt(DAILY_STREAK_KEY, streak);
            PlayerPrefs.SetString(DAILY_LAST_DATE_KEY, today);
            PlayerPrefs.Save();

            // Calculate bonus with streak multiplier
            int bonusCoins = _dailyBonusCoins + (streak * 5);

            OnDailyChallengeCompleted?.Invoke(bonusCoins);

            Debug.Log($"[DailyChallenge] Completed! Streak: {streak}, Bonus: {bonusCoins}");
        }

        /// <summary>
        /// Get time remaining until next daily challenge.
        /// </summary>
        public TimeSpan GetTimeUntilNextDaily()
        {
            DateTime now = DateTime.Now;
            DateTime tomorrow = now.Date.AddDays(1);
            return tomorrow - now;
        }

        /// <summary>
        /// Get today's date key for PlayerPrefs.
        /// </summary>
        private string GetTodayKey()
        {
            return DAILY_COMPLETED_PREFIX + DateTime.Now.ToString("yyyyMMdd");
        }

        /// <summary>
        /// Get the daily challenge display name.
        /// </summary>
        public string GetDailyDisplayName()
        {
            DateTime today = DateTime.Now;
            return $"Daily Challenge - {today:MMMM d}";
        }
    }
}
