using System;
using UnityEngine;
using SerapKeremGameKit._Singletons;
using SerapKeremGameKit._Managers;

namespace _Game.UI
{
    public class LivesManager : MonoSingleton<LivesManager>
    {
        [Header("Energy Settings")]
        [SerializeField] private int _maxLives = 5;
        
        private int _currentLives;
        private int _lastLifeLossFrame = -1;

        public int CurrentLives => _currentLives;
        public int MaxLivesCount => _maxLives;

        public event Action<int> OnLivesChanged;
        public event Action OnLivesDepleted;

        protected override void Awake()
        {
            base.Awake();
        }

        public void Initialize()
        {
            ResetLives();
        }

        public void ResetLives()
        {
            _currentLives = _maxLives;
            _lastLifeLossFrame = -1;
            OnLivesChanged?.Invoke(_currentLives);
        }

        public void LoseLife()
        {
            int currentFrame = Time.frameCount;
            
            if (_lastLifeLossFrame == currentFrame)
            {
                return;
            }

            if (_currentLives <= 0)
            {
                return;
            }

            _lastLifeLossFrame = currentFrame;
            _currentLives--;
            OnLivesChanged?.Invoke(_currentLives);

            if (_currentLives <= 0)
            {
                OnLivesDepleted?.Invoke();
            }
        }

        /// <summary>
        /// Add lives (e.g., from rewarded ad).
        /// Clamps to max lives.
        /// </summary>
        public void AddLives(int count)
        {
            if (count <= 0) return;
            
            _currentLives = Mathf.Min(_currentLives + count, _maxLives);
            _lastLifeLossFrame = -1; // Reset frame guard
            OnLivesChanged?.Invoke(_currentLives);
            
            Debug.Log($"[LivesManager] Added {count} lives. Current: {_currentLives}/{_maxLives}");
        }

        /// <summary>
        /// Continue from fail state by restoring lives without resetting the level.
        /// Used when player watches a rewarded ad for extra lives.
        /// </summary>
        public void ContinueFromFail(int livesToRestore)
        {
            _currentLives = Mathf.Min(livesToRestore, _maxLives);
            _lastLifeLossFrame = -1;
            OnLivesChanged?.Invoke(_currentLives);
            
            Debug.Log($"[LivesManager] Continuing from fail with {_currentLives} lives");
        }

        /// <summary>
        /// Set max lives for special modes (e.g., daily challenge).
        /// </summary>
        public void SetMaxLives(int max)
        {
            _maxLives = Mathf.Max(1, max);
        }

        /// <summary>
        /// Reset max lives to default (5).
        /// </summary>
        public void ResetMaxLives()
        {
            _maxLives = 5;
        }

        public bool HasLivesRemaining()
        {
            return _currentLives > 0;
        }
    }
}
