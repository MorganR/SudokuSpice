using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice
{
    /// <summary>
    /// Restricts that each column contains all unique values.
    /// </summary>
    public class ColumnRestrict : ISudokuRestrict, IColumnRestrict
    {
        private readonly Puzzle _puzzle;
        private readonly BitVector[] _unsetColumnValues;

        public ColumnRestrict(Puzzle puzzle)
        {
            _puzzle = puzzle;
            _unsetColumnValues = new BitVector[puzzle.Size];
            for (int i = 0; i < puzzle.Size; i++)
            {
                _unsetColumnValues[i] = BitVector.CreateWithSize(puzzle.Size);
            }
            for (int row = 0; row < puzzle.Size; row++)
            {
                for (int col = 0; col < puzzle.Size; col++)
                {
                    var val = puzzle[row, col];
                    if (!val.HasValue)
                    {
                        continue;
                    }
                    int bit = val.Value - 1;
                    if (!_unsetColumnValues[col].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle does not satisfy restrict at ({row}, {col}).");
                    }
                    _unsetColumnValues[col].UnsetBit(bit);
                }
            }
        }

        public BitVector GetPossibleValues(in Coordinate c) => _unsetColumnValues[c.Column];
        
        public BitVector GetPossibleColumnValues(int col) => _unsetColumnValues[col];

        public void Revert(in Coordinate c, int val, IList<Coordinate> affectedCoords)
        {
            Debug.Assert(_puzzle[in c].HasValue, "Cannot revert a restrict for an unset puzzle coordinate");
            _unsetColumnValues[c.Column].SetBit(val - 1);
            _AddUnsetFromColumn(in c, affectedCoords);
        }

        public void Update(in Coordinate c, int val, IList<Coordinate> affectedCoords)
        {
            Debug.Assert(_puzzle[in c].HasValue, "Cannot update a restrict for an unset puzzle coordinate");
            _unsetColumnValues[c.Column].UnsetBit(val - 1);
            _AddUnsetFromColumn(in c, affectedCoords);
        }

        private void _AddUnsetFromColumn(in Coordinate c, IList<Coordinate> unsetCoords)
        {
            for (int row = 0; row < _puzzle.Size; row++)
            {
                if (row != c.Row && !_puzzle[row, c.Column].HasValue)
                {
                    unsetCoords.Add(new Coordinate(row, c.Column));
                }
            }
        }
    }
}
