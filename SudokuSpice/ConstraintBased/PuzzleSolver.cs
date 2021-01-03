using SudokuSpice.ConstraintBased.Constraints;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Solves <see cref="IPuzzle"/>s using an <see cref="ExactCoverMatrix"/>.
    /// </summary>
    /// <remarks>
    /// This class is thread-safe.
    /// </remarks>
    public class PuzzleSolver<TPuzzle> where TPuzzle : IPuzzle
    {
        private readonly IReadOnlyList<IConstraint> _constraints;

        /// <summary>
        /// Creates a solver that can solve <see cref="IPuzzle"/>s using the given
        /// <see cref="IConstraint"/>s. The same solver can be reused for multiple puzzles.
        /// </summary>
        /// <param name="constraints">The constraints to satisfy when solving puzzles.</param>
        public PuzzleSolver(IReadOnlyList<IConstraint> constraints)
        {
            _constraints = constraints;
        }

        /// <summary>
        /// Solves the given <paramref name="puzzle"/>. This modifies the puzzle's data.
        /// </summary>
        /// <remarks>
        /// It is safe to call this method from different threads on the same solver object,
        /// although note that the given <paramref name="puzzle"/> is modified.
        /// </remarks>
        /// <param name="puzzle">The puzzle to solve.</param>
        /// <param name="randomizeGuesses">
        /// If true, will guess in a random order when forced to guess. Else, this tries to guess
        /// in a clever order, which may have better performance.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if the puzzle cannot be solved with this solver's constraints, or if the
        /// possible values are not unique.
        /// </exception>
        public void Solve(TPuzzle puzzle, bool randomizeGuesses = false)
        {
            if (!_AreValuesUnique(puzzle.AllPossibleValuesSpan))
            {
                throw new ArgumentException(
                    $"{nameof(puzzle.AllPossibleValuesSpan)} must all be unique. Received values: {puzzle.AllPossibleValuesSpan.ToString()}.");
            }
            var matrix = new ExactCoverMatrix(puzzle);
            foreach (IConstraint? constraint in _constraints)
            {
                if (!constraint.TryConstrain(puzzle, matrix))
                {
                    throw new ArgumentException("Puzzle violates this solver's constraints.");
                };
            }
            if (!(randomizeGuesses ?
                _TrySolveRandomly(new Random(), new SquareTracker<TPuzzle>(puzzle, matrix)) :
                _TrySolve(new SquareTracker<TPuzzle>(puzzle, matrix))))
            {
                throw new ArgumentException("Failed to solve the given puzzle.");
            }
        }

        /// <summary>
        /// Solves the given <paramref name="puzzle"/>. This modifies the puzzle's data.
        /// </summary>
        /// <remarks>
        /// It is safe to call this method from different threads on the same solver object,
        /// although note that the given <paramref name="puzzle"/> is modified.
        /// </remarks>
        /// <param name="puzzle">The puzzle to solve.</param>
        /// <param name="randomizeGuesses">
        /// If true, will guess in a random order when forced to guess. Else, this tries to guess
        /// in a clever order, which may have better performance.
        /// </param>
        /// <returns>True if the puzzle was solved successfully.</returns>
        public bool TrySolve(TPuzzle puzzle, bool randomizeGuesses = false)
        {
            if (!_AreValuesUnique(puzzle.AllPossibleValuesSpan))
            {
                return false;
            }
            var matrix = new ExactCoverMatrix(puzzle);
            foreach (IConstraint? constraint in _constraints)
            {
                if (!constraint.TryConstrain(puzzle, matrix))
                {
                    return false;
                }
            }
            return randomizeGuesses ?
                _TrySolveRandomly(new Random(), new SquareTracker<TPuzzle>(puzzle, matrix)) :
                _TrySolve(new SquareTracker<TPuzzle>(puzzle, matrix));
        }

        public SolveStats GetStatsForAllSolutions(TPuzzle puzzle, CancellationToken? token = null)
        {
            if (!_AreValuesUnique(puzzle.AllPossibleValuesSpan))
            {
                return new SolveStats();
            }
            var puzzleCopy = (TPuzzle)puzzle.DeepCopy();
            var matrix = new ExactCoverMatrix(puzzleCopy);
            foreach (IConstraint? constraint in _constraints)
            {
                if (!constraint.TryConstrain(puzzleCopy, matrix))
                {
                    return new SolveStats();
                }
            }
            return _TryAllSolutions(new SquareTracker<TPuzzle>(puzzleCopy, matrix), token);
        }

        private static bool _AreValuesUnique(ReadOnlySpan<int> values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (values[j] == values[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool _TrySolve(SquareTracker<TPuzzle> tracker)
        {
            if (tracker.IsSolved)
            {
                return true;
            }
            (Coordinate c, int[]? possibleValues) = tracker.GetBestGuess();
            foreach (int possibleValue in possibleValues)
            {
                if (tracker.TrySet(in c, possibleValue))
                {
                    if (_TrySolve(tracker))
                    {
                        return true;
                    }
                    tracker.UnsetLast();
                }
            }
            return false;
        }

        private static bool _TrySolveRandomly(Random rand, SquareTracker<TPuzzle> tracker)
        {
            if (tracker.IsSolved)
            {
                return true;
            }
            (Coordinate c, int[]? possibleValues) = tracker.GetBestGuess();
            var possibleValuesList = new List<int>(possibleValues);
            while (possibleValuesList.Count > 0)
            {
                int possibleValue = possibleValuesList[rand.Next(0, possibleValuesList.Count)];
                if (tracker.TrySet(in c, possibleValue))
                {
                    if (_TrySolveRandomly(rand, tracker))
                    {
                        return true;
                    }
                    tracker.UnsetLast();
                }
                _ = possibleValuesList.Remove(possibleValue);
            }
            return false;
        }

        private static SolveStats _TryAllSolutions(SquareTracker<TPuzzle> tracker, CancellationToken? cancellationToken)
        {
            if (tracker.IsSolved)
            {
                return new SolveStats() { NumSolutionsFound = 1 };
            }
            cancellationToken?.ThrowIfCancellationRequested();
            (Coordinate c, int[]? possibleValues) = tracker.GetBestGuess();
            if (possibleValues.Length == 1)
            {
                if (tracker.TrySet(in c, possibleValues[0]))
                {
                    return _TryAllSolutions(tracker, cancellationToken);
                }
                return new SolveStats();
            }
            return _TryAllSolutionsWithGuess(in c, possibleValues, tracker, cancellationToken);
        }

        private static SolveStats _TryAllSolutionsWithGuess(
            in Coordinate guessCoordinate,
            ReadOnlySpan<int> valuesToGuess,
            SquareTracker<TPuzzle> tracker,
            CancellationToken? cancellationToken)
        {
            var solveStats = new SolveStats();
            for (int i = 0; i < valuesToGuess.Length - 1; i++)
            {
                cancellationToken?.ThrowIfCancellationRequested();
                SquareTracker<TPuzzle>? trackerCopy = tracker.CopyForContinuation();
                if (trackerCopy.TrySet(in guessCoordinate, valuesToGuess[i]))
                {
                    SolveStats stats = _TryAllSolutions(trackerCopy, cancellationToken);
                    solveStats.NumSolutionsFound += stats.NumSolutionsFound;
                    solveStats.NumSquaresGuessed += stats.NumSquaresGuessed;
                    solveStats.NumTotalGuesses += stats.NumTotalGuesses;
                }
            }
            if (tracker.TrySet(in guessCoordinate, valuesToGuess[^1]))
            {
                SolveStats stats = _TryAllSolutions(tracker, cancellationToken);
                solveStats.NumSolutionsFound += stats.NumSolutionsFound;
                solveStats.NumSquaresGuessed += stats.NumSquaresGuessed;
                solveStats.NumTotalGuesses += stats.NumTotalGuesses;
            }
            if (solveStats.NumSolutionsFound == 0)
            {
                return new SolveStats();
            }
            solveStats.NumSquaresGuessed++;
            solveStats.NumTotalGuesses += valuesToGuess.Length;
            return solveStats;
        }
    }
}