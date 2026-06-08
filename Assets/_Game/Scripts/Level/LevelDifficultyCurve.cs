using UnityEngine;

namespace _Game.Level
{
    /// <summary>
    /// Defines how difficulty scales across 1000+ levels in LineRush.
    /// Uses AnimationCurves for smooth difficulty transitions.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelDifficultyCurve", menuName = "LineRush/Level Difficulty Curve")]
    public class LevelDifficultyCurve : ScriptableObject
    {
        [Header("Line Count (how many lines per level)")]
        [SerializeField] private AnimationCurve _lineCountCurve = new AnimationCurve(
            new Keyframe(0f, 2f),      // Level 1: 2 lines
            new Keyframe(0.05f, 3f),   // Level 50: 3 lines
            new Keyframe(0.2f, 5f),    // Level 200: 5 lines
            new Keyframe(0.5f, 8f),    // Level 500: 8 lines
            new Keyframe(0.8f, 12f),   // Level 800: 12 lines
            new Keyframe(1f, 15f)      // Level 1000: 15 lines
        );

        [Header("Grid Size (playfield area)")]
        [SerializeField] private AnimationCurve _gridSizeCurve = new AnimationCurve(
            new Keyframe(0f, 4f),
            new Keyframe(0.3f, 6f),
            new Keyframe(0.6f, 8f),
            new Keyframe(1f, 10f)
        );

        [Header("Line Length (segments per line, 2-8)")]
        [SerializeField] private AnimationCurve _lineLengthCurve = new AnimationCurve(
            new Keyframe(0f, 2f),
            new Keyframe(0.3f, 3f),
            new Keyframe(0.6f, 5f),
            new Keyframe(1f, 8f)
        );

        [Header("Direction Complexity (0=cardinal only, 1=all 8 directions)")]
        [SerializeField] private AnimationCurve _directionComplexityCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.2f, 0.2f),
            new Keyframe(0.5f, 0.6f),
            new Keyframe(1f, 1f)
        );

        [Header("Intersection Density (how many lines cross paths, 0-1)")]
        [SerializeField] private AnimationCurve _intersectionDensityCurve = new AnimationCurve(
            new Keyframe(0f, 0.1f),
            new Keyframe(0.2f, 0.3f),
            new Keyframe(0.5f, 0.5f),
            new Keyframe(0.8f, 0.7f),
            new Keyframe(1f, 0.85f)
        );

        [Header("Line Speed Multiplier")]
        [SerializeField] private AnimationCurve _speedMultiplierCurve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.5f, 1.2f),
            new Keyframe(1f, 1.5f)
        );

        [Header("Settings")]
        [SerializeField] private int _maxLevelNumber = 1000;

        public int MaxLevelNumber => _maxLevelNumber;

        /// <summary>
        /// Get difficulty parameters for a given level number.
        /// </summary>
        public LevelParameters GetParameters(int levelNumber)
        {
            float t = Mathf.Clamp01((float)(levelNumber - 1) / (_maxLevelNumber - 1));

            return new LevelParameters
            {
                LevelNumber = levelNumber,
                LineCount = Mathf.RoundToInt(_lineCountCurve.Evaluate(t)),
                GridSize = _gridSizeCurve.Evaluate(t),
                LineLength = Mathf.RoundToInt(_lineLengthCurve.Evaluate(t)),
                DirectionComplexity = _directionComplexityCurve.Evaluate(t),
                IntersectionDensity = _intersectionDensityCurve.Evaluate(t),
                SpeedMultiplier = _speedMultiplierCurve.Evaluate(t)
            };
        }
    }

    /// <summary>
    /// Parameters for a single procedural level.
    /// </summary>
    [System.Serializable]
    public struct LevelParameters
    {
        public int LevelNumber;
        public int LineCount;
        public float GridSize;
        public int LineLength;
        public float DirectionComplexity;
        public float IntersectionDensity;
        public float SpeedMultiplier;
    }
}
