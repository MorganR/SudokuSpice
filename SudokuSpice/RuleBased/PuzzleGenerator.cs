using System;
using System.Threading;
using System.Threading.Tasks;

namespace SudokuSpice.RuleBased
{
    public class PuzzleGenerator
    {
        private readonly Random _random = new Random();
        private readonly PuzzleSolver _solver;

        private int _size;
        private int _numSquaresToSet;

        /// <summary>
        /// Creates a puzzle generator to create puzzles with custom rules and type.
        /// </summary>
        /// <param name="solver">
        /// The solver to use to complete puzzles and enforce the appropriate rules.
        /// </param>
        public PuzzleGenerator(PuzzleSolver solver)
        {
            _solver = solver;
        }

        /// <summary>
        /// Generates a puzzle that has a unique solution with the given number of squares set.
        /// </summary>
        /// <remarks>
        /// Be careful calling this with low values, as it can take a very long time to generate
        /// unique puzzles as the value of <paramref name="numSquaresToSet"/> approaches the
        /// minimum number of clues necessary to provide a unique puzzle of the given
        /// <paramref name="puzzleSize"/>.
        /// </remarks>
        /// <param name="puzzleSize">
        /// The size (i.e. side length) of the puzzle to generate.
        /// </param>
        /// <param name="numSquaresToSet">
        /// The number of squares that will be preset in the generated puzzle.
        /// </param>
        /// <param name="timeout">
        /// The maximum timeout during which this function can search for a unique puzzle.
        /// Especially useful when trying to generate puzzles with low
        /// <paramref name="numSquaresToSet"/>. Use <c>TimeSpan.Zero</c> to disable the timeout.
        /// </param>
        /// <returns>
        /// A <paramref name="puzzleSize"/>-x-<paramref name="puzzleSize"/> puzzle with a unique
        /// solution and <paramref name="numSquaresToSet"/> preset squares.
        /// </returns>
        /// <exception cref="TimeoutException">
        /// Thrown if no valid unique puzzle is found within the specified
        /// <paramref name="timeout"/>.
        /// </exception>
        public virtual int?[,] Generate(int puzzleSize, int numSquaresToSet, TimeSpan timeout)
        {
            _size = puzzleSize;
            _numSquaresToSet = numSquaresToSet;
            if (timeout == TimeSpan.Zero)
            {
                return _Generate(null);
            }
            using var timeoutCancellationSource = new CancellationTokenSource(timeout);
            var timeoutToken = timeoutCancellationSource.Token;
            var puzzleTask = new Task<int?[,]>(() => _Generate(timeoutToken), timeoutToken);
            try
            {
                puzzleTask.RunSynchronously();
            } catch (InvalidOperationException ex)
            {
                if (ex.Message == "RunSynchronously may not be called on a task that has already completed.")
                {
                    throw new TimeoutException(
                        $"Failed to generate a puzzle of size {puzzleSize} and {nameof(numSquaresToSet)} {numSquaresToSet} within {timeout}.");
                }
                throw;
            }

            if (puzzleTask.IsCompletedSuccessfully)
            {
                return puzzleTask.Result;
            }
            if (puzzleTask.IsCanceled)
            {
                throw new TimeoutException(
                    $"Failed to generate a puzzle of size {puzzleSize} and {nameof(numSquaresToSet)} {numSquaresToSet} within {timeout}.");
            }
            if (puzzleTask.Exception is null)
            {
                throw new ApplicationException("Something went wrong while trying to generate the puzzle.");
            }
            throw puzzleTask.Exception;
        }

        private int?[,] _Generate(CancellationToken? cancellationToken)
        {
            bool foundValidPuzzle = false;
            int?[,] puzzle;
            CoordinateTracker setCoordinates;
            while (true)
            {
                cancellationToken?.ThrowIfCancellationRequested();
                puzzle = new int?[_size, _size];
                _FillPuzzle(puzzle);
                setCoordinates = new CoordinateTracker(_size);
                _TrackAllCoordinates(setCoordinates, _size);
                foundValidPuzzle = _TryUnsetSquaresWhileSolvable(
                        _size * _size, setCoordinates, puzzle, cancellationToken);
                if (foundValidPuzzle)
                {
                    return puzzle;
                }
            }
        }

        private bool _TryUnsetSquaresWhileSolvable(
            int currentNumSet, CoordinateTracker setCoordinatesToTry,
            int?[,] puzzle, CancellationToken? cancellationToken)
        {
            if (currentNumSet == _numSquaresToSet)
            {
                return true;
            }
            while (setCoordinatesToTry.NumTracked > 0)
            {
                cancellationToken?.ThrowIfCancellationRequested();
                Coordinate randomCoord = _GetRandomTrackedCoordinate(setCoordinatesToTry);
                int? previousValue = puzzle[randomCoord.Row, randomCoord.Column];
                setCoordinatesToTry.Untrack(in randomCoord);
                if (!_TryUnsetSquareAt(randomCoord, _size * _size - currentNumSet, puzzle, cancellationToken))
                {
                    continue;
                }
                if (_TryUnsetSquaresWhileSolvable(
                    currentNumSet - 1,
                    new CoordinateTracker(setCoordinatesToTry), puzzle, cancellationToken))
                {
                    return true;
                }
                // Child update failed :( replace value and try a different coordinate.
                puzzle[randomCoord.Row, randomCoord.Column] = previousValue;
            }
            return false;
        }

        private Coordinate _GetRandomTrackedCoordinate(CoordinateTracker tracker) => tracker.GetTrackedCoords()[_random.Next(0, tracker.NumTracked)];

        private void _FillPuzzle(int?[,] puzzle) => _solver.SolveRandomly(puzzle);

        private bool _TryUnsetSquareAt(
            in Coordinate c,
            int numEmptySquares,
            int?[,] puzzle,
            CancellationToken? cancellationToken)
        {
            // Set without checks when there can't be conflicts.
            if (numEmptySquares < 3)
            {
                puzzle[c.Row, c.Column] = null;
                return true;
            }
            int? previousValue = puzzle[c.Row, c.Column];
            puzzle[c.Row, c.Column] = null;
            if (_solver.HasUniqueSolution(puzzle, cancellationToken))
            {
                return true;
            }
            puzzle[c.Row, c.Column] = previousValue;
            return false;
        }

        private static void _TrackAllCoordinates(CoordinateTracker tracker, int puzzleSize)
        {
            for (int row = 0; row < puzzleSize; row++)
            {
                for (int col = 0; col < puzzleSize; col++)
                {
                    tracker.AddOrTrackIfUntracked(new Coordinate(row, col));
                }
            }
        }
    }
}