using System.Collections.Generic;
using UnityEngine;
using _Game.Line;
using SerapKeremGameKit._LevelSystem;

namespace _Game.Level
{
    /// <summary>
    /// Generates levels from pre-defined templates in LevelTemplateLibrary.
    /// Similar to ProceduralLevelGenerator but uses hand-crafted designs.
    /// </summary>
    public class TemplateBasedLevelGenerator : MonoBehaviour
    {
        [Header("Template Library")]
        [SerializeField] private LevelTemplateLibrary _templateLibrary;

        [Header("Prefab References")]
        [SerializeField] private GameObject _levelBasePrefab;
        [SerializeField] private GameObject _linePrefab;

        [Header("Line Settings")]
        [SerializeField] private float _lineWidth = 0.15f;

        /// <summary>Check if this generator is configured.</summary>
        public bool IsConfigured => _templateLibrary != null && _levelBasePrefab != null && _linePrefab != null;

        /// <summary>Check if a template exists for the given level number.</summary>
        public bool HasTemplate(int levelNumber)
        {
            return _templateLibrary != null && _templateLibrary.HasTemplate(levelNumber);
        }

        /// <summary>
        /// Generate a level from a template.
        /// Returns null if no template exists for the level number.
        /// </summary>
        public Level GenerateFromTemplate(int levelNumber)
        {
            if (_templateLibrary == null)
            {
                Debug.LogWarning("[TemplateBasedLevelGenerator] Template library not assigned!");
                return null;
            }

            var template = _templateLibrary.GetTemplate(levelNumber);
            if (template == null)
            {
                Debug.LogWarning($"[TemplateBasedLevelGenerator] No template for level {levelNumber}");
                return null;
            }

            // Instantiate base level
            GameObject levelObj = InstantiateLevelBase(levelNumber, template.Name);
            if (levelObj == null) return null;

            Level level = levelObj.GetComponent<Level>();
            if (level == null)
            {
                level = levelObj.AddComponent<Level>();
            }

            // Create lines parent
            Transform linesParent = CreateLinesParent(levelObj.transform);

            // Create lines from template
            foreach (var lineTemplate in template.Lines)
            {
                CreateLineFromTemplate(lineTemplate, linesParent);
            }

            // Configure level
            level.IsProceduralLevel = true;
            level.SetLinesParent(linesParent);

            Debug.Log($"[TemplateBasedLevelGenerator] Generated level {levelNumber}: '{template.Name}' ({template.DifficultyTier}) with {template.Lines.Count} lines");

            return level;
        }

        private GameObject InstantiateLevelBase(int levelNumber, string levelName)
        {
            if (_levelBasePrefab == null)
            {
                GameObject levelObj = new GameObject($"TemplateLevel_{levelNumber}_{levelName}");
                levelObj.AddComponent<Level>();

                GameObject lineManagerObj = new GameObject("LineManager");
                lineManagerObj.transform.SetParent(levelObj.transform);
                LineManager lineManager = lineManagerObj.AddComponent<LineManager>();

                GameObject poolObj = new GameObject("ArrayPool");
                poolObj.transform.SetParent(lineManagerObj.transform);
                poolObj.AddComponent<Vector3ArrayPool>();

                Level level = levelObj.GetComponent<Level>();
                level.LineManager = lineManager;

                return levelObj;
            }

            GameObject instance = Instantiate(_levelBasePrefab);
            instance.name = $"TemplateLevel_{levelNumber}_{levelName}";

            Level baseLevel = instance.GetComponent<Level>();
            if (baseLevel != null && baseLevel.LineManager != null)
            {
                baseLevel.LineManager.ClearLines();
            }

            return instance;
        }

        private Transform CreateLinesParent(Transform levelRoot)
        {
            Transform existingParent = levelRoot.Find("Lines");
            if (existingParent != null)
            {
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

        private void CreateLineFromTemplate(LevelTemplateLibrary.LineTemplate lineTemplate, Transform parent)
        {
            if (lineTemplate.Points.Count < 2) return;

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

            LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = lineObj.AddComponent<LineRenderer>();
            }

            // Configure
            lineRenderer.positionCount = lineTemplate.Points.Count;
            lineRenderer.useWorldSpace = false;
            lineRenderer.startWidth = _lineWidth;
            lineRenderer.endWidth = _lineWidth;
            lineRenderer.numCapVertices = 4;
            lineRenderer.numCornerVertices = 4;

            for (int i = 0; i < lineTemplate.Points.Count; i++)
            {
                lineRenderer.SetPosition(i, new Vector3(lineTemplate.Points[i].x, lineTemplate.Points[i].y, 0f));
            }
        }

        private GameObject CreateLineFromScratch(Transform parent)
        {
            GameObject lineObj = new GameObject("Line");
            lineObj.transform.SetParent(parent);

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

            GameObject headObj = new GameObject("LineHead");
            headObj.transform.SetParent(lineObj.transform);
            headObj.AddComponent<SpriteRenderer>();
            headObj.AddComponent<LineRendererHead>();
            headObj.AddComponent<LineHeadCollisionDetector>();

            GameObject colliderSpawnerObj = new GameObject("ColliderSpawner");
            colliderSpawnerObj.transform.SetParent(lineObj.transform);
            colliderSpawnerObj.AddComponent<LineSegmentColliderSpawner2D>();

            return lineObj;
        }
    }
}
