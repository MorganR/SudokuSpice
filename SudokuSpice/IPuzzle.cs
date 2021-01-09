using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice
{
    /// <summary>
    /// Provides read and write access to a Sudoku puzzle.
    /// </summary>
    public interface IPuzzle<T> : IReadOnlyPuzzle where T : class, IPuzzle<T>
    {
        /// <summary>
        /// Gets or sets the current value of a given square. A square can be 'unset' by setting
        /// its value to <c>null</c>.
        /// </summary>
        public new int? this[int row, int col] { get; set; }

        /// <summary>
        /// Gets or sets the value of the given square, like <see cref="this[int, int]"/>, but
        /// using a <see cref="Coordinate"/> instead of <see langword="int"/> accessors.
        /// </summary>
        /// <param name="c">The location of the square to get/set the value of.</param>
        /// <returns>The value of the square at <paramref name="c"/></returns>
        [SuppressMessage("Design", "CA1043:Use Integral Or String Argument For Indexers", Justification = "This makes sense with Coordinate, which removes any ambiguity between first and second arguments")]
        public new int? this[in Coordinate c] { get; set; }

        /// <summary>
        /// Creates a deep-copy of this puzzle.
        /// </summary>
        public T DeepCopy();
    }
}