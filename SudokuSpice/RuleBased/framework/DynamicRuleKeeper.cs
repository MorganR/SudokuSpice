using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Enforces an arbitrary injected set of rules.
    /// </summary>
    public class DynamicRuleKeeper : ISudokuRuleKeeper
    {
        private readonly IReadOnlyList<ISudokuRule> _rules;
        private IReadOnlyPuzzleWithMutablePossibleValues? _puzzle;
        private CoordinateTracker? _coordTracker;

        /// <summary>
        /// Constructs a rule keeper that will enforce all the given rules.
        /// </summary>
        /// <param name="rules">The rules to enforce.</param>
        public DynamicRuleKeeper(IReadOnlyList<ISudokuRule> rules)
        {
            _rules = rules;
        }

        private DynamicRuleKeeper(DynamicRuleKeeper existing, IReadOnlyPuzzleWithMutablePossibleValues? puzzle)
        {
            var rules = new List<ISudokuRule>(existing._rules.Count);
            foreach (ISudokuRule? rule in existing._rules)
            {
                rules.Add(rule.CopyWithNewReference(puzzle));
            }
            _rules = rules;
            _puzzle = puzzle;
            _coordTracker = puzzle is null ? null : new CoordinateTracker(puzzle.Size);
        }

        /// <inheritdoc/>
        public ISudokuRuleKeeper CopyWithNewReferences(IReadOnlyPuzzleWithMutablePossibleValues? puzzle)
        {
            return new DynamicRuleKeeper(this, puzzle);
        }

        /// <inheritdoc/>
        public bool TryInit(IReadOnlyPuzzleWithMutablePossibleValues puzzle)
        {
            foreach (ISudokuRule r in _rules)
            {
                if (!r.TryInit(puzzle))
                {
                    return false;
                }
            }
            foreach (Coordinate c in puzzle.GetUnsetCoords())
            {
                foreach (ISudokuRule r in _rules)
                {
                    puzzle.IntersectPossibleValues(in c, r.GetPossibleValues(in c));
                }
                if (puzzle.GetPossibleValues(in c).IsEmpty())
                {
                    return false;    
                }
            }
            if (_coordTracker is null || _coordTracker.Size != puzzle.Size)
            {
                _coordTracker = new CoordinateTracker(puzzle.Size);
            }
            _puzzle = puzzle;
            return true;
        }

        /// <inheritdoc/>
        public IReadOnlyList<ISudokuRule> GetRules() => _rules;

        /// <inheritdoc/>
        public bool TrySet(in Coordinate c, int value)
        {
            Debug.Assert(_puzzle is not null
                         && _coordTracker is not null,
                         $"Rule keeper must be initialized before calling {nameof(TrySet)}.");
            if (!_puzzle.GetPossibleValues(in c).IsBitSet(value))
            {
                return false;
            }
            _coordTracker.UntrackAll();
            foreach (ISudokuRule? r in _rules)
            {
                r.Update(in c, value, _coordTracker);
            }
            ReadOnlySpan<Coordinate> trackedCoords = _coordTracker.GetTrackedCoords();
            for (int i = 0; i < trackedCoords.Length; i++)
            {
                Coordinate affectedCoord = trackedCoords[i];
                foreach (ISudokuRule? r in _rules)
                {
                    _puzzle.IntersectPossibleValues(in affectedCoord, r.GetPossibleValues(in affectedCoord));
                }
                if (_puzzle.GetPossibleValues(in affectedCoord).IsEmpty())
                {
                    foreach (ISudokuRule? r in _rules)
                    {
                        r.Revert(in c, value);
                    }
                    for (; i >= 0; i--)
                    {
                        affectedCoord = trackedCoords[i];
                        _puzzle.ResetPossibleValues(in affectedCoord);
                        foreach (ISudokuRule? r in _rules)
                        {
                            _puzzle.IntersectPossibleValues(in affectedCoord, r.GetPossibleValues(in affectedCoord));
                        }
                    }
                    return false;
                }
            }
            return true;
        }

        /// <inheritdoc/>
        public void Unset(in Coordinate c, int value)
        {
            Debug.Assert(_puzzle is not null
                         && _coordTracker is not null,
                         $"Rule keeper must be initialized before calling {nameof(Unset)}.");
            _coordTracker.UntrackAll();
            foreach (ISudokuRule? r in _rules)
            {
                r.Revert(in c, value, _coordTracker);
            }
            foreach (Coordinate affectedCoord in _coordTracker.GetTrackedCoords())
            {
                _puzzle.ResetPossibleValues(in affectedCoord);
                foreach (ISudokuRule? r in _rules)
                {
                    _puzzle.IntersectPossibleValues(in affectedCoord, r.GetPossibleValues(in affectedCoord));
                }
            }
        }
    }
}
