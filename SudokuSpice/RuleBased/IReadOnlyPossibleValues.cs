namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Provides read-only access to possible value data for a puzzle.
    /// </summary>
    public interface IReadOnlyPossibleValues
    {
        /// <summary>All the unique possible values any square can be set to.</summary>
        BitVector UniquePossibleValues { get; }

        /// <summary>
        /// Gets the current possible values for a given coordinate.
        ///
        /// If the value is already set for the given coordinate, the result is undefined.
        ///</summary>
        BitVector GetPossibleValues(in Coordinate c);
    }
}