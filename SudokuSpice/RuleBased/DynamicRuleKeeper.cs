using SudokuSpice.RuleBased.Rules;
using System;
using System.Diagnostics;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Enforces an arbitrary injected set of rules.
    /// </summary>
    public class DynamicRuleKeeper : IRuleKeeper
    {
        private readonly IRule[] _rules;
        private IReadOnlyPuzzleWithMutablePossibleValues? _puzzle;
        private CoordinateTracker? _coordTracker;

        /// <summary>
        /// Constructs a rule keeper that will enforce all the given rules.
        /// </summary>
        /// <param name="rules">The rules to enforce.</param>
        public DynamicRuleKeeper(IRule[] rules)
        {
            _rules = rules;
        }

        private DynamicRuleKeeper(DynamicRuleKeeper existing, IReadOnlyPuzzleWithMutablePossibleValues? puzzle)
        {
            _rules = new IRule[existing._rules.Length];
            for (int i = 0; i < _rules.Length; ++i)
            {
                _rules[i] = existing._rules[i].CopyWithNewReference(puzzle);
            }
            _puzzle = puzzle;
            _coordTracker = puzzle is null ? null : new CoordinateTracker(puzzle.Size);
        }

        /// <inheritdoc/>
        public IRuleKeeper CopyWithNewReferences(IReadOnlyPuzzleWithMutablePossibleValues? puzzle)
        {
            return new DynamicRuleKeeper(this, puzzle);
        }

        /// <inheritdoc/>
        public bool TryInit(IReadOnlyPuzzleWithMutablePossibleValues puzzle)
        {
            foreach (IRule r in _rules)
            {
                if (!r.TryInit(puzzle, puzzle.AllPossibleValues))
                {
                    return false;
                }
            }
            foreach (Coordinate c in puzzle.GetUnsetCoords())
            {
                foreach (IRule r in _rules)
                {
                    puzzle.IntersectPossibleValues(in c, r.GetPossibleValues(in c));
                }
                if (puzzle.GetPossibleValues(in c).IsEmpty)
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
        public ReadOnlySpan<IRule> GetRules() => _rules;

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
            foreach (IRule? r in _rules)
            {
                r.Update(in c, value, _coordTracker);
            }
            ReadOnlySpan<Coordinate> trackedCoords = _coordTracker.TrackedCoords;
            for (int i = 0; i < trackedCoords.Length; i++)
            {
                Coordinate affectedCoord = trackedCoords[i];
                foreach (IRule? r in _rules)
                {
                    _puzzle.IntersectPossibleValues(in affectedCoord, r.GetPossibleValues(in affectedCoord));
                }
                if (_puzzle.GetPossibleValues(in affectedCoord).IsEmpty)
                {
                    foreach (IRule? r in _rules)
                    {
                        r.Revert(in c, value);
                    }
                    for (; i >= 0; i--)
                    {
                        affectedCoord = trackedCoords[i];
                        _puzzle.ResetPossibleValues(in affectedCoord);
                        foreach (IRule? r in _rules)
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
            foreach (IRule? r in _rules)
            {
                r.Revert(in c, value, _coordTracker);
            }
            foreach (Coordinate affectedCoord in _coordTracker.TrackedCoords)
            {
                _puzzle.ResetPossibleValues(in affectedCoord);
                foreach (IRule? r in _rules)
                {
                    _puzzle.IntersectPossibleValues(in affectedCoord, r.GetPossibleValues(in affectedCoord));
                }
            }
        }
    }
}