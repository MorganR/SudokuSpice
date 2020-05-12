using SudokuSpice.Data;
using SudokuSpice.Heuristics;
using SudokuSpice.Rules;
using System;
using System.Collections.Generic;

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
            for (int i = 0; i < valuesToGuess.Count - 1; i++)
            {
                var trackerCopy = new SquareTracker(tracker);
                if (trackerCopy.TrySet(in c, valuesToGuess[i]))
                {
                    var guessStats = _TryAllSolutions(trackerCopy);
                    solveStats.NumSolutionsFound += guessStats.NumSolutionsFound;
                    solveStats.NumSquaresGuessed += guessStats.NumSquaresGuessed;
                    solveStats.NumTotalGuesses += guessStats.NumTotalGuesses;
                }
            }
            if (tracker.TrySet(in c, valuesToGuess[^1]))
            {
                var guessStats = _TryAllSolutions(tracker);
                solveStats.NumSolutionsFound += guessStats.NumSolutionsFound;
                solveStats.NumSquaresGuessed += guessStats.NumSquaresGuessed;
                solveStats.NumTotalGuesses += guessStats.NumTotalGuesses;
            }
            if (solveStats.NumSolutionsFound == 0)
            {
                return new SolveStats();
            }
            solveStats.NumSquaresGuessed++;
            solveStats.NumTotalGuesses += valuesToGuess.Count;
            return solveStats;
        }
    }
}
