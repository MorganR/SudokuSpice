using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice
{
    public class SquareTrackerTest
    {
        [Fact]
        public void Constructor_FiltersCorrectly()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */,            1, null /* 2 */, null /* 3 */},
                {           3,            2,            4, null /* 1 */}
            });
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);
            var squareTracker = new SquareTracker(
                puzzle,
                restricts,
                _CreateStandardHeuristics(puzzle, restricts));
            Assert.True(new HashSet<int> { 3, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(0, 1))));
            Assert.True(new HashSet<int> { 3 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(0, 2))));
            Assert.True(new HashSet<int> { 2, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(1, 0))));
            Assert.True(new HashSet<int> { 3, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(1, 1))));
            Assert.True(new HashSet<int> { 3, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(1, 3))));
            Assert.True(new HashSet<int> { 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(2, 0))));
            Assert.True(new HashSet<int> { 2, 3 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(2, 2))));
            Assert.True(new HashSet<int> { 3 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(2, 3))));
            Assert.True(new HashSet<int> { 1 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(3, 3))));
        }

        [Fact]
        public void GetBestCoordinateToGuess_ReturnsCoordinateWithFewestPossibles()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */,            1, null /* 2 */, null /* 3 */},
                {           3,            2,            4, null /* 1 */}
            });
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);
            var squareTracker = new SquareTracker(
                puzzle,
                restricts,
                _CreateStandardHeuristics(puzzle, restricts));
            var best = squareTracker.GetBestCoordinateToGuess();
            Assert.True(best.Equals(new Coordinate(0, 2))
                        || best.Equals(new Coordinate(2, 0))
                        || best.Equals(new Coordinate(2, 3))
                        || best.Equals(new Coordinate(3, 3)));
        }

        [Fact]
        public void GetNumEmptySquares_MatchesInitialNumUnset()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */,            1, null /* 2 */, null /* 3 */},
                {           3,            2,            4, null /* 1 */}
            });
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);
            var squareTracker = new SquareTracker(
                puzzle,
                restricts,
                _CreateStandardHeuristics(puzzle, restricts));
            Assert.Equal(9, squareTracker.GetNumEmptySquares());
        }

        [Fact]
        public void GetNumEmptySquares_IsZeroOnCompletePuzzle()
        {
            var puzzle = new Puzzle(new int?[,] {
                {1, 4, 3, 2},
                {2, 3, 1, 4},
                {4, 1, 2, 3},
                {3, 2, 4, 1}
            });
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);
            var squareTracker = new SquareTracker(
                puzzle,
                restricts,
                _CreateStandardHeuristics(puzzle, restricts));
            Assert.Equal(0, squareTracker.GetNumEmptySquares());
        }

        [Fact]
        public void GetNumEmptySquares_UpdatesOnSet()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */,            1, null /* 2 */, null /* 3 */},
                {           3,            2,            4, null /* 1 */}
            });
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);
            var squareTracker = new SquareTracker(
                puzzle,
                restricts,
                _CreateStandardHeuristics(puzzle, restricts));
            int numEmptyToStart = squareTracker.GetNumEmptySquares();

            squareTracker.TrySet(new Coordinate(0, 1), 4);
            Assert.Equal(numEmptyToStart - 1, squareTracker.GetNumEmptySquares());
        }

        [Fact]
        public void GetNumEmptySquares_UpdatesOnUnset()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */,            1, null /* 2 */, null /* 3 */},
                {           3,            2,            4, null /* 1 */}
            });
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);
            var squareTracker = new SquareTracker(
                puzzle,
                restricts,
                _CreateStandardHeuristics(puzzle, restricts));
            var c = new Coordinate(0, 1);
            squareTracker.TrySet(in c, 4);
            int numEmptyToStart = squareTracker.GetNumEmptySquares();

            squareTracker.Unset(in c);
            Assert.Equal(numEmptyToStart + 1, squareTracker.GetNumEmptySquares());
        }

        [Fact]
        public void TrySet_WithCorrectValue_SetsValueAndClearsOtherPossibles()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */,            1, null /* 2 */, null /* 3 */},
                {           3, null /* 2 */,            4, null /* 1 */}
            });
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);
            var squareTracker = new SquareTracker(
                puzzle,
                restricts,
                _CreateStandardHeuristics(puzzle, restricts));
            var coord = new Coordinate(2, 0);
            var value = 4;

            Assert.True(squareTracker.TrySet(in coord, value));
            Assert.Equal(value, puzzle[coord]);
            int numUnset = 0;
            foreach (var unsetCoord in puzzle.YieldUnsetCoordsForRow(coord.Row))
            {
                Assert.DoesNotContain(value, squareTracker.GetPossibleValues(unsetCoord));
                numUnset++;
            }
            Assert.Equal(2, numUnset);
            numUnset = 0;
            foreach (var unsetCoord in puzzle.YieldUnsetCoordsForColumn(coord.Column))
            {
                Assert.DoesNotContain(value, squareTracker.GetPossibleValues(unsetCoord));
                numUnset++;
            }
            Assert.Equal(1, numUnset);
            numUnset = 0;
            foreach (var unsetCoord in puzzle.YieldUnsetCoordsForBox(puzzle.GetBoxIndex(coord.Row, coord.Column)))
            {
                Assert.DoesNotContain(value, squareTracker.GetPossibleValues(unsetCoord));
                numUnset++;
            }
            Assert.Equal(1, numUnset);
        }

        [Fact]
        public void TrySet_WithIncorrectValueForRow_LeavesPuzzleUnchanged()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */,            1, null /* 2 */, null /* 3 */},
                {           3,            2,            4, null /* 1 */}
            });
            var expectedUnsetCoords = puzzle.GetUnsetCoords().ToArray();
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);
            var squareTracker = new SquareTracker(
                puzzle,
                restricts,
                _CreateStandardHeuristics(puzzle, restricts));
            var coord = new Coordinate(0, 1);
            var wrongValue = 3;

            Assert.False(squareTracker.TrySet(in coord, wrongValue));

            Assert.Null(puzzle[coord]);
            Assert.Equal(
                new HashSet<Coordinate>(expectedUnsetCoords),
                new HashSet<Coordinate>(puzzle.GetUnsetCoords().ToArray()));
            Assert.True(new HashSet<int> { 3, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(0, 1))));
            Assert.True(new HashSet<int> { 3 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(0, 2))));
            Assert.True(new HashSet<int> { 2, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(1, 0))));
            Assert.True(new HashSet<int> { 3, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(1, 1))));
            Assert.True(new HashSet<int> { 3, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(1, 3))));
            Assert.True(new HashSet<int> { 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(2, 0))));
            Assert.True(new HashSet<int> { 2, 3 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(2, 2))));
            Assert.True(new HashSet<int> { 3 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(2, 3))));
            Assert.True(new HashSet<int> { 1 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(3, 3))));
        }

        [Fact]
        public void TrySet_WithIncorrectValueForCol_LeavesPuzzleUnchanged()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */,            1, null /* 2 */, null /* 3 */},
                {           3,            2,            4, null /* 1 */}
            });
            var expectedUnsetCoords = puzzle.GetUnsetCoords().ToArray();
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);
            var squareTracker = new SquareTracker(
                puzzle,
                restricts,
                _CreateStandardHeuristics(puzzle, restricts));
            var coord = new Coordinate(1, 0);
            var wrongValue = 4;

            Assert.False(squareTracker.TrySet(in coord, wrongValue));

            Assert.Null(puzzle[coord]);
            Assert.Equal(
                new HashSet<Coordinate>(expectedUnsetCoords),
                new HashSet<Coordinate>(puzzle.GetUnsetCoords().ToArray()));
            Assert.True(new HashSet<int> { 3, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(0, 1))));
            Assert.True(new HashSet<int> { 3 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(0, 2))));
            Assert.True(new HashSet<int> { 2, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(1, 0))));
            Assert.True(new HashSet<int> { 3, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(1, 1))));
            Assert.True(new HashSet<int> { 3, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(1, 3))));
            Assert.True(new HashSet<int> { 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(2, 0))));
            Assert.True(new HashSet<int> { 2, 3 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(2, 2))));
            Assert.True(new HashSet<int> { 3 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(2, 3))));
            Assert.True(new HashSet<int> { 1 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(3, 3))));
        }

        [Fact]
        public void Unset_ReturnsPuzzleToPreviousState()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */,            1, null /* 2 */, null /* 3 */},
                {           3,            2,            4, null /* 1 */}
            });
            var expectedUnsetCoords = puzzle.GetUnsetCoords().ToArray();
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);
            var squareTracker = new SquareTracker(
                puzzle,
                restricts,
                _CreateStandardHeuristics(puzzle, restricts));
            var coord = new Coordinate(0, 2);
            Assert.True(squareTracker.TrySet(in coord, 3));

            squareTracker.Unset(in coord);

            Assert.Null(puzzle[coord]);
            Assert.Equal(
                new HashSet<Coordinate>(expectedUnsetCoords),
                new HashSet<Coordinate>(puzzle.GetUnsetCoords().ToArray()));
            Assert.True(new HashSet<int> { 3, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(0, 1))));
            Assert.True(new HashSet<int> { 3 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(0, 2))));
            Assert.True(new HashSet<int> { 2, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(1, 0))));
            Assert.True(new HashSet<int> { 3, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(1, 1))));
            Assert.True(new HashSet<int> { 3, 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(1, 3))));
            Assert.True(new HashSet<int> { 4 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(2, 0))));
            Assert.True(new HashSet<int> { 2, 3 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(2, 2))));
            Assert.True(new HashSet<int> { 3 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(2, 3))));
            Assert.True(new HashSet<int> { 1 }.SetEquals(squareTracker.GetPossibleValues(new Coordinate(3, 3))));
        }

        [Fact]
        public void Unset_OnUnsetValue_Throws()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */,            1, null /* 2 */, null /* 3 */},
                {           3,            2,            4, null /* 1 */}
            });
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);
            var squareTracker = new SquareTracker(
                puzzle,
                restricts,
                _CreateStandardHeuristics(puzzle, restricts));
            var coord = new Coordinate(0, 1);

            Assert.Throws<InvalidOperationException>(() => squareTracker.Unset(in coord));
        }

        [Fact]
        public void GetPossibleValues_WithoutIdealCoordinate_UsesHeuristics()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */, null /* 1 */, null /* 4 */},
                {null /* 4 */,            1, null /* 2 */, null /* 3 */},
                {           3, null /* 2 */, null /* 4 */,            1}
            });
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);
            var squareTracker = new SquareTracker(
                puzzle,
                restricts,
                _CreateStandardHeuristics(puzzle, restricts));

            Assert.Equal(new int[] { 3, 4 }, squareTracker.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new int[] { 3, 4 }, squareTracker.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new int[] { 2, 4 }, squareTracker.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new int[] { 2, 3, 4 }, squareTracker.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(new int[] { 1, 3, 4 }, squareTracker.GetPossibleValues(new Coordinate(1, 2)));
            Assert.Equal(new int[] { 3, 4 }, squareTracker.GetPossibleValues(new Coordinate(1, 3)));
            Assert.Equal(new int[] { 2, 4 }, squareTracker.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new int[] { 2, 3, 4 }, squareTracker.GetPossibleValues(new Coordinate(2, 2)));
            Assert.Equal(new int[] { 3, 4 }, squareTracker.GetPossibleValues(new Coordinate(2, 3)));
            Assert.Equal(new int[] { 2, 4 }, squareTracker.GetPossibleValues(new Coordinate(3, 1)));
            Assert.Equal(new int[] { 2, 4 }, squareTracker.GetPossibleValues(new Coordinate(3, 2)));

            var bestCoord = squareTracker.GetBestCoordinateToGuess();

            Assert.Equal(new int[] { 3, 4 }, squareTracker.GetPossibleValues(new Coordinate(0, 1)));
            Assert.Equal(new int[] { 3, 4 }, squareTracker.GetPossibleValues(new Coordinate(0, 2)));
            Assert.Equal(new int[] { 2, 4 }, squareTracker.GetPossibleValues(new Coordinate(1, 0)));
            Assert.Equal(new int[] { 2, 3, 4 }, squareTracker.GetPossibleValues(new Coordinate(1, 1)));
            Assert.Equal(new int[] { 1 }, squareTracker.GetPossibleValues(new Coordinate(1, 2)));
            Assert.Equal(new int[] { 3, 4 }, squareTracker.GetPossibleValues(new Coordinate(1, 3)));
            Assert.Equal(new int[] { 2, 4 }, squareTracker.GetPossibleValues(new Coordinate(2, 0)));
            Assert.Equal(new int[] { 2, 3, 4 }, squareTracker.GetPossibleValues(new Coordinate(2, 2)));
            Assert.Equal(new int[] { 3, 4 }, squareTracker.GetPossibleValues(new Coordinate(2, 3)));
            Assert.Equal(new int[] { 2, 4 }, squareTracker.GetPossibleValues(new Coordinate(3, 1)));
            Assert.Equal(new int[] { 2, 4 }, squareTracker.GetPossibleValues(new Coordinate(3, 2)));
            Assert.Equal(new Coordinate(1, 2), bestCoord);
        }

        private IReadOnlyList<IHeuristic> _CreateStandardHeuristics(
            Puzzle puzzle, IReadOnlyList<IRestrict> standardRestricts)
        {
            return new List<IHeuristic>
                {
                    new UniqueInRowHeuristic(puzzle, (RowRestrict) standardRestricts[0]),
                    new UniqueInColumnHeuristic(puzzle, (ColumnRestrict) standardRestricts[1]),
                    new UniqueInBoxHeuristic(puzzle, (BoxRestrict) standardRestricts[2]),
                };
        }
    }
}
