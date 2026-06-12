using System.Collections.Generic;
using UnityEngine;
using _Game.Line;
using SerapKeremGameKit._LevelSystem;
using Level = SerapKeremGameKit._LevelSystem.Level;

namespace _Game.Level
{
    /// <summary>
    /// Procedural level generator for LineRush.
    /// Creates line configurations at runtime using seeded randomness
    /// so the same level number always produces the same layout.
    /// </summary>
    public class ProceduralLevelGenerator : MonoBehaviour
    {
        [Header("Difficulty Curve")]
        [SerializeField] private LevelDifficultyCurve _difficultyCurve;

        [Header("Prefab References")]
        [Tooltip("The base level prefab to instantiate (must have Level + LineManager components)")]
        [SerializeField] private GameObject _levelBasePrefab;
        
        [Tooltip("Line prefab to instantiate for each generated line")]
        [SerializeField] private GameObject _linePrefab;

        [Header("Generation Settings")]
        [SerializeField] private float _lineWidth = 0.15f;
        [SerializeField] private float _snapSize = 1f;
        [SerializeField] private int _seed = 42;

        // Cardinal directions
        private static readonly Vector2[] CardinalDirections = new Vector2[]
        {
            Vector2.right,
            Vector2.left,
            Vector2.up,
            Vector2.down
        };

        // All 8 directions (cardinal + diagonal)
        private static readonly Vector2[] AllDirections = new Vector2[]
        {
            Vector2.right,
            Vector2.left,
            Vector2.up,
            Vector2.down,
            new Vector2(1, 1).normalized,
            new Vector2(-1, 1).normalized,
            new Vector2(1, -1).normalized,
            new Vector2(-1, -1).normalized
        };

        /// <summary>
        /// Check if this generator has all required references to function.
        /// </summary>
        public bool IsConfigured => _difficultyCurve != null && _levelBasePrefab != null && _linePrefab != null;

        /// <summary>
        /// Generate a complete level for the given level number.
        /// Returns a fully configured Level component ready to be loaded.
        /// </summary>
        public Level GenerateLevel(int levelNumber)
        {
            if (_difficultyCurve == null)
            {
                Debug.LogError("[ProceduralLevelGenerator] Difficulty curve is not assigned!");
                return null;
            }

            LevelParameters parameters = _difficultyCurve.GetParameters(levelNumber);

            // Use seeded random so levels are deterministic
            System.Random rng = new System.Random(_seed + levelNumber * 7919);

            // Instantiate the base level prefab
            GameObject levelObj = InstantiateLevelBase(levelNumber);
            if (levelObj == null) return null;

            Level level = levelObj.GetComponent<Level>();
            if (level == null)
            {
                level = levelObj.AddComponent<Level>();
            }

            // Create lines parent
            Transform linesParent = CreateLinesParent(levelObj.transform);

            // Generate line configurations
            List<LineConfig> lineConfigs = GenerateLineConfigs(parameters, rng);

            // Create line GameObjects
            foreach (var config in lineConfigs)
            {
                CreateLine(config, linesParent, parameters.SpeedMultiplier);
            }

            // Configure the level
            level.IsProceduralLevel = true;
            level.SetLinesParent(linesParent);

            return level;
        }

        private GameObject InstantiateLevelBase(int levelNumber)
        {
            if (_levelBasePrefab == null)
            {
                // Create a minimal level structure if no prefab is available
                GameObject levelObj = new GameObject($"ProceduralLevel_{levelNumber}");
                levelObj.AddComponent<Level>();
                
                // Add LineManager
                GameObject lineManagerObj = new GameObject("LineManager");
                lineManagerObj.transform.SetParent(levelObj.transform);
                LineManager lineManager = lineManagerObj.AddComponent<LineManager>();

                // Add Vector3ArrayPool
                GameObject poolObj = new GameObject("ArrayPool");
                poolObj.transform.SetParent(lineManagerObj.transform);
                poolObj.AddComponent<Vector3ArrayPool>();

                Level level = levelObj.GetComponent<Level>();
                level.LineManager = lineManager;

                return levelObj;
            }

            GameObject instance = Instantiate(_levelBasePrefab);
            instance.name = $"ProceduralLevel_{levelNumber}";
            
            // Clear any existing line children from the base prefab
            Level baseLevel = instance.GetComponent<Level>();
            if (baseLevel != null && baseLevel.LineManager != null)
            {
                baseLevel.LineManager.ClearLines();
            }

            return instance;
        }

