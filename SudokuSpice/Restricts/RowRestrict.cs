using SudokuSpice.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice
{
    /// <summary>
    /// Restricts that each row contains all unique values.
    /// </summary>
    public class RowRestrict : ISudokuRestrict, IRowRestrict
    {
        private readonly Puzzle _puzzle;
        private readonly BitVector[] _unsetRowValues;

        public RowRestrict(Puzzle puzzle)
        {
            _puzzle = puzzle;
            _unsetRowValues = new BitVector[puzzle.Size];
            BitVector allPossible = BitVector.CreateWithSize(puzzle.Size);
            for (int i = 0; i < puzzle.Size; i++)
            {
                _unsetRowValues[i] = allPossible;
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
                    if (!_unsetRowValues[row].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle has duplicate value in row at ({row}, {col}).");
                    }
                    _unsetRowValues[row].UnsetBit(bit);
                }
            }
        }

        private RowRestrict(RowRestrict existing, Puzzle puzzle)
        {
            _puzzle = puzzle;
            _unsetRowValues = (BitVector[])existing._unsetRowValues.Clone();
        }

        public ISudokuRestrict CopyWithNewReference(Puzzle puzzle)
        {
            return new RowRestrict(this, puzzle);
        }

        public BitVector GetPossibleValues(in Coordinate c) => _unsetRowValues[c.Row];

        public BitVector GetPossibleRowValues(int row) => _unsetRowValues[row];

        public void Revert(in Coordinate c, int val)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot revert a restrict for a set puzzle coordinate");
            _unsetRowValues[c.Row].SetBit(val - 1);
        }

        public void Revert(in Coordinate c, int val, IList<Coordinate> affectedCoords)
        {
            this.Revert(in c, val);
            _AddUnsetFromRow(in c, affectedCoords);
        }

        public void Update(in Coordinate c, int val, IList<Coordinate> affectedCoords)
        {
            Debug.Assert(!_puzzle[in c].HasValue, "Cannot update a restrict for a set puzzle coordinate");
            _unsetRowValues[c.Row].UnsetBit(val - 1);
            _AddUnsetFromRow(in c, affectedCoords);
        }

        private void _AddUnsetFromRow(in Coordinate c, IList<Coordinate> unsetCoords)
        {
            for (int col = 0; col < _puzzle.Size; col++)
            {
                if (col != c.Column && !_puzzle[c.Row, col].HasValue)
                {
                    unsetCoords.Add(new Coordinate(c.Row, col));
                }
            }
        }
    }
}
