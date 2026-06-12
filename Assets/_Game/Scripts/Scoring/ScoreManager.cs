using System;
using UnityEngine;
using SerapKeremGameKit._Singletons;

namespace _Game.Scoring
{
    /// <summary>
    /// Manages scoring, combos, and time bonuses for Line Rush.
    /// Tracks per-level scoring with combo multipliers and perfect clear bonuses.
    /// </summary>
    public class ScoreManager : MonoSingleton<ScoreManager>
    {
        [Header("Score Settings")]
        [SerializeField] private int _basePointsPerLine = 100;
        [SerializeField] private int _perfectClearBonus = 500;
        [SerializeField] private int _timeBonusMultiplier = 10;
        [SerializeField] private float _timeBonusThreshold = 30f;

        [Header("Combo Settings")]
        [SerializeField] private float _comboResetTime = 3f;

        // Current level state
        private int _currentScore;
        private int _currentCombo;
        private int _maxComboThisLevel;
        private int _linesCleared;
        private int _totalLinesInLevel;
        private int _collisionsThisLevel;
        private float _levelStartTime;
        private float _lastLineClearTime;
        private bool _isTracking;

        // Combo multiplier tiers
        private static readonly float[] ComboMultipliers = { 1f, 1.5f, 2f, 3f, 5f };

        // Events
        public event Action<int> OnScoreChanged;
        public event Action<int, float> OnComboChanged; // combo count, multiplier
        public event Action OnComboReset;
        public event Action<int, int, int, bool> OnLevelScoreFinalized; // score, stars, coins, isPerfect

        // Public accessors
        public int CurrentScore => _currentScore;
        public int CurrentCombo => _currentCombo;
        public int MaxCombo => _maxComboThisLevel;
        public int CollisionsThisLevel => _collisionsThisLevel;
        public float ComboMultiplier => GetComboMultiplier(_currentCombo);
        public bool IsPerfectClear => _collisionsThisLevel == 0 && _linesCleared >= _totalLinesInLevel;

        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Start tracking score for a new level.
        /// </summary>
        public void StartLevel(int totalLines)
        {
            _currentScore = 0;
            _currentCombo = 0;
            _maxComboThisLevel = 0;
            _linesCleared = 0;
            _totalLinesInLevel = totalLines;
            _collisionsThisLevel = 0;
            _levelStartTime = Time.time;
            _lastLineClearTime = Time.time;
            _isTracking = true;

            OnScoreChanged?.Invoke(0);
            OnComboChanged?.Invoke(0, 1f);
        }

        /// <summary>
        /// Called when a line is successfully cleared (no collision).
        /// </summary>
        public void OnLineClear()
        {
            if (!_isTracking) return;

            float timeSinceLastClear = Time.time - _lastLineClearTime;
            _lastLineClearTime = Time.time;
            _linesCleared++;

            // Check combo timing
            if (timeSinceLastClear <= _comboResetTime)
            {
                _currentCombo++;
            }
            else
            {
                _currentCombo = 1;
            }

            if (_currentCombo > _maxComboThisLevel)
                _maxComboThisLevel = _currentCombo;

            // Calculate points with combo multiplier
            float multiplier = GetComboMultiplier(_currentCombo);
            int points = Mathf.RoundToInt(_basePointsPerLine * multiplier);
            _currentScore += points;

            OnScoreChanged?.Invoke(_currentScore);
            OnComboChanged?.Invoke(_currentCombo, multiplier);
        }

        /// <summary>
        /// Called when a collision occurs (line reversed).
        /// </summary>
        public void OnCollision()
        {
            if (!_isTracking) return;

            _collisionsThisLevel++;

            // Reset combo
            if (_currentCombo > 0)
            {
                _currentCombo = 0;
                OnComboReset?.Invoke();
                OnComboChanged?.Invoke(0, 1f);
            }
        }

        /// <summary>
        /// Finalize the level score and return results.
        /// Called when level is won.
        /// </summary>
        public LevelScoreResult FinalizeLevel()
        {
            _isTracking = false;

            float completionTime = Time.time - _levelStartTime;
            bool isPerfect = _collisionsThisLevel == 0;

            // Time bonus
            int timeBonus = 0;
            if (completionTime < _timeBonusThreshold)
            {
                timeBonus = Mathf.RoundToInt((_timeBonusThreshold - completionTime) * _timeBonusMultiplier);
            }

            // Perfect clear bonus
            int perfectBonus = isPerfect ? _perfectClearBonus : 0;

            int totalScore = _currentScore + timeBonus + perfectBonus;

            // Calculate stars
            int stars = CalculateStars();

            // Calculate coin reward
            int coinReward = CalculateCoinReward(stars, isPerfect);

            var result = new LevelScoreResult
            {
                BaseScore = _currentScore,
                TimeBonus = timeBonus,
                PerfectBonus = perfectBonus,
                TotalScore = totalScore,
                Stars = stars,
                CoinReward = coinReward,
                MaxCombo = _maxComboThisLevel,
                Collisions = _collisionsThisLevel,
                CompletionTime = completionTime,
                IsPerfect = isPerfect
            };

            OnLevelScoreFinalized?.Invoke(totalScore, stars, coinReward, isPerfect);

            return result;
        }

        /// <summary>
        /// Calculate star rating based on collisions.
        /// </summary>
        public int CalculateStars()
        {
            if (_collisionsThisLevel == 0) return 3; // Perfect
            if (_collisionsThisLevel <= 2) return 2;  // Good
            return 1; // Completed
        }

        private int CalculateCoinReward(int stars, bool isPerfect)
        {
            int baseReward = 10;
            int starBonus = stars * 5;
            int perfectMultiplier = isPerfect ? 2 : 1;

            return (baseReward + starBonus) * perfectMultiplier;
        }

        private float GetComboMultiplier(int combo)
        {
            if (combo <= 0) return 1f;
            int index = Mathf.Min(combo - 1, ComboMultipliers.Length - 1);
            return ComboMultipliers[index];
        }

        /// <summary>
        /// Reset score tracking (e.g., on level restart).
        /// </summary>
        public void ResetTracking()
        {
            _isTracking = false;
            _currentScore = 0;
            _currentCombo = 0;
            _collisionsThisLevel = 0;
        }
    }

    /// <summary>
    /// Result data from a completed level.
    /// </summary>
    [Serializable]
    public struct LevelScoreResult
    {
        public int BaseScore;
        public int TimeBonus;
        public int PerfectBonus;
        public int TotalScore;
        public int Stars;
        public int CoinReward;
        public int MaxCombo;
        public int Collisions;
        public float CompletionTime;
        public bool IsPerfect;
    }
}
