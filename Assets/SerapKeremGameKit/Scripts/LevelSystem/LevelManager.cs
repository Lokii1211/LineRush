using SerapKeremGameKit._LevelSystem;
using SerapKeremGameKit._Singletons;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;
using SerapKeremGameKit._Logging;
using SerapKeremGameKit._Utilities;
using _Game.Level;

namespace SerapKeremGameKit._Managers
{
    [DefaultExecutionOrder(-2)]
    public class LevelManager : MonoSingleton<LevelManager>
    {
        #region Properties & Data Access

        private const string ProgressKey = PreferencesKeys.ProgressData;
        public int ActiveLevelNumber
        {
            get => PlayerPrefs.GetInt(ProgressKey, 1);
            set { PlayerPrefs.SetInt(ProgressKey, value); SaveUtility.SaveImmediate(); }
        }

        [Tooltip("Use random selection after tutorials are completed.")]
        [SerializeField] private bool _useRandomSelection = true;

        [Title("Level Collections")]
        [ListDrawerSettings(Draggable = true, AlwaysExpanded = false)]
        [FormerlySerializedAs("_gameplayLevels")]
        [SerializeField, Required] private Level[] _levels;

        [Title("Procedural Level Generation")]
        [SerializeField] private ProceduralLevelGenerator _levelGenerator;
        [SerializeField] private int _totalProceduralLevels = 1000;

        public Level ActiveLevelInstance { get; private set; }
        public int ProcessedLevelIndex { get; private set; }

        // Public accessors for external systems
        public Level[] GameplayLevels => _levels;
        public int GameplayLevelCount => _levels != null ? _levels.Length : 0;
        
        /// <summary>Total available levels (prefab + procedural).</summary>
        public int TotalLevelCount => GameplayLevelCount + _totalProceduralLevels;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            PerformInitialValidation();
        }

        void Start()
        {
            StartCurrentLevelInstance();
        }

        public void StartCurrentLevelInstance()
        {
            ConfigureEnvironment();
            LoadCurrentLevel();
        }

        #endregion

        #region Core Level Management

        public void LoadCurrentLevel()
        {
            int currentProgress = ActiveLevelNumber;
            int prefabCount = GameplayLevelCount;

            if (currentProgress <= prefabCount)
            {
                // Use existing prefab levels
                var selection = ComputePrefabLevelSelection(currentProgress);
                ProcessedLevelIndex = selection.targetIndex;
                InstantiateAndBegin(selection.selectedLevel);
            }
            else
            {
                // Use procedural generation for levels beyond prefab count
                int proceduralLevelNumber = currentProgress - prefabCount;
                ProcessedLevelIndex = currentProgress;
                LoadProceduralLevel(proceduralLevelNumber);
            }
        }

        private (Level selectedLevel, int targetIndex) ComputePrefabLevelSelection(int currentProgress)
        {
            return ResolveGameplaySelection(currentProgress);
        }

        private (Level selectedLevel, int targetIndex) ResolveGameplaySelection(int adjustedProgress)
        {
            int totalGameplayLevels = _levels.Length;
            int calculatedIndex = ClampOrWrapIndex(adjustedProgress, totalGameplayLevels);

            return (_levels[calculatedIndex - 1], calculatedIndex);
        }

        private int ClampOrWrapIndex(int progressValue, int totalAvailable)
        {
            if (progressValue <= totalAvailable)
                return progressValue;

            if (_useRandomSelection)
                return GetRandomIndex(totalAvailable);

            return WrapIndex(progressValue, totalAvailable);
        }

        private int GetRandomIndex(int maxRange) => Random.Range(1, maxRange + 1);

        private int WrapIndex(int value, int wrapLimit)
        {
            int remainder = value % wrapLimit;
            return remainder == 0 ? wrapLimit : remainder;
        }

        private void LoadProceduralLevel(int proceduralLevelNumber)
        {
            if (_levelGenerator == null || !_levelGenerator.IsConfigured)
            {
                TraceLogger.LogWarning($"ProceduralLevelGenerator is not configured. Falling back to random prefab level.", this);
                // Fallback: load a random prefab level
                int randomIndex = GetRandomIndex(_levels.Length);
                InstantiateAndBegin(_levels[randomIndex - 1]);
                return;
            }

            // Clamp procedural level number to the configured max
            int clampedNumber = Mathf.Clamp(proceduralLevelNumber, 1, _totalProceduralLevels);

            Level generatedLevel = _levelGenerator.GenerateLevel(clampedNumber);
            if (generatedLevel == null)
            {
                TraceLogger.LogWarning($"Failed to generate procedural level {clampedNumber}. Falling back to random prefab level.", this);
                int randomIndex = GetRandomIndex(_levels.Length);
                InstantiateAndBegin(_levels[randomIndex - 1]);
                return;
            }

            ActiveLevelInstance = generatedLevel;
            ActiveLevelInstance.Load();
            Time.timeScale = 1f;
            if (SerapKeremGameKit._InputSystem.InputHandler.Instance != null)
            {
                SerapKeremGameKit._InputSystem.InputHandler.Instance.UnlockInput();
            }
            StateManager.Instance.SetLoading();
            StartLevel();
        }

