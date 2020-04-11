using SudokuSpice.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SudokuSpice
{
    public class PuzzleGenerator<TPuzzle> where TPuzzle : IPuzzle
    {
        private readonly Random _random = new Random();
        private readonly Func<TPuzzle> _puzzleFactory;
        private readonly Func<TPuzzle, SquareTracker> _trackerFactory;

        /// <summary>
        /// Creates a puzzle generator to create puzzles with custom rules and type.
        /// </summary>
        /// <param name="puzzleFactory">
        /// A function that constructs an empty <see cref="IPuzzle"/> of the desired type and shape.
        /// </param>
        /// <param name="trackerFactory">
        /// A function that constructs a <see cref="SquareTracker"/> for the desired puzzle type.
        /// This allows callers to use non-standard rules and heuristics.
        /// </param>
        public PuzzleGenerator(Func<TPuzzle> puzzleFactory, Func<TPuzzle, SquareTracker> trackerFactory)
        {
            _puzzleFactory = puzzleFactory;
            _trackerFactory = trackerFactory;
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
        /// </param>
        /// <param name="timeout">
        /// The maximum timeout during which this function can search for a unique puzzle.
        /// Especially useful when trying to generate puzzles with low
        /// <paramref name="numSquaresToSet"/>.
        /// </param>
        /// <returns>
        /// A puzzle of type <c>TPuzzle</c> with a unique solution and
        /// <paramref name="numSquaresToSet"/> preset squares.
        /// </returns>
        /// <exception cref="TimeoutException">
        /// Thrown if no valid unique puzzle is found within the specified
        /// <paramref name="timeout"/>.
        /// </exception>
        public TPuzzle Generate(int numSquaresToSet, TimeSpan timeout)
        {
            using var timeoutCancellationSource = new CancellationTokenSource(timeout);
            var puzzleTask = new Task<TPuzzle>(() => _Generate(
                numSquaresToSet, timeoutCancellationSource.Token), timeoutCancellationSource.Token);
            puzzleTask.RunSynchronously();

            if (puzzleTask.IsCompletedSuccessfully)
            {
                return puzzleTask.Result;
            }
            if (puzzleTask.IsCanceled)
            {
                throw new TimeoutException(
                    $"Failed to generate a puzzle of type {typeof(TPuzzle).Name} and {nameof(numSquaresToSet)} {numSquaresToSet} within {timeout}.");
            }
#pragma warning disable CS8597 // Thrown value may be null.
            throw puzzleTask.Exception;
#pragma warning restore CS8597 // Thrown value may be null.
        }

        private TPuzzle _Generate(int numSquaresToSet, CancellationToken cancellationToken)
        {
            TPuzzle puzzle = _puzzleFactory.Invoke();
            bool foundValidPuzzle = false;
            while (!foundValidPuzzle)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _FillPuzzle(puzzle);
                var setCoordinates = new CoordinateTracker(puzzle.Size);
                _TrackAllCoordinates(setCoordinates, puzzle.Size);
                foundValidPuzzle = _TryUnsetSquaresWhileSolvable(
                    numSquaresToSet, puzzle.NumSetSquares, setCoordinates, puzzle, cancellationToken);
            }
            return puzzle;
        }

        private bool _TryUnsetSquaresWhileSolvable(
            int totalNumSquaresToSet, int currentNumSet, CoordinateTracker setCoordinatesToTry,
            TPuzzle puzzle, CancellationToken cancellationToken)
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

                private void _FillPuzzle(TPuzzle puzzle)
        {
            var tracker = _trackerFactory.Invoke(puzzle);
            var solver = new Solver(tracker);
            solver.SolveRandomly();
        }

        private bool _TryUnsetSquareAt(in Coordinate c, TPuzzle puzzle)
        {
            // Set without checks when there can't be conflicts.
            if (puzzle.NumEmptySquares < 3)
            {
                puzzle[in c] = null;
                return true;
            }
            var previousValue = puzzle[in c];
            puzzle[in c] = null;
            var puzzleCopy = (TPuzzle)puzzle.DeepCopy();
            var tracker = _trackerFactory.Invoke(puzzleCopy);
            var solver = new Solver(tracker);
            var solveStats = solver.GetStatsForAllSolutions();
            if (solveStats.NumSolutionsFound == 1)
            {
                return true;
            }
            puzzle[in c] = previousValue;
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
