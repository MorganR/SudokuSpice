using System;
using System.Collections.Generic;

namespace SudokuSpice
{
    /// <summary>
    /// Restricts that each row contains all unique values.
    /// </summary>
    public class RowRestrict : BasicRestrict
    {
        public RowRestrict(Puzzle puzzle) : base(puzzle)
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
                    if (!UnsetValues[row].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle does not satisfy restrict at ({row}, {col}).");
                    }
                    UnsetValues[row].UnsetBit(bit);
                }
            }
        }

        public BitVector GetPossibleRowValues(int row) => UnsetValues[row];

        protected internal override int GetIndex(in Coordinate c)
        {
            return c.Row;
        }

        protected internal override void AddUnsetFromIndex(int row, IList<Coordinate> unsetCoords)
        {
            for (int col = 0; col < Puzzle.Size; col++)
            {
                if (!Puzzle[row, col].HasValue)
                {
                    unsetCoords.Add(new Coordinate(row, col));
                }
            }
        }
    }
}
