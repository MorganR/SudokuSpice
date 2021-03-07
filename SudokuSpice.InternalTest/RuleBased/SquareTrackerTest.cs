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
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {           3,            2,            4,            1}
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
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */,            3},
                new int?[] {           3,            2,            4,            1}
            });
            var tracker = _CreateStandardTracker(puzzle);

            var coord = new Coordinate(0, 2);
            int val = 4;
            var possibleValuesBuffer = new int[puzzle.AllPossibleValuesSpan.Length];
            int numPossible = tracker.PopulatePossibleValues(in coord, possibleValuesBuffer);
            var expectedPossibleValues = possibleValuesBuffer[0..numPossible];
            Assert.False(tracker.TrySet(in coord, val));
            Assert.False(puzzle[in coord].HasValue);
            numPossible = tracker.PopulatePossibleValues(in coord, possibleValuesBuffer);
            Assert.Equal(expectedPossibleValues, possibleValuesBuffer[0..numPossible]);
        }

        [Fact]
        public void TrySet_WithInvalidValue_ReturnsFalseWithNoChange()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {           3,            2,            4,            1}
            });
            var tracker = _CreateStandardTracker(puzzle);

            var coord = new Coordinate(1, 1);
            int val = 2;
            var possibleValuesBuffer = new int[puzzle.AllPossibleValuesSpan.Length];
            int numPossible = tracker.PopulatePossibleValues(in coord, possibleValuesBuffer);
            var expectedPossibleValues = possibleValuesBuffer[0..numPossible];
            Assert.False(tracker.TrySet(in coord, val));
            Assert.False(puzzle[in coord].HasValue);
            numPossible = tracker.PopulatePossibleValues(in coord, possibleValuesBuffer);
            Assert.Equal(expectedPossibleValues, possibleValuesBuffer[0..numPossible]);
        }

        [Fact]
        public void GetBestCoordinateToGuess_ReturnsCoordinateWithMinimumNumPossibles()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1,            4},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {           3,            2,            4,            1}
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
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1,            4},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {           3,            2,            4,            1}
            });
            var tracker = _CreateStandardTracker(puzzle);
            Assert.True(tracker.TrySet(new Coordinate(0, 1), 4));

            foreach (Coordinate coord in puzzle.GetUnsetCoords())
            {
                var possibleValuesBuffer = new int[puzzle.AllPossibleValuesSpan.Length];
                int numPossible = tracker.PopulatePossibleValues(in coord, possibleValuesBuffer);
                var expectedPossibleValuesBuffer = new int[puzzle.AllPossibleValuesSpan.Length];
                int numExpected = puzzle.GetPossibleValues(in coord).PopulateSetBits(expectedPossibleValuesBuffer);
                Assert.Equal(
                    expectedPossibleValuesBuffer[0..numExpected],
                    possibleValuesBuffer[0..numPossible]);
            }
        }

        [Fact]
        public void UnsetLast_UndoesPreviousUpdate()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1,            4},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {           3,            2,            4,            1}
            });
            var tracker = _CreateStandardTracker(puzzle);
            var expectedPossibles = puzzle.GetUnsetCoords().ToArray().ToDictionary(
                c => c,
                c =>
                {
                    var possibleValuesBuffer = new int[puzzle.AllPossibleValuesSpan.Length];
                    int numPossibleValues = tracker.PopulatePossibleValues(in c, possibleValuesBuffer);
                    return possibleValuesBuffer[0..numPossibleValues];
                });

            var coord = new Coordinate(0, 1);
            Assert.True(tracker.TrySet(in coord, 4));
            tracker.UnsetLast();

            Assert.False(puzzle[in coord].HasValue);
            System.ReadOnlySpan<Coordinate> unsetCoords = puzzle.GetUnsetCoords();
            Assert.Equal(expectedPossibles.Count, unsetCoords.Length);
            foreach (Coordinate c in unsetCoords)
            {
                var possibleValuesBuffer = new int[puzzle.AllPossibleValuesSpan.Length];
                int numPossible = tracker.PopulatePossibleValues(in c, possibleValuesBuffer);
                Assert.Equal(expectedPossibles[c], possibleValuesBuffer[0..numPossible]);
            }
        }

        [Fact]
        public void DeepCopy_Succeeds()
        {
            var puzzle = new PuzzleWithPossibleValues(new int?[][] {
                new int?[] {           1, null /* 4 */, null /* 3 */,            2},
                new int?[] {null /* 2 */, null /* 3 */,            1,            4},
                new int?[] {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                new int?[] {           3,            2,            4,            1}
            });
            var tracker = _CreateStandardTracker(puzzle);
            var expectedPossibles = puzzle.GetUnsetCoords().ToArray().ToDictionary(
                c => c,
                c =>
                {
                    var possibleValuesBuffer = new int[puzzle.AllPossibleValuesSpan.Length];
                    int numPossible = tracker.PopulatePossibleValues(in c, possibleValuesBuffer);
                    return possibleValuesBuffer[0..numPossible];
                });

            var trackerCopy = new SquareTracker<PuzzleWithPossibleValues>(tracker);

            var coord = new Coordinate(0, 1);
            int val = 4;
            tracker.TrySet(coord, val);

            foreach (Coordinate c in expectedPossibles.Keys)
            {
                var possibleValuesBuffer = new int[puzzle.AllPossibleValuesSpan.Length];
                int numPossible = trackerCopy.PopulatePossibleValues(in c, possibleValuesBuffer);
                Assert.Equal(expectedPossibles[c], possibleValuesBuffer[0..numPossible]);
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
                out SquareTracker<PuzzleWithPossibleValues>? tracker));
            return tracker!;
        }
    }
}