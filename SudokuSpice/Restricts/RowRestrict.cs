using System.Collections.Generic;

namespace SudokuSpice
{
    /// <summary>
    /// Restricts that each row contains all unique values.
    /// </summary>
    public class RowRestrict : BasicRestrict
    {
        public RowRestrict(Puzzle puzzle) : base(puzzle) { }

        public BitVector GetPossibleRowValues(int row) => unsetValues[row];

        protected override int GetIndex(in Coordinate c)
        {
            return c.Row;
        }

        protected override void AddUnsetFromIndex(int row, IList<Coordinate> unsetCoords)
        {
            for (int col = 0; col < puzzle.Size; col++)
            {
                if (!puzzle[row, col].HasValue)
                {
                    unsetCoords.Add(new Coordinate(row, col));
                }
            }
        }
    }
}
