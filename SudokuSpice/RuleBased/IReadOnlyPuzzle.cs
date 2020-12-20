using System;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Provides read-only access to a puzzle's data.
    /// </summary>
    public interface IReadOnlyPuzzle
    {
        /// <summary>The side-length of the puzzle (puzzles must be square).</summary>
        int Size { get; }
        /// <summary>The total number of squares in the puzzle.</summary>
        int NumSquares { get; }
        /// <summary>The current number of empty/unknown squares in the puzzle.</summary>
        int NumEmptySquares { get; }
        /// <summary>The number of set/known squares in the puzzle.</summary>
        int NumSetSquares { get; }
        /// <summary>All the possible values any square can be set to.</summary>
        BitVector AllPossibleValues { get; }
        /// <summary>
        /// Gets the current possible values for a given coordinate.
        ///
        /// If the value is already set for the given coordinate, the result is undefined.
        ///</summary>
        BitVector GetPossibleValues(in Coordinate c);

        /// <summary>
        /// Gets the current value of a given square.
        /// </summary>
        int? this[int row, int col] { get; }

        /// <summary>
        /// Gets the current value of a given square.
        /// </summary>
        [SuppressMessage(
            "Design",
            "CA1043:Use Integral Or String Argument For Indexers",
            Justification = "This makes sense with Coordinate, which removes any ambiguity between first and second arguments")]
        int? this[in Coordinate c] { get; }

        /// <summary>Gets a span of <c>Coordinate</c>s for all the unset squares.</summary>
        ReadOnlySpan<Coordinate> GetUnsetCoords();
    }
}