using System;
using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Restricts that each column contains all unique values.
    /// </summary>
    public class ColumnUniquenessRule : ISudokuRule, IMissingColumnValuesTracker
    {
        private readonly IReadOnlyPuzzle _puzzle;
        private readonly BitVector[] _unsetColumnValues;

        public ColumnUniquenessRule(IReadOnlyPuzzle puzzle, BitVector allUniqueValues)
        {
            Debug.Assert(puzzle.Size == allUniqueValues.Count,
                $"Can't enforce box uniqueness for mismatched puzzle size {puzzle.Size} and number of unique values {allUniqueValues.Count}");
            _puzzle = puzzle;
            _unsetColumnValues = new BitVector[puzzle.Size];
            _unsetColumnValues.AsSpan().Fill(allUniqueValues);
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    var val = puzzle[row, col];
                    if (!val.HasValue)
                    {
                        continue;
                    }
                    if (!_unsetColumnValues[col].IsBitSet(val.Value))
                    {
                        throw new ArgumentException($"Puzzle has duplicate value in column at ({row}, {col}).");
                    }
                    _unsetColumnValues[col].UnsetBit(val.Value);
                }
            }
        }

        private ColumnUniquenessRule(ColumnUniquenessRule existing, IReadOnlyPuzzle puzzle)
        {
            _puzzle = puzzle;
            _unsetColumnValues = existing._unsetColumnValues.AsSpan().ToArray();
        }

        /// <inheritdoc/>
        public ISudokuRule CopyWithNewReference(IReadOnlyPuzzle puzzle)
        {
            return new ColumnUniquenessRule(this, puzzle);
        }

        /// <inheritdoc/>
        public BitVector GetPossibleValues(in Coordinate c) => _unsetColumnValues[c.Column];
        
        /// <inheritdoc/>
        public BitVector GetMissingValuesForColumn(int col) => _unsetColumnValues[col];

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            _unsetColumnValues[c.Column].SetBit(val);
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            this.Revert(in c, val);
            _AddUnsetFromColumn(in c, coordTracker);
        }

        /// <inheritdoc/>
        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Update for a set puzzle coordinate");
            _unsetColumnValues[c.Column].UnsetBit(val);
            _AddUnsetFromColumn(in c, coordTracker);
        }

        private void _AddUnsetFromColumn(in Coordinate c, CoordinateTracker coordTracker)
        {
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
