using SudokuSpice.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice.Rules
{
    /// <summary>
    /// Restricts that each 'box' contains all unique values.
    /// </summary>
    public class BoxUniquenessRule : ISudokuRule, IMissingBoxValuesTracker
    {
        private readonly Puzzle _puzzle;
        private readonly BitVector[] _unsetBoxValues;
        private readonly bool _skipMatchingRowAndCol;

        public BoxUniquenessRule(Puzzle puzzle, bool skipMatchingRowAndCol)
        {
            _puzzle = puzzle;
            _unsetBoxValues = new BitVector[puzzle.Size];
            _skipMatchingRowAndCol = skipMatchingRowAndCol;
            BitVector allPossible = BitVector.CreateWithSize(puzzle.Size);
            for (int i = 0; i < puzzle.Size; i++)
            {
                _unsetBoxValues[i] = allPossible;
            }
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
                    var val = puzzle[row, col];
                    if (!val.HasValue)
                    {
                        continue;
                    }
                    int bit = val.Value - 1;
                    if (!_unsetBoxValues[boxIdx].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle has duplicate value in box at ({row}, {col}).");
                    }
                    _unsetBoxValues[boxIdx].UnsetBit(bit);
                }
            }
        }

        private BoxUniquenessRule(BoxUniquenessRule existing, Puzzle puzzle)
        {
            _puzzle = puzzle;
            _unsetBoxValues = (BitVector[])existing._unsetBoxValues.Clone();
            _skipMatchingRowAndCol = existing._skipMatchingRowAndCol;
        }

        public ISudokuRule CopyWithNewReference(Puzzle puzzle)
        {
            return new BoxUniquenessRule(this, puzzle);
        }

        public BitVector GetPossibleValues(in Coordinate c) => _unsetBoxValues[_puzzle.GetBoxIndex(c.Row, c.Column)];

        public BitVector GetMissingValuesForBox(int box) => _unsetBoxValues[box];

        public void Revert(in Coordinate c, int val)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            _unsetBoxValues[_puzzle.GetBoxIndex(c.Row, c.Column)].SetBit(val - 1);
        }

        public void Revert(in Coordinate c, int val, IList<Coordinate> affectedCoords)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            int idx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[idx].SetBit(val - 1);
            _AddUnsetFromBox(in c, idx, affectedCoords);
        }

        public void Update(in Coordinate c, int val, IList<Coordinate> affectedCoords)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Update for a set puzzle coordinate");
            int idx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[idx].UnsetBit(val - 1);
            _AddUnsetFromBox(in c, idx, affectedCoords);
        }

        private void _AddUnsetFromBox(in Coordinate c, int box, IList<Coordinate> unsetCoords)
        {
            foreach (var unsetCoord in _puzzle.YieldUnsetCoordsForBox(box))
            {
                if ((_skipMatchingRowAndCol &&
                    (c.Column == unsetCoord.Column || c.Row == unsetCoord.Row))
                    || (c.Column == unsetCoord.Column && c.Row == unsetCoord.Row))
                {
                    continue;
                }
                unsetCoords.Add(unsetCoord);
            }
        }
    }
}
