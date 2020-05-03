using SudokuSpice.Data;
using SudokuSpice.Heuristics;
using SudokuSpice.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SudokuSpice
{
    /// <summary>
    /// Solves a single <see cref="IPuzzle"/> using a <see cref="SquareTracker"/>.
    /// </summary>
    public class Solver
    {
        private readonly SquareTracker _tracker;

        /// <summary>
        /// Constructs a solver for a standard Sudoku puzzle that uses a standard heuristic and
        /// standard rule keeper. Provided for convenience.
        /// </summary>
        public Solver(Puzzle puzzle)
        {
            _tracker = new SquareTracker(puzzle);
        }

        /// <summary>
        /// Constructs a solver for the given square tracker.
        /// </summary>
        /// <param name="tracker">A square tracker referencing the puzzle to solve.</param>
        public Solver(
            IPuzzle puzzle,
            PossibleValues possibleValues,
            ISudokuRuleKeeper ruleKeeper,
            ISudokuHeuristic? heuristic = null)
        {
            _tracker = new SquareTracker(puzzle, possibleValues, ruleKeeper, heuristic);
        }

        /// <summary>
        /// Solves the internal puzzle reference.
        /// </summary>
        public void Solve()
        {
            if (!_TrySolve())
            {
                throw new ArgumentException($"Failed to solve the given puzzle.");
            }
        }

        /// <summary>
        /// Solves the internal puzzle reference, choosing randomly when it has to guess a square's
        /// value.
        /// </summary>
        public void SolveRandomly()
        {
            var random = new Random();
            if (!_TrySolveRandomly(random))
            {
                throw new ArgumentException($"Failed to solve the given puzzle.");
            }
        }

        /// <summary>
        /// Finds stats for all the solutions to the internal puzzle reference. The internal puzzle
        /// is left unchanged.
        /// </summary>
        public SolveStats GetStatsForAllSolutions()
        {
            return _TryAllSolutions(new SquareTracker(_tracker));
        }

        /// <summary>
        /// Finds stats for all the solutions to the internal puzzle reference. The internal puzzle
        /// is left unchanged. Work may be parallelized onto multiple threads where possible.
        /// </summary>
        public SolveStats GetStatsForAllSolutionsInParallel()
        {
            if (Environment.ProcessorCount == 1)
            {
                return GetStatsForAllSolutions();
            }
            return _TryAllSolutionsParallel(new SquareTracker(_tracker));
        }

        private bool _TrySolve()
        {
            if (_tracker.Puzzle.NumEmptySquares == 0)
            {
                return true;
            }
            var c = _tracker.GetBestCoordinateToGuess();
            foreach (var possibleValue in _tracker.GetPossibleValues(in c))
            {
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
            if (_tracker.Puzzle.NumEmptySquares == 0)
            {
                return true;
            }
            var c = _tracker.GetBestCoordinateToGuess();
            var possibleValues = _tracker.GetPossibleValues(in c);
            while (possibleValues.Count > 0)
            {
                int possibleValue = possibleValues[random.Next(0, possibleValues.Count)];
                if (_tracker.TrySet(in c, possibleValue))
                {
                    if (_TrySolveRandomly(random))
                    {
                        return true;
                    }
                    _tracker.UnsetLast();
                }
                possibleValues.Remove(possibleValue);
            }
            return false;
        }

        private static SolveStats _TryAllSolutions(SquareTracker tracker)
        {
            if (tracker.Puzzle.NumEmptySquares == 0)
            {
                return new SolveStats()
                {
                    NumSolutionsFound = 1,
                };
            }
            var c = tracker.GetBestCoordinateToGuess();
            var possibleValues = tracker.GetPossibleValues(in c);
            if (possibleValues.Count == 1)
            {
                if (tracker.TrySet(in c, possibleValues[0]))
                {
                    return _TryAllSolutions(tracker);
                }
                return new SolveStats();
            }
            return _TryAllSolutionsWithGuess(tracker, c, possibleValues);
        }

        private static SolveStats _TryAllSolutionsWithGuess(
            SquareTracker tracker,
            Coordinate c,
            List<int> valuesToGuess)
        {
            var solveStats = new SolveStats();
            int idx = 0;
            foreach (var possibleValue in valuesToGuess)
            {
                var trackerCopy = new SquareTracker(tracker);
                if (trackerCopy.TrySet(in c, possibleValue))
                {
                    var guessStats = _TryAllSolutions(trackerCopy);
                    solveStats.NumSolutionsFound += guessStats.NumSolutionsFound;
                    solveStats.NumSquaresGuessed += guessStats.NumSquaresGuessed;
                    solveStats.NumTotalGuesses += guessStats.NumTotalGuesses;
                }
                idx++;
            }
            if (solveStats.NumSolutionsFound == 0)
            {
                return new SolveStats();
            }
            solveStats.NumSquaresGuessed++;
            solveStats.NumTotalGuesses += valuesToGuess.Count;
            return solveStats;
        }

        private static SolveStats _TryAllSolutionsParallel(SquareTracker tracker)
        {
            if (tracker.Puzzle.NumEmptySquares == 0)
            {
                return new SolveStats()
                {
                    NumSolutionsFound = 1,
                };
            }
            var c = tracker.GetBestCoordinateToGuess();
            var possibleValues = tracker.GetPossibleValues(in c);
            if (possibleValues.Count == 1)
            {
                if (tracker.TrySet(in c, possibleValues.Single()))
                {
                    return _TryAllSolutionsParallel(tracker);
                }
                return new SolveStats();
            }
            return  _TryAllSolutionsWithGuessParallel(tracker, c, possibleValues);
        }

        private static SolveStats _TryAllSolutionsWithGuessParallel(
            SquareTracker tracker,
            Coordinate c,
            List<int> valuesToGuess)
        {
            var guessingTasks = new Task<SolveStats>[valuesToGuess.Count - 1];
            for (int i = 0; i < guessingTasks.Length; i++)
            {
                var guess = valuesToGuess[i];
                var trackerCopy = new SquareTracker(tracker);
                guessingTasks[i] = Task.Run(() =>
                {
                    if (trackerCopy.TrySet(in c, guess))
                    {
                        return _TryAllSolutionsParallel(trackerCopy);
                    }
                    return new SolveStats();
                });
            }
            // Run the last guess in the current thread to avoid an extra tracker copy.
            SolveStats stats;
            if (tracker.TrySet(in c, valuesToGuess[^1]))
            {
                stats = _TryAllSolutionsParallel(tracker);
            }
            else
            {
                stats = new SolveStats();
            }
            int tasksRemaining = guessingTasks.Length;
            Task.WaitAll(guessingTasks);
            foreach (var guessTask in guessingTasks)
            {
                var guessStats = guessTask.Result;
                stats.NumSolutionsFound += guessStats.NumSolutionsFound;
                stats.NumSquaresGuessed += guessStats.NumSquaresGuessed;
                stats.NumTotalGuesses += guessStats.NumTotalGuesses;
            }
            if (stats.NumSolutionsFound == 0)
            {
                return new SolveStats();
            }
            stats.NumSquaresGuessed++;
            stats.NumTotalGuesses += valuesToGuess.Count;
            return stats;
        }
    }
}
