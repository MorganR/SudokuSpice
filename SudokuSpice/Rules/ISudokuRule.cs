using SudokuSpice.Data;

namespace SudokuSpice.Rules
{
    /// <summary>
    /// Enforces a rule for a puzzle, such as "all values must be unique within a row."
    /// </summary>
    public interface ISudokuRule
    {
        /// <summary>
        /// Gets the possible values for the given coordinate based on this rule.
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
        /// Undoes an update for the given value at the specified coordinate. Tracks affected
        /// coordinates in the given tracker.
        /// </summary>
        /// <param name="c">The coordinate where a value is being unset.</param>
        /// <param name="val">The value being unset.</param>
        /// <param name="coordTracker">A coordinate tracker to add to where the possible values
        ///     should be modified.</param>
        void Revert(in Coordinate c, int val, CoordinateTracker coordTracker);
        /// <summary>
        /// Updates possible values based on setting the given coordinate to the given value.
        /// Tracks affected coordinates in the given tracker.
        /// </summary>
        /// <param name="c">The coordinate to update.</param>
        /// <param name="val">The value to set <c>c</c> to.</param>
        /// <param name="coordTracker">A coordinate tracker to add to where the possible values
        ///     should be modified.</param>
        void Update(in Coordinate c, int val, CoordinateTracker coordTrackers);
        /// <summary>
        /// Creates a deep copy of this ISudokuRule, with any internal <c>IReadOnlyPuzzle</c>
        /// references updated to the given puzzle.
        /// </summary>
        /// <param name="puzzle">New puzzle reference to use.</param>
        ISudokuRule CopyWithNewReference(IReadOnlyPuzzle puzzle);
    }
}