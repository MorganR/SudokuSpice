using System.Collections.Generic;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Expands IReadOnlyPuzzle to provide box-related functionality. A 'box' is a square region
    /// within the puzzle. 
    /// </summary>
    public interface IReadOnlyBoxPuzzle : IReadOnlyPuzzle
    {
        /// <summary>
        /// The length of one side of a square region within the puzzle with special rules.
        ///
        /// For example, with a standard Sudoku puzzle, each 3x3 box enforces uniqueness of the
        /// values inside.
        /// </summary>
        public int BoxSize { get; }
        /// <summary>Returns the index of the box that the given coordinates are in.</summary>
        public int GetBoxIndex(int row, int col);

        /// <summary>Returns the top-left coordinate for the given box.</summary>
        public Coordinate GetStartingBoxCoordinate(int box);

        /// <summary>
        /// Yields an enumerable of <see cref="Coordinate"/>s for all the unset squares in the given box.
        /// </summary>
        public IEnumerable<Coordinate> YieldUnsetCoordsForBox(int box);
    }
}