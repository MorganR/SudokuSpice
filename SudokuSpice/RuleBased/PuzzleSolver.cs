using SudokuSpice.RuleBased.Heuristics;
using System;
using System.Diagnostics;
using System.Threading;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Solves a single <see cref="IPuzzle"/> using a <see cref="SquareTracker"/>.
    /// </summary>
    public class PuzzleSolver
    {
        private readonly SquareTracker _tracker;

        /// <summary>
        /// Constructs a solver for the given square tracker.
        /// </summary>
        /// <param name="tracker">A square tracker referencing the puzzle to solve.</param>
        public PuzzleSolver(
            IRuleKeeper ruleKeeper,
            IHeuristic? heuristic = null)
        {
            _tracker = new SquareTracker(ruleKeeper, heuristic);
        }

        /// <summary>
        /// Attempts to solve the given puzzle. Unlike <see cref="TrySolveRandomly(IPuzzle)"/>,
        /// when it has to guess, this will try values in guaranteed order.
        /// </summary>
        /// <param name="puzzle">The puzzle to solve. This will be solved in place.</param>
        /// <return>
        /// True if solved, or false if it couldn't be solved within this solver's rules.
        /// </return>
        public bool TrySolve(IPuzzle puzzle)
        {
            return _tracker.TryInit(puzzle) && _TrySolve();
        }

        /// <summary>
        /// Attempts to solve the given puzzle. Unlike <see cref="TrySolve(IPuzzle)"/>, this will
        /// try values in a random order when it has to guess the value for a square.
        /// </summary>
        /// <param name="puzzle">The puzzle to solve. This will be solved in place.</param>
        /// <return>
        /// True if solved, or false if it couldn't be solved within this solver's rules.
        /// </return>
        public bool TrySolveRandomly(IPuzzle puzzle)
        {
            var random = new Random();
            return _tracker.TryInit(puzzle) && _TrySolveRandomly(random);
        }

        /// <summary>
        /// Solves the given puzzle in place. Unlike <see cref="SolveRandomly(IPuzzle)"/>,
        /// when it has to guess, this will try values in the order they are given.
        /// </summary>
        /// <param name="puzzle">The puzzle to solve. This will be copied instead of solved in-place.</param>
        /// <returns>A solved copy of the given puzzle.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if this puzzle can't be solved within the bounds of this solver's rules.
        /// </exception>
        public TPuzzle Solve<TPuzzle>(TPuzzle puzzle) where TPuzzle : IPuzzle
        {
            var copy = (TPuzzle)puzzle.DeepCopy();
            if (!TrySolve(copy))
            {
                throw new ArgumentException("Failed to solve the given puzzle.");
            }
            return copy;
        }

        /// <summary>
        /// Solves the given puzzle in place. Unlike <see cref="Solve(IPuzzle)"/>, this will
        /// try values in a random order when it has to guess the value for a square.
        /// </summary>
        /// <param name="puzzle">The puzzle to solve. This will be copied instead of solved in-place.</param>
        /// <returns>A solved copy of the given puzzle.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if this puzzle can't be solved within the bounds of this solver's rules.
        /// </exception>
        public TPuzzle SolveRandomly<TPuzzle>(TPuzzle puzzle) where TPuzzle : IPuzzle
        {
            var copy = (TPuzzle)puzzle.DeepCopy();
            if (!TrySolveRandomly(copy))
            {
                throw new ArgumentException("Failed to solve the given puzzle.");
            }
            return copy;
        }

        /// <summary>
        /// Finds stats for all the solutions to the given puzzle. The puzzle is left unchanged.
        /// </summary>
        /// <exception cref="OperationCanceledException">
        /// May be thrown if the given cancellation token is canceled during the operation.
        /// </exception>
        public SolveStats GetStatsForAllSolutions(IPuzzle puzzle, CancellationToken? token = null)
        {
            return _GetStatsForAllSolutions(puzzle, validateUniquenessOnly: false, token);
        }

        /// <summary>
        /// Determines if the given puzzle has a unique solution. The puzzle is left unchanged.
        /// </summary>
        /// <exception cref="OperationCanceledException">
        /// May be thrown if the given cancellation token is canceled during the operation.
        /// </exception>
        public bool HasUniqueSolution(IPuzzle puzzle, CancellationToken? token = null)
        {
            return _GetStatsForAllSolutions(puzzle, validateUniquenessOnly: true, token)
                .NumSolutionsFound == 1;
        }

        private SolveStats _GetStatsForAllSolutions(
            IPuzzle puzzle, bool validateUniquenessOnly, CancellationToken? cancellationToken)
        {
            cancellationToken?.ThrowIfCancellationRequested();
            // Copy the puzzle so that the given puzzle is not modified.
            if (!_tracker.TryInit(puzzle.DeepCopy()))
            {
                // No solutions.
                return new SolveStats();
            }
            return _TryAllSolutions(_tracker, validateUniquenessOnly, cancellationToken);
        }

        private bool _TrySolve()
        {
            var puzzle = _tracker.Puzzle;
            Debug.Assert(puzzle is not null, "Puzzle is null, cannot solve.");
            if (puzzle!.NumEmptySquares == 0)
            {
                return true;
            }
            Coordinate c = _tracker.GetBestCoordinateToGuess();
            Span<int> possibleValues = stackalloc int[puzzle.Size];
            int numPossible = _tracker.PopulatePossibleValues(in c, possibleValues);
            for (int i = 0; i < numPossible; ++i)
            {
                int possibleValue = possibleValues[i];
                if (_tracker.TrySet(in c, possibleValue))
                {
                    if (_TrySolve())
                    {
                        return true;
                    }
                    _tracker.UnsetLast();
                }
            }
            return false;
        }

        private bool _TrySolveRandomly(Random random)
        {
            Debug.Assert(_tracker.Puzzle is not null, "Puzzle is null, cannot solve.");
            if (_tracker.Puzzle!.NumEmptySquares == 0)
            {
                return true;
            }
            Coordinate c = _tracker.GetBestCoordinateToGuess();
            Span<int> possibleValues = stackalloc int[_tracker.Puzzle.Size];
            int numPossible = _tracker.PopulatePossibleValues(in c, possibleValues);
            while (numPossible > 0)
            {
                int index = random.Next(0, numPossible);
                int possibleValue = possibleValues[index];
                if (_tracker.TrySet(in c, possibleValue))
                {
                    if (_TrySolveRandomly(random))
                    {
                        return true;
                    }
                    _tracker.UnsetLast();
                }
                for (int i = index; i < numPossible - 1; ++i)
                {
                    possibleValues[i] = possibleValues[i + 1];
                }
                --numPossible;
            }
            return false;
        }

        private static SolveStats _TryAllSolutions(
            SquareTracker tracker,
            bool validateUniquenessOnly,
            CancellationToken? cancellationToken)
        {
            cancellationToken?.ThrowIfCancellationRequested();
            var puzzle = tracker.Puzzle;
            Debug.Assert(puzzle is not null, "Puzzle is null, cannot solve.");
            if (puzzle.NumEmptySquares == 0)
            {
                return new SolveStats() {
                    NumSolutionsFound = 1,
                };
            }
            Coordinate c = tracker.GetBestCoordinateToGuess();
            Span<int> possibleValues = stackalloc int[puzzle.Size];
            int numPossible = tracker.PopulatePossibleValues(in c, possibleValues);
            if (numPossible == 1)
            {
                if (tracker.TrySet(in c, possibleValues[0]))
                {
                    return _TryAllSolutions(tracker, validateUniquenessOnly, cancellationToken);
                }
                return new SolveStats();
            }
            return _TryAllSolutionsWithGuess(tracker, c, possibleValues[0..numPossible], validateUniquenessOnly, cancellationToken);
        }

        private static SolveStats _TryAllSolutionsWithGuess(
            SquareTracker tracker,
            Coordinate c,
            ReadOnlySpan<int> valuesToGuess,
            bool validateUniquenessOnly,
            CancellationToken? cancellationToken)
        {
            cancellationToken?.ThrowIfCancellationRequested();
            var solveStats = new SolveStats();
            for (int i = 0; i < valuesToGuess.Length - 1; i++)
            {
                var trackerCopy = new SquareTracker(tracker);
                if (trackerCopy.TrySet(in c, valuesToGuess[i]))
                {
                    SolveStats guessStats = _TryAllSolutions(trackerCopy, validateUniquenessOnly, cancellationToken);
                    solveStats.NumSolutionsFound += guessStats.NumSolutionsFound;
                    solveStats.NumSquaresGuessed += guessStats.NumSquaresGuessed;
                    solveStats.NumTotalGuesses += guessStats.NumTotalGuesses;
                    if (validateUniquenessOnly && solveStats.NumSolutionsFound > 1)
                    {
                        return solveStats;
                    }
                }
            }
            if (tracker.TrySet(in c, valuesToGuess[^1]))
            {
                SolveStats guessStats = _TryAllSolutions(tracker, validateUniquenessOnly, cancellationToken);
                solveStats.NumSolutionsFound += guessStats.NumSolutionsFound;
                solveStats.NumSquaresGuessed += guessStats.NumSquaresGuessed;
                solveStats.NumTotalGuesses += guessStats.NumTotalGuesses;
                if (validateUniquenessOnly && solveStats.NumSolutionsFound > 1)
                {
                    return solveStats;
                }
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