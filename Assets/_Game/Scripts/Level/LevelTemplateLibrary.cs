using System.Collections.Generic;
using UnityEngine;

namespace _Game.Level
{
    /// <summary>
    /// Contains hand-crafted level templates for levels 11-30.
    /// Each template defines specific line positions and patterns
    /// for a curated puzzle experience.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelTemplateLibrary", menuName = "LineRush/Level Template Library")]
    public class LevelTemplateLibrary : ScriptableObject
    {
        [System.Serializable]
        public class LevelTemplate
        {
            public string Name;
            public string DifficultyTier;
            public List<LineTemplate> Lines = new List<LineTemplate>();
        }

        [System.Serializable]
        public class LineTemplate
        {
            public List<Vector2> Points = new List<Vector2>();
        }

        /// <summary>
        /// Get a level template by level number (11-30).
        /// Returns null if the level number doesn't have a template.
        /// </summary>
        public LevelTemplate GetTemplate(int levelNumber)
        {
            int index = levelNumber - 11; // Templates start at level 11
            var templates = GenerateAllTemplates();
            
            if (index < 0 || index >= templates.Count) return null;
            return templates[index];
        }

        /// <summary>
        /// Check if a template exists for the given level number.
        /// </summary>
        public bool HasTemplate(int levelNumber)
        {
            return levelNumber >= 11 && levelNumber <= 30;
        }

        /// <summary>
        /// Total number of template levels available.
        /// </summary>
        public int TemplateCount => 20;

        /// <summary>
        /// Generate all 20 hand-crafted level templates.
        /// </summary>
        private List<LevelTemplate> GenerateAllTemplates()
        {
            var templates = new List<LevelTemplate>();

            // ============================================
            // Levels 11-15: Cross Patterns (4-6 lines)
            // ============================================

            // Level 11: Simple Cross
            templates.Add(CreateTemplate("Simple Cross", "Intermediate", new float[][][]
            {
                new float[][] { new float[] {-3, 0}, new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0} },
                new float[][] { new float[] {0, -3}, new float[] {0, -2}, new float[] {0, -1}, new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2}, new float[] {0, 3} },
                new float[][] { new float[] {-2, 2}, new float[] {-1, 2}, new float[] {0, 2}, new float[] {1, 2}, new float[] {2, 2} },
                new float[][] { new float[] {-2, -2}, new float[] {-1, -2}, new float[] {0, -2}, new float[] {1, -2}, new float[] {2, -2} }
            }));

