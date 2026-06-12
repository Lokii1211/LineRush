using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Game.UI;
using _Game.Gameplay;

namespace SerapKeremGameKit._UI
{
    public sealed class HUDPanel : UIPanel
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _hintButton;
        [SerializeField] private Button _levelSelectButton;
        [SerializeField] private UIRootController _uiRoot;
        [SerializeField] private EnergyPanel _energyPanel;

        [Header("Hint Display")]
        [SerializeField] private TextMeshProUGUI _hintCountText;
        [SerializeField] private Image _hintButtonImage;

        [Header("Score Display")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        // Legacy field kept for backward compatibility with existing prefabs
        [SerializeField] private HeartPanel _heartPanel;

        private bool _isInitialized = false;

        private void Awake()
        {
            if (_restartButton != null) _restartButton.BindOnClick(this, OnRestartClicked);
            if (_settingsButton != null) _settingsButton.BindOnClick(this, OnSettingsClicked);
            if (_hintButton != null) _hintButton.BindOnClick(this, OnHintClicked);
            if (_levelSelectButton != null) _levelSelectButton.BindOnClick(this, OnLevelSelectClicked);
        }

        public override void Show(bool playSound = true)
        {
            base.Show(playSound);
            
            if (!_isInitialized)
            {
                Initialize();
            }
            
            SubscribeToLivesManager();
            InitializeEnergyPanel();
            UpdateHintDisplay();
            SubscribeToScoreManager();
        }

        private void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnsubscribeFromLivesManager();
            UnsubscribeFromScoreManager();
            // Auto-unsubscribe handled by ButtonExtensions
        }

        private void SubscribeToLivesManager()
        {
            if (_uiRoot != null && _uiRoot.LivesManagerInstance != null)
            {
                _uiRoot.LivesManagerInstance.OnLivesChanged += HandleLivesChanged;
            }
        }

        private void UnsubscribeFromLivesManager()
        {
            if (_uiRoot != null && _uiRoot.LivesManagerInstance != null)
            {
                _uiRoot.LivesManagerInstance.OnLivesChanged -= HandleLivesChanged;
            }
        }

        private void SubscribeToScoreManager()
        {
            if (_Game.Scoring.ScoreManager.IsInitialized)
            {
                _Game.Scoring.ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
            }
        }

        private void UnsubscribeFromScoreManager()
        {
            if (_Game.Scoring.ScoreManager.IsInitialized)
            {
                _Game.Scoring.ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
            }
        }

        private void HandleScoreChanged(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = score.ToString("N0");
            }
        }

        private void InitializeEnergyPanel()
        {
            // Use new EnergyPanel if assigned, fall back to legacy HeartPanel
            if (_energyPanel != null)
            {
                _energyPanel.Initialize();
                if (_uiRoot != null && _uiRoot.LivesManagerInstance != null)
                {
                    _energyPanel.UpdateEnergy(_uiRoot.LivesManagerInstance.CurrentLives);
                }
            }
            else if (_heartPanel != null)
            {
                _heartPanel.Initialize();
                if (_uiRoot != null && _uiRoot.LivesManagerInstance != null)
                {
                    _heartPanel.UpdateHearts(_uiRoot.LivesManagerInstance.CurrentLives);
                }
            }
        }

        private void HandleLivesChanged(int currentLives)
        {
            if (_energyPanel != null)
            {
                _energyPanel.UpdateEnergy(currentLives);
            }
            else if (_heartPanel != null)
            {
                _heartPanel.UpdateHearts(currentLives);
            }
        }

        public void SetLevelIndex(int levelIndex)
        {
            if (_levelText != null)
                _levelText.text = $"Stage {levelIndex + 1}";
        }

        public void UpdateTimeDisplay(float remainingTime)
        {
            if (_timeText != null)
            {
                _timeText.text = FormatTime(remainingTime);
            }
        }

        private void UpdateHintDisplay()
        {
            HintManager hintMgr = FindFirstObjectByType<HintManager>();
            if (hintMgr != null)
            {
                UpdateHintCount(hintMgr.HintsRemaining);
                hintMgr.OnHintCountChanged -= UpdateHintCount;
                hintMgr.OnHintCountChanged += UpdateHintCount;
            }
        }

        private void UpdateHintCount(int count)
        {
            if (_hintCountText != null)
            {
                _hintCountText.text = count > 0 ? count.ToString() : "AD";
            }
            if (_hintButtonImage != null)
            {
                _hintButtonImage.color = count > 0 ? ThemeConfig.HintButtonActiveColor : ThemeConfig.HintButtonInactiveColor;
            }
        }

        private static string FormatTime(float seconds)
        {
            int totalSeconds = Mathf.Max(0, Mathf.FloorToInt(seconds));
            int minutes = totalSeconds / 60;
            int remainingSeconds = totalSeconds % 60;

            return $"{minutes:D2}:{remainingSeconds:D2}";
        }

        private void OnRestartClicked()
        {
            if (_uiRoot != null) _uiRoot.OnRestartRequested();
        }

        private void OnSettingsClicked()
        {
            if (_uiRoot != null) _uiRoot.OnOpenSettings();
        }

        private void OnHintClicked()
        {
            if (_uiRoot != null) _uiRoot.OnHintRequested();
        }

        private void OnLevelSelectClicked()
        {
            if (_uiRoot != null) _uiRoot.OnLevelSelectRequested();
        }

        public void SetUIRoot(UIRootController uiRoot)
        {
            _uiRoot = uiRoot;
        }
    }
}