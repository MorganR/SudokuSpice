using SudokuSpice.Data;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace SudokuSpice
{
    /// <summary>
    /// Generates standard Sudoku puzzles.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional")]
    public class StandardPuzzleGenerator
    {
        private readonly Random _random = new Random();
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
        public Puzzle Generate(int numSquaresToSet, TimeSpan timeout)
        {
            _ValidateNumSquaresToSet(numSquaresToSet);

            using var timeoutCancellationSource = new CancellationTokenSource(timeout);
            var puzzleTask = new Task<Puzzle>(() => _Generate(numSquaresToSet, timeoutCancellationSource.Token), timeoutCancellationSource.Token);
            puzzleTask.RunSynchronously();

            if (puzzleTask.IsCompletedSuccessfully)
            {
                return puzzleTask.Result;
            }
            if (puzzleTask.IsCanceled)
            {
                throw new TimeoutException($"Failed to generate a puzzle of size {_size} and {nameof(numSquaresToSet)} {numSquaresToSet} within {timeout}.");
            }
#pragma warning disable CS8597 // Thrown value may be null.
            throw puzzleTask.Exception;
#pragma warning restore CS8597 // Thrown value may be null.
        }

        private Puzzle _Generate(int numSquaresToSet, CancellationToken cancellationToken)
        {
            Puzzle? puzzle = null;
            var setCoordinates = new CoordinateTracker(_size);
            while (puzzle is null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                puzzle = new Puzzle(new int?[_size, _size]);
                _FillPuzzle(puzzle);
                _TrackAllCoordinates(setCoordinates);
                if (_TryUnsetSquaresWhileSolvable(numSquaresToSet, _size * _size, setCoordinates, puzzle, cancellationToken))
                {
                    break;
                }
                puzzle = null;
            }
            return puzzle;
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

        private bool _TryUnsetSquaresWhileSolvable(
            int totalNumSquaresToSet, int currentNumSet, CoordinateTracker setCoordinatesToTry,
            Puzzle puzzle, CancellationToken cancellationToken)
        {
            if (currentNumSet == totalNumSquaresToSet)
            {
                return true;
            }
            while (setCoordinatesToTry.NumTracked > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var randomCoord = _GetRandomTrackedCoordinate(setCoordinatesToTry);
                var previousValue = puzzle[in randomCoord];
                setCoordinatesToTry.Untrack(in randomCoord);
                if (!_TryUnsetSquareAt(randomCoord, puzzle))
                {
                    continue;
                }
                if (_TryUnsetSquaresWhileSolvable(
                    totalNumSquaresToSet, currentNumSet - 1,
                    new CoordinateTracker(setCoordinatesToTry), puzzle, cancellationToken))
                {
                    return true;
                }
                // Child update failed :( replace value and try a different coordinate.
                puzzle[in randomCoord] = previousValue;
            }
            return false;
        }

        private Coordinate _GetRandomTrackedCoordinate(CoordinateTracker tracker)
        {
            return tracker.GetTrackedCoords()[_random.Next(0, tracker.NumTracked)];
        }

        private void _TrackAllCoordinates(CoordinateTracker tracker)
        {
            for (int row = 0; row < _size; row++)
            {
                for (int col = 0; col < _size; col++)
                {
                    tracker.AddOrTrackIfUntracked(new Coordinate(row, col));
                }
            }
        }

        private static void _FillPuzzle(Puzzle puzzle)
        {
            var solver = new Solver(puzzle);
            solver.SolveRandomly();
        }

        private static bool _TryUnsetSquareAt(in Coordinate c, Puzzle puzzle)
        {
            // Set without checks when there can't be conflicts.
            if (puzzle.NumEmptySquares < 3)
            {
                puzzle[in c] = null;
                return true;
            }
            var previousValue = puzzle[in c];
            puzzle[in c] = null;
            var solver = new Solver(new Puzzle(puzzle));
            var solveStats = solver.GetStatsForAllSolutions();
            if (solveStats.NumSolutionsFound == 1)
            {
                return true;
            }
            puzzle[in c] = previousValue;
            return false;
        }
    }
}