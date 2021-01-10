using SudokuSpice.RuleBased.Heuristics;
using System;
using System.Diagnostics;
using System.Threading;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Solves puzzles of the given type.
    /// 
    /// This solver uses a rule-based approach based on the <see cref="Rules.IRule"/>s provided in
    /// the constructor. An optional heuristic can also be provided.
    /// </summary>
    /// <remarks>This solver is not thread-safe.</remarks>
    /// <typeparam name="TPuzzle">The type of puzzle to solve.</typeparam>
    public class PuzzleSolver<TPuzzle> : IPuzzleSolver<TPuzzle> where TPuzzle : class, IPuzzleWithPossibleValues<TPuzzle>
    {
        private readonly IRuleKeeper _ruleKeeper;
        private readonly IHeuristic? _heuristic;

        /// <summary>
        /// Constructs a solver with the given rules and optional heuristic.
        /// 
        /// This solver can be used to solve multiple puzzles.
        /// </summary>
        /// <param name="ruleKeeper">The rule keeper to satisfy when solving puzzles.</param>
        /// <param name="heuristic">
        /// A heuristic to use to solve this puzzle efficiently. Can be set to null to skip using
        /// heuristics.
        /// Note that only one heuristic can be provided. To use multiple heuristics, create a
        /// wrapper heuristic like <see cref="StandardHeuristic"/>.
        /// </param>
        public PuzzleSolver(
            IRuleKeeper ruleKeeper,
            IHeuristic? heuristic = null)
        {
            _ruleKeeper = ruleKeeper;
            _heuristic = heuristic;
        }

        /// <inheritdoc/>
        public bool TrySolve(TPuzzle puzzle, bool randomizeGuesses = false)
        {
            if (!SquareTracker<TPuzzle>.TryInit(puzzle, _ruleKeeper, _heuristic, out SquareTracker<TPuzzle>? tracker))
            {
                return false;
            }
            return randomizeGuesses ? _TrySolveRandomly(tracker!, new Random()) : _TrySolve(tracker!);
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
            if (!SquareTracker<TPuzzle>.TryInit(puzzle.DeepCopy(), _ruleKeeper, _heuristic, out SquareTracker<TPuzzle>? tracker))
            {
                // No solutions.
                return new SolveStats();
            }
            return _TryAllSolutions(tracker!, validateUniquenessOnly, cancellationToken);
        }

        private bool _TrySolve(SquareTracker<TPuzzle> tracker)
        {
            var puzzle = tracker.Puzzle;
            if (puzzle.NumEmptySquares == 0)
            {
                return true;
            }
            Coordinate c = tracker.GetBestCoordinateToGuess();
            Span<int> possibleValues = stackalloc int[puzzle.Size];
            int numPossible = tracker.PopulatePossibleValues(in c, possibleValues);
            for (int i = 0; i < numPossible; ++i)
            {
                int possibleValue = possibleValues[i];
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

        private bool _TrySolveRandomly(SquareTracker<TPuzzle> tracker, Random random)
        {
            if (tracker.Puzzle.NumEmptySquares == 0)
            {
                return true;
            }
            Coordinate c = tracker.GetBestCoordinateToGuess();
            Span<int> possibleValues = stackalloc int[tracker.Puzzle.Size];
            int numPossible = tracker.PopulatePossibleValues(in c, possibleValues);
            while (numPossible > 0)
            {
                int index = random.Next(0, numPossible);
                int possibleValue = possibleValues[index];
                if (tracker.TrySet(in c, possibleValue))
                {
                    if (_TrySolveRandomly(tracker, random))
                    {
                        return true;
                    }
                    tracker.UnsetLast();
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