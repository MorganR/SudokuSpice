using SudokuSpice.Data;
using System;
using System.Collections.Generic;

namespace SudokuSpice.Rules
{
    /// <summary>
    /// Enforces an arbitrary injected set of rules.
    /// </summary>
    public class DynamicRuleKeeper : ISudokuRuleKeeper
    {
        private readonly Puzzle _puzzle;
        private readonly PossibleValues _possibleValues;
        private readonly IReadOnlyList<ISudokuRule> _rules;
        private readonly IList<Coordinate> _affectedCoords;

        public DynamicRuleKeeper(Puzzle puzzle, PossibleValues possibleValues, IReadOnlyList<ISudokuRule> rules)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _rules = rules;
            _affectedCoords = new List<Coordinate>(puzzle.Size * rules.Count);
            foreach (var c in _puzzle.GetUnsetCoords())
            {
                foreach (var r in _rules)
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

        private DynamicRuleKeeper(DynamicRuleKeeper existing, Puzzle puzzle, PossibleValues possibleValues)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _affectedCoords = new List<Coordinate>(puzzle.Size * existing._rules.Count);
            var rules = new List<ISudokuRule>(existing._rules.Count);
            foreach (var rule in existing._rules)
            {
                rules.Add(rule.CopyWithNewReference(puzzle));
            }
            _rules = rules;
        }

        public ISudokuRuleKeeper CopyWithNewReferences(Puzzle puzzle, PossibleValues possibleValues)
        {
            return new DynamicRuleKeeper(this, puzzle, possibleValues);
        }

        public IReadOnlyList<ISudokuRule> GetRules()
        {
            return _rules;
        }

        public bool TrySet(in Coordinate c, int value)
        {
            _affectedCoords.Clear();
            foreach (var r in _rules)
            {
                r.Update(in c, value, _affectedCoords);
            }
            for (int i = 0; i < _affectedCoords.Count; i++)
            {
                Coordinate affectedCoord = _affectedCoords[i];
                foreach (var r in _rules)
                {
                    _possibleValues.Intersect(in affectedCoord, r.GetPossibleValues(in affectedCoord));
                }
                if (_possibleValues[in affectedCoord].IsEmpty())
                {
                    foreach (var r in _rules)
                    {
                        r.Revert(in c, value);
                    }
                    for (; i >= 0; i--)
                    {
                        affectedCoord = _affectedCoords[i];
                        _possibleValues.Reset(in affectedCoord);
                        foreach (var r in _rules)
                        {
                            _possibleValues.Intersect(in affectedCoord, r.GetPossibleValues(in affectedCoord));
                        }
                    }
                    return false;
                }
            }
            return true;
        }

        public void Unset(in Coordinate c, int value)
        {
            _affectedCoords.Clear();
            foreach (var r in _rules)
            {
                r.Revert(in c, value, _affectedCoords);
            }
            foreach (var affectedCoord in _affectedCoords)
            {
                _possibleValues.Reset(in affectedCoord);
                foreach (var r in _rules)
                {
                    _possibleValues.Intersect(in affectedCoord, r.GetPossibleValues(in affectedCoord));
                }
            }
        }
    }
}
