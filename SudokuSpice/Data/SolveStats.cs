using System;

namespace SudokuSpice.Data
{
    /// <summary>
    /// Contains statistics about an attempt to solve a puzzle.
    /// </summary>
    public struct SolveStats : IEquatable<SolveStats>
    {
        /// <summary>
        /// The number of solutions found for this puzzle. For any real Sudoku puzzle, this should
        /// be 1.
        /// </summary>
        public int NumSolutionsFound { get; set; }
        /// <summary>
        /// The number of squares for which the solver had to guess while finding a solution.
        /// </summary>
        /// <remarks>
        /// Unlike <c>NumTotalGuesses</c>, if the solver was setting a squar that had three
        /// possible values, this would only add 1 to <c>NumSquaresGuessed</c>. 
        /// If NumSolutionsFound is greater than 1, then the precise meaning of this value is 
        /// undefined.
        /// </remarks>
        public int NumSquaresGuessed { get; set; }
        /// <summary>
        /// The total number of guesses used to solve the puzzle. A 'guess' is any time the
        /// solver tries setting a square to one value when there are multiple possible values
        /// for that square.
        /// </summary>
        /// <remarks>
        /// Example Guesses: if the solver tried to set square (0,1) with possible values [2, 3, 5]
        /// then this would add three guesses to the puzzle solve since there were three possible
        /// values for this coordinate. This would be true even if only one of the values actually
        /// led to a solution.
        /// </remarks>
        public int NumTotalGuesses { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is SolveStats other)
            {
                return Equals(other);
            }
            return false;
        }

        public bool Equals(SolveStats other)
        {
            return NumSolutionsFound == other.NumSolutionsFound
                && NumSquaresGuessed == other.NumSquaresGuessed
                && NumTotalGuesses == other.NumTotalGuesses;
        }

        public override int GetHashCode()
        {
            return NumSolutionsFound ^ NumSquaresGuessed ^ NumTotalGuesses;
        }

        public static bool operator ==(SolveStats left, SolveStats right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SolveStats left, SolveStats right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"NumSolutionsFound: {NumSolutionsFound}, NumSquaresGuessed: {NumSquaresGuessed}, NumTotalGuesses: {NumTotalGuesses}";
        }
    }
}
