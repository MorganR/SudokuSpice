using SudokuSpice.Data;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice
{
    [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional")]
    public class Generator
    {
        private const int MAX_ATTEMPTS = 5;
        private readonly Random _random = new Random();
        private readonly int _size;
        private readonly int _boxSize;

        public Generator(int size)
        {
            _size = size;
            _boxSize = (int)Math.Sqrt(size);
            if (_boxSize * _boxSize != size || _boxSize < 1 || _boxSize > 5)
            {
                throw new ArgumentException($"{nameof(size)} must be a perfect square in the range [1, 25].");
            }
        }

        public Puzzle Generate(int numSquaresToSet)
        {
            _ValidateNumSquaresToSet(numSquaresToSet);
            
            Puzzle? puzzle = null;
            for (int attempts = 0; puzzle is null && attempts < MAX_ATTEMPTS; attempts++)
            {
                puzzle = new Puzzle(new int?[_size, _size]);
                _FillPuzzle(puzzle);
                var setCoordinates = new CoordinateTracker(_size);
                _TrackAllCoordinates(setCoordinates);
                if (_TryUnsetSquaresWhileSolvable(numSquaresToSet, _size * _size, setCoordinates, puzzle))
                {
                    break;
                }
                puzzle = null;
            }
            if (puzzle is null)
            {
                throw new TimeoutException($"Failed to generate a puzzle of size {_size} and {nameof(numSquaresToSet)} {numSquaresToSet} after {MAX_ATTEMPTS} attempts.");
            }
            return puzzle;
        }

        private void _ValidateNumSquaresToSet(int numToSet)
        {
            // Inclusive bounds
            int lowerBound = 0;
            int upperBound = 1;
            switch (_boxSize)
            {
                case 1:
                    break;
                case 2:
                    lowerBound = 4;
                    upperBound = 16;
                    break;
                case 3:
                    lowerBound = 17;
                    upperBound = 81;
                    break;
                case 4:
                    // Lower-bound estimate comes from
                    // http://forum.enjoysudoku.com/minimum-givens-on-larger-puzzles-t4801.html
                    lowerBound = 55;
                    upperBound = 256;
                    break;
                case 5:
                    // Lower-bound estimate comes from
                    // http://forum.enjoysudoku.com/minimum-givens-on-larger-puzzles-t4801.html
                    lowerBound = 185;
                    upperBound = 625;
                    break;
            }
            if (numToSet < lowerBound || numToSet > upperBound)
            {
                throw new ArgumentException(
                    $"{nameof(numToSet)} must be in the range [{lowerBound}, {upperBound}] for puzzles of size {_size}.");
            }
        }

        private bool _TryUnsetSquaresWhileSolvable(int totalNumSquaresToSet, int currentNumSet, CoordinateTracker setCoordinatesToTry, Puzzle puzzle)
        {
            if (currentNumSet == totalNumSquaresToSet)
            {
                return true;
            }
            while (setCoordinatesToTry.NumTracked > 0)
            {
                var randomCoord = _GetRandomTrackedCoordinate(setCoordinatesToTry);
                var previousValue = puzzle[in randomCoord];
                setCoordinatesToTry.Untrack(in randomCoord);
                if (!_TryUnsetSquareAt(randomCoord, puzzle))
                {
                    continue;
                }
                if (_TryUnsetSquaresWhileSolvable(totalNumSquaresToSet, currentNumSet - 1, new CoordinateTracker(setCoordinatesToTry), puzzle))
                {
                    return true;
                }
                // Child update failed :( replace value and try a different coordinate.
                puzzle[in randomCoord] = previousValue;
            }
            return false;
        }

        private Coordinate _GetRandomTrackedCoordinate(CoordinateTracker tracker)
        {
            return tracker.GetTrackedCoords()[_random.Next(0, tracker.NumTracked)];
        }

        private void _TrackAllCoordinates(CoordinateTracker tracker)
        {
            for (int row = 0; row < _size; row++)
            {
                for (int col = 0; col < _size; col++)
                {
                    tracker.Add(new Coordinate(row, col));
                }
            }
        }

        private static void _FillPuzzle(Puzzle puzzle)
        {
            var solver = new Solver(puzzle);
            solver.SolveRandomly();
        }

        private static bool _TryUnsetSquareAt(in Coordinate c, Puzzle puzzle)
        {
            // Set without checks when there can't be conflicts.
            if (puzzle.NumEmptySquares < 3)
            {
                puzzle[in c] = null;
                return true;
            }
            var previousValue = puzzle[in c];
            puzzle[in c] = null;
            var solver = new Solver(new Puzzle(puzzle));
            var solveStats = solver.GetStatsForAllSolutions();
            if (solveStats.NumSolutionsFound == 1)
            {
                return true;
            }
            puzzle[in c] = previousValue;
            return false;
        }
    }
}