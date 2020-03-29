using SudokuSpice.Data;
using System;
using System.Diagnostics;

namespace SudokuSpice.Rules
{
    /// <summary>
    /// Restricts that the forward and backward diagonals each contain all unique values.
    /// </summary>
    public class DiagonalUniquenessRule : ISudokuRule
    {
        private readonly IReadOnlyPuzzle _puzzle;
        private readonly BitVector _allUnset;
        private BitVector _unsetBackwardDiag;
        private BitVector _unsetForwardDiag;

        public DiagonalUniquenessRule(IReadOnlyPuzzle puzzle, BitVector allUniqueValues)
        {
            Debug.Assert(puzzle.Size == allUniqueValues.Count,
                $"Can't enforce box uniqueness for mismatched puzzle size {puzzle.Size} and number of unique values {allUniqueValues.Count}");
            _puzzle = puzzle;
            _allUnset = _unsetForwardDiag = _unsetBackwardDiag = allUniqueValues;
            // Iterate through the backward diagonal (like a backslash '\')
            for (int row = 0, col = 0; row < puzzle.Size; row++, col++)
            {
                var val = puzzle[row, col];
                if (val.HasValue)
                {
                    if (!_unsetBackwardDiag.IsBitSet(val.Value))
                    {
                        throw new ArgumentException(
                            $"Puzzle does not satisfy diagonal uniqueness rule at ({row}, {col}).");
                    }
                    _unsetBackwardDiag.UnsetBit(val.Value);
                }
            }
            // Iterate through the forward diagonal (like a forward slash '/')
            for (int row = 0, col = puzzle.Size - 1; row < puzzle.Size; row++, col--)
            {
                var val = puzzle[row, col];
                if (val.HasValue)
                {
                    if (!_unsetForwardDiag.IsBitSet(val.Value))
                    {
                        throw new ArgumentException(
                            $"Puzzle does not satisfy diagonal uniqueness rule at ({row}, {col}).");
                    }
                    _unsetForwardDiag.UnsetBit(val.Value);
                }
            }
        }

        private DiagonalUniquenessRule(DiagonalUniquenessRule existing, IReadOnlyPuzzle puzzle)
        {
            _puzzle = puzzle;
            _unsetBackwardDiag = existing._unsetBackwardDiag;
            _unsetForwardDiag = existing._unsetForwardDiag;
            _allUnset = existing._allUnset;
        }

        public ISudokuRule CopyWithNewReference(IReadOnlyPuzzle puzzle)
        {
            return new DiagonalUniquenessRule(this, puzzle);
        }

        public BitVector GetPossibleValues(in Coordinate c)
        {
            if (_IsOnBackwardDiag(in c))
            {
                return _unsetBackwardDiag;
            } else if (_IsOnForwardDiag(in c))
            {
                return _unsetForwardDiag;
            } else
            {
                return _allUnset;
            }
        }

        public void Revert(in Coordinate c, int val)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            if (_IsOnBackwardDiag(in c))
            {
                _unsetBackwardDiag.SetBit(val);
            }
            if (_IsOnForwardDiag(in c))
            {
                _unsetForwardDiag.SetBit(val);
            }
        }

        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            if (_IsOnBackwardDiag(in c))
            {
                _unsetBackwardDiag.SetBit(val);
                _AddUnsetFromBackwardDiag(in c, coordTracker);
            }
            if (_IsOnForwardDiag(in c))
            {
                _unsetForwardDiag.SetBit(val);
                _AddUnsetFromForwardDiag(in c, coordTracker);
            }
        }

        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Update for a set puzzle coordinate");
            if (_IsOnBackwardDiag(in c))
            {
                _unsetBackwardDiag.UnsetBit(val);
                _AddUnsetFromBackwardDiag(in c, coordTracker);
            }
            if (_IsOnForwardDiag(in c))
            {
                _unsetForwardDiag.UnsetBit(val);
                _AddUnsetFromForwardDiag(in c, coordTracker);
            }
        }

        private static bool _IsOnBackwardDiag(in Coordinate c)
        {
            return c.Row == c.Column;
        }

        private bool _IsOnForwardDiag(in Coordinate c)
        {
            return c.Column == _puzzle.Size - c.Row - 1;
        }

        private void _AddUnsetFromBackwardDiag(in Coordinate c, CoordinateTracker coordTracker)
        {
            for (int row = 0, col = 0; row < _puzzle.Size; row++, col++)
            {
                if ((row == c.Row && col == c.Column) || _puzzle[row, col].HasValue)
                {
                    continue;
                }
                coordTracker.AddOrTrackIfUntracked(new Coordinate(row, col));
            }
        }

        private void _AddUnsetFromForwardDiag(in Coordinate c, CoordinateTracker coordTracker)
        {
            for (int row = 0, col = _puzzle.Size - 1; row < _puzzle.Size; row++, col--)
            {
                if ((row == c.Row && col == c.Column) || _puzzle[row, col].HasValue)
                {
                    continue;
                }
                coordTracker.AddOrTrackIfUntracked(new Coordinate(row, col));
            }
        }
    }
}
