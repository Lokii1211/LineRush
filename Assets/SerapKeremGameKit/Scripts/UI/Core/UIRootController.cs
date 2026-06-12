using SerapKeremGameKit._Economy;
using SerapKeremGameKit._LevelSystem;
using SerapKeremGameKit._Levels;
using SerapKeremGameKit._Managers;
using SerapKeremGameKit._Time;
using UnityEngine;
using SerapKeremGameKit._Audio;
using SerapKeremGameKit._Haptics;
using _Game.UI;
using _Game.Scoring;
using _Game.Gameplay;

namespace SerapKeremGameKit._UI
{
    public sealed class UIRootController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HUDPanel _hud;
        [SerializeField] private WinPanel _win;
        [SerializeField] private FailPanel _fail;
        [SerializeField] private SettingsPanel _settings;
        [SerializeField] private RetryPanel _retry;
        [SerializeField] private LevelSelectPanel _levelSelect;

        [Header("Data")]
        [SerializeField] private LevelConfig _fallbackConfig;

        private GameState _lastState = GameState.None;

        [Header("Audio Keys")]
        [SerializeField] private string _keyOnWin = "stage_win";
        [SerializeField] private string _keyOnLose = "stage_lose";
        [SerializeField] private string _keyOnOpenSettings = "btn_tap";
        [SerializeField] private string _keyOnRestartDialog = "btn_tap";
        [SerializeField] private string _keyOnRestartConfirm = "btn_confirm";
        [SerializeField] private string _keyOnNext = "btn_tap";
        [SerializeField] private string _keyOnHint = "hint_reveal";

        private void Awake()
        {
            // Auto-wire if not assigned
            if (_hud == null) _hud = GetComponentInChildren<HUDPanel>(true);
            if (_win == null) _win = GetComponentInChildren<WinPanel>(true);
            if (_fail == null) _fail = GetComponentInChildren<FailPanel>(true);
            if (_settings == null) _settings = GetComponentInChildren<SettingsPanel>(true);
            if (_retry == null) _retry = GetComponentInChildren<RetryPanel>(true);
            if (_levelSelect == null) _levelSelect = GetComponentInChildren<LevelSelectPanel>(true);

            // Inject UIRoot into screens to avoid FindObjectOfType
            if (_hud != null) _hud.SetUIRoot(this);
            if (_win != null) _win.SetUIRoot(this);
            if (_fail != null) _fail.SetUIRoot(this);
            if (_retry != null) _retry.SetUIRoot(this);
            if (_levelSelect != null) _levelSelect.SetUIRoot(this);

            // Ensure startup state: only HUD hidden initially (will be shown in Start)
            if (_win != null) _win.HideImmediate();
            if (_fail != null) _fail.HideImmediate();
            if (_settings != null) _settings.HideImmediate();
            if (_retry != null) _retry.HideImmediate();
            if (_hud != null) _hud.HideImmediate();
            if (_levelSelect != null) _levelSelect.HideImmediate();
        }

        private void Start()
        {
            ApplyInitialState();
        }

        private void Update()
        {
            SyncWithGameState();
        }

        private void ApplyInitialState()
        {
            HideAll();
            InitializeHUD();
        }

        public void InitializeHUD()
        {
            if (_hud != null)
            {
                _hud.Show(false);
                _hud.SetLevelIndex(LevelManager.Instance.ActiveLevelNumber - 1);
            }
        }

        private void SyncWithGameState()
        {
            GameState current = StateManager.Instance.CurrentState;
            if (current == _lastState) return;
            _lastState = current;

            if (current == GameState.OnStart)
            {
                HideAll();
                if (_hud != null)
                {
                    _hud.Show(false);
                    _hud.SetLevelIndex(LevelManager.Instance.ActiveLevelNumber - 1);
                }
            }
            else if (current == GameState.OnWin)
            {
                ShowWin();
                if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_keyOnWin)) AudioManager.Instance.Play(_keyOnWin);
                if (HapticManager.IsInitialized) HapticManager.Instance.Play(HapticType.Success);
                