        private Transform CreateLinesParent(Transform levelRoot)
        {
            // Check if a lines parent already exists
            Transform existingParent = levelRoot.Find("Lines");
            if (existingParent != null)
            {
                // Clear existing children
                for (int i = existingParent.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(existingParent.GetChild(i).gameObject);
                }
                return existingParent;
            }

            GameObject linesParent = new GameObject("Lines");
            linesParent.transform.SetParent(levelRoot);
            linesParent.transform.localPosition = Vector3.zero;
            return linesParent.transform;
        }

        /// <summary>
        /// Generate configurations for all lines in a level.
        /// </summary>
        private List<LineConfig> GenerateLineConfigs(LevelParameters parameters, System.Random rng)
        {
            List<LineConfig> configs = new List<LineConfig>();
            float halfGrid = parameters.GridSize / 2f;
            int maxAttempts = parameters.LineCount * 20; // Prevent infinite loops
            int attempts = 0;

            // Determine available directions based on complexity
            Vector2[] availableDirections = GetAvailableDirections(parameters.DirectionComplexity, rng);

            while (configs.Count < parameters.LineCount && attempts < maxAttempts)
            {
                attempts++;

                // Generate random start position on grid
                float startX = SnapToGrid(RandomRange(rng, -halfGrid, halfGrid));
                float startY = SnapToGrid(RandomRange(rng, -halfGrid, halfGrid));
                Vector2 startPos = new Vector2(startX, startY);

                // Pick a random direction
                Vector2 direction = availableDirections[rng.Next(availableDirections.Length)];

                // Generate line segments
                int segmentCount = Mathf.Max(2, parameters.LineLength + rng.Next(-1, 2));
                List<Vector2> points = GenerateLinePoints(startPos, direction, segmentCount, parameters.GridSize);

                if (points.Count < 2)
                    continue;

                // Check if this line overlaps too much with existing lines
                bool shouldIntersect = (float)rng.NextDouble() < parameters.IntersectionDensity;
                bool hasIntersection = CheckIntersections(points, configs);

                // We want some intersections but not all
                if (!shouldIntersect && hasIntersection && configs.Count > 1)
                    continue;

                // Check bounds
                if (!AllPointsInBounds(points, parameters.GridSize))
                    continue;

                configs.Add(new LineConfig
                {
                    Points = points,
                    Direction = direction
                });
            }

            return configs;
        }

        private Vector2[] GetAvailableDirections(float complexity, System.Random rng)
        {
            if (complexity < 0.3f)
            {
                return CardinalDirections;
            }
            else if (complexity < 0.7f)
            {
                // Mix of cardinal and some diagonals
                List<Vector2> dirs = new List<Vector2>(CardinalDirections);
                int diagonalsToAdd = Mathf.RoundToInt((complexity - 0.3f) / 0.4f * 4f);
                for (int i = 0; i < diagonalsToAdd && i < 4; i++)
                {
                    dirs.Add(AllDirections[4 + i]);
                }
                return dirs.ToArray();
            }
            else
            {
                return AllDirections;
            }
        }

        private List<Vector2> GenerateLinePoints(Vector2 start, Vector2 direction, int segmentCount, float gridSize)
        {
            List<Vector2> points = new List<Vector2>();
            points.Add(start);

            Vector2 current = start;
            for (int i = 0; i < segmentCount; i++)
            {
                Vector2 next = current + direction * _snapSize;
                next = new Vector2(SnapToGrid(next.x), SnapToGrid(next.y));

                // Check bounds
                float halfGrid = gridSize / 2f + 1f;
                if (Mathf.Abs(next.x) > halfGrid || Mathf.Abs(next.y) > halfGrid)
                    break;

                points.Add(next);
                current = next;
            }

            return points;
        }

