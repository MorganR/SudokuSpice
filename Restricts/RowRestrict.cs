using System.Collections.Generic;

namespace MorganRoff.Sudoku
{
    /// <summary>
    /// Restricts that each row contains all unique values.
    /// </summary>
    public class RowRestrict : BasicRestrict
    {
        public RowRestrict(Puzzle puzzle) : base(puzzle) { }

        public int GetPossibleRowValues(int row) => unsetValues[row];

        protected override int GetIndex(in Coordinate c)
        {
            return c.Row;
        }

        protected override void AddUnsetFromIndex(int row, IList<Coordinate> unsetCoords)
        {
            for (int col = 0; col < puzzle.Size; col++)
            {
                if (!puzzle.Get(row, col).HasValue)
                {
                    unsetCoords.Add(new Coordinate(row, col));
                }
            }
        }
    }
}
