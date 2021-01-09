using SudokuSpice.RuleBased.Heuristics;
using System;
using System.Diagnostics;
using System.Threading;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Solves a single <see cref="IPuzzle{T}"/> using a <see cref="SquareTracker"/>.
    /// </summary>
    public class PuzzleSolver<TPuzzle> : IPuzzleSolver<TPuzzle> where TPuzzle : class, IPuzzleWithPossibleValues<TPuzzle>
    {
        private readonly SquareTracker<TPuzzle> _tracker;

        /// <summary>
        /// Constructs a solver for the given square tracker.
        /// </summary>
        /// <param name="tracker">A square tracker referencing the puzzle to solve.</param>
        public PuzzleSolver(
            IRuleKeeper ruleKeeper,
            IHeuristic? heuristic = null)
        {
            _tracker = new SquareTracker<TPuzzle>(ruleKeeper, heuristic);
        }

        /// <inheritdoc/>
        public bool TrySolve(TPuzzle puzzle, bool randomizeGuesses = false)
        {
            return _tracker.TryInit(puzzle) &&
                (randomizeGuesses ? _TrySolveRandomly(new Random()) : _TrySolve());
        }

        /// <inheritdoc/>
        public TPuzzle Solve(TPuzzle puzzle, bool randomizeGuesses = false)
        {
            var copy = puzzle.DeepCopy();
            if (!TrySolve(copy, randomizeGuesses))
            {
                throw new ArgumentException("Failed to solve the given puzzle.");
            }
            return copy;
        }

        /// <inheritdoc/>
        public SolveStats ComputeStatsForAllSolutions(TPuzzle puzzle, CancellationToken? token = null)
        {
            return _ComputeStatsForAllSolutions(puzzle, validateUniquenessOnly: false, token);
        }

        /// <inheritdoc/>
        public bool HasUniqueSolution(TPuzzle puzzle, CancellationToken? token = null)
        {
            return _ComputeStatsForAllSolutions(puzzle, validateUniquenessOnly: true, token)
                .NumSolutionsFound == 1;
        }

        private SolveStats _ComputeStatsForAllSolutions(
            TPuzzle puzzle, bool validateUniquenessOnly, CancellationToken? cancellationToken)
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
            SquareTracker<TPuzzle> tracker,
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
            SquareTracker<TPuzzle> tracker,
            Coordinate c,
            ReadOnlySpan<int> valuesToGuess,
            bool validateUniquenessOnly,
            CancellationToken? cancellationToken)
        {
            var solveStats = new SolveStats();
            for (int i = 0; i < valuesToGuess.Length - 1; i++)
            {
                cancellationToken?.ThrowIfCancellationRequested();
                var trackerCopy = new SquareTracker<TPuzzle>(tracker);
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
            cancellationToken?.ThrowIfCancellationRequested();
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