using System;
using System.Collections.Generic;

namespace SudokuSpice
{
    /// <summary>
    /// Restricts that each column contains all unique values.
    /// </summary>
    public class ColumnRestrict : BasicRestrict
    {
        public ColumnRestrict(Puzzle puzzle) : base(puzzle)
        {
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
                    if (!UnsetValues[col].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle does not satisfy restrict at ({row}, {col}).");
                    }
                    UnsetValues[col].UnsetBit(bit);
                }
            }
        }

        public BitVector GetPossibleColumnValues(int col) => UnsetValues[col];

        protected internal override int GetIndex(in Coordinate c)
        {
            return c.Column;
        }

        protected internal override void AddUnsetFromIndex(int col, IList<Coordinate> unsetCoords)
        {
            for (int row = 0; row < Puzzle.Size; row++)
            {
                if (!Puzzle[row, col].HasValue)
                {
                    unsetCoords.Add(new Coordinate(row, col));
                }
            }
        }
    }
}
