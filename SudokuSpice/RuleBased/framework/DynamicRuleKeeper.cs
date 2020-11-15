using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Enforces an arbitrary injected set of rules.
    /// </summary>
    public class DynamicRuleKeeper : ISudokuRuleKeeper
    {
        private readonly IReadOnlyPuzzle _puzzle;
        private readonly PossibleValues _possibleValues;
        private readonly CoordinateTracker _coordTracker;
        private readonly IReadOnlyList<ISudokuRule> _rules;

        public DynamicRuleKeeper(IReadOnlyPuzzle puzzle, PossibleValues possibleValues, IReadOnlyList<ISudokuRule> rules)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _coordTracker = new CoordinateTracker(puzzle.Size);
            _rules = rules;
            foreach (Coordinate c in _puzzle.GetUnsetCoords())
            {
                foreach (ISudokuRule? r in _rules)
                {
                    _possibleValues.Intersect(in c, r.GetPossibleValues(in c));
                }
                if (_possibleValues[in c].IsEmpty())
                {
                    throw new ArgumentException(
                        "Puzzle could not be solved with the given values.");
                }
            }
        }

        private DynamicRuleKeeper(DynamicRuleKeeper existing, IReadOnlyPuzzle puzzle, PossibleValues possibleValues)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _coordTracker = new CoordinateTracker(puzzle.Size);
            var rules = new List<ISudokuRule>(existing._rules.Count);
            foreach (ISudokuRule? rule in existing._rules)
            {
                rules.Add(rule.CopyWithNewReference(puzzle));
            }
            _rules = rules;
        }

        /// <inheritdoc/>
        public ISudokuRuleKeeper CopyWithNewReferences(IReadOnlyPuzzle puzzle, PossibleValues possibleValues) => new DynamicRuleKeeper(this, puzzle, possibleValues);

        /// <inheritdoc/>
        public IReadOnlyList<ISudokuRule> GetRules() => _rules;

        /// <inheritdoc/>
        public bool TrySet(in Coordinate c, int value)
        {
            if (!_possibleValues[in c].IsBitSet(value))
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
                    _possibleValues.Intersect(in affectedCoord, r.GetPossibleValues(in affectedCoord));
                }
                if (_possibleValues[in affectedCoord].IsEmpty())
                {
                    foreach (ISudokuRule? r in _rules)
                    {
                        r.Revert(in c, value);
                    }
                    for (; i >= 0; i--)
                    {
                        affectedCoord = trackedCoords[i];
                        _possibleValues.Reset(in affectedCoord);
                        foreach (ISudokuRule? r in _rules)
                        {
                            _possibleValues.Intersect(in affectedCoord, r.GetPossibleValues(in affectedCoord));
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
            _coordTracker.UntrackAll();
            foreach (ISudokuRule? r in _rules)
            {
                r.Revert(in c, value, _coordTracker);
            }
            foreach (Coordinate affectedCoord in _coordTracker.GetTrackedCoords())
            {
                _possibleValues.Reset(in affectedCoord);
                foreach (ISudokuRule? r in _rules)
                {
                    _possibleValues.Intersect(in affectedCoord, r.GetPossibleValues(in affectedCoord));
                }
            }
        }
    }
}
