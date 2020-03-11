using System.Collections.Generic;

namespace SudokuSpice
{
    /// <summary>
    /// Enforces a restriction for a puzzle, such as "all values must be unique within a row."
    /// </summary>
    public interface ISudokuRestrict
    {
        /// <summary>
        /// Gets the possible values for the given coordinate based on this restriction.
        /// </summary>
        /// <returns>The possible values represented as a bit-vector.</returns>
        BitVector GetPossibleValues(in Coordinate c);
        /// <summary>
        /// Undoes an update for the given value at the specified coordinate.
        /// </summary>
        /// <param name="c">The coordinate where a value is being unset.</param>
        /// <param name="val">The value being unset.</param>
        void Revert(in Coordinate c, int val);
        /// <summary>
        /// Undoes an update for the given value at the specified coordinate. Appends affected
        /// coordinates to the given list.
        /// </summary>
        /// <param name="c">The coordinate where a value is being unset.</param>
        /// <param name="val">The value being unset.</param>
        /// <param name="affectedCoords">A list to append coordinates to where the possible values
        ///     should be modified.</param>
        void Revert(in Coordinate c, int val, IList<Coordinate> affectedCoords);
        /// <summary>
        /// Updates restricts based on setting the given coordinate to the given value. Appends
        /// affected coordinates to the given list.
        /// </summary>
        /// <param name="c">The coordinate to update.</param>
        /// <param name="val">The value to set <c>c</c> to.</param>
        /// <param name="affectedCoords">A list to append coordinates to where the possible values
        ///     should be modified.</param>
        void Update(in Coordinate c, int val, IList<Coordinate> affectedCoords);
        /// <summary>
        /// Creates a deep copy of this ISudokuRestrict, with any internal <c>Puzzle</c> references
        /// updated to the given puzzle.
        /// </summary>
        /// <param name="puzzle">New puzzle reference to use.</param>
        ISudokuRestrict CopyWithNewReference(Puzzle puzzle);
    }
}