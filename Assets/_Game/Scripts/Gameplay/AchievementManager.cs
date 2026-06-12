using System;
using System.Collections.Generic;
using UnityEngine;
using _Game.Scoring;

namespace _Game.Gameplay
{
    /// <summary>
    /// Achievement system for Line Rush.
    /// Tracks player milestones and unlocks achievements.
    /// Persists achievement state via PlayerPrefs.
    /// </summary>
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }

        private const string PREFIX = "LR_Ach_";
        
        // Events
        public event Action<Achievement> OnAchievementUnlocked;

        // Achievement definitions
        private List<Achievement> _achievements;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeAchievements();
        }

        private void InitializeAchievements()
        {
            _achievements = new List<Achievement>
            {
                new Achievement("first_clear", "First Clear", "Complete your first level", "🏆"),
                new Achievement("perfect_start", "Perfect Start", "Get 3 stars on Level 1", "⭐"),
                new Achievement("combo_king", "Combo King", "Reach a 5x combo", "👑"),
                new Achievement("untouchable", "Untouchable", "Complete 5 levels without any collisions", "🛡️"),
                new Achievement("marathon", "Marathon Runner", "Complete 50 levels", "🏃"),
                new Achievement("speed_demon", "Speed Demon", "Complete a level in under 10 seconds", "⚡"),
                new Achievement("line_master", "Line Master", "Complete all 30 hand-crafted levels", "🎯"),
                new Achievement("daily_warrior", "Daily Warrior", "Complete 7 daily challenges", "📅"),
                new Achievement("star_collector", "Star Collector", "Collect 50 total stars", "⭐"),
                new Achievement("centurion", "Centurion", "Complete 100 levels total", "💯"),
                new Achievement("combo_chain", "Chain Reaction", "Get a 3x combo 10 times", "🔗"),
                new Achievement("no_hints", "Self Reliant", "Complete 20 levels without using hints", "🧠"),
                new Achievement("streak_master", "Streak Master", "Achieve a 7-day daily challenge streak", "🔥"),
                new Achievement("collector", "Collector", "Earn 1000 total coins", "💰"),
                new Achievement("perfectionist", "Perfectionist", "Get 3 stars on 10 different levels", "✨")
            };
        }

        /// <summary>
        /// Get all achievements with their current unlock status.
        /// </summary>
        public List<AchievementStatus> GetAllAchievements()
        {
            var statuses = new List<AchievementStatus>();
            foreach (var ach in _achievements)
            {
                statuses.Add(new AchievementStatus
                {
                    Achievement = ach,
                    IsUnlocked = IsUnlocked(ach.Id),
                    UnlockDate = GetUnlockDate(ach.Id)
                });
            }
            return statuses;
        }

        /// <summary>
        /// Get count of unlocked achievements.
        /// </summary>
        public int GetUnlockedCount()
        {
            int count = 0;
            foreach (var ach in _achievements)
            {
                if (IsUnlocked(ach.Id)) count++;
            }
            return count;
        }

        /// <summary>
        /// Get total achievement count.
        /// </summary>
        public int GetTotalCount() => _achievements.Count;

        // ============================================
        // Check Methods — Call these after game events
        // ============================================

        /// <summary>Check achievements after a level is completed.</summary>
        public void CheckAfterLevelComplete(int levelNumber, int stars, float completionTime, int maxCombo, bool isPerfect)
        {
            // First Clear
            TryUnlock("first_clear");

            // Perfect Start
            if (levelNumber == 1 && stars >= 3)
                TryUnlock("perfect_start");

            // Combo King
            if (maxCombo >= 5)
                TryUnlock("combo_king");

            // Speed Demon
            if (completionTime < 10f)
                TryUnlock("speed_demon");

            // Untouchable (check via progress tracker)
            int perfectCount = LevelProgressTracker.GetPerfectCount();
            if (perfectCount >= 5)
                TryUnlock("untouchable");

            // Marathon Runner
            int totalCompleted = LevelProgressTracker.GetTotalCompleted();
            if (totalCompleted >= 50)
                TryUnlock("marathon");

            // Centurion
            if (totalCompleted >= 100)
                TryUnlock("centurion");

            // Line Master
            bool allHandcraftedDone = true;
            for (int i = 1; i <= 30; i++)
            {
                if (!LevelProgressTracker.IsLevelCompleted(i))
                {
                    allHandcraftedDone = false;
                    break;
                }
            }
            if (allHandcraftedDone)
                TryUnlock("line_master");

            // Star Collector
            int totalStars = LevelProgressTracker.GetTotalStars();
            if (totalStars >= 50)
                TryUnlock("star_collector");

            // Perfectionist
            int perfectStarLevels = 0;
            for (int i = 1; i <= 1000; i++)
            {
                if (LevelProgressTracker.GetBestStars(i) >= 3)
                {
                    perfectStarLevels++;
                    if (perfectStarLevels >= 10)
                    {
                        TryUnlock("perfectionist");
                        break;
                    }
                }
            }
        }

        /// <summary>Check achievements after daily challenge.</summary>
        public void CheckAfterDailyChallenge(int streak)
        {
            // Daily Warrior
            IncrementCounter("daily_count");
            if (GetCounter("daily_count") >= 7)
                TryUnlock("daily_warrior");

            // Streak Master
            if (streak >= 7)
                TryUnlock("streak_master");
        }

        /// <summary>Check coin-based achievements.</summary>
        public void CheckCoinMilestone(int totalCoins)
        {
            if (totalCoins >= 1000)
                TryUnlock("collector");
        }

        // ============================================
        // Internal Methods
        // ============================================

        private bool IsUnlocked(string id)
        {
            return PlayerPrefs.GetInt(PREFIX + id, 0) == 1;
        }

        private bool TryUnlock(string id)
        {
            if (IsUnlocked(id)) return false;

            PlayerPrefs.SetInt(PREFIX + id, 1);
            PlayerPrefs.SetString(PREFIX + id + "_date", DateTime.Now.ToString("yyyy-MM-dd"));
            PlayerPrefs.Save();

            // Find achievement and fire event
            foreach (var ach in _achievements)
            {
                if (ach.Id == id)
                {
                    Debug.Log($"[Achievement] Unlocked: {ach.Name} — {ach.Description}");
                    OnAchievementUnlocked?.Invoke(ach);
                    break;
                }
            }

            return true;
        }

        private string GetUnlockDate(string id)
        {
            return PlayerPrefs.GetString(PREFIX + id + "_date", "");
        }

        private int GetCounter(string key)
        {
            return PlayerPrefs.GetInt(PREFIX + "counter_" + key, 0);
        }

        private void IncrementCounter(string key)
        {
            int count = GetCounter(key) + 1;
            PlayerPrefs.SetInt(PREFIX + "counter_" + key, count);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Definition of a single achievement.
    /// </summary>
    [Serializable]
    public struct Achievement
    {
        public string Id;
        public string Name;
        public string Description;
        public string Icon;

        public Achievement(string id, string name, string description, string icon)
        {
            Id = id;
            Name = name;
            Description = description;
            Icon = icon;
        }
    }

    /// <summary>
    /// Achievement with unlock status.
    /// </summary>
    public struct AchievementStatus
    {
        public Achievement Achievement;
        public bool IsUnlocked;
        public string UnlockDate;
    }
}
