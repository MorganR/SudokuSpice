using System.Collections.Generic;

namespace SudokuSpice
{
    /// <summary>
    /// Restricts that each 'box' contains all unique values.
    /// </summary>
    public class BoxRestrict : BasicRestrict
    {


        public BoxRestrict(Puzzle puzzle) : base(puzzle) { }

        public BitVector GetPossibleBoxValues(int box) => unsetValues[box];

        protected override int GetIndex(in Coordinate c)
        {
            return puzzle.GetBoxIndex(c.Row, c.Column);
        }

        protected override void AddUnsetFromIndex(int box, IList<Coordinate> unsetCoords)
        {
            foreach (var c in puzzle.YieldUnsetCoordsForBox(box))
            {
                unsetCoords.Add(c);
            }
        }
    }
}
