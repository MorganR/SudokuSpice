using SudokuSpice.Test;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    public class MagicSquaresConstraintTest
    {
        [Fact]
        public void Solve_WithManySolutions_CanSolve()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] { null, null,    9, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null,    3,    5, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
            });
            var boxesToConstrain = new Box[] {
                new Box(new Coordinate(0, 0), 3),
                new Box(new Coordinate(3, 3), 3),
                new Box(new Coordinate(6, 6), 3),
            };
            var constraint = new MagicSquaresConstraint(
                _CreateStandardPossibleValues(9),
                boxesToConstrain, includeDiagonals: false);
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> {
                    new RowUniquenessConstraint(),
                    constraint,
                });

            var solution = solver.Solve(puzzle);

            Assert.Equal(0, solution.NumEmptySquares);
            MagicSquareTests.AssertMagicSquaresSatisfied(
                solution, boxesToConstrain, expectedSum: 15, verifyDiagonals: false);
        }

        [Fact]
        public void SolvesCorrectly()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] { null, null,    9, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null,    1, null },
                new int?[] { null, null, null, null, null,    5, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null,    8 },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] {    7, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null,    3, null, null, null, null, null },
                new int?[] { null,    4, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null,    6, null, null },
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
                    new MagicSquaresConstraint(
                        _CreateStandardPossibleValues(puzzle.Size),
                        boxesToConstrain, includeDiagonals: false)
                });

            var solution = solver.Solve(puzzle);
            PuzzleTestUtils.AssertStandardPuzzleSolved(solution);
            MagicSquareTests.AssertMagicSquaresSatisfied(solution, 15, verifyDiagonals: false);
        }

        [Fact]
        public void SolvesCorrectly_WithDiagonals()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] { null, null, null, null, null, null, null,    1,    2 },
                new int?[] { null,    5, null, null,    9, null, null, null, null },
                new int?[] { null, null, null, null, null,    8, null, null, null },
                new int?[] { null,    3, null, null, null, null,    5, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null,    9, null },
                new int?[] { null, null, null, null,    4,    3,    9, null, null },
                new int?[] {    6, null,    1, null, null, null,    7, null, null },
                new int?[] {    2, null, null, null, null, null, null, null, null },
            });
            var boxesToConstrain = new Box[] {
                new Box(new Coordinate(3, 3), 3),
            };
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> {
                    new RowUniquenessConstraint(),
                    new ColumnUniquenessConstraint(),
                    new BoxUniquenessConstraint(),
                    new MagicSquaresConstraint(
                        _CreateStandardPossibleValues(9),
                        boxesToConstrain, includeDiagonals: true)
                });

            var solution = solver.Solve(puzzle);
            PuzzleTestUtils.AssertStandardPuzzleSolved(solution);
            MagicSquareTests.AssertMagicSquaresSatisfied(solution, boxesToConstrain, 15, verifyDiagonals: true);
        }

        [Fact]
        public void TryConstrain_ConstrainsCorrectly()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] { null, null,    9, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null,    3,    5, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
            });
            var boxesToConstrain = new Box[] {
                new Box(new Coordinate(0, 0), 3),
            };
            var constraint = new MagicSquaresConstraint(
                _CreateStandardPossibleValues(9),
                boxesToConstrain, includeDiagonals: false);
            var graph = ExactCoverGraph.Create(puzzle);

            Assert.True(constraint.TryConstrain(puzzle, graph));

            _AssertPossibleValuesAtSquare(new(0, 0), new int[] { 1, 2, 4, 5 }, graph);
            _AssertPossibleValuesAtSquare(new(0, 1), new int[] { 4, 5 }, graph);
            _AssertPossibleValuesAtSquare(new(1, 0), new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, graph);
            _AssertPossibleValuesAtSquare(new(1, 1), new int[] { 4, 5, 7, 8 }, graph);
            _AssertPossibleValuesAtSquare(new(1, 2), new int[] { 1 }, graph);
            _AssertPossibleValuesAtSquare(new(2, 0), new int[] { 7 }, graph);
            _AssertPossibleValuesAtSquare(new(3, 0), new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, graph);
        }

        [Fact]
        public void TryConstrain_IncludingDiagonals_ConstrainsCorrectly()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null,    6, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null,    3,    8, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
            });
            var boxesToConstrain = new Box[] {
                new Box(new Coordinate(3, 3), 3),
            };
            var constraint = new MagicSquaresConstraint(
                _CreateStandardPossibleValues(9),
                boxesToConstrain, includeDiagonals: true);
            var graph = ExactCoverGraph.Create(puzzle);

            Assert.True(constraint.TryConstrain(puzzle, graph));

            // rows: 18, 27, 45
            // rows: all
            // rows: 4

            // cols: all
            // cols: 84 75
            // cols: 1

            // 124578    | 4578 | x
            // 123456789 | 4578 | 1
            // 4         | x    | x

            // diagonals \: 16 25 34 -> Has to be 25 when combining top left and middle possibles.
            // diagonals /: 18 27 45 -> Has to be 45 when combining bottom left and middle possibles.

            // 25        | 4578 | x
            // 123456789 | 5    | 1
            // 4         | x    | x

            // Note this doesn't filter further because failed optional objectives can't drop
            // possiblities.

            _AssertPossibleValuesAtSquare(new(3, 3), new int[] { 2, 5 }, graph);
            _AssertPossibleValuesAtSquare(new(3, 4), new int[] { 4, 5, 7, 8 }, graph);
            _AssertPossibleValuesAtSquare(new(4, 3), new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, graph);
            _AssertPossibleValuesAtSquare(new(4, 4), new int[] { 5 }, graph);
            _AssertPossibleValuesAtSquare(new(4, 5), new int[] { 1 }, graph);
            _AssertPossibleValuesAtSquare(new(5, 3), new int[] { 4 }, graph);
            _AssertPossibleValuesAtSquare(new(6, 3), new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, graph);
        }

        [Fact]
        public void TryConstrain_NotPossible_Fails()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] { null, null,    6, null, null, null, null, null, null },
                new int?[] {    1, null, null, null, null, null, null, null, null },
                new int?[] { null,    3,    8, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
            });
            var boxesToConstrain = new Box[] {
                new Box(new Coordinate(0, 0), 3),
            };
            var constraint = new MagicSquaresConstraint(
                _CreateStandardPossibleValues(9),
                boxesToConstrain, includeDiagonals: false);
            var graph = ExactCoverGraph.Create(puzzle);

            Assert.False(constraint.TryConstrain(puzzle, graph));
        }

        private static void _AssertPossibleValuesAtSquare(Coordinate coord, int[] possibleValues, ExactCoverGraph graph)
        {
            var square = graph.GetAllPossibilitiesAt(coord);
            Assert.NotNull(square);
            var foundValues = new List<int>();
            for (int valueIndex = 0; valueIndex < square.Length; ++valueIndex)
            {
                var possibleValue = square[valueIndex];
                if (possibleValue is null)
                {
                    continue;
                }
                int value = graph.AllPossibleValues[valueIndex];
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
                $"Found {foundValues.Count} possible values when expected {possibleValues.Length} at {coord}. Expected {string.Join(',', possibleValues)}, found: {string.Join(',', foundValues)}.");
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

    public class MagicSquaresConstraintNotUniqueSolutionTest
    {
        [Fact]
        public void HasUniqueSolution_NotUnique()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] { null, null,    9, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null,    3,    5, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
            });
            var boxesToConstrain = new Box[] {
                new Box(new Coordinate(0, 0), 3),
                new Box(new Coordinate(3, 3), 3),
                new Box(new Coordinate(6, 6), 3),
            };
            var constraint = new MagicSquaresConstraint(
                _CreateStandardPossibleValues(9),
                boxesToConstrain, includeDiagonals: false);
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> {
                    new RowUniquenessConstraint(),
                    constraint,
                });

            Assert.False(solver.HasUniqueSolution(puzzle));
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

    public class MagicSquaresConstraintUniqueSolutionTest
    {
        [Fact]
        public void HasUniqueSolution_IsUnique()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] { null, null,    9, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null,    1, null },
                new int?[] { null, null, null, null, null,    5, null, null, null },
                new int?[] { null, null, null, null, null, null, null, null,    8 },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] {    7, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null,    3, null, null, null, null, null },
                new int?[] { null,    4, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null,    6, null, null },
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
                    new MagicSquaresConstraint(
                        _CreateStandardPossibleValues(puzzle.Size),
                        boxesToConstrain, includeDiagonals: false)
                });

            Assert.True(solver.HasUniqueSolution(puzzle));
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

    public class MagicSquaresConstraintUniqueSolutionWithDiagonalsTest
    {
        [Fact]
        public void HasUniqueSolution_IsUniqueWithDiagonals()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] { null, null, null, null, null, null, null,    1,    2 },
                new int?[] { null,    5, null, null,    9, null, null, null, null },
                new int?[] { null, null, null, null, null,    8, null, null, null },
                new int?[] { null,    3, null, null, null, null,    5, null, null },
                new int?[] { null, null, null, null, null, null, null, null, null },
                new int?[] { null, null, null, null, null, null, null,    9, null },
                new int?[] { null, null, null, null,    4,    3,    9, null, null },
                new int?[] {    6, null,    1, null, null, null,    7, null, null },
                new int?[] {    2, null, null, null, null, null, null, null, null },
            });
            var boxesToConstrain = new Box[] {
                new Box(new Coordinate(3, 3), 3),
            };
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> {
                    new RowUniquenessConstraint(),
                    new ColumnUniquenessConstraint(),
                    new BoxUniquenessConstraint(),
                    new MagicSquaresConstraint(
                        _CreateStandardPossibleValues(9),
                        boxesToConstrain, includeDiagonals: true)
                });

            Assert.True(solver.HasUniqueSolution(puzzle));
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
