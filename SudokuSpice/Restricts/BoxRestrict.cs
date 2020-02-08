using System;
using System.Collections.Generic;

namespace SudokuSpice
{
    /// <summary>
    /// Restricts that each 'box' contains all unique values.
    /// </summary>
    public class BoxRestrict : BasicRestrict
    {
        public BoxRestrict(Puzzle puzzle) : base(puzzle)
        {
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
                    if (!UnsetValues[boxIdx].IsBitSet(bit))
                    {
                        throw new ArgumentException($"Puzzle does not satisfy restrict at ({row}, {col}).");
                    }
                    UnsetValues[boxIdx].UnsetBit(bit);
                }
            }
        }

        public BitVector GetPossibleBoxValues(int box) => UnsetValues[box];

        protected internal override int GetIndex(in Coordinate c)
        {
            return Puzzle.GetBoxIndex(c.Row, c.Column);
        }

        protected internal override void AddUnsetFromIndex(int box, IList<Coordinate> unsetCoords)
        {
            foreach (var c in Puzzle.YieldUnsetCoordsForBox(box))
            {
                unsetCoords.Add(c);
            }
        }
    }
}
