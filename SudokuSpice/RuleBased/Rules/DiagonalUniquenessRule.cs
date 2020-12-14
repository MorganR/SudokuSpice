using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Restricts that the forward and backward diagonals each contain all unique values.
    /// </summary>
    public class DiagonalUniquenessRule : ISudokuRule
    {
        private BitVector _unsetBackwardDiag;
        private BitVector _unsetForwardDiag;
        private IReadOnlyPuzzle? _puzzle;

        public DiagonalUniquenessRule() {}

        /// <inheritdoc/>
        public bool TryInit(IReadOnlyPuzzle puzzle)
        {
            _unsetForwardDiag = _unsetBackwardDiag = puzzle.AllPossibleValues;
            int size = puzzle.Size;
            // Iterate through the backward diagonal (like a backslash '\')
            for (int row = 0, col = 0; row < size; row++, col++)
            {
                int? val = puzzle[row, col];
                if (val.HasValue)
                {
                    if (!_unsetBackwardDiag.IsBitSet(val.Value))
                    {
                        // Puzzle does not satisfy diagonal uniqueness rule at ({row}, {col}).
                        return false;
                    }
                    _unsetBackwardDiag.UnsetBit(val.Value);
                }
            }
            // Iterate through the forward diagonal (like a forward slash '/')
            for (int row = 0, col = puzzle.Size - 1; row < size; row++, col--)
            {
                int? val = puzzle[row, col];
                if (val.HasValue)
                {
                    if (!_unsetForwardDiag.IsBitSet(val.Value))
                    {
                        // Puzzle does not satisfy diagonal uniqueness rule at ({row}, {col}).
                        return false;
                    }
                    _unsetForwardDiag.UnsetBit(val.Value);
                }
            }
            _puzzle = puzzle;
            return true;
        }

        private DiagonalUniquenessRule(DiagonalUniquenessRule existing, IReadOnlyPuzzle? puzzle)
        {
            _unsetBackwardDiag = existing._unsetBackwardDiag;
            _unsetForwardDiag = existing._unsetForwardDiag;
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public ISudokuRule CopyWithNewReference(IReadOnlyPuzzle? puzzle) => new DiagonalUniquenessRule(this, puzzle);

        /// <inheritdoc/>
        public BitVector GetPossibleValues(in Coordinate c)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(GetPossibleValues)} with a null puzzle.");
            if (_IsOnBackwardDiag(in c))
            {
                return _unsetBackwardDiag;
            } else if (_IsOnForwardDiag(in c))
            {
                return _unsetForwardDiag;
            } else
            {
                return _puzzle.AllPossibleValues;
            }
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(Revert)} with a null puzzle.");
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

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(Revert)} with a null puzzle.");
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

        /// <inheritdoc/>
        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(Update)} with a null puzzle.");
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

        private static bool _IsOnBackwardDiag(in Coordinate c) => c.Row == c.Column;

        private bool _IsOnForwardDiag(in Coordinate c) => c.Column == _puzzle!.Size - c.Row - 1;

        private void _AddUnsetFromBackwardDiag(in Coordinate c, CoordinateTracker coordTracker)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(_AddUnsetFromBackwardDiag)} with a null puzzle.");
            int size = _puzzle.Size;
            for (int row = 0, col = 0; row < size; row++, col++)
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
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(_AddUnsetFromForwardDiag)} with a null puzzle.");
            int size = _puzzle.Size;
            for (int row = 0, col = size - 1; row < size; row++, col--)
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
