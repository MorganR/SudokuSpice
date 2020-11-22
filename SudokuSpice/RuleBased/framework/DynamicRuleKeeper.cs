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
        private readonly PossibleValues _possibleValues;
        private readonly CoordinateTracker _coordTracker;
        private readonly IReadOnlyList<ISudokuRule> _rules;

        /// <summary>
        /// Constructs a rule keeper that will enforce all the given rules.
        /// </summary>
        /// <param name="possibleValues">
        /// The shared possible values instance to use when solving.
        /// </param>
        /// <param name="rules">The rules to enforce.</param>
        public DynamicRuleKeeper(PossibleValues possibleValues, IReadOnlyList<ISudokuRule> rules)
        {
            _possibleValues = possibleValues;
            _coordTracker = new CoordinateTracker(_possibleValues.Size);
            _rules = rules;
        }

        private DynamicRuleKeeper(DynamicRuleKeeper existing, IReadOnlyPuzzle puzzle, PossibleValues possibleValues)
        {
            _coordTracker = new CoordinateTracker(puzzle.Size);
            var rules = new List<ISudokuRule>(existing._rules.Count);
            foreach (ISudokuRule? rule in existing._rules)
            {
                rules.Add(rule.CopyWithNewReference(puzzle));
            }
            _rules = rules;
            _possibleValues = possibleValues;
        }

        /// <inheritdoc/>
        public ISudokuRuleKeeper CopyWithNewReferences(IReadOnlyPuzzle puzzle, PossibleValues possibleValues)
        {
            Debug.Assert(puzzle.Size == _possibleValues.Size,
                $"Puzzle size ({puzzle.Size}) must match current rule keeper size ({_possibleValues.Size})");
            Debug.Assert(puzzle.Size == possibleValues.Size,
                $"Puzzle size ({puzzle.Size}) must match possible values size ({possibleValues.Size})");
            return new DynamicRuleKeeper(this, puzzle, possibleValues);
        }

        /// <inheritdoc/>
        public bool TryInitFor(IReadOnlyPuzzle puzzle)
        {
            if (puzzle.Size != _possibleValues.Size)
            {
                return false;
            }
            foreach (ISudokuRule r in _rules)
            {
                if (!r.TryInitFor(puzzle))
                {
                    return false;
                }
            }
            foreach (Coordinate c in puzzle.GetUnsetCoords())
            {
                foreach (ISudokuRule r in _rules)
                {
                    _possibleValues.Intersect(in c, r.GetPossibleValues(in c));
                }
                if (_possibleValues[in c].IsEmpty())
                {
                    return false;    
                }
            }
            return true;
        }

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
