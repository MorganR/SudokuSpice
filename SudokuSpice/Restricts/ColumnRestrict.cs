﻿using System.Collections.Generic;

namespace SudokuSpice
{
    /// <summary>
    /// Restricts that each column contains all unique values.
    /// </summary>
    public class ColumnRestrict : BasicRestrict
    {
        public ColumnRestrict(Puzzle puzzle) : base(puzzle) { }

        public int GetPossibleColumnValues(int col) => unsetValues[col];

        protected override int GetIndex(in Coordinate c)
        {
            return c.Column;
        }

        protected override void AddUnsetFromIndex(int col, IList<Coordinate> unsetCoords)
        {
            for (int row = 0; row < puzzle.Size; row++)
            {
                if (!puzzle[row, col].HasValue)
                {
                    unsetCoords.Add(new Coordinate(row, col));
                }
            }
        }
    }
}
