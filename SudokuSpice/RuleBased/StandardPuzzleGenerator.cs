using System;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Generates standard Sudoku puzzles.
    /// </summary>
    public class StandardPuzzleGenerator : PuzzleGenerator<Puzzle>
    {
        /// <inheritdoc/>
        public StandardPuzzleGenerator()
            : base(
                size => new Puzzle(size),
                StandardPuzzles.CreateSolver())
        { }

        /// <summary>
        /// Generates a puzzle that has a unique solution with the given number of squares set.
        ///
        /// Be careful calling this with low values, as it can take a very long time to generate
        /// unique puzzles as the value of <paramref name="numSquaresToSet"/> approaches the
        /// minimum number of clues necessary to provide a unique puzzle of the given
        /// <paramref name="puzzleSize"/>.
        /// </summary>
        /// <param name="puzzleSize">
        /// The size (i.e. side-length) of the puzzle to generate.
        /// </param>
        /// <param name="numSquaresToSet">
        /// The number of squares that will be preset in the generated puzzle.
        ///
        /// Valid ranges are 0-1 for puzzles of size 1, 4-16 for puzzles of size 4, 17-81 for
        /// puzzles of size 9, 55-256 for puzzles of size 16, and 185 - 625 for puzzles of size 25.
        /// Note that the lower bounds for puzzles sized 16 or 25 are estimates from
        /// this forum: http://forum.enjoysudoku.com/minimum-givens-on-larger-puzzles-t4801.html
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
        public new Puzzle Generate(int puzzleSize, int numSquaresToSet, TimeSpan timeout)
        {
            _ValidateUniqueSolutionExists(puzzleSize, numSquaresToSet);
            return base.Generate(puzzleSize, numSquaresToSet, timeout);
        }

        private static void _ValidateUniqueSolutionExists(int puzzleSize, int numToSet)
        {
            int boxSize = puzzleSize switch
            {
                1 => 1,
                4 => 2,
                9 => 3,
                16 => 4,
                25 => 5,
                _ => throw new ArgumentException($"{nameof(puzzleSize)} must be one of [1, 4, 9, 16, 25]."),
            };
            // Inclusive bounds
            int lowerBound = 0;
            int upperBound = 1;
            switch (boxSize)
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
                    $"Must be in the range [{lowerBound}, {upperBound}] for puzzles of size {puzzleSize}.");
            }
        }
    }
}