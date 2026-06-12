using UnityEngine;

namespace _Game.Scoring
{
    /// <summary>
    /// Tracks per-level progress: best score, best stars, completion status.
    /// Persists data using PlayerPrefs.
    /// </summary>
    public static class LevelProgressTracker
    {
        private const string PREFIX = "LR_Level_";
        private const string STARS_SUFFIX = "_Stars";
        private const string SCORE_SUFFIX = "_Score";
        private const string COMPLETED_SUFFIX = "_Done";
        private const string TOTAL_STARS_KEY = "LR_TotalStars";
        private const string TOTAL_COMPLETED_KEY = "LR_TotalCompleted";
        private const string PERFECT_COUNT_KEY = "LR_PerfectCount";
        private const string MAX_COMBO_KEY = "LR_MaxComboEver";

        // ============================================
        // Per-Level Data
        // ============================================

        /// <summary>Get the best star rating for a level (0-3).</summary>
        public static int GetBestStars(int levelNumber)
        {
            return PlayerPrefs.GetInt($"{PREFIX}{levelNumber}{STARS_SUFFIX}", 0);
        }

        /// <summary>Set the best star rating for a level (only if better).</summary>
        public static bool SetBestStars(int levelNumber, int stars)
        {
            int current = GetBestStars(levelNumber);
            if (stars > current)
            {
                int oldTotal = GetTotalStars();
                PlayerPrefs.SetInt($"{PREFIX}{levelNumber}{STARS_SUFFIX}", stars);
                // Update total stars
                int newTotal = oldTotal + (stars - current);
                PlayerPrefs.SetInt(TOTAL_STARS_KEY, newTotal);
                PlayerPrefs.Save();
                return true; // New best
            }
            return false;
        }

        /// <summary>Get the best score for a level.</summary>
        public static int GetBestScore(int levelNumber)
        {
            return PlayerPrefs.GetInt($"{PREFIX}{levelNumber}{SCORE_SUFFIX}", 0);
        }

        /// <summary>Set the best score for a level (only if better). Returns true if new best.</summary>
        public static bool SetBestScore(int levelNumber, int score)
        {
            int current = GetBestScore(levelNumber);
            if (score > current)
            {
                PlayerPrefs.SetInt($"{PREFIX}{levelNumber}{SCORE_SUFFIX}", score);
                PlayerPrefs.Save();
                return true;
            }
            return false;
        }

        /// <summary>Check if a level has been completed.</summary>
        public static bool IsLevelCompleted(int levelNumber)
        {
            return PlayerPrefs.GetInt($"{PREFIX}{levelNumber}{COMPLETED_SUFFIX}", 0) == 1;
        }

        /// <summary>Mark a level as completed.</summary>
        public static void SetLevelCompleted(int levelNumber)
        {
            if (!IsLevelCompleted(levelNumber))
            {
                PlayerPrefs.SetInt($"{PREFIX}{levelNumber}{COMPLETED_SUFFIX}", 1);
                int totalCompleted = GetTotalCompleted() + 1;
                PlayerPrefs.SetInt(TOTAL_COMPLETED_KEY, totalCompleted);
                PlayerPrefs.Save();
            }
        }

        /// <summary>Check if a level is unlocked (previous level completed or level 1).</summary>
        public static bool IsLevelUnlocked(int levelNumber)
        {
            if (levelNumber <= 1) return true;
            return IsLevelCompleted(levelNumber - 1);
        }

        // ============================================
        // Aggregate Stats
        // ============================================

        /// <summary>Get total stars collected across all levels.</summary>
        public static int GetTotalStars()
        {
            return PlayerPrefs.GetInt(TOTAL_STARS_KEY, 0);
        }

        /// <summary>Get total levels completed.</summary>
        public static int GetTotalCompleted()
        {
            return PlayerPrefs.GetInt(TOTAL_COMPLETED_KEY, 0);
        }

        /// <summary>Get total perfect clears.</summary>
        public static int GetPerfectCount()
        {
            return PlayerPrefs.GetInt(PERFECT_COUNT_KEY, 0);
        }

        /// <summary>Increment perfect clear counter.</summary>
        public static void IncrementPerfectCount()
        {
            int count = GetPerfectCount() + 1;
            PlayerPrefs.SetInt(PERFECT_COUNT_KEY, count);
            PlayerPrefs.Save();
        }

        /// <summary>Get the highest combo ever achieved.</summary>
        public static int GetMaxComboEver()
        {
            return PlayerPrefs.GetInt(MAX_COMBO_KEY, 0);
        }

        /// <summary>Update the max combo if higher.</summary>
        public static bool SetMaxComboEver(int combo)
        {
            int current = GetMaxComboEver();
            if (combo > current)
            {
                PlayerPrefs.SetInt(MAX_COMBO_KEY, combo);
                PlayerPrefs.Save();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Record full results for a completed level.
        /// Returns a summary of what changed.
        /// </summary>
        public static LevelRecordResult RecordLevelResult(int levelNumber, int score, int stars, bool isPerfect, int maxCombo)
        {
            var result = new LevelRecordResult();

            result.IsNewBestScore = SetBestScore(levelNumber, score);
            result.IsNewBestStars = SetBestStars(levelNumber, stars);
            result.IsFirstCompletion = !IsLevelCompleted(levelNumber);

            SetLevelCompleted(levelNumber);

            if (isPerfect)
            {
                IncrementPerfectCount();
            }

            result.IsNewMaxCombo = SetMaxComboEver(maxCombo);

            return result;
        }

        /// <summary>Reset all progress (for debugging).</summary>
        public static void ResetAllProgress()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }

    public struct LevelRecordResult
    {
        public bool IsNewBestScore;
        public bool IsNewBestStars;
        public bool IsFirstCompletion;
        public bool IsNewMaxCombo;
    }
}
