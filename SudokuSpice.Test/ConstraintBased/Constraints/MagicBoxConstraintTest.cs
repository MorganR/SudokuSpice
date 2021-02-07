using SudokuSpice.Test;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    public class MagicBoxConstraintTest
    {
        [Fact]
        public void Solve_WithManySolutions_CanSolve()
        {
            var puzzle = new Puzzle(new int?[,] {
                { null, null,    9, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null,    3,    5, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
            });
            var boxesToConstrain = new Box[] { 
                new Box(new Coordinate(0, 0), 3),
                new Box(new Coordinate(3, 3), 3),
                new Box(new Coordinate(6, 6), 3),
            };
            var constraint = new MagicBoxConstraint(
                _CreateStandardPossibleValues(9),
                boxesToConstrain, includeDiagonals: false);
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { 
                    new RowUniquenessConstraint(),
                    constraint,
                });

            var solution = solver.Solve(puzzle);

            Assert.Equal(0, solution.NumEmptySquares);
            _AssertMagicSquaresSatisfied(solution, boxesToConstrain, expectedSum: 15, verifyDiagonals: false);
        }

        [Fact]
        public void SolvesCorrectly()
        {
            var puzzle = new Puzzle(new int?[,] {
                { null, null,    9, null, null, null, null, null, null },
                { null, null, null, null, null, null, null,    1, null },
                { null, null, null, null, null,    5, null, null, null },
                { null, null, null, null, null, null, null, null,    8 },
                { null, null, null, null, null, null, null, null, null },
                {    7, null, null, null, null, null, null, null, null },
                { null, null, null,    3, null, null, null, null, null },
                { null,    4, null, null, null, null, null, null, null },
                { null, null, null, null, null, null,    6, null, null },
            });

            var boxesToConstrain = new Box[] { 
                new Box(new Coordinate(0, 0), 3),
                new Box(new Coordinate(0, 3), 3),
                new Box(new Coordinate(0, 6), 3),
                new Box(new Coordinate(3, 0), 3),
                new Box(new Coordinate(3, 3), 3),
                new Box(new Coordinate(3, 6), 3),
                new Box(new Coordinate(6, 0), 3),
                new Box(new Coordinate(6, 3), 3),
                new Box(new Coordinate(6, 6), 3),
            };
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> {
                    new RowUniquenessConstraint(),
                    new ColumnUniquenessConstraint(),
                    new BoxUniquenessConstraint(),
                    new MagicBoxConstraint(
                        _CreateStandardPossibleValues(puzzle.Size),
                        boxesToConstrain, includeDiagonals: false)
                });

            var solution = solver.Solve(puzzle);
            PuzzleTestUtils.AssertStandardPuzzleSolved(solution);
            _AssertMagicSquaresSatisfied(solution, 15, verifyDiagonals: false);
        }

        [Fact]
        public void SolvesCorrectly_WithDiagonals()
        {
            var puzzle = new Puzzle(new int?[,] {
                { null, null, null, null, null, null, null,    1,    2 },
                { null,    5, null, null,    9, null, null, null, null },
                { null, null, null, null, null,    8, null, null, null },
                { null,    3, null, null, null, null,    5, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null,    9, null },
                { null, null, null, null,    4,    3,    9, null, null },
                {    6, null,    1, null, null, null,    7, null, null },
                {    2, null, null, null, null, null, null, null, null },
            });
            var boxesToConstrain = new Box[] {
                new Box(new Coordinate(3, 3), 3),
            };
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> {
                    new RowUniquenessConstraint(),
                    new ColumnUniquenessConstraint(),
                    new BoxUniquenessConstraint(),
                    new MagicBoxConstraint(
                        _CreateStandardPossibleValues(9),
                        boxesToConstrain, includeDiagonals: true)
                });

            var solution = solver.Solve(puzzle);
            PuzzleTestUtils.AssertStandardPuzzleSolved(solution);
            _AssertMagicSquaresSatisfied(solution, boxesToConstrain, 15, verifyDiagonals: true);
        }

        [Fact]
        public void TryConstrain_ConstrainsCorrectly()
        {
            var puzzle = new Puzzle(new int?[,] {
                { null, null,    9, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null,    3,    5, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
            });
            var boxesToConstrain = new Box[] {
                new Box(new Coordinate(0, 0), 3),
            };
            var constraint = new MagicBoxConstraint(
                _CreateStandardPossibleValues(9),
                boxesToConstrain, includeDiagonals: false);
            var matrix = ExactCoverMatrix.Create(puzzle);

            Assert.True(constraint.TryConstrain(puzzle, matrix));

            _AssertPossibleValuesAtSquare(new (0, 0), new int[] { 1, 2, 4, 5 }, matrix);
            _AssertPossibleValuesAtSquare(new (0, 1), new int[] { 4, 5 }, matrix);
            _AssertPossibleValuesAtSquare(new (1, 0), new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, matrix);
            _AssertPossibleValuesAtSquare(new (1, 1), new int[] { 4, 5, 7, 8 }, matrix);
            _AssertPossibleValuesAtSquare(new (1, 2), new int[] { 1 }, matrix);
            _AssertPossibleValuesAtSquare(new (2, 0), new int[] { 7 }, matrix);
            _AssertPossibleValuesAtSquare(new (3, 0), new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, matrix);
        }

        [Fact]
        public void TryConstrain_IncludingDiagonals_ConstrainsCorrectly()
        {
            var puzzle = new Puzzle(new int?[,] {
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null,    6, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null,    3,    8, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
            });
            var boxesToConstrain = new Box[] {
                new Box(new Coordinate(3, 3), 3),
            };
            var constraint = new MagicBoxConstraint(
                _CreateStandardPossibleValues(9),
                boxesToConstrain, includeDiagonals: true);
            var matrix = ExactCoverMatrix.Create(puzzle);

            Assert.True(constraint.TryConstrain(puzzle, matrix));

            _AssertPossibleValuesAtSquare(new (3, 3), new int[] { 1, 2, 4, 5 }, matrix);
            _AssertPossibleValuesAtSquare(new (3, 4), new int[] { 4, 5, 7, 8 }, matrix);
            _AssertPossibleValuesAtSquare(new (4, 3), new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, matrix);
            _AssertPossibleValuesAtSquare(new (4, 4), new int[] { 4, 5 }, matrix);
            _AssertPossibleValuesAtSquare(new (4, 5), new int[] { 1 }, matrix);
            _AssertPossibleValuesAtSquare(new (5, 3), new int[] { 4 }, matrix);
            _AssertPossibleValuesAtSquare(new (6, 3), new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, matrix);
        }

        [Fact]
        public void TryConstrain_NotPossible_Fails()
        {
            var puzzle = new Puzzle(new int?[,] {
                { null, null,    6, null, null, null, null, null, null },
                {    1, null, null, null, null, null, null, null, null },
                { null,    3,    8, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null },
            });
            var boxesToConstrain = new Box[] {
                new Box(new Coordinate(0, 0), 3),
            };
            var constraint = new MagicBoxConstraint(
                _CreateStandardPossibleValues(9),
                boxesToConstrain, includeDiagonals: false);
            var matrix = ExactCoverMatrix.Create(puzzle);

            Assert.False(constraint.TryConstrain(puzzle, matrix));
        }

        private static void _AssertMagicSquaresSatisfied(IReadOnlyPuzzle puzzle, int expectedSum, bool verifyDiagonals)
        {
            int boxSize = Boxes.CalculateBoxSize(puzzle.Size);
            var boxes = new Box[puzzle.Size];
            for (int boxIdx = 0; boxIdx < boxes.Length; ++boxIdx)
            {
                boxes[boxIdx] = new Box(Boxes.GetStartingBoxCoordinate(boxIdx, boxSize), boxSize);
            }
            _AssertMagicSquaresSatisfied(puzzle, boxes, expectedSum, verifyDiagonals);
        }

        private static void _AssertMagicSquaresSatisfied(IReadOnlyPuzzle puzzle, Box[] boxesToCheck, int expectedSum, bool verifyDiagonals)
        {
            foreach (Box box in boxesToCheck)
            {
                int boxSize = box.Size;
                var rowSums = new int[boxSize];
                var colSums = new int[boxSize];
                var startCoord = box.TopLeft;
                var endCoord = new Coordinate(startCoord.Row + boxSize, startCoord.Column + boxSize);
                for (int row = startCoord.Row; row < endCoord.Row; ++row)
                {
                    for (int col = startCoord.Column; col < endCoord.Column; ++col)
                    {
                        var value = puzzle[row, col].Value;
                        rowSums[row - startCoord.Row] += value;
                        colSums[col - startCoord.Column] += value;
                    }
                }
                Assert.All(rowSums, sum => Assert.Equal(expectedSum, sum));
                Assert.All(colSums, sum => Assert.Equal(expectedSum, sum));
                if (verifyDiagonals)
                {
                    int diagSum = 0;
                    for (Coordinate coord = startCoord; coord != endCoord; coord = new Coordinate(coord.Row + 1, coord.Column + 1))
                    {
                        var value = puzzle[in coord].Value;
                        diagSum += value;
                    }
                    Assert.Equal(expectedSum, diagSum);
                    diagSum = 0;
                    var topRightCoord = new Coordinate(startCoord.Row, endCoord.Column - 1);
                    var bottomLeftEndCoord = new Coordinate(endCoord.Row, startCoord.Column - 1);
                    for (Coordinate coord = topRightCoord; coord != bottomLeftEndCoord; coord = new Coordinate(coord.Row + 1, coord.Column - 1))
                    {
                        var value = puzzle[in coord].Value;
                        diagSum += value;
                    }
                    Assert.Equal(expectedSum, diagSum);
                }
            }
        }

        private static void _AssertPossibleValuesAtSquare(Coordinate coord, int[] possibleValues, ExactCoverMatrix matrix)
        {
            var square = matrix.GetAllPossibilitiesAt(coord);
            Assert.NotNull(square);
            var foundValues = new List<int>();
            for (int valueIndex = 0; valueIndex < square.Length; ++valueIndex)
            {
                var possibleValue = square[valueIndex];
                if (possibleValue is null)
                {
                    continue;
                }
                int value = matrix.AllPossibleValues[valueIndex];
                if (possibleValue.State == NodeState.UNKNOWN)
                {
                    foundValues.Add(value);
                    Assert.True(
                        possibleValues.Contains(value),
                        $"Unexpected possible value {value} found at {coord}. Expected: {string.Join(',', possibleValues)}.");
                } else
                {
                    Assert.False(
                        possibleValues.Contains(value), 
                        $"Missing possible value {value} at {coord}. Expected: {string.Join(',', possibleValues)}.");
                }
            }
            Assert.True(
                possibleValues.Length == foundValues.Count,
                $"Found {foundValues.Count} possible values when expected {possibleValues.Length} at {coord}.");
        }

        private static int[] _CreateStandardPossibleValues(int size)
        {
            int[] values = new int[size];
            for (int i = 0; i < size; ++i)
            {
                values[i] = i + 1;
            }
            return values;
        }
    }
}
