namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Provides read-only access to possible value data for a puzzle.
    /// </summary>
    public interface IReadOnlyPossibleValues
    {
        /// <summary>All the possible values any square can be set to.</summary>
        BitVector AllPossibleValues { get; }

        /// <summary>
        /// Gets the current possible values for a given coordinate.
        ///
        /// If the value is already set for the given coordinate, the result is undefined.
        ///</summary>
        BitVector GetPossibleValues(in Coordinate c);
    }
}