using System;
using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Restricts that each 'box' contains all unique values.
    /// </summary>
    public class BoxUniquenessRule : ISudokuRule, IMissingBoxValuesTracker
    {
        private readonly int _size;
        private readonly BitVector _allUniqueValues;
        private readonly BitVector[] _unsetBoxValues;
        private IReadOnlyBoxPuzzle? _puzzle;

        public BoxUniquenessRule(BitVector allUniqueValues)
        {
            _size = allUniqueValues.Count;
            _allUniqueValues = allUniqueValues;
            _unsetBoxValues = new BitVector[_size];
        }

        private BoxUniquenessRule(BoxUniquenessRule existing, IReadOnlyBoxPuzzle puzzle)
        {
            _size = existing._size;
            _allUniqueValues = existing._allUniqueValues;
            _unsetBoxValues = existing._unsetBoxValues.AsSpan().ToArray();
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public ISudokuRule CopyWithNewReference(IReadOnlyPuzzle puzzle)
        {
            if (puzzle is not IReadOnlyBoxPuzzle boxPuzzle)
            {
                throw new ArgumentException($"An {nameof(IReadOnlyBoxPuzzle)} is required to copy {nameof(BoxUniquenessRule)}.");
            }
            return new BoxUniquenessRule(this, boxPuzzle);
        }

        /// <summary>
        /// Tries to initialize this rule to prepare to solve the given puzzle.
        /// </summary>
        /// <param name="puzzle">
        /// The puzzle to be solved. Must implement <see cref="IReadOnlyBoxPuzzle"/>, else this
        /// fails and returns false.
        /// </param>
        /// <returns>
        /// False if the puzzle violates this rule and initialization fails, else true.
        /// </returns>
        public bool TryInitFor(IReadOnlyPuzzle puzzle)
        {
            // This should be enforced by the rule keeper.
            Debug.Assert(puzzle.Size == _size,
                $"Puzzle size ({puzzle.Size}) did not match expected size ({_size}).");
            if (puzzle is not IReadOnlyBoxPuzzle boxPuzzle)
            {
                return false;
            }
            _unsetBoxValues.AsSpan().Fill(_allUniqueValues);
            int boxIdx = -1;
            int boxSize = boxPuzzle.BoxSize;
            for (int row = 0; row < _size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    if (col == 0)
                    {
                        boxIdx = (row / boxSize) * boxSize;
                    } else if (col % boxSize == 0)
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
                        // Puzzle has duplicate value in box at ({row}, {col}).
                        return false;
                    }
                    _unsetBoxValues[boxIdx].UnsetBit(val.Value);
                }
            }
            _puzzle = boxPuzzle;
            return true;
        }

        /// <inheritdoc/>
        public BitVector GetPossibleValues(in Coordinate c)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(GetPossibleValues)} with a null puzzle.");
            return _unsetBoxValues[_puzzle.GetBoxIndex(c.Row, c.Column)];
        }

        /// <inheritdoc/>
        public BitVector GetMissingValuesForBox(int box) => _unsetBoxValues[box];

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(Revert)} with a null puzzle.");
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            _unsetBoxValues[_puzzle.GetBoxIndex(c.Row, c.Column)].SetBit(val);
        }

        /// <inheritdoc/>
        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(Revert)} with a null puzzle.");
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            int idx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[idx].SetBit(val);
            _AddUnsetFromBox(in c, idx, coordTracker);
        }

        /// <inheritdoc/>
        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(Update)} with a null puzzle.");
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Update for a set puzzle coordinate");
            int idx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[idx].UnsetBit(val);
            _AddUnsetFromBox(in c, idx, coordTracker);
        }

        private void _AddUnsetFromBox(in Coordinate c, int box, CoordinateTracker coordTracker)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(Update)} with a null puzzle.");
            foreach (Coordinate unsetCoord in _puzzle.YieldUnsetCoordsForBox(box))
            {
                if (c.Column == unsetCoord.Column && c.Row == unsetCoord.Row)
                {
                    continue;
                }
                coordTracker.AddOrTrackIfUntracked(unsetCoord);
            }
        }
    }
}
