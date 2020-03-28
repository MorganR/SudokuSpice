using SudokuSpice.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice
{
    /// <summary>
    /// Combines the standard restricts (Row, Column, and Box) for efficiency and
    /// convenience.
    /// </summary>
    public class StandardRestrict : ISudokuRestrict, IRowRestrict, IColumnRestrict, IBoxRestrict
    {
        private readonly Puzzle _puzzle;
        private readonly BitVector[] _unsetRowValues;
        private readonly BitVector[] _unsetColValues;
        private readonly BitVector[] _unsetBoxValues;

        public StandardRestrict(Puzzle puzzle)
        {
            _puzzle = puzzle;
            _unsetRowValues = new BitVector[puzzle.Size];
            _unsetColValues = new BitVector[puzzle.Size];
            _unsetBoxValues = new BitVector[puzzle.Size];
            BitVector allPossibles = BitVector.CreateWithSize(puzzle.Size);
            for (int i = 0; i < puzzle.Size; i++)
            {
                _unsetRowValues[i] = allPossibles;
                _unsetColValues[i] = allPossibles;
                _unsetBoxValues[i] = allPossibles;
            }

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
                        throw new ArgumentException($"Puzzle does not satisfy restrict at ({row}, {col}).");
                    }
                    if (!_unsetColValues[col].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle does not satisfy restrict at ({row}, {col}).");
                    }
                    if (!_unsetBoxValues[boxIdx].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle does not satisfy restrict at ({row}, {col}).");
                    }
                    _unsetRowValues[row].UnsetBit(bit);
                    _unsetColValues[col].UnsetBit(bit);
                    _unsetBoxValues[boxIdx].UnsetBit(bit);
                }
            }
        }

        private StandardRestrict(StandardRestrict existing, Puzzle puzzle)
        {
            _puzzle = puzzle;
            _unsetRowValues = (BitVector[])existing._unsetRowValues.Clone();
            _unsetColValues = (BitVector[])existing._unsetColValues.Clone();
            _unsetBoxValues = (BitVector[])existing._unsetBoxValues.Clone();
        }

        public ISudokuRestrict CopyWithNewReference(Puzzle puzzle)
        {
            return new StandardRestrict(this, puzzle);
        }

        public BitVector GetPossibleRowValues(int row) => _unsetRowValues[row];

        public BitVector GetPossibleColumnValues(int column) => _unsetColValues[column];

        public BitVector GetPossibleBoxValues(int box) => _unsetBoxValues[box];

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
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot revert a restrict for a set puzzle coordinate");
            _unsetRowValues[c.Row].SetBit(val - 1);
            _unsetColValues[c.Column].SetBit(val - 1);
            _unsetBoxValues[_puzzle.GetBoxIndex(c.Row, c.Column)].SetBit(val - 1);
        }

        public void Revert(in Coordinate c, int val, IList<Coordinate> affectedCoords)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot revert a restrict for a set puzzle coordinate");
            _unsetRowValues[c.Row].SetBit(val - 1);
            _unsetColValues[c.Column].SetBit(val - 1);
            int boxIdx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[boxIdx].SetBit(val - 1);
            _AddUnset(in c, boxIdx, affectedCoords);
        }

        public void Update(in Coordinate c, int val, IList<Coordinate> affectedCoords)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot update a restrict for a set puzzle coordinate");
            _unsetRowValues[c.Row].UnsetBit(val - 1);
            _unsetColValues[c.Column].UnsetBit(val - 1);
            int boxIdx = _puzzle.GetBoxIndex(c.Row, c.Column);
            _unsetBoxValues[boxIdx].UnsetBit(val - 1);
            _AddUnset(in c, boxIdx, affectedCoords);
        }

        private void _AddUnset(in Coordinate c, int boxIdx, IList<Coordinate> unsetCoords)
        {
            for (int col = 0; col < _puzzle.Size; col++)
            {
                if (col != c.Column && !_puzzle[c.Row, col].HasValue)
                {
                    unsetCoords.Add(new Coordinate(c.Row, col));
                }
            }
            for (int row = 0; row < _puzzle.Size; row++)
            {
                if (row != c.Row && !_puzzle[row, c.Column].HasValue)
                {
                    unsetCoords.Add(new Coordinate(row, c.Column));
                }
            }
            foreach (var inBoxCoord in _puzzle.YieldUnsetCoordsForBox(boxIdx))
            {
                if (inBoxCoord.Row == c.Row || inBoxCoord.Column == c.Column)
                {
                    continue;
                }
                unsetCoords.Add(inBoxCoord);
            }
        }
    }
}
