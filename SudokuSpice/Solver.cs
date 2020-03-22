using SudokuSpice.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SudokuSpice
{
    public class Solver
    {
        private readonly ISquareTracker _tracker;

        public Solver(Puzzle puzzle) : this(new SquareTracker(puzzle)) {}

        public Solver(ISquareTracker tracker)
        {
            _tracker = tracker;
        }

        public void Solve()
        {
            if (!_TrySolve())
            {
                throw new ArgumentException($"Failed to solve the given puzzle.");
            }
        }

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
            if (_tracker.GetNumEmptySquares() == 0)
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
            if (_tracker.GetNumEmptySquares() == 0)
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

        public SolveStats GetStatsForAllSolutions()
        {
            return _TryAllSolutionsAsync(_tracker.DeepCopy()).Result;
        }

        private static Task<SolveStats> _TryAllSolutionsAsync(ISquareTracker tracker)
        {
            if (tracker.GetNumEmptySquares() == 0)
            {
                return Task.FromResult(new SolveStats()
                {
                    NumSolutionsFound = 1,
                });
            }
            var c = tracker.GetBestCoordinateToGuess();
            var possibleValues = tracker.GetPossibleValues(in c);
            int numPossibleValues = possibleValues.Count();
            if (numPossibleValues == 1)
            {
                if (tracker.TrySet(in c, possibleValues.Single()))
                {
                    return _TryAllSolutionsAsync(tracker);
                }
                return Task.FromResult(new SolveStats());
            }
            return _TryAllSolutionsWithGuessAsync(tracker, c, possibleValues, numPossibleValues);
        }

        private static async Task<SolveStats> _TryAllSolutionsWithGuessAsync(
            ISquareTracker tracker,
            Coordinate c,
            IEnumerable<int> valuesToGuess,
            int numValuesToGuess)
        {
            var guessingTasks = new Task<SolveStats>[numValuesToGuess];
            int idx = 0;
            foreach (var possibleValue in valuesToGuess)
            {
                guessingTasks[idx++] = Task.Run(() =>
                {
                    var trackerCopy = tracker.DeepCopy();
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
            aggregatedStats.NumTotalGuesses += numValuesToGuess;
            return aggregatedStats;
        }
    }
}