                // Show interstitial ad on level complete
                if (AdManager.Instance != null) AdManager.Instance.ShowInterstitialOnLevelComplete();
            }
            else if (current == GameState.OnLose)
            {
                ShowFail();
                if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_keyOnLose)) AudioManager.Instance.Play(_keyOnLose);
                if (HapticManager.IsInitialized) HapticManager.Instance.Play(HapticType.Failure);
            }
        }

        private void HideAll()
        {
            if (_hud != null) _hud.Hide(false);
            if (_win != null) _win.Hide(false);
            if (_fail != null) _fail.Hide(false);
            if (_settings != null) _settings.Hide(false);
            if (_retry != null) _retry.Hide(false);
            if (_levelSelect != null) _levelSelect.Hide(false);
        }

        private void ShowWin()
        {
            HideExcept(_win);
            if (_win != null)
            {
                Level active = LevelManager.Instance.ActiveLevelInstance;
                LevelConfig config = ResolveConfig(active);

                int stars = 1;
                int reward = Mathf.Max(0, config != null ? config.WinCoins : 10);
                int totalBefore = 0;
                bool isNewBest = false;

                if (CurrencyWallet.Instance != null)
                {
                    totalBefore = CurrencyWallet.Instance.Coins;
                }

                // Use ScoreManager for enhanced scoring if available
                if (ScoreManager.IsInitialized)
                {
                    LevelScoreResult scoreResult = ScoreManager.Instance.FinalizeLevel();
                    stars = scoreResult.Stars;
                    reward = scoreResult.CoinReward;

                    int levelNumber = LevelManager.Instance.ActiveLevelNumber;
                    LevelRecordResult record = LevelProgressTracker.RecordLevelResult(
                        levelNumber, scoreResult.TotalScore, stars, scoreResult.IsPerfect, scoreResult.MaxCombo
                    );
                    isNewBest = record.IsNewBestScore;

                    // Check achievements
                    if (AchievementManager.Instance != null)
                    {
                        AchievementManager.Instance.CheckAfterLevelComplete(
                            levelNumber, stars, scoreResult.CompletionTime, 
                            scoreResult.MaxCombo, scoreResult.IsPerfect
                        );
                    }

                    _win.SetupWithScore(stars, reward, totalBefore, this, scoreResult, isNewBest);
                }
                else
                {
                    stars = StarEvaluator.EvaluateStarsByLives();
                    _win.Setup(stars, reward, totalBefore, this);
                }
                
                _win.Show();
            }
        }

        
        private float CalculateCompletionTime(Level level)
        {
            if (level == null)
            {
                return 0f;
            }

            float levelTime = level.LevelTime;
            float remainingTime = 0f;

            if (TimeManager.IsInitialized)
            {
                remainingTime = TimeManager.Instance.RemainingTime;
            }

            float completionTime = Mathf.Max(0f, levelTime - remainingTime);
            return completionTime;
        }

        private void ShowFail()
        {
            HideExcept(_fail);
            if (_fail != null)
            {
                Level active = LevelManager.Instance.ActiveLevelInstance;
                LevelConfig config = ResolveConfig(active);
                int reward = Mathf.Max(0, config != null ? config.FailCoins : 0);
                if (reward > 0 && CurrencyWallet.Instance != null) CurrencyWallet.Instance.Add(reward);
                _fail.Setup(reward, this);
                _fail.Show();
            }
        }

        private LevelConfig ResolveConfig(Level level)
        {
            if (level != null)
            {
                // Try to get LevelConfig from Level GameObject or its children
                LevelConfig attached = level.GetComponent<LevelConfig>();
                if (attached == null)
                {
                    attached = level.GetComponentInChildren<LevelConfig>(true);
                }
                if (attached != null) return attached;
            }
            return _fallbackConfig;
        }

        private void HideExcept(UIPanel screen)
        {
            if (_hud != null && _hud != screen) _hud.Hide(true);
            if (_win != null && _win != screen) _win.Hide(true);
            if (_fail != null && _fail != screen) _fail.Hide(true);
            if (_settings != null && _settings != screen) _settings.Hide(true);
            if (_retry != null && _retry != screen) _retry.Hide(true);
            if (_levelSelect != null && _levelSelect != screen) _levelSelect.Hide(true);
        }

        public void OnRestartRequested()
        {
            if (_retry != null) _retry.Show();
            if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_keyOnRestartDialog)) AudioManager.Instance.Play(_keyOnRestartDialog);
            if (HapticManager.IsInitialized) HapticManager.Instance.Play(HapticType.Medium);
        }

        public void OnRestartConfirmed()
        {
            HideAll();
            
            // Show interstitial ad on retry
            if (AdManager.Instance != null) AdManager.Instance.ShowInterstitialOnRetry();
            
            LevelManager.Instance.RestartLevel();
            if (_hud != null)
            {
                _hud.Show(false);
                _hud.SetLevelIndex(LevelManager.Instance.ActiveLevelNumber - 1);
            }

            // Reset score tracking
            if (ScoreManager.IsInitialized) ScoreManager.Instance.ResetTracking();

            if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_keyOnRestartConfirm)) AudioManager.Instance.Play(_keyOnRestartConfirm);
            if (HapticManager.IsInitialized) HapticManager.Instance.Play(HapticType.Medium);
        }

        public void OnNextLevelRequested()
        {
            HideAll();
            if (_hud != null)
            {
                _hud.Show(false);
            }
            LevelManager.Instance.IncreaseLevelNumber();
            LevelManager.Instance.LoadCurrentLevel();
            if (_hud != null)
            {
                _hud.SetLevelIndex(LevelManager.Instance.ActiveLevelNumber - 1);
            }
            if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_keyOnNext)) AudioManager.Instance.Play(_keyOnNext);
            if (HapticManager.IsInitialized) HapticManager.Instance.Play(HapticType.Light);
        }

        public void ProceedNextLevelAfterReward(int reward)
        {
            if (CurrencyWallet.Instance != null && reward > 0)
            {
                CurrencyWallet.Instance.Add(reward);
                
                // Check coin achievements
                if (AchievementManager.Instance != null)
                {
                    AchievementManager.Instance.CheckCoinMilestone(CurrencyWallet.Instance.Coins);
                }
            }
            OnNextLevelRequested();
        }

        public void OnOpenSettings()
        {
            if (_settings != null) _settings.Show();
            if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_keyOnOpenSettings)) AudioManager.Instance.Play(_keyOnOpenSettings);
            if (HapticManager.IsInitialized) HapticManager.Instance.Play(HapticType.Selection);
        }

        /// <summary>
        /// Handle hint request from HUD.
        /// </summary>
        public void OnHintRequested()
        {
            HintManager hintMgr = FindFirstObjectByType<HintManager>();
            if (hintMgr == null) return;

            bool hintUsed = hintMgr.RequestHint();
            if (!hintUsed)
            {
                // No free hints — show rewarded ad
                if (AdManager.Instance != null)
                {
                    AdManager.Instance.ShowRewardedAdForHint(() =>
                    {
                        hintMgr.GrantHintFromAd();
                    });
                }
            }

            if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_keyOnHint)) AudioManager.Instance.Play(_keyOnHint);
            if (HapticManager.IsInitialized) HapticManager.Instance.Play(HapticType.Light);
        }

        /// <summary>
        /// Handle continue after watching rewarded ad on fail screen.
        /// </summary>
        public void OnContinueAfterAd()
        {
            HideAll();
            if (_hud != null)
            {
                _hud.Show(false);
                _hud.SetLevelIndex(LevelManager.Instance.ActiveLevelNumber - 1);
            }
            // Resume game state
            if (StateManager.Instance != null)
            {
                StateManager.Instance.ChangeState(GameState.OnStart);
            }
        }

        /// <summary>
        /// Open level select screen.
        /// </summary>
        public void OnLevelSelectRequested()
        {
            HideAll();
            if (_levelSelect != null) _levelSelect.Show();
        }

        /// <summary>
        /// Handle level selected from level select screen.
        /// </summary>
        public void OnLevelSelected(int levelNumber)
        {
            HideAll();
            LevelManager.Instance.SetLevelNumber(levelNumber);
            LevelManager.Instance.LoadCurrentLevel();
            if (_hud != null)
            {
                _hud.Show(false);
                _hud.SetLevelIndex(levelNumber - 1);
            }
        }

        /// <summary>
        /// Handle daily challenge request from level select.
        /// </summary>
        public void OnDailyChallengeRequested()
        {
            if (DailyChallengeManager.Instance == null || !DailyChallengeManager.Instance.IsDailyAvailable()) return;

            HideAll();
            
            // Configure daily challenge parameters
            if (LivesManager.IsInitialized)
            {
                LivesManager.Instance.SetMaxLives(DailyChallengeManager.Instance.DailyLives);
                LivesManager.Instance.ResetLives();
            }
            
            // Use daily seed for procedural generation
            int dailySeed = DailyChallengeManager.Instance.GetDailySeed();
            // The ProceduralLevelGenerator uses seeds, so set the level number to daily range
            LevelManager.Instance.SetLevelNumber(9000 + System.DateTime.Now.DayOfYear);
            LevelManager.Instance.LoadCurrentLevel();
            
            if (_hud != null)
            {
                _hud.Show(false);
                _hud.SetLevelIndex(-1); // Special display for daily
            }
        }

        public void UpdateTimeDisplay(float remainingTime)
        {
            if (_hud != null)
            {
                _hud.UpdateTimeDisplay(remainingTime);
            }
        }

        public LivesManager LivesManagerInstance
        {
            get
            {
                if (LivesManager.IsInitialized)
                {
                    return LivesManager.Instance;
                }
                return null;
            }
        }
    }
}