using System;
using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Restricts that each column contains all unique values.
    /// </summary>
    public class ColumnUniquenessRule : ISudokuRule, IMissingColumnValuesTracker
    {
        private readonly int _size;
        private readonly BitVector _allUniqueValues;
        private readonly BitVector[] _unsetColumnValues;
        private IReadOnlyPuzzle? _puzzle;

        public ColumnUniquenessRule(BitVector allUniqueValues)
        {
            _size = allUniqueValues.Count;
            _allUniqueValues = allUniqueValues;
            _unsetColumnValues = new BitVector[_size];
        }

        private ColumnUniquenessRule(ColumnUniquenessRule existing, IReadOnlyPuzzle puzzle)
        {
            _size = existing._size;
            _allUniqueValues = existing._allUniqueValues;
            _unsetColumnValues = existing._unsetColumnValues.AsSpan().ToArray();
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public ISudokuRule CopyWithNewReference(IReadOnlyPuzzle puzzle) => new ColumnUniquenessRule(this, puzzle);

        /// <inheritdoc/>
        public bool TryInitFor(IReadOnlyPuzzle puzzle)
        {
            // This should be enforced by the rule keeper.
            Debug.Assert(puzzle.Size == _size,
                $"Puzzle size ({puzzle.Size}) did not match expected size ({_size}).");
            _unsetColumnValues.AsSpan().Fill(_allUniqueValues);
            for (int row = 0; row < _size; row++)
            {
                for (int col = 0; col < _size; col++)
                {
                    int? val = puzzle[row, col];
                    if (!val.HasValue)
                    {
                        continue;
                    }
                    if (!_unsetColumnValues[col].IsBitSet(val.Value))
                    {
                        // Puzzle has duplicate value in column at ({row}, {col}).
                        return false;
                    }
                    _unsetColumnValues[col].UnsetBit(val.Value);
                }
            }
            _puzzle = puzzle;
            return true;
        }

        /// <inheritdoc/>
        public BitVector GetPossibleValues(in Coordinate c) => _unsetColumnValues[c.Column];

        /// <inheritdoc/>
        public BitVector GetMissingValuesForColumn(int col) => _unsetColumnValues[col];

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val)
        {
            Debug.Assert(_puzzle is not null, $"Cannot call {nameof(Revert)} when puzzle is null.");
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            _unsetColumnValues[c.Column].SetBit(val);
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Revert(in c, val);
            _AddUnsetFromColumn(in c, coordTracker);
        }

        /// <inheritdoc/>
        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(_puzzle is not null, $"Cannot call {nameof(Update)} when puzzle is null.");
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Update for a set puzzle coordinate");
            _unsetColumnValues[c.Column].UnsetBit(val);
            _AddUnsetFromColumn(in c, coordTracker);
        }

        private void _AddUnsetFromColumn(in Coordinate c, CoordinateTracker coordTracker)
        {
            Debug.Assert(_puzzle is not null, $"Cannot call {nameof(_AddUnsetFromColumn)} when puzzle is null.");
            for (int row = 0; row < _puzzle.Size; row++)
            {
                if (row != c.Row && !_puzzle[row, c.Column].HasValue)
                {
                    coordTracker.AddOrTrackIfUntracked(new Coordinate(row, c.Column));
                }
            }
        }
    }
}
