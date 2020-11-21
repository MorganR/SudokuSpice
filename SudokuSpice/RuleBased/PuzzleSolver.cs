using SudokuSpice.RuleBased.Heuristics;
using System;
using System.Collections.Generic;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Solves a single <see cref="IPuzzle"/> using a <see cref="SquareTracker"/>.
    /// </summary>
    public class PuzzleSolver
    {
        private readonly SquareTracker _tracker;

        /// <summary>
        /// Constructs a solver for a standard Sudoku puzzle that uses a standard heuristic and
        /// standard rule keeper. Provided for convenience.
        /// </summary>
        public PuzzleSolver(Puzzle puzzle)
        {
            _tracker = new SquareTracker(puzzle);
        }

        /// <summary>
        /// Constructs a solver for the given square tracker.
        /// </summary>
        /// <param name="tracker">A square tracker referencing the puzzle to solve.</param>
        public PuzzleSolver(
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
        public SolveStats GetStatsForAllSolutions() => _TryAllSolutions(new SquareTracker(_tracker));

        private bool _TrySolve()
        {
            if (_tracker.Puzzle.NumEmptySquares == 0)
            {
                return true;
            }
            Coordinate c = _tracker.GetBestCoordinateToGuess();
            Span<int> possibleValues = stackalloc int[_tracker.Puzzle.Size];
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
            if (_tracker.Puzzle.NumEmptySquares == 0)
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

        private static SolveStats _TryAllSolutions(SquareTracker tracker)
        {
            if (tracker.Puzzle.NumEmptySquares == 0)
            {
                return new SolveStats() {
                    NumSolutionsFound = 1,
                };
            }
            Coordinate c = tracker.GetBestCoordinateToGuess();
            Span<int> possibleValues = stackalloc int[tracker.Puzzle.Size];
            int numPossible = tracker.PopulatePossibleValues(in c, possibleValues);
            if (numPossible == 1)
            {
                if (tracker.TrySet(in c, possibleValues[0]))
                {
                    return _TryAllSolutions(tracker);
                }
                return new SolveStats();
            }
            return _TryAllSolutionsWithGuess(tracker, c, possibleValues[0..numPossible]);
        }

        private static SolveStats _TryAllSolutionsWithGuess(
            SquareTracker tracker,
            Coordinate c,
            ReadOnlySpan<int> valuesToGuess)
        {
            var solveStats = new SolveStats();
            for (int i = 0; i < valuesToGuess.Length - 1; i++)
            {
                var trackerCopy = new SquareTracker(tracker);
                if (trackerCopy.TrySet(in c, valuesToGuess[i]))
                {
                    SolveStats guessStats = _TryAllSolutions(trackerCopy);
                    solveStats.NumSolutionsFound += guessStats.NumSolutionsFound;
                    solveStats.NumSquaresGuessed += guessStats.NumSquaresGuessed;
                    solveStats.NumTotalGuesses += guessStats.NumTotalGuesses;
                }
            }
            if (tracker.TrySet(in c, valuesToGuess[^1]))
            {
                SolveStats guessStats = _TryAllSolutions(tracker);
                solveStats.NumSolutionsFound += guessStats.NumSolutionsFound;
                solveStats.NumSquaresGuessed += guessStats.NumSquaresGuessed;
                solveStats.NumTotalGuesses += guessStats.NumTotalGuesses;
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