            // Level 12: Double Cross
            templates.Add(CreateTemplate("Double Cross", "Intermediate", new float[][][]
            {
                new float[][] { new float[] {-3, 1}, new float[] {-2, 1}, new float[] {-1, 1}, new float[] {0, 1}, new float[] {1, 1}, new float[] {2, 1}, new float[] {3, 1} },
                new float[][] { new float[] {-3, -1}, new float[] {-2, -1}, new float[] {-1, -1}, new float[] {0, -1}, new float[] {1, -1}, new float[] {2, -1}, new float[] {3, -1} },
                new float[][] { new float[] {1, -3}, new float[] {1, -2}, new float[] {1, -1}, new float[] {1, 0}, new float[] {1, 1}, new float[] {1, 2}, new float[] {1, 3} },
                new float[][] { new float[] {-1, -3}, new float[] {-1, -2}, new float[] {-1, -1}, new float[] {-1, 0}, new float[] {-1, 1}, new float[] {-1, 2}, new float[] {-1, 3} },
                new float[][] { new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0} }
            }));

            // Level 13: Star Shape
            templates.Add(CreateTemplate("Star Shape", "Intermediate", new float[][][]
            {
                new float[][] { new float[] {-3, 0}, new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0} },
                new float[][] { new float[] {0, -3}, new float[] {0, -2}, new float[] {0, -1}, new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2}, new float[] {0, 3} },
                new float[][] { new float[] {-2, -2}, new float[] {-1, -1}, new float[] {0, 0}, new float[] {1, 1}, new float[] {2, 2} },
                new float[][] { new float[] {2, -2}, new float[] {1, -1}, new float[] {0, 0}, new float[] {-1, 1}, new float[] {-2, 2} }
            }));

            // Level 14: Hashtag
            templates.Add(CreateTemplate("Hashtag", "Intermediate", new float[][][]
            {
                new float[][] { new float[] {-1, -3}, new float[] {-1, -2}, new float[] {-1, -1}, new float[] {-1, 0}, new float[] {-1, 1}, new float[] {-1, 2}, new float[] {-1, 3} },
                new float[][] { new float[] {1, -3}, new float[] {1, -2}, new float[] {1, -1}, new float[] {1, 0}, new float[] {1, 1}, new float[] {1, 2}, new float[] {1, 3} },
                new float[][] { new float[] {-3, -1}, new float[] {-2, -1}, new float[] {-1, -1}, new float[] {0, -1}, new float[] {1, -1}, new float[] {2, -1}, new float[] {3, -1} },
                new float[][] { new float[] {-3, 1}, new float[] {-2, 1}, new float[] {-1, 1}, new float[] {0, 1}, new float[] {1, 1}, new float[] {2, 1}, new float[] {3, 1} },
                new float[][] { new float[] {0, -2}, new float[] {0, -1}, new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2} }
            }));

            // Level 15: Diamond Cross
            templates.Add(CreateTemplate("Diamond Cross", "Intermediate", new float[][][]
            {
                new float[][] { new float[] {0, 3}, new float[] {1, 2}, new float[] {2, 1}, new float[] {3, 0} },
                new float[][] { new float[] {0, 3}, new float[] {-1, 2}, new float[] {-2, 1}, new float[] {-3, 0} },
                new float[][] { new float[] {0, -3}, new float[] {1, -2}, new float[] {2, -1}, new float[] {3, 0} },
                new float[][] { new float[] {0, -3}, new float[] {-1, -2}, new float[] {-2, -1}, new float[] {-3, 0} },
                new float[][] { new float[] {-3, 0}, new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0} },
                new float[][] { new float[] {0, -3}, new float[] {0, -2}, new float[] {0, -1}, new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2}, new float[] {0, 3} }
            }));

            // ============================================
            // Levels 16-20: Diagonal Mazes (5-7 lines)
            // ============================================

            // Level 16: Zigzag
            templates.Add(CreateTemplate("Zigzag", "Advanced", new float[][][]
            {
                new float[][] { new float[] {-3, -2}, new float[] {-2, -1}, new float[] {-1, 0}, new float[] {0, 1}, new float[] {1, 2} },
                new float[][] { new float[] {-1, -2}, new float[] {0, -1}, new float[] {1, 0}, new float[] {2, 1}, new float[] {3, 2} },
                new float[][] { new float[] {-3, 0}, new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0} },
                new float[][] { new float[] {0, -3}, new float[] {0, -2}, new float[] {0, -1}, new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2}, new float[] {0, 3} },
                new float[][] { new float[] {3, -2}, new float[] {2, -1}, new float[] {1, 0}, new float[] {0, 1}, new float[] {-1, 2} }
            }));

            // Level 17: Woven Pattern
            templates.Add(CreateTemplate("Woven Pattern", "Advanced", new float[][][]
            {
                new float[][] { new float[] {-3, -1}, new float[] {-2, 0}, new float[] {-1, 1}, new float[] {0, 2}, new float[] {1, 3} },
                new float[][] { new float[] {-3, 1}, new float[] {-2, 0}, new float[] {-1, -1}, new float[] {0, -2}, new float[] {1, -3} },
                new float[][] { new float[] {-1, -3}, new float[] {0, -2}, new float[] {1, -1}, new float[] {2, 0}, new float[] {3, 1} },
                new float[][] { new float[] {-1, 3}, new float[] {0, 2}, new float[] {1, 1}, new float[] {2, 0}, new float[] {3, -1} },
                new float[][] { new float[] {-2, -2}, new float[] {-1, -1}, new float[] {0, 0}, new float[] {1, 1}, new float[] {2, 2} },
                new float[][] { new float[] {2, -2}, new float[] {1, -1}, new float[] {0, 0}, new float[] {-1, 1}, new float[] {-2, 2} }
            }));

            // Level 18: Arrow Rain
            templates.Add(CreateTemplate("Arrow Rain", "Advanced", new float[][][]
            {
                new float[][] { new float[] {-3, 3}, new float[] {-3, 2}, new float[] {-3, 1}, new float[] {-3, 0}, new float[] {-3, -1}, new float[] {-3, -2}, new float[] {-3, -3} },
                new float[][] { new float[] {-1, 3}, new float[] {-1, 2}, new float[] {-1, 1}, new float[] {-1, 0}, new float[] {-1, -1}, new float[] {-1, -2} },
                new float[][] { new float[] {1, 3}, new float[] {1, 2}, new float[] {1, 1}, new float[] {1, 0}, new float[] {1, -1} },
                new float[][] { new float[] {3, 3}, new float[] {3, 2}, new float[] {3, 1}, new float[] {3, 0} },
                new float[][] { new float[] {-3, 0}, new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0} },
                new float[][] { new float[] {-2, -2}, new float[] {-1, -1}, new float[] {0, 0}, new float[] {1, 1}, new float[] {2, 2} }
            }));

            // Level 19: Diagonal Grid
            templates.Add(CreateTemplate("Diagonal Grid", "Advanced", new float[][][]
            {
                new float[][] { new float[] {-3, -1}, new float[] {-2, 0}, new float[] {-1, 1}, new float[] {0, 2}, new float[] {1, 3} },
                new float[][] { new float[] {-1, -3}, new float[] {0, -2}, new float[] {1, -1}, new float[] {2, 0}, new float[] {3, 1} },
                new float[][] { new float[] {3, -1}, new float[] {2, 0}, new float[] {1, 1}, new float[] {0, 2}, new float[] {-1, 3} },
                new float[][] { new float[] {1, -3}, new float[] {0, -2}, new float[] {-1, -1}, new float[] {-2, 0}, new float[] {-3, 1} },
                new float[][] { new float[] {-3, 0}, new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0} },
                new float[][] { new float[] {0, -3}, new float[] {0, -2}, new float[] {0, -1}, new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2}, new float[] {0, 3} },
                new float[][] { new float[] {-2, 2}, new float[] {-1, 1}, new float[] {0, 0}, new float[] {1, -1}, new float[] {2, -2} }
            }));

            // Level 20: Converging Arrows
            templates.Add(CreateTemplate("Converging Arrows", "Advanced", new float[][][]
            {
                new float[][] { new float[] {-4, 0}, new float[] {-3, 0}, new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0} },
                new float[][] { new float[] {4, 0}, new float[] {3, 0}, new float[] {2, 0}, new float[] {1, 0}, new float[] {0, 0} },
                new float[][] { new float[] {0, 4}, new float[] {0, 3}, new float[] {0, 2}, new float[] {0, 1}, new float[] {0, 0} },
                new float[][] { new float[] {0, -4}, new float[] {0, -3}, new float[] {0, -2}, new float[] {0, -1}, new float[] {0, 0} },
                new float[][] { new float[] {-3, -3}, new float[] {-2, -2}, new float[] {-1, -1}, new float[] {0, 0} },
                new float[][] { new float[] {3, 3}, new float[] {2, 2}, new float[] {1, 1}, new float[] {0, 0} },
                new float[][] { new float[] {3, -3}, new float[] {2, -2}, new float[] {1, -1}, new float[] {0, 0} }
            }));

            // ============================================
            // Levels 21-25: Starburst (6-8 lines)
            // ============================================

            // Level 21: Sunburst
            templates.Add(CreateTemplate("Sunburst", "Expert", new float[][][]
            {
                new float[][] { new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0}, new float[] {4, 0} },
                new float[][] { new float[] {0, 0}, new float[] {-1, 0}, new float[] {-2, 0}, new float[] {-3, 0}, new float[] {-4, 0} },
                new float[][] { new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2}, new float[] {0, 3}, new float[] {0, 4} },
                new float[][] { new float[] {0, 0}, new float[] {0, -1}, new float[] {0, -2}, new float[] {0, -3}, new float[] {0, -4} },
                new float[][] { new float[] {0, 0}, new float[] {1, 1}, new float[] {2, 2}, new float[] {3, 3} },
                new float[][] { new float[] {0, 0}, new float[] {-1, -1}, new float[] {-2, -2}, new float[] {-3, -3} }
            }));

            // Level 22: Compass Rose
            templates.Add(CreateTemplate("Compass Rose", "Expert", new float[][][]
            {
                new float[][] { new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0}, new float[] {4, 0} },
                new float[][] { new float[] {0, 0}, new float[] {-1, 0}, new float[] {-2, 0}, new float[] {-3, 0}, new float[] {-4, 0} },
                new float[][] { new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2}, new float[] {0, 3}, new float[] {0, 4} },
                new float[][] { new float[] {0, 0}, new float[] {0, -1}, new float[] {0, -2}, new float[] {0, -3}, new float[] {0, -4} },
                new float[][] { new float[] {0, 0}, new float[] {1, 1}, new float[] {2, 2}, new float[] {3, 3} },
                new float[][] { new float[] {0, 0}, new float[] {-1, -1}, new float[] {-2, -2}, new float[] {-3, -3} },
                new float[][] { new float[] {0, 0}, new float[] {1, -1}, new float[] {2, -2}, new float[] {3, -3} },
                new float[][] { new float[] {0, 0}, new float[] {-1, 1}, new float[] {-2, 2}, new float[] {-3, 3} }
            }));

            // Level 23: Parallel Universes
            templates.Add(CreateTemplate("Parallel Universes", "Expert", new float[][][]
            {
                new float[][] { new float[] {-3, 3}, new float[] {-2, 2}, new float[] {-1, 1}, new float[] {0, 0}, new float[] {1, -1}, new float[] {2, -2}, new float[] {3, -3} },
                new float[][] { new float[] {-3, 2}, new float[] {-2, 1}, new float[] {-1, 0}, new float[] {0, -1}, new float[] {1, -2}, new float[] {2, -3} },
                new float[][] { new float[] {-2, 3}, new float[] {-1, 2}, new float[] {0, 1}, new float[] {1, 0}, new float[] {2, -1}, new float[] {3, -2} },
                new float[][] { new float[] {-3, 0}, new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0} },
                new float[][] { new float[] {0, -3}, new float[] {0, -2}, new float[] {0, -1}, new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2}, new float[] {0, 3} },
                new float[][] { new float[] {3, 3}, new float[] {2, 2}, new float[] {1, 1}, new float[] {0, 0}, new float[] {-1, -1}, new float[] {-2, -2}, new float[] {-3, -3} }
            }));

            // Level 24: Web
            templates.Add(CreateTemplate("Web", "Expert", new float[][][]
            {
                new float[][] { new float[] {-2, 2}, new float[] {-1, 1}, new float[] {0, 0}, new float[] {1, -1}, new float[] {2, -2} },
                new float[][] { new float[] {2, 2}, new float[] {1, 1}, new float[] {0, 0}, new float[] {-1, -1}, new float[] {-2, -2} },
                new float[][] { new float[] {-3, 0}, new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0} },
                new float[][] { new float[] {0, -3}, new float[] {0, -2}, new float[] {0, -1}, new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2}, new float[] {0, 3} },
                new float[][] { new float[] {-3, 1}, new float[] {-2, 1}, new float[] {-1, 1}, new float[] {0, 1}, new float[] {1, 1}, new float[] {2, 1}, new float[] {3, 1} },
                new float[][] { new float[] {-3, -1}, new float[] {-2, -1}, new float[] {-1, -1}, new float[] {0, -1}, new float[] {1, -1}, new float[] {2, -1}, new float[] {3, -1} },
                new float[][] { new float[] {-1, -3}, new float[] {-1, -2}, new float[] {-1, -1}, new float[] {-1, 0}, new float[] {-1, 1}, new float[] {-1, 2}, new float[] {-1, 3} }
            }));

            // Level 25: Pinwheel
            templates.Add(CreateTemplate("Pinwheel", "Expert", new float[][][]
            {
                new float[][] { new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0}, new float[] {3, 1}, new float[] {3, 2} },
                new float[][] { new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2}, new float[] {0, 3}, new float[] {-1, 3}, new float[] {-2, 3} },
                new float[][] { new float[] {0, 0}, new float[] {-1, 0}, new float[] {-2, 0}, new float[] {-3, 0}, new float[] {-3, -1}, new float[] {-3, -2} },
                new float[][] { new float[] {0, 0}, new float[] {0, -1}, new float[] {0, -2}, new float[] {0, -3}, new float[] {1, -3}, new float[] {2, -3} },
                new float[][] { new float[] {-2, 2}, new float[] {-1, 1}, new float[] {0, 0}, new float[] {1, -1}, new float[] {2, -2} },
                new float[][] { new float[] {2, 2}, new float[] {1, 1}, new float[] {0, 0}, new float[] {-1, -1}, new float[] {-2, -2} },
                new float[][] { new float[] {-3, 0}, new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0} },
                new float[][] { new float[] {0, -3}, new float[] {0, -2}, new float[] {0, -1}, new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2}, new float[] {0, 3} }
            }));

            // ============================================
            // Levels 26-28: Spiral Patterns (7-10 lines)
            // ============================================

            // Level 26: Clockwork
            templates.Add(CreateTemplate("Clockwork", "Nightmare", new float[][][]
            {
                new float[][] { new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0} },
                new float[][] { new float[] {3, 0}, new float[] {3, 1}, new float[] {3, 2}, new float[] {3, 3} },
                new float[][] { new float[] {3, 3}, new float[] {2, 3}, new float[] {1, 3}, new float[] {0, 3} },
                new float[][] { new float[] {0, 3}, new float[] {0, 2}, new float[] {0, 1}, new float[] {0, 0} },
                new float[][] { new float[] {-1, 0}, new float[] {-2, 0}, new float[] {-3, 0}, new float[] {-4, 0} },
                new float[][] { new float[] {0, -1}, new float[] {0, -2}, new float[] {0, -3}, new float[] {0, -4} },
                new float[][] { new float[] {-3, 0}, new float[] {-3, -1}, new float[] {-3, -2}, new float[] {-3, -3} },
                new float[][] { new float[] {0, -3}, new float[] {-1, -3}, new float[] {-2, -3}, new float[] {-3, -3} },
                new float[][] { new float[] {-2, -2}, new float[] {-1, -1}, new float[] {0, 0}, new float[] {1, 1}, new float[] {2, 2} }
            }));

            // Level 27: Vortex
            templates.Add(CreateTemplate("Vortex", "Nightmare", new float[][][]
            {
                new float[][] { new float[] {-4, 0}, new float[] {-3, 0}, new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0}, new float[] {4, 0} },
                new float[][] { new float[] {0, -4}, new float[] {0, -3}, new float[] {0, -2}, new float[] {0, -1}, new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2}, new float[] {0, 3}, new float[] {0, 4} },
                new float[][] { new float[] {-3, -3}, new float[] {-2, -2}, new float[] {-1, -1}, new float[] {0, 0}, new float[] {1, 1}, new float[] {2, 2}, new float[] {3, 3} },
                new float[][] { new float[] {3, -3}, new float[] {2, -2}, new float[] {1, -1}, new float[] {0, 0}, new float[] {-1, 1}, new float[] {-2, 2}, new float[] {-3, 3} },
                new float[][] { new float[] {-2, 1}, new float[] {-1, 1}, new float[] {0, 1}, new float[] {1, 1}, new float[] {2, 1} },
                new float[][] { new float[] {-2, -1}, new float[] {-1, -1}, new float[] {0, -1}, new float[] {1, -1}, new float[] {2, -1} },
                new float[][] { new float[] {1, -2}, new float[] {1, -1}, new float[] {1, 0}, new float[] {1, 1}, new float[] {1, 2} },
                new float[][] { new float[] {-1, -2}, new float[] {-1, -1}, new float[] {-1, 0}, new float[] {-1, 1}, new float[] {-1, 2} },
                new float[][] { new float[] {-2, 2}, new float[] {-1, 1}, new float[] {0, 0}, new float[] {1, -1}, new float[] {2, -2} },
                new float[][] { new float[] {2, 2}, new float[] {1, 1}, new float[] {0, 0}, new float[] {-1, -1}, new float[] {-2, -2} }
            }));

            // Level 28: Labyrinth
            templates.Add(CreateTemplate("Labyrinth", "Nightmare", new float[][][]
            {
                new float[][] { new float[] {-4, -4}, new float[] {-4, -3}, new float[] {-4, -2}, new float[] {-4, -1}, new float[] {-4, 0}, new float[] {-4, 1}, new float[] {-4, 2}, new float[] {-4, 3}, new float[] {-4, 4} },
                new float[][] { new float[] {-2, 4}, new float[] {-2, 3}, new float[] {-2, 2}, new float[] {-2, 1}, new float[] {-2, 0}, new float[] {-2, -1}, new float[] {-2, -2} },
                new float[][] { new float[] {0, -4}, new float[] {0, -3}, new float[] {0, -2}, new float[] {0, -1}, new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2} },
                new float[][] { new float[] {2, 4}, new float[] {2, 3}, new float[] {2, 2}, new float[] {2, 1}, new float[] {2, 0}, new float[] {2, -1}, new float[] {2, -2} },
                new float[][] { new float[] {4, -4}, new float[] {4, -3}, new float[] {4, -2}, new float[] {4, -1}, new float[] {4, 0}, new float[] {4, 1}, new float[] {4, 2}, new float[] {4, 3}, new float[] {4, 4} },
                new float[][] { new float[] {-4, 0}, new float[] {-3, 0}, new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0}, new float[] {4, 0} },
                new float[][] { new float[] {-3, 2}, new float[] {-2, 2}, new float[] {-1, 2}, new float[] {0, 2}, new float[] {1, 2}, new float[] {2, 2}, new float[] {3, 2} },
                new float[][] { new float[] {-3, -2}, new float[] {-2, -2}, new float[] {-1, -2}, new float[] {0, -2}, new float[] {1, -2}, new float[] {2, -2}, new float[] {3, -2} }
            }));

            // ============================================
            // Levels 29-30: Boss Levels (10-12 lines)
            // ============================================

            // Level 29: Chaos Theory
            templates.Add(CreateTemplate("Chaos Theory", "Impossible", new float[][][]
            {
                new float[][] { new float[] {-4, 0}, new float[] {-3, 0}, new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0}, new float[] {4, 0} },
                new float[][] { new float[] {0, -4}, new float[] {0, -3}, new float[] {0, -2}, new float[] {0, -1}, new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2}, new float[] {0, 3}, new float[] {0, 4} },
                new float[][] { new float[] {-4, -4}, new float[] {-3, -3}, new float[] {-2, -2}, new float[] {-1, -1}, new float[] {0, 0}, new float[] {1, 1}, new float[] {2, 2}, new float[] {3, 3}, new float[] {4, 4} },
                new float[][] { new float[] {4, -4}, new float[] {3, -3}, new float[] {2, -2}, new float[] {1, -1}, new float[] {0, 0}, new float[] {-1, 1}, new float[] {-2, 2}, new float[] {-3, 3}, new float[] {-4, 4} },
                new float[][] { new float[] {-2, 3}, new float[] {-1, 2}, new float[] {0, 1}, new float[] {1, 0}, new float[] {2, -1}, new float[] {3, -2} },
                new float[][] { new float[] {2, 3}, new float[] {1, 2}, new float[] {0, 1}, new float[] {-1, 0}, new float[] {-2, -1}, new float[] {-3, -2} },
                new float[][] { new float[] {-3, -1}, new float[] {-2, -1}, new float[] {-1, -1}, new float[] {0, -1}, new float[] {1, -1}, new float[] {2, -1}, new float[] {3, -1} },
                new float[][] { new float[] {-3, 1}, new float[] {-2, 1}, new float[] {-1, 1}, new float[] {0, 1}, new float[] {1, 1}, new float[] {2, 1}, new float[] {3, 1} },
                new float[][] { new float[] {-1, -4}, new float[] {-1, -3}, new float[] {-1, -2}, new float[] {-1, -1}, new float[] {-1, 0}, new float[] {-1, 1}, new float[] {-1, 2}, new float[] {-1, 3}, new float[] {-1, 4} },
                new float[][] { new float[] {1, -4}, new float[] {1, -3}, new float[] {1, -2}, new float[] {1, -1}, new float[] {1, 0}, new float[] {1, 1}, new float[] {1, 2}, new float[] {1, 3}, new float[] {1, 4} },
                new float[][] { new float[] {-3, 3}, new float[] {-2, 2}, new float[] {-1, 1}, new float[] {0, 0}, new float[] {1, -1}, new float[] {2, -2}, new float[] {3, -3} }
            }));

            // Level 30: The Nexus (Final Boss)
            templates.Add(CreateTemplate("The Nexus", "Impossible", new float[][][]
            {
                new float[][] { new float[] {-5, 0}, new float[] {-4, 0}, new float[] {-3, 0}, new float[] {-2, 0}, new float[] {-1, 0}, new float[] {0, 0}, new float[] {1, 0}, new float[] {2, 0}, new float[] {3, 0}, new float[] {4, 0}, new float[] {5, 0} },
                new float[][] { new float[] {0, -5}, new float[] {0, -4}, new float[] {0, -3}, new float[] {0, -2}, new float[] {0, -1}, new float[] {0, 0}, new float[] {0, 1}, new float[] {0, 2}, new float[] {0, 3}, new float[] {0, 4}, new float[] {0, 5} },
                new float[][] { new float[] {-4, -4}, new float[] {-3, -3}, new float[] {-2, -2}, new float[] {-1, -1}, new float[] {0, 0}, new float[] {1, 1}, new float[] {2, 2}, new float[] {3, 3}, new float[] {4, 4} },
                new float[][] { new float[] {4, -4}, new float[] {3, -3}, new float[] {2, -2}, new float[] {1, -1}, new float[] {0, 0}, new float[] {-1, 1}, new float[] {-2, 2}, new float[] {-3, 3}, new float[] {-4, 4} },
                new float[][] { new float[] {-2, -4}, new float[] {-2, -3}, new float[] {-2, -2}, new float[] {-2, -1}, new float[] {-2, 0}, new float[] {-2, 1}, new float[] {-2, 2}, new float[] {-2, 3}, new float[] {-2, 4} },
                new float[][] { new float[] {2, -4}, new float[] {2, -3}, new float[] {2, -2}, new float[] {2, -1}, new float[] {2, 0}, new float[] {2, 1}, new float[] {2, 2}, new float[] {2, 3}, new float[] {2, 4} },
                new float[][] { new float[] {-4, -2}, new float[] {-3, -2}, new float[] {-2, -2}, new float[] {-1, -2}, new float[] {0, -2}, new float[] {1, -2}, new float[] {2, -2}, new float[] {3, -2}, new float[] {4, -2} },
                new float[][] { new float[] {-4, 2}, new float[] {-3, 2}, new float[] {-2, 2}, new float[] {-1, 2}, new float[] {0, 2}, new float[] {1, 2}, new float[] {2, 2}, new float[] {3, 2}, new float[] {4, 2} },
                new float[][] { new float[] {-3, -3}, new float[] {-2, -2}, new float[] {-1, -1}, new float[] {0, 0}, new float[] {1, 1}, new float[] {2, 2}, new float[] {3, 3} },
                new float[][] { new float[] {3, -3}, new float[] {2, -2}, new float[] {1, -1}, new float[] {0, 0}, new float[] {-1, 1}, new float[] {-2, 2}, new float[] {-3, 3} },
                new float[][] { new float[] {-3, 1}, new float[] {-2, 0}, new float[] {-1, -1}, new float[] {0, -2}, new float[] {1, -3} },
                new float[][] { new float[] {3, 1}, new float[] {2, 0}, new float[] {1, -1}, new float[] {0, -2}, new float[] {-1, -3} }
            }));

            return templates;
        }

        /// <summary>
        /// Helper to create a LevelTemplate from raw point data.
        /// </summary>
        private LevelTemplate CreateTemplate(string name, string tier, float[][][] linesData)
        {
            var template = new LevelTemplate
            {
                Name = name,
                DifficultyTier = tier,
                Lines = new List<LineTemplate>()
            };

            foreach (var lineData in linesData)
            {
                var line = new LineTemplate { Points = new List<Vector2>() };
                foreach (var point in lineData)
                {
                    line.Points.Add(new Vector2(point[0], point[1]));
                }
                template.Lines.Add(line);
            }

            return template;
        }
    }
}
