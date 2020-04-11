using SudokuSpice.Data;
using SudokuSpice.Heuristics;
using SudokuSpice.Rules;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.Test
{
    public class SquareTrackerTest
    {
        [Fact]
        public void Constructor_WithPuzzle_Works()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var tracker = new SquareTracker(puzzle);
            Assert.NotNull(tracker);
        }

        [Fact]
        public void Constructor_WithFullInjection_Works()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var possibleValues = new PossibleValues(puzzle);
            var ruleKeeper = new StandardRuleKeeper(puzzle, possibleValues);
            var heuristic = new StandardHeuristic(puzzle, possibleValues, ruleKeeper, ruleKeeper, ruleKeeper);
            var tracker = new SquareTracker(puzzle, possibleValues, ruleKeeper, heuristic);
            Assert.NotNull(tracker);
        }

        [Fact]
        public void TrySet_WithValidValue_SetsAndReturnsTrue()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var tracker = new SquareTracker(puzzle);

            var coord = new Coordinate(1, 1);
            var val = 3;
            Assert.True(tracker.TrySet(in coord, val));
            Assert.Equal(puzzle[in coord], val);
        }

        [Fact]
        public void TrySet_WhichLeadsToZeroPossibleForOtherSquare_ReturnsFalseWithNoChange()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */,            3},
                {           3,            2,            4,            1}
            });
            var tracker = new SquareTracker(puzzle);

            var coord = new Coordinate(0, 2);
            var val = 4;
            var expectedPossibleValues = tracker.GetPossibleValues(in coord);
            Assert.False(tracker.TrySet(in coord, val));
            Assert.False(puzzle[in coord].HasValue);
            Assert.Equal(expectedPossibleValues, tracker.GetPossibleValues(in coord));
        }

        [Fact]
        public void TrySet_WithInvalidValue_ReturnsFalseWithNoChange()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var tracker = new SquareTracker(puzzle);

            var coord = new Coordinate(1, 1);
            var val = 2;
            var expectedPossibleValues = tracker.GetPossibleValues(in coord);
            Assert.False(tracker.TrySet(in coord, val));
            Assert.False(puzzle[in coord].HasValue);
            Assert.Equal(expectedPossibleValues, tracker.GetPossibleValues(in coord));
        }

        [Fact]
        public void GetBestCoordinateToGuess_ReturnsCoordinateWithMinimumNumPossibles()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1,            4},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var tracker = new SquareTracker(puzzle);

            Assert.Contains(
                tracker.GetBestCoordinateToGuess(),
                new HashSet<Coordinate>()
                {
                    new Coordinate(0, 2),
                    new Coordinate(2, 3),
                });
        }

        [Fact]
        public void GetPossibleValues_MatchesPossibleValues()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1,            4},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var possibleValues = new PossibleValues(puzzle);
            var ruleKeeper = new StandardRuleKeeper(puzzle, possibleValues);
            var heuristic = new StandardHeuristic(puzzle, possibleValues, ruleKeeper, ruleKeeper, ruleKeeper);
            var tracker = new SquareTracker(puzzle, possibleValues, ruleKeeper, heuristic);
            tracker.TrySet(new Coordinate(0, 1), 4);

            foreach(var coord in puzzle.GetUnsetCoords())
            {
                Assert.Equal(possibleValues[in coord].GetSetBits(), tracker.GetPossibleValues(in coord));
            }
        }

        [Fact]
        public void UnsetLast_UndoesPreviousUpdate()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1,            4},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var tracker = new SquareTracker(puzzle);
            var expectedPossibles = puzzle.GetUnsetCoords().ToArray().ToDictionary(c => c, c => tracker.GetPossibleValues(in c));

            var coord = new Coordinate(0, 1);
            Assert.True(tracker.TrySet(in coord, 4));
            tracker.UnsetLast();

            Assert.False(puzzle[in coord].HasValue);
            var unsetCoords = puzzle.GetUnsetCoords();
            Assert.Equal(expectedPossibles.Count, unsetCoords.Length);
            foreach (var c in unsetCoords)
            {
                Assert.Equal(expectedPossibles[c], tracker.GetPossibleValues(in c));
            }
        }

        [Fact]
        public void DeepCopy_Succeeds()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1,            4},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var tracker = new SquareTracker(puzzle);
            var expectedPossibles = puzzle.GetUnsetCoords().ToArray().ToDictionary(c => c, c => tracker.GetPossibleValues(in c));

            var trackerCopy = new SquareTracker(tracker);

            var coord = new Coordinate(0, 1);
            var val = 4;
            tracker.TrySet(coord, val);

            foreach (var c in expectedPossibles.Keys)
            {
                Assert.Equal(expectedPossibles[c], trackerCopy.GetPossibleValues(in c));
            }
        }
    }
}