        private void InstantiateAndBegin(Level targetLevel)
        {
            ActiveLevelInstance = Instantiate(targetLevel);
            ActiveLevelInstance.Load();
            Time.timeScale = 1f;
            if (SerapKeremGameKit._InputSystem.InputHandler.Instance != null)
            {
                SerapKeremGameKit._InputSystem.InputHandler.Instance.UnlockInput();
            }
            StateManager.Instance.SetLoading();
            StartLevel();
        }

        #endregion

        #region Level Control Methods

        public void StartLevel()
        {
            ActiveLevelInstance.Play();
            StateManager.Instance.SetOnStart();
        }

        public void RetryLevel()
        {
            TerminateCurrentLevel();
            
            int currentProgress = ActiveLevelNumber;
            int prefabCount = GameplayLevelCount;

            if (currentProgress <= prefabCount)
            {
                var retryTarget = _levels[ProcessedLevelIndex - 1];
                InstantiateAndBegin(retryTarget);
            }
            else
            {
                int proceduralLevelNumber = currentProgress - prefabCount;
                LoadProceduralLevel(proceduralLevelNumber);
            }
        }

        public void RestartLevel()
        {
            StateManager.Instance.SetOnRestart();
            RetryLevel();
        }

        public void CleanCurrentLevel()
        {
            TerminateCurrentLevel();
        }

        public void IncreaseLevelNumber()
        {
            TerminateCurrentLevel();
            
            int nextLevel = ActiveLevelNumber + 1;
            int maxLevel = GameplayLevelCount + _totalProceduralLevels;
            
            // Clamp to max level count
            if (nextLevel > maxLevel)
            {
                nextLevel = maxLevel;
            }
            
            ActiveLevelNumber = nextLevel;
        }

        /// <summary>
        /// Set the active level number directly.
        /// Used by level select screen and daily challenge.
        /// </summary>
        public void SetLevelNumber(int levelNumber)
        {
            TerminateCurrentLevel();
            int maxLevel = GameplayLevelCount + _totalProceduralLevels;
            ActiveLevelNumber = Mathf.Clamp(levelNumber, 1, maxLevel);
        }

        private void TerminateCurrentLevel()
        {
            if (ActiveLevelInstance != null)
                Destroy(ActiveLevelInstance.gameObject);
        }

        #endregion

        #region Game Result Handlers
        [Button("Test LevelWin")]
        public void Win()
        {
            if (!ValidateGameStateForEvents()) return;
            StateManager.Instance.SetOnWin();
        }

        [Button("Test LevelWin")]
        public void Win(int moveCount)
        {
            if (!ValidateGameStateForEvents()) return;
            StateManager.Instance.SetOnWin();
        }

        [Button("Test LevelLose")]
        public void Lose()
        {
            if (!ValidateGameStateForEvents()) return;
            StateManager.Instance.SetOnLose();
        }

        private bool ValidateGameStateForEvents()
        {
            return StateManager.Instance.CurrentState == GameState.OnStart;
        }

        #endregion

        #region Utility & Validation Methods

        private void PerformInitialValidation()
        {
            if (_levels == null || _levels.Length == 0)
                TraceLogger.LogWarning($"{name}: Levels array is not configured.", this);
            
            if (_levelGenerator == null)
                TraceLogger.LogWarning($"{name}: ProceduralLevelGenerator is not assigned. Only prefab levels will be available.", this);
        }

        private void ConfigureEnvironment()
        {
#if UNITY_EDITOR
            CleanupExistingLevelsInEditor();
#endif
        }

#if UNITY_EDITOR
        private void CleanupExistingLevelsInEditor()
        {
            var existingLevelInstances = FindObjectsByType<Level>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var levelInstance in existingLevelInstances)
                levelInstance.gameObject.SetActive(false);
        }
#endif

        #endregion

        public Level GetLevelByNumber(int levelNumber)
        {
            int gameplayIndex = levelNumber;

            if (gameplayIndex <= 0 || gameplayIndex > _levels.Length) return null;

            return _levels[gameplayIndex - 1];
        }

        #region Utility & Validation Methods
        public Level GetCurrentLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber);
        }

        public Level GetNextLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber + 1) ?? GetLevelByNumber(1);
        }

        public Level GetNextestLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber + 2) ?? GetLevelByNumber(1);
        }

        public Level GetFinalLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber + 3) ?? GetLevelByNumber(1);
        }

        public Level GetFinalNextLevel()
        {
            return GetLevelByNumber(ActiveLevelNumber + 4) ?? GetLevelByNumber(1);
        }
        #endregion
    }
}