using System;
using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Restricts that each row contains all unique values.
    /// </summary>
    public class RowUniquenessRule : ISudokuRule, IMissingRowValuesTracker
    {
        private readonly int _size;
        private readonly BitVector _allUniqueValues;
        private readonly BitVector[] _unsetRowValues;
        private IReadOnlyPuzzle? _puzzle;

        public RowUniquenessRule(BitVector allUniqueValues)
        {
            _size = allUniqueValues.Count;
            _allUniqueValues = allUniqueValues;
            _unsetRowValues = new BitVector[_size];
        }

        private RowUniquenessRule(RowUniquenessRule existing, IReadOnlyPuzzle puzzle)
        {
            _size = existing._size;
            _allUniqueValues = existing._allUniqueValues;
            _unsetRowValues = existing._unsetRowValues.AsSpan().ToArray();
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public ISudokuRule CopyWithNewReference(IReadOnlyPuzzle puzzle) => new RowUniquenessRule(this, puzzle);

        /// <inheritdoc/>
        public bool TryInitFor(IReadOnlyPuzzle puzzle)
        {
            // This should be enforced by the rule keeper.
            Debug.Assert(puzzle.Size == _size,
                $"Puzzle size ({puzzle.Size}) did not match expected size ({_size}).");
            _unsetRowValues.AsSpan().Fill(_allUniqueValues);
            for (int row = 0; row < _size; row++)
            {
                for (int col = 0; col < _size; col++)
                {
                    int? val = puzzle[row, col];
                    if (!val.HasValue)
                    {
                        continue;
                    }
                    if (!_unsetRowValues[row].IsBitSet(val.Value))
                    {
                        // Puzzle has duplicate value in this row.
                        return false;
                    }
                    _unsetRowValues[row].UnsetBit(val.Value);
                }
            }
            _puzzle = puzzle;
            return true;
        }

        /// <inheritdoc/>
        public BitVector GetPossibleValues(in Coordinate c) => _unsetRowValues[c.Row];

        /// <inheritdoc/>
        public BitVector GetMissingValuesForRow(int row) => _unsetRowValues[row];

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val)
        {
            Debug.Assert(_puzzle is not null, $"Cannot call {nameof(Revert)} when puzzle is null.");
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            _unsetRowValues[c.Row].SetBit(val);
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Revert(in c, val);
            _AddUnsetFromRow(in c, coordTracker);
        }

        /// <inheritdoc/>
        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(_puzzle is not null, $"Cannot call {nameof(Update)} when puzzle is null.");
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Update for a set puzzle coordinate");
            _unsetRowValues[c.Row].UnsetBit(val);
            _AddUnsetFromRow(in c, coordTracker);
        }

        private void _AddUnsetFromRow(in Coordinate c, CoordinateTracker coordTracker)
        {
            Debug.Assert(_puzzle is not null, "Cannot call RowUniquenessRule._AddUnsetFromRow when puzzle is null.");
            for (int col = 0; col < _size; col++)
            {
                if (col != c.Column && !_puzzle[c.Row, col].HasValue)
                {
                    coordTracker.AddOrTrackIfUntracked(new Coordinate(c.Row, col));
                }
            }
        }
    }
}