        private bool CheckIntersections(List<Vector2> newPoints, List<LineConfig> existingLines)
        {
            for (int i = 0; i < newPoints.Count - 1; i++)
            {
                Vector2 a1 = newPoints[i];
                Vector2 a2 = newPoints[i + 1];

                foreach (var existingLine in existingLines)
                {
                    for (int j = 0; j < existingLine.Points.Count - 1; j++)
                    {
                        Vector2 b1 = existingLine.Points[j];
                        Vector2 b2 = existingLine.Points[j + 1];

                        if (SegmentsIntersect(a1, a2, b1, b2))
                            return true;
                    }
                }
            }
            return false;
        }

        private bool SegmentsIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            float d1 = Cross(b2 - b1, a1 - b1);
            float d2 = Cross(b2 - b1, a2 - b1);
            float d3 = Cross(a2 - a1, b1 - a1);
            float d4 = Cross(a2 - a1, b2 - a1);

            if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
                ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))
                return true;

            return false;
        }

        private float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        private bool AllPointsInBounds(List<Vector2> points, float gridSize)
        {
            float halfGrid = gridSize / 2f;
            foreach (var p in points)
            {
                if (Mathf.Abs(p.x) > halfGrid || Mathf.Abs(p.y) > halfGrid)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Create a Line GameObject from configuration.
        /// </summary>
        private void CreateLine(LineConfig config, Transform parent, float speedMultiplier)
        {
            GameObject lineObj;

            if (_linePrefab != null)
            {
                lineObj = Instantiate(_linePrefab, parent);
            }
            else
            {
                lineObj = CreateLineFromScratch(parent);
            }

            lineObj.name = $"Line_{parent.childCount}";
            lineObj.transform.localPosition = Vector3.zero;

            // Setup LineRenderer
            LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = lineObj.AddComponent<LineRenderer>();
            }

            // Configure line renderer
            lineRenderer.positionCount = config.Points.Count;
            lineRenderer.useWorldSpace = false;
            lineRenderer.startWidth = _lineWidth;
            lineRenderer.endWidth = _lineWidth;
            lineRenderer.numCapVertices = 4;
            lineRenderer.numCornerVertices = 4;

            for (int i = 0; i < config.Points.Count; i++)
            {
                lineRenderer.SetPosition(i, new Vector3(config.Points[i].x, config.Points[i].y, 0f));
            }

            // Setup animation speed
            LineAnimation lineAnimation = lineObj.GetComponent<LineAnimation>();
            if (lineAnimation != null)
            {
                // Speed is serialized, but we can use reflection or just rely on the default
                // The prefab's default speed works fine for most levels
            }
        }

        private GameObject CreateLineFromScratch(Transform parent)
        {
            GameObject lineObj = new GameObject("Line");
            lineObj.transform.SetParent(parent);

            // Add required components
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.startWidth = _lineWidth;
            lr.endWidth = _lineWidth;

            lineObj.AddComponent<_Game.Line.Line>();
            lineObj.AddComponent<LineAnimation>();
            lineObj.AddComponent<LineClick>();
            lineObj.AddComponent<LineDestroyer>();
            lineObj.AddComponent<LineMaterialHandler>();
            lineObj.AddComponent<LineRendererSnapFixer>();

            // Create line head child
            GameObject headObj = new GameObject("LineHead");
            headObj.transform.SetParent(lineObj.transform);
            headObj.AddComponent<SpriteRenderer>();
            headObj.AddComponent<LineRendererHead>();
            headObj.AddComponent<LineHeadCollisionDetector>();

            // Create collider spawner
            GameObject colliderSpawnerObj = new GameObject("ColliderSpawner");
            colliderSpawnerObj.transform.SetParent(lineObj.transform);
            colliderSpawnerObj.AddComponent<LineSegmentColliderSpawner2D>();

            return lineObj;
        }

        private float SnapToGrid(float value)
        {
            return Mathf.Round(value / _snapSize) * _snapSize;
        }

        private float RandomRange(System.Random rng, float min, float max)
        {
            return min + (float)rng.NextDouble() * (max - min);
        }

        /// <summary>
        /// Internal line configuration during generation.
        /// </summary>
        private struct LineConfig
        {
            public List<Vector2> Points;
            public Vector2 Direction;
        }
    }
}
