using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using SerapKeremGameKit._UI;
using SerapKeremGameKit._Managers;
using _Game.Scoring;
using _Game.Gameplay;

namespace _Game.UI
{
    /// <summary>
    /// Level select screen showing a scrollable grid of levels.
    /// Displays completion status, star ratings, and provides level replay.
    /// </summary>
    public class LevelSelectPanel : UIPanel
    {
        [Header("Level Grid")]
        [SerializeField] private Transform _gridParent;
        [SerializeField] private GameObject _levelNodePrefab;
        [SerializeField] private int _totalDisplayedLevels = 30;

        [Header("UI Elements")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _endlessModeButton;
        [SerializeField] private TextMeshProUGUI _totalStarsText;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private ScrollRect _scrollRect;

        [Header("Daily Challenge")]
        [SerializeField] private Button _dailyChallengeButton;
        [SerializeField] private TextMeshProUGUI _dailyStatusText;

        [Header("References")]
        [SerializeField] private UIRootController _uiRoot;

        private List<LevelSelectNode> _nodes = new List<LevelSelectNode>();
        private bool _isPopulated;

        private void Awake()
        {
            if (_backButton != null) _backButton.onClick.AddListener(OnBackClicked);
            if (_endlessModeButton != null) _endlessModeButton.onClick.AddListener(OnEndlessModeClicked);
            if (_dailyChallengeButton != null) _dailyChallengeButton.onClick.AddListener(OnDailyChallengeClicked);
        }

        public override void Show(bool playSound = true)
        {
            base.Show(playSound);
            PopulateGrid();
            UpdateStats();
            UpdateDailyChallenge();
        }

        /// <summary>
        /// Populate the level grid with nodes.
        /// </summary>
        private void PopulateGrid()
        {
            if (_gridParent == null || _levelNodePrefab == null) return;

            // Clear existing
            if (_isPopulated)
            {
                RefreshNodes();
                return;
            }

            int currentLevel = LevelManager.Instance.ActiveLevelNumber;

            for (int i = 1; i <= _totalDisplayedLevels; i++)
            {
                GameObject nodeObj = Instantiate(_levelNodePrefab, _gridParent);
                LevelSelectNode node = nodeObj.GetComponent<LevelSelectNode>();

                if (node != null)
                {
                    int bestStars = LevelProgressTracker.GetBestStars(i);
                    bool isCompleted = LevelProgressTracker.IsLevelCompleted(i);
                    bool isUnlocked = LevelProgressTracker.IsLevelUnlocked(i);
                    bool isCurrent = i == currentLevel;

                    node.Setup(i, bestStars, isCompleted, isUnlocked, isCurrent, OnLevelSelected);
                    _nodes.Add(node);
                }
            }

            _isPopulated = true;
        }

        /// <summary>
        /// Refresh existing nodes without recreating them.
        /// </summary>
        private void RefreshNodes()
        {
            int currentLevel = LevelManager.Instance.ActiveLevelNumber;

            for (int i = 0; i < _nodes.Count; i++)
            {
                int levelNumber = i + 1;
                int bestStars = LevelProgressTracker.GetBestStars(levelNumber);
                bool isCompleted = LevelProgressTracker.IsLevelCompleted(levelNumber);
                bool isUnlocked = LevelProgressTracker.IsLevelUnlocked(levelNumber);
                bool isCurrent = levelNumber == currentLevel;

                _nodes[i].Setup(levelNumber, bestStars, isCompleted, isUnlocked, isCurrent, OnLevelSelected);
            }
        }

        private void UpdateStats()
        {
            if (_totalStarsText != null)
            {
                int totalStars = LevelProgressTracker.GetTotalStars();
                int maxStars = _totalDisplayedLevels * 3;
                _totalStarsText.text = $"★ {totalStars}/{maxStars}";
            }

            if (_titleText != null)
            {
                _titleText.text = "SELECT STAGE";
            }
        }

        private void UpdateDailyChallenge()
        {
            if (DailyChallengeManager.Instance == null) return;

            bool isAvailable = DailyChallengeManager.Instance.IsDailyAvailable();

            if (_dailyChallengeButton != null)
            {
                _dailyChallengeButton.interactable = isAvailable;
            }

            if (_dailyStatusText != null)
            {
                if (isAvailable)
                {
                    int streak = DailyChallengeManager.Instance.CurrentStreak;
                    _dailyStatusText.text = streak > 0 ? $"Daily Challenge ★ Streak: {streak}" : "Daily Challenge Available!";
                    _dailyStatusText.color = ThemeConfig.SuccessColor;
                }
                else
                {
                    var remaining = DailyChallengeManager.Instance.GetTimeUntilNextDaily();
                    _dailyStatusText.text = $"Next daily in {remaining.Hours}h {remaining.Minutes}m";
                    _dailyStatusText.color = ThemeConfig.TextSecondary;
                }
            }
        }

        private void OnLevelSelected(int levelNumber)
        {
            if (_uiRoot != null)
            {
                _uiRoot.OnLevelSelected(levelNumber);
            }
        }

        private void OnBackClicked()
        {
            Hide();
            if (_uiRoot != null)
            {
                _uiRoot.InitializeHUD();
            }
        }

        private void OnEndlessModeClicked()
        {
            // Jump to procedural levels (level 31+)
            if (_uiRoot != null)
            {
                _uiRoot.OnLevelSelected(31);
            }
        }

        private void OnDailyChallengeClicked()
        {
            if (_uiRoot != null)
            {
                _uiRoot.OnDailyChallengeRequested();
            }
        }

        public void SetUIRoot(UIRootController uiRoot)
        {
            _uiRoot = uiRoot;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_backButton != null) _backButton.onClick.RemoveAllListeners();
            if (_endlessModeButton != null) _endlessModeButton.onClick.RemoveAllListeners();
            if (_dailyChallengeButton != null) _dailyChallengeButton.onClick.RemoveAllListeners();
        }
    }
}
