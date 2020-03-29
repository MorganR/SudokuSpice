using SudokuSpice.Data;
using System;
using System.Diagnostics;

namespace SudokuSpice.Rules
{
    /// <summary>
    /// Combines the standard rules (row, column, and box uniqueness) for efficiency and
    /// convenience.
    /// </summary>
    public class StandardRules : ISudokuRule, IMissingRowValuesTracker, IMissingColumnValuesTracker, IMissingBoxValuesTracker
    {
        private readonly IReadOnlyPuzzle _puzzle;
        private readonly BitVector[] _unsetRowValues;
        private readonly BitVector[] _unsetColValues;
        private readonly BitVector[] _unsetBoxValues;

        public StandardRules(IReadOnlyPuzzle puzzle, BitVector allUniqueValues)
        {
            _puzzle = puzzle;
            _unsetRowValues = new BitVector[puzzle.Size];
            _unsetRowValues.AsSpan().Fill(allUniqueValues);
            _unsetColValues = _unsetRowValues.AsSpan().ToArray();
            _unsetBoxValues = _unsetRowValues.AsSpan().ToArray();

            int boxIdx = 0;
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    if (col == 0)
                    {
                        boxIdx = (row / puzzle.BoxSize) * puzzle.BoxSize;
                    }
                    else if (col % puzzle.BoxSize == 0)
                    {
                        boxIdx++;
                    }
                    var val = puzzle[row, col];
                    if (!val.HasValue)
                    {
                        continue;
                    }
                    int bit = val.Value - 1;
                    if (!_unsetRowValues[row].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle does not satisfy rule at ({row}, {col}).");
                    }
                    if (!_unsetColValues[col].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle does not satisfy rule at ({row}, {col}).");
                    }
                    if (!_unsetBoxValues[boxIdx].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle does not satisfy rule at ({row}, {col}).");
                    }
                    _unsetRowValues[row].UnsetBit(bit);
                    _unsetColValues[col].UnsetBit(bit);
                    _unsetBoxValues[boxIdx].UnsetBit(bit);
                }
            }
        }

        private StandardRules(StandardRules existing, IReadOnlyPuzzle puzzle)
        {
            _puzzle = puzzle;
            _unsetRowValues = existing._unsetRowValues.AsSpan().ToArray();
            _unsetColValues = existing._unsetColValues.AsSpan().ToArray();
            _unsetBoxValues = existing._unsetBoxValues.AsSpan().ToArray();
        }

        public ISudokuRule CopyWithNewReference(IReadOnlyPuzzle puzzle)
        {
            return new StandardRules(this, puzzle);
        }

        public BitVector GetMissingValuesForRow(int row) => _unsetRowValues[row];

        public BitVector GetMissingValuesForColumn(int column) => _unsetColValues[column];

        public BitVector GetMissingValuesForBox(int box) => _unsetBoxValues[box];

        public BitVector GetPossibleValues(in Coordinate c)
        {
            return BitVector.FindIntersect(
                _unsetRowValues[c.Row],
                BitVector.FindIntersect(
                    _unsetColValues[c.Column],
                    _unsetBoxValues[_puzzle.GetBoxIndex(c.Row, c.Column)]));
        }

        public void Revert(in Coordinate c, int val)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            _unsetRowValues[c.Row].SetBit(val - 1);
            _unsetColValues[c.Column].SetBit(val - 1);
            _unsetBoxValues[_puzzle.GetBoxIndex(c.Row, c.Column)].SetBit(val - 1);
        }

        public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Revert for a set puzzle coordinate");
            _unsetRowValues[c.Row].SetBit(val - 1);
            _unsetColValues[c.Column].SetBit(val - 1);
            int boxIdx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[boxIdx].SetBit(val - 1);
            _AddUnset(in c, boxIdx, coordTracker);
        }

        public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot call ISudokuRule.Update for a set puzzle coordinate");
            _unsetRowValues[c.Row].UnsetBit(val - 1);
            _unsetColValues[c.Column].UnsetBit(val - 1);
            int boxIdx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[boxIdx].UnsetBit(val - 1);
            _AddUnset(in c, boxIdx, coordTracker);
        }

        private void _AddUnset(in Coordinate c, int boxIdx, CoordinateTracker coordTracker)
        {
            for (int col = 0; col < _puzzle.Size; col++)
            {
                if (col != c.Column && !_puzzle[c.Row, col].HasValue)
                {
                    coordTracker.AddOrTrackIfUntracked(new Coordinate(c.Row, col));
                }
            }
            for (int row = 0; row < _puzzle.Size; row++)
            {
                if (row != c.Row && !_puzzle[row, c.Column].HasValue)
                {
                    coordTracker.AddOrTrackIfUntracked(new Coordinate(row, c.Column));
                }
            }
            foreach (var inBoxCoord in _puzzle.YieldUnsetCoordsForBox(boxIdx))
            {
                if (inBoxCoord.Row == c.Row || inBoxCoord.Column == c.Column)
                {
                    continue;
                }
                coordTracker.AddOrTrackIfUntracked(inBoxCoord);
            }
        }
    }
}
