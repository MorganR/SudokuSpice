using System;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice
{
    /// <summary>
    /// Generates standard Sudoku puzzles.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional")]
    public class StandardPuzzleGenerator : PuzzleGenerator<Puzzle>
    {
        private readonly int _size;
        private readonly int _boxSize;

        /// <summary>
        /// Creates a puzzle generator to create puzzles of the given side-length.
        /// </summary>
        /// <param name="size">
        /// The side-length for the Sudoku puzzles. This must be a one of: 1, 4, 9, 16, 25.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if <c>size</c> is anything except the values 1, 4, 9, 16, or 25.
        /// </exception>
        public StandardPuzzleGenerator(int size)
            : base(() => new Puzzle(size), puzzle => new SquareTracker(puzzle))
        {
            _size = size;
            _boxSize = (int)Math.Sqrt(size);
            if (_boxSize * _boxSize != size || _boxSize < 1 || _boxSize > 5)
            {
                throw new ArgumentException($"{nameof(size)} must be a perfect square in the range [1, 25].");
            }
        }

        /// <summary>
        /// Generates a puzzle that has a unique solution with the given number of squares set.
        /// </summary>
        /// <remarks>
        /// Be careful calling this with low values, as it can take a very long time to generate
        /// unique puzzles as numSquaresToSet approaches the minimum number of clues necessary to
        /// provide a unique puzzle for this generator's size.
        /// </remarks>
        /// <param name="numSquaresToSet">
        /// The number of squares that will be preset in the generated puzzle.
        /// <para>
        /// Valid ranges are 0-1 for puzzles of size 1, 4-16 for puzzles of size 4, 17-81 for
        /// puzzles of size 9, 55-256 for puzzles of size 16, and 185 - 625 for puzzles of size 25.
        /// Note that the lower bounds for puzzles sized 16 or 25 are estimates from
        /// this forum: http://forum.enjoysudoku.com/minimum-givens-on-larger-puzzles-t4801.html
        /// </para>
        /// </param>
        /// <param name="timeout">
        /// The maximum timeout during which this function can search for a unique puzzle.
        /// Especially useful when trying to generate puzzles with low
        /// <paramref name="numSquaresToSet"/>.
        /// </param>
        /// <returns>
        /// A standard Sudoku puzzle with a unique solution and <c>numSquaresToSet</c> preset
        /// squares.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="numSquaresToSet"/> is impossible for the given puzzle size.
        /// </exception>
        /// <exception cref="TimeoutException">
        /// Thrown if no valid unique puzzle is found within the specified
        /// <paramref name="timeout"/>.
        /// </exception>
        public new Puzzle Generate(int numSquaresToSet, TimeSpan timeout)
        {
            _ValidateNumSquaresToSet(numSquaresToSet);
            return base.Generate(numSquaresToSet, timeout);
        }

        private void _ValidateNumSquaresToSet(int numToSet)
        {
            // Inclusive bounds
            int lowerBound = 0;
            int upperBound = 1;
            switch (_boxSize)
            {
                case 1:
                    break;
                case 2:
                    lowerBound = 4;
                    upperBound = 16;
                    break;
                case 3:
                    lowerBound = 17;
                    upperBound = 81;
                    break;
                case 4:
                    // Lower-bound estimate comes from
                    // http://forum.enjoysudoku.com/minimum-givens-on-larger-puzzles-t4801.html
                    lowerBound = 55;
                    upperBound = 256;
                    break;
                case 5:
                    // Lower-bound estimate comes from
                    // http://forum.enjoysudoku.com/minimum-givens-on-larger-puzzles-t4801.html
                    lowerBound = 185;
                    upperBound = 625;
                    break;
            }
            if (numToSet < lowerBound || numToSet > upperBound)
            {
                throw new ArgumentOutOfRangeException(nameof(numToSet),
                    $"Must be in the range [{lowerBound}, {upperBound}] for puzzles of size {_size}.");
            }
        }
    }
}