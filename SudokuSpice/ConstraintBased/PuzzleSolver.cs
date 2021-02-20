using SudokuSpice.ConstraintBased.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Solves puzzles of the given type using an <see cref="ExactCoverGraph"/>.
    /// </summary>
    /// <remarks>
    /// This class is thread-safe as long as the given constraints' implementations of
    /// <see cref="IConstraint.TryConstrain(IReadOnlyPuzzle, ExactCoverGraph)"/> are also
    /// thread-safe. If that's true, then it's safe to solve multiple puzzles concurrently via
    /// the same solver object.
    /// </remarks>
    /// <typeparam name="TPuzzle">The type of puzzle to solve.</typeparam>
    public class PuzzleSolver<TPuzzle> : IPuzzleSolver<TPuzzle> where TPuzzle : class, IPuzzle<TPuzzle>
    {
        private readonly IReadOnlyList<IConstraint> _constraints;

        /// <summary>
        /// Creates a solver that can solve puzzles using the given
        /// <see cref="IConstraint"/>s. The same solver can be reused for multiple puzzles.
        /// </summary>
        /// <param name="constraints">The constraints to satisfy when solving puzzles.</param>
        public PuzzleSolver(IReadOnlyList<IConstraint> constraints)
        {
            _constraints = constraints;
        }

        /// <inheritdoc/>
        public TPuzzle Solve(TPuzzle puzzle, bool randomizeGuesses = false)
        {
            if (!_AreValuesUnique(puzzle.AllPossibleValuesSpan))
            {
                throw new ArgumentException(
                    $"{nameof(puzzle.AllPossibleValuesSpan)} must all be unique. Received values: {puzzle.AllPossibleValuesSpan.ToString()}.");
            }
            var puzzleCopy = puzzle.DeepCopy();
            var graph = ExactCoverGraph.Create(puzzleCopy);
            foreach (IConstraint? constraint in _constraints)
            {
                if (!constraint.TryConstrain(puzzleCopy, graph))
                {
                    throw new ArgumentException("Puzzle violates this solver's constraints.");
                };
            }
            if (!(randomizeGuesses ?
                _TrySolveRandomly(new Random(), new Guesser<TPuzzle>(puzzleCopy, graph)) :
                _TrySolve(new Guesser<TPuzzle>(puzzleCopy, graph))))
            {
                throw new ArgumentException("Failed to solve the given puzzle.");
            }
            return puzzleCopy;
        }

        /// <inheritdoc/>
        public bool TrySolve(TPuzzle puzzle, bool randomizeGuesses = false)
        {
            if (!_AreValuesUnique(puzzle.AllPossibleValuesSpan))
            {
                return false;
            }
            var graph = ExactCoverGraph.Create(puzzle);
            foreach (IConstraint? constraint in _constraints)
            {
                if (!constraint.TryConstrain(puzzle, graph))
                {
                    return false;
                }
            }
            return randomizeGuesses ?
                _TrySolveRandomly(new Random(), new Guesser<TPuzzle>(puzzle, graph)) :
                _TrySolve(new Guesser<TPuzzle>(puzzle, graph));
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

        private SolveStats _ComputeStatsForAllSolutions(TPuzzle puzzle, bool validateUniquenessOnly, CancellationToken? token)
        {
            if (!_AreValuesUnique(puzzle.AllPossibleValuesSpan))
            {
                return new SolveStats();
            }
            var puzzleCopy = puzzle.DeepCopy();
            var graph = ExactCoverGraph.Create(puzzleCopy);
            foreach (IConstraint? constraint in _constraints)
            {
                if (!constraint.TryConstrain(puzzleCopy, graph))
                {
                    return new SolveStats();
                }
            }
            return _TryAllSolutions(new Guesser<TPuzzle>(puzzleCopy, graph), validateUniquenessOnly, token);
        }

        private static bool _AreValuesUnique(ReadOnlySpan<int> values)
        {
            for (int i = 1; i < values.Length; ++i)
            {
                for (int j = i - 1; j >= 0; --j)
                {
                    if (values[i] == values[j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool _TrySolve(Guesser<TPuzzle> tracker)
        {
            if (tracker.IsSolved)
            {
                return true;
            }
            var guesses = tracker.GetBestGuesses();
            foreach (Guess guess in guesses)
            {
                if (tracker.TrySet(in guess))
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

        private static bool _TrySolveRandomly(Random rand, Guesser<TPuzzle> tracker)
        {
            if (tracker.IsSolved)
            {
                return true;
            }
            var guessList = new LinkedList<Guess>(tracker.GetBestGuesses());
            while (guessList.Count > 0)
            {
                
                var guessNode = guessList.First!;
                for(int randomIndex = rand.Next(0, guessList.Count); randomIndex > 0; --randomIndex)
                {
                    guessNode = guessNode.Next!;
                }
                ref Guess guess = ref guessNode.ValueRef;
                if (tracker.TrySet(in guess))
                {
                    if (_TrySolveRandomly(rand, tracker))
                    {
                        return true;
                    }
                    tracker.UnsetLast();
                }
                guessList.Remove(guessNode);
            }
            return false;
        }

        private static SolveStats _TryAllSolutions(
            Guesser<TPuzzle> tracker, bool validateUniquenessOnly, CancellationToken? cancellationToken)
        {
            if (tracker.IsSolved)
            {
                return new SolveStats() { NumSolutionsFound = 1 };
            }
            cancellationToken?.ThrowIfCancellationRequested();
            var guesses = tracker.GetBestGuesses().ToArray();
            if (guesses.Length == 1)
            {
                if (tracker.TrySet(in guesses[0]))
                {
                    return _TryAllSolutions(tracker, validateUniquenessOnly, cancellationToken);
                }
                return new SolveStats();
            }
            return _TryAllSolutionsWithGuess(guesses, tracker, validateUniquenessOnly, cancellationToken);
        }

        private static SolveStats _TryAllSolutionsWithGuess(
            ReadOnlySpan<Guess> guesses,
            Guesser<TPuzzle> tracker,
            bool validateUniquenessOnly,
            CancellationToken? cancellationToken)
        {
            var solveStats = new SolveStats();
            for (int i = 0; i < guesses.Length - 1; i++)
            {
                cancellationToken?.ThrowIfCancellationRequested();
                Guesser<TPuzzle>? trackerCopy = tracker.CopyForContinuation();
                if (trackerCopy.TrySet(in guesses[i]))
                {
                    SolveStats stats = _TryAllSolutions(trackerCopy, validateUniquenessOnly, cancellationToken);
                    solveStats.NumSolutionsFound += stats.NumSolutionsFound;
                    solveStats.NumSquaresGuessed += stats.NumSquaresGuessed;
                    solveStats.NumTotalGuesses += stats.NumTotalGuesses;
                    if (validateUniquenessOnly && solveStats.NumSolutionsFound > 1)
                    {
                        return solveStats;
                    }
                }
            }
            if (tracker.TrySet(in guesses[^1]))
            {
                SolveStats stats = _TryAllSolutions(tracker, validateUniquenessOnly, cancellationToken);
                solveStats.NumSolutionsFound += stats.NumSolutionsFound;
                solveStats.NumSquaresGuessed += stats.NumSquaresGuessed;
                solveStats.NumTotalGuesses += stats.NumTotalGuesses;
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
            solveStats.NumTotalGuesses += guesses.Length;
            return solveStats;
        }
    }
}