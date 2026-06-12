using System;
using System.Collections.Generic;
using UnityEngine;
using _Game.Line;
using _Game.UI;
using DG.Tweening;

namespace _Game.Gameplay
{
    /// <summary>
    /// Hint system for Line Rush.
    /// Analyzes the current level state and highlights the safest line to click.
    /// Players get 1 free hint per level; additional hints require watching rewarded ads.
    /// </summary>
    public class HintManager : MonoBehaviour
    {
        [Header("Hint Visual Settings")]
        [SerializeField] private float _pulseSpeed = 2f;
        [SerializeField] private float _pulseMin = 0.4f;
        [SerializeField] private float _pulseMax = 1.0f;
        [SerializeField] private float _hintDuration = 3f;

        private int _hintsRemaining;
        private LineManager _currentLineManager;
        private Line.Line _highlightedLine;
        private Tween _pulseTween;
        private List<Material> _hintMaterials = new List<Material>();
        private bool _isHinting;

        public int HintsRemaining => _hintsRemaining;
        public bool IsHinting => _isHinting;

        public event Action<int> OnHintCountChanged;
        public event Action OnHintRevealed;

        /// <summary>
        /// Initialize hint manager for a new level.
        /// </summary>
        public void InitializeForLevel(LineManager lineManager)
        {
            StopHint();
            _currentLineManager = lineManager;
            _hintsRemaining = AdConfig.FREE_HINTS_PER_LEVEL;
            OnHintCountChanged?.Invoke(_hintsRemaining);
        }

        /// <summary>
        /// Request a hint. Uses free hint if available, otherwise requires ad.
        /// </summary>
        public bool RequestHint()
        {
            if (_currentLineManager == null) return false;
            if (_isHinting) return false;

            if (_hintsRemaining > 0)
            {
                _hintsRemaining--;
                OnHintCountChanged?.Invoke(_hintsRemaining);
                RevealHint();
                return true;
            }
            else
            {
                // Need to watch ad - caller should handle this
                return false;
            }
        }

        /// <summary>
        /// Grant an additional hint (from rewarded ad).
        /// </summary>
        public void GrantHintFromAd()
        {
            _hintsRemaining++;
            OnHintCountChanged?.Invoke(_hintsRemaining);
            RevealHint();
        }

        /// <summary>
        /// Find and highlight the safest line to click.
        /// </summary>
        private void RevealHint()
        {
            if (_currentLineManager == null) return;

            Line.Line safestLine = FindSafestLine();
            if (safestLine == null) return;

            HighlightLine(safestLine);
            OnHintRevealed?.Invoke();
        }

        /// <summary>
        /// Find the line that is least likely to collide with other active lines.
        /// Simple heuristic: choose the line with fewest nearby intersections.
        /// </summary>
        private Line.Line FindSafestLine()
        {
            var activeLines = _currentLineManager.ActiveLines;
            if (activeLines == null || activeLines.Count == 0) return null;

            Line.Line bestLine = null;
            int minRisk = int.MaxValue;

            foreach (var line in activeLines)
            {
                if (line == null || !line.IsClickable) continue;

                LineRenderer lr = line.LineRenderer;
                if (lr == null || lr.positionCount < 2) continue;

                // Calculate risk: how many other lines could this collide with
                int risk = CalculateCollisionRisk(line, activeLines);

                if (risk < minRisk)
                {
                    minRisk = risk;
                    bestLine = line;
                }
            }

            return bestLine;
        }

        /// <summary>
        /// Calculate approximate collision risk for a line.
        /// </summary>
        private int CalculateCollisionRisk(Line.Line line, IReadOnlyList<Line.Line> allLines)
        {
            LineRenderer lr = line.LineRenderer;
            if (lr == null) return 100;

            int lastIdx = lr.positionCount - 1;
            Vector3 headPos = lr.GetPosition(lastIdx);
            Vector3 direction = (lr.GetPosition(lastIdx) - lr.GetPosition(lastIdx - 1)).normalized;
            
            int risk = 0;
            float checkDistance = 5f;

            foreach (var otherLine in allLines)
            {
                if (otherLine == null || otherLine == line) continue;

                LineRenderer otherLr = otherLine.LineRenderer;
                if (otherLr == null) continue;

                // Check if this line's path would intersect with the other line
                for (int i = 0; i < otherLr.positionCount; i++)
                {
                    Vector3 otherPoint = otherLr.GetPosition(i);
                    float dist = Vector2.Distance(headPos + direction * checkDistance * 0.5f, otherPoint);
                    
                    if (dist < 2f)
                    {
                        risk++;
                    }
                }
            }

            return risk;
        }

        /// <summary>
        /// Highlight a line with a pulsing glow effect.
        /// </summary>
        private void HighlightLine(Line.Line line)
        {
            _highlightedLine = line;
            _isHinting = true;
            _hintMaterials.Clear();

            LineRenderer lr = line.LineRenderer;
            if (lr != null && lr.material != null)
            {
                Material mat = lr.material;
                _hintMaterials.Add(mat);

                // Pulse the alpha/color
                Color hintColor = ThemeConfig.HintHighlightColor;
                mat.color = hintColor;

                _pulseTween = DOTween.To(
                    () => mat.color.a,
                    (a) =>
                    {
                        Color c = hintColor;
                        c.a = a;
                        if (mat != null) mat.color = c;
                    },
                    _pulseMin, 
                    1f / _pulseSpeed
                )
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true)
                .SetLink(line.gameObject, LinkBehaviour.KillOnDestroy);
            }

            // Auto-stop after duration
            Invoke(nameof(StopHint), _hintDuration);
        }

        /// <summary>
        /// Stop the current hint display.
        /// </summary>
        public void StopHint()
        {
            CancelInvoke(nameof(StopHint));
            _pulseTween?.Kill();
            _pulseTween = null;
            _isHinting = false;
            _highlightedLine = null;
            _hintMaterials.Clear();
        }

        private void OnDestroy()
        {
            StopHint();
        }
    }
}
