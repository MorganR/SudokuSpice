using SudokuSpice.RuleBased.Heuristics;
using System;
using System.Diagnostics;

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
            ISudokuRuleKeeper ruleKeeper,
            ISudokuHeuristic? heuristic = null)
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
        /// <param name="puzzle">The puzzle to solve. This will be solved in place.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if this puzzle can't be solved within the bounds of this solver's rules.
        /// </exception>
        public void Solve(IPuzzle puzzle)
        {
            if (!TrySolve(puzzle))
            {
                throw new ArgumentException($"Failed to solve the given puzzle.");
            }
        }

        /// <summary>
        /// Solves the given puzzle in place. Unlike <see cref="Solve(IPuzzle)"/>, this will
        /// try values in a random order when it has to guess the value for a square.
        /// </summary>
        /// <param name="puzzle">The puzzle to solve. This will be solved in place.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if this puzzle can't be solved within the bounds of this solver's rules.
        /// </exception>
        public void SolveRandomly(IPuzzle puzzle)
        {
            if (!TrySolveRandomly(puzzle))
            {
                throw new ArgumentException($"Failed to solve the given puzzle.");
            }
        }

        /// <summary>
        /// Finds stats for all the solutions to the given puzzle. The puzzle is left unchanged.
        /// </summary>
        public SolveStats GetStatsForAllSolutions(IPuzzle puzzle)
        {
            if (!_tracker.TryInit(puzzle.DeepCopy()))
            {
                // No solutions.
                return new SolveStats();
            }
            return _TryAllSolutions(_tracker);
        }

        private bool _TrySolve()
        {
            Debug.Assert(_tracker.Puzzle is not null, "Puzzle is null, cannot solve.");
            if (_tracker.Puzzle!.NumEmptySquares == 0)
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

        private static SolveStats _TryAllSolutions(SquareTracker tracker)
        {
            Debug.Assert(tracker.Puzzle is not null, "Puzzle is null, cannot solve.");
            if (tracker.Puzzle!.NumEmptySquares == 0)
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
