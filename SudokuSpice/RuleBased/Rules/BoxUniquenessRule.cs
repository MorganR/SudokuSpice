using System;
using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Restricts that each 'box' contains all unique values.
    /// </summary>
    public class BoxUniquenessRule : ISudokuRule, IMissingBoxValuesTracker
    {
        private readonly IReadOnlyBoxPuzzle _puzzle;
        private readonly BitVector[] _unsetBoxValues;
        private readonly bool _skipMatchingRowAndCol;

        public BoxUniquenessRule(IReadOnlyBoxPuzzle puzzle, BitVector allUniqueValues, bool skipMatchingRowAndCol)
        {
            Debug.Assert(puzzle.Size == allUniqueValues.Count,
                $"Can't enforce box uniqueness for mismatched puzzle size {puzzle.Size} and number of unique values {allUniqueValues.Count}");
            _puzzle = puzzle;
            _unsetBoxValues = new BitVector[puzzle.Size];
            _unsetBoxValues.AsSpan().Fill(allUniqueValues);
            _skipMatchingRowAndCol = skipMatchingRowAndCol;
            int boxIdx = -1;
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    if (col == 0)
                    {
                        boxIdx = (row / puzzle.BoxSize) * puzzle.BoxSize;
                    } else if (col % puzzle.BoxSize == 0)
                    {
                        boxIdx++;
                    }
                    int? val = puzzle[row, col];
                    if (!val.HasValue)
                    {
                        continue;
                    }
                    if (!_unsetBoxValues[boxIdx].IsBitSet(val.Value))
                    {
                        throw new ArgumentException($"Puzzle has duplicate value in box at ({row}, {col}).");
                    }
                    _unsetBoxValues[boxIdx].UnsetBit(val.Value);
                }
            }
        }

        private BoxUniquenessRule(BoxUniquenessRule existing, IReadOnlyBoxPuzzle puzzle)
        {
            _puzzle = puzzle;
            _unsetBoxValues = existing._unsetBoxValues.AsSpan().ToArray();
            _skipMatchingRowAndCol = existing._skipMatchingRowAndCol;
        }

        /// <inheritdoc/>
        public ISudokuRule CopyWithNewReference(IReadOnlyPuzzle puzzle)
        {
            if (puzzle is IReadOnlyBoxPuzzle boxPuzzle)
            {
                return new BoxUniquenessRule(this, boxPuzzle);
            }
            throw new ArgumentException($"An {nameof(IReadOnlyBoxPuzzle)} is required to copy {nameof(BoxUniquenessRule)}.");
        }

        /// <inheritdoc/>
        public BitVector GetPossibleValues(in Coordinate c) => _unsetBoxValues[_puzzle.GetBoxIndex(c.Row, c.Column)];

        /// <inheritdoc/>
        public BitVector GetMissingValuesForBox(int box) => _unsetBoxValues[box];

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            _unsetBoxValues[_puzzle.GetBoxIndex(c.Row, c.Column)].SetBit(val);
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            int idx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[idx].SetBit(val);
            _AddUnsetFromBox(in c, idx, coordTracker);
        }

        /// <inheritdoc/>
        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Update for a set puzzle coordinate");
            int idx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[idx].UnsetBit(val);
            _AddUnsetFromBox(in c, idx, coordTracker);
        }

        private void _AddUnsetFromBox(in Coordinate c, int box, CoordinateTracker coordTracker)
        {
            foreach (Coordinate unsetCoord in _puzzle.YieldUnsetCoordsForBox(box))
            {
                if ((_skipMatchingRowAndCol &&
                    (c.Column == unsetCoord.Column || c.Row == unsetCoord.Row))
                    || (c.Column == unsetCoord.Column && c.Row == unsetCoord.Row))
                {
                    continue;
                }
                coordTracker.AddOrTrackIfUntracked(unsetCoord);
            }
        }
    }
}
