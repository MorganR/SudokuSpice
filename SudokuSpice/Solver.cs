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

        /// <summary>
        /// Finds stats for all the solutions to the internal puzzle reference.
        /// </summary>
        public SolveStats GetStatsForAllSolutions()
        {
            return _TryAllSolutionsAsync(new SquareTracker(_tracker)).Result;
        }

        private static Task<SolveStats> _TryAllSolutionsAsync(SquareTracker tracker)
        {
            if (tracker.Puzzle.NumEmptySquares == 0)
            {
                return Task.FromResult(new SolveStats()
                {
                    NumSolutionsFound = 1,
                });
            }
            var c = tracker.GetBestCoordinateToGuess();
            var possibleValues = tracker.GetPossibleValues(in c);
            if (possibleValues.Count == 1)
            {
                if (tracker.TrySet(in c, possibleValues.Single()))
                {
                    return _TryAllSolutionsAsync(tracker);
                }
                return Task.FromResult(new SolveStats());
            }
            return _TryAllSolutionsWithGuessAsync(tracker, c, possibleValues);
        }

        private static async Task<SolveStats> _TryAllSolutionsWithGuessAsync(
            SquareTracker tracker,
            Coordinate c,
            List<int> valuesToGuess)
        {
            var guessingTasks = new Task<SolveStats>[valuesToGuess.Count];
            int idx = 0;
            foreach (var possibleValue in valuesToGuess)
            {
                guessingTasks[idx++] = Task.Run(() =>
                {
                    var trackerCopy = new SquareTracker(tracker);
                    if (trackerCopy.TrySet(in c, possibleValue))
                    {
                        return _TryAllSolutionsAsync(trackerCopy);
                    }
                    return Task.FromResult(new SolveStats());
                });
            }
            
            var allStats = await Task.WhenAll(guessingTasks).ConfigureAwait(false);
            var aggregatedStats = allStats.Where(s => s.NumSolutionsFound > 0).DefaultIfEmpty().Aggregate((agg, stats) =>
            {
                agg.NumSolutionsFound += stats.NumSolutionsFound;
                agg.NumSquaresGuessed += stats.NumSquaresGuessed;
                agg.NumTotalGuesses += stats.NumTotalGuesses;
                return agg;
            });
            aggregatedStats.NumSquaresGuessed++;
            aggregatedStats.NumTotalGuesses += valuesToGuess.Count;
            return aggregatedStats;
        }
    }
}
