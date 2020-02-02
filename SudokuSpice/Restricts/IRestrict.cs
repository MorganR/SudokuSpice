using System.Collections.Generic;

namespace SudokuSpice
{
    /// <summary>
    /// Enforces a restriction for a puzzle, such as "all values must be unique within a row."
    /// </summary>
    public interface IRestrict
    {
        /// <summary>
        /// Updates restricts based on setting the given coordinate to the given value. Appends
        /// modified coordinates to the given list.
        /// </summary>
        /// <param name="c">The coordinate to update.</param>
        /// <param name="val">The value to set <c>c</c> to.</param>
        /// <param name="modifiedCoords">A list to append coordinates to where the possible values
        ///     were modified.</param>
        void Update(in Coordinate c, int val, IList<Coordinate> modifiedCoords);
        /// <summary>
        /// Undoes an update for the given value at the specified coordinate. Appends modified
        /// coordinates to the given list.
        /// </summary>
        /// <param name="c">The coordinate where a value is being unset.</param>
        /// <param name="val">The value being unset.</param>
        /// <param name="modifiedCoords">A list to append coordinates to where the possible values
        ///     were modified.</param>
        void Revert(in Coordinate c, int val, IList<Coordinate> modifiedCoords);
        /// <summary>
        /// Gets the possible values for the given coordinate based on this restriction.
        /// </summary>
        /// <returns>The possible values represented as a bit-vector.</returns>
        int GetPossibleValues(in Coordinate c);
    }
}