using System.Collections.Generic;

namespace Sudoku
{
    /// <summary>
    /// Performs some logical trickery to reduce the number of possible values for a square. Unlike
    /// an <c>IRestrict</c>, this depends on the current possible values in the puzzle, not just
    /// the currently set values.
    /// </summary>
    interface IHeuristic
    {
        /// <summary>
        /// Updates all the current possible values. This should be called when the underlying
        /// puzzle's possible values have been updated based on new restricts.
        /// </summary>
        void UpdateAll();
        /// <summary>
        /// Updates restricts based on the given list of coordinates that were recently modified.
        /// </summary>
        /// <param name="setCoordinate">A single coordinate for the value that is being set or
        /// unset by the related operations.</param>
        /// <param name="modifiedCoords">A list of coordinates where the possible values were
        /// recently modified.</param>
        void Update(in Coordinate setCoordinate, IList<Coordinate> modifiedCoords);
    }
}
