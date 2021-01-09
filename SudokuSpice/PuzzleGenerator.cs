using System;
using System.Threading;
using System.Threading.Tasks;

namespace SudokuSpice
{
    /// <summary>
    /// Generates puzzles of the given type based on the provided solver.
    /// </summary>
    /// <typeparam name="TPuzzle">The type of puzzle to generate.</typeparam>
    public class PuzzleGenerator<TPuzzle> where TPuzzle : class, IPuzzle<TPuzzle>
    {
        private readonly Random _random = new Random();
        private readonly Func<int, TPuzzle> _puzzleFactory;
        private readonly IPuzzleSolver<TPuzzle> _solver;

        /// <summary>
        /// Creates a puzzle generator for generating puzzles.
        /// </summary>
        /// <param name="puzzleFromSize">
        /// A function that constructs an empty <see cref="IPuzzle{TPuzzle}"/> of the desired type and shape.
        /// The requested puzzle size (i.e. side-length) is provided as an argument.
        /// </param>
        /// <param name=solver">
        /// A solver to be used to generate puzzles. The solver determines the rules or constraints
        /// a puzzle must satisfy.
        /// </param>
        public PuzzleGenerator(Func<int, TPuzzle> puzzleFromSize, IPuzzleSolver<TPuzzle> solver)
        {
            _puzzleFactory = puzzleFromSize;
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
        /// The size (i.e. side-length) of the puzzle to generate.
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
        /// A puzzle of type <c>TPuzzle</c> with a unique solution and
        /// <paramref name="numSquaresToSet"/> preset squares.
        /// </returns>
        /// <exception cref="TimeoutException">
        /// Thrown if no valid unique puzzle is found within the specified
        /// <paramref name="timeout"/>.
        /// </exception>
        public TPuzzle Generate(int puzzleSize, int numSquaresToSet, TimeSpan timeout)
        {
            if (timeout == TimeSpan.Zero)
            {
                return _Generate(puzzleSize, numSquaresToSet, null);
            }
            using var timeoutCancellationSource = new CancellationTokenSource(timeout);
            var timeoutToken = timeoutCancellationSource.Token;
            var puzzleTask = new Task<TPuzzle>(() => _Generate(puzzleSize, numSquaresToSet, timeoutToken), timeoutToken);
            try
            {
                puzzleTask.RunSynchronously();
            } catch (InvalidOperationException ex)
            {
                if (ex.Message == "RunSynchronously may not be called on a task that has already completed.")
                {
                    throw new TimeoutException(
                        $"Failed to generate a puzzle of size {puzzleSize} with {numSquaresToSet} set squares within {timeout}.");
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
                    $"Failed to generate a puzzle of size {puzzleSize} with {numSquaresToSet} set squares within {timeout}.");
            }
            if (puzzleTask.Exception is null)
            {
                throw new ApplicationException("Something went wrong while trying to generate the puzzle.");
            }
            throw puzzleTask.Exception;
        }

        private TPuzzle _Generate(int puzzleSize, int numSquaresToSet, CancellationToken? cancellationToken)
        {
            bool foundValidPuzzle = false;
            CoordinateTracker setCoordinates;
            while (true)
            {
                cancellationToken?.ThrowIfCancellationRequested();
                TPuzzle puzzle = _puzzleFactory.Invoke(puzzleSize);
                _FillPuzzle(puzzle);
                setCoordinates = new CoordinateTracker(puzzleSize);
                _TrackAllCoordinates(setCoordinates, puzzleSize);
                foundValidPuzzle = _TryUnsetSquaresWhileSolvable(
                        numSquaresToSet, puzzle.NumSquares, setCoordinates, puzzle, cancellationToken);
                if (foundValidPuzzle)
                {
                    return puzzle;
                }
            }
        }

        private bool _TryUnsetSquaresWhileSolvable(
            int totalNumSquaresToSet, int currentNumSet, CoordinateTracker setCoordinatesToTry,
            TPuzzle puzzle, CancellationToken? cancellationToken)
        {
            if (currentNumSet == totalNumSquaresToSet)
            {
                return true;
            }
            while (setCoordinatesToTry.NumTracked > 0)
            {
                cancellationToken?.ThrowIfCancellationRequested();
                Coordinate randomCoord = _GetRandomTrackedCoordinate(setCoordinatesToTry);
                int? previousValue = puzzle[randomCoord.Row, randomCoord.Column];
                setCoordinatesToTry.Untrack(in randomCoord);
                if (!_TryUnsetSquareAt(randomCoord, puzzle.NumSetSquares, puzzle, cancellationToken))
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
                puzzle[randomCoord.Row, randomCoord.Column] = previousValue;
            }
            return false;
        }

        private Coordinate _GetRandomTrackedCoordinate(CoordinateTracker tracker) => tracker.GetTrackedCoords()[_random.Next(0, tracker.NumTracked)];

        private void _FillPuzzle(TPuzzle puzzle) => _solver.TrySolve(puzzle, randomizeGuesses: true);

        private bool _TryUnsetSquareAt(
            in Coordinate c,
            int numEmptySquares,
            TPuzzle puzzle,
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