using SudokuSpice.RuleBased.Heuristics;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.RuleBased.Test
{
    public class SquareTrackerTest
    {
        [Fact]
        public void TrySet_WithValidValue_SetsAndReturnsTrue()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var tracker = _CreateStandardTracker(puzzle);

            var coord = new Coordinate(1, 1);
            int val = 3;
            Assert.True(tracker.TrySet(in coord, val));
            Assert.Equal(puzzle[in coord], val);
        }

        [Fact]
        public void TrySet_WhichLeadsToZeroPossibleForOtherSquare_ReturnsFalseWithNoChange()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */,            3},
                {           3,            2,            4,            1}
            });
            var tracker = _CreateStandardTracker(puzzle);

            var coord = new Coordinate(0, 2);
            int val = 4;
            List<int> expectedPossibleValues = tracker.GetPossibleValues(in coord);
            Assert.False(tracker.TrySet(in coord, val));
            Assert.False(puzzle[in coord].HasValue);
            Assert.Equal(expectedPossibleValues, tracker.GetPossibleValues(in coord));
        }

        [Fact]
        public void TrySet_WithInvalidValue_ReturnsFalseWithNoChange()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var tracker = _CreateStandardTracker(puzzle);

            var coord = new Coordinate(1, 1);
            int val = 2;
            List<int> expectedPossibleValues = tracker.GetPossibleValues(in coord);
            Assert.False(tracker.TrySet(in coord, val));
            Assert.False(puzzle[in coord].HasValue);
            Assert.Equal(expectedPossibleValues, tracker.GetPossibleValues(in coord));
        }

        [Fact]
        public void GetBestCoordinateToGuess_ReturnsCoordinateWithMinimumNumPossibles()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1,            4},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var tracker = _CreateStandardTracker(puzzle);

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
            var puzzle = new PuzzleWithPossibleValues(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1,            4},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var tracker = _CreateStandardTracker(puzzle);
            Assert.True(tracker.TrySet(new Coordinate(0, 1), 4));

            foreach (Coordinate coord in puzzle.GetUnsetCoords())
            {
                Assert.Equal(puzzle.GetPossibleValues(in coord).GetSetBits(), tracker.GetPossibleValues(in coord));
            }
        }

        [Fact]
        public void UnsetLast_UndoesPreviousUpdate()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1,            4},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var tracker = _CreateStandardTracker(puzzle);
            var expectedPossibles = puzzle.GetUnsetCoords().ToArray().ToDictionary(c => c, c => tracker.GetPossibleValues(in c));

            var coord = new Coordinate(0, 1);
            Assert.True(tracker.TrySet(in coord, 4));
            tracker.UnsetLast();

            Assert.False(puzzle[in coord].HasValue);
            System.ReadOnlySpan<Coordinate> unsetCoords = puzzle.GetUnsetCoords();
            Assert.Equal(expectedPossibles.Count, unsetCoords.Length);
            foreach (Coordinate c in unsetCoords)
            {
                Assert.Equal(expectedPossibles[c], tracker.GetPossibleValues(in c));
            }
        }

        [Fact]
        public void DeepCopy_Succeeds()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1,            4},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var tracker = _CreateStandardTracker(puzzle);
            var expectedPossibles = puzzle.GetUnsetCoords().ToArray().ToDictionary(c => c, c => tracker.GetPossibleValues(in c));

            var trackerCopy = new SquareTracker<PuzzleWithPossibleValues>(tracker);

            var coord = new Coordinate(0, 1);
            int val = 4;
            tracker.TrySet(coord, val);

            foreach (Coordinate c in expectedPossibles.Keys)
            {
                Assert.Equal(expectedPossibles[c], trackerCopy.GetPossibleValues(in c));
            }
        }

        private static SquareTracker<PuzzleWithPossibleValues> _CreateStandardTracker(PuzzleWithPossibleValues puzzle)
        {
            var ruleKeeper = new StandardRuleKeeper();
            Assert.True(SquareTracker<PuzzleWithPossibleValues>.TryInit(
                puzzle,
                ruleKeeper,
                new StandardHeuristic(
                    ruleKeeper,
                    ruleKeeper,
                    ruleKeeper),
                out SquareTracker<PuzzleWithPossibleValues> tracker));
            return tracker;
        }
    }
}