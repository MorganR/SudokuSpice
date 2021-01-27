﻿using SudokuSpice.Test;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    public class MagicBoxConstraintTest
    {
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

            var boxesToConstrain = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> {
                    new RowUniquenessConstraint(),
                    new ColumnUniquenessConstraint(),
                    new BoxUniquenessConstraint(),
                    new MagicBoxConstraint(9, boxesToConstrain, includeDiagonals: false)
                });

            var solution = solver.Solve(puzzle);
            PuzzleTestUtils.AssertStandardPuzzleSolved(solution);
            _AssertMagicSquaresSatisfied(solution, 15, verifyDiagonals: false);
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
            var boxesToConstrain = new int[] { 0 };
            var constraint = new MagicBoxConstraint(9, boxesToConstrain, includeDiagonals: false);
            var matrix = new ExactCoverMatrix(puzzle);

            Assert.True(constraint.TryConstrain(puzzle, matrix));

            _AssertPossibleValuesAtSquare(new (0, 0), new int[] { 1, 2, 4, 5 }, matrix);
            _AssertPossibleValuesAtSquare(new (0, 1), new int[] { 4, 5 }, matrix);
            _AssertPossibleValuesAtSquare(new (1, 0), new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, matrix);
            _AssertPossibleValuesAtSquare(new (1, 1), new int[] { 4, 5, 7, 8 }, matrix);
            _AssertPossibleValuesAtSquare(new (1, 2), new int[] { 1 }, matrix);
            _AssertPossibleValuesAtSquare(new (2, 0), new int[] { 7 }, matrix);
            _AssertPossibleValuesAtSquare(new (3, 0), new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, matrix);
        }

        private static void _AssertMagicSquaresSatisfied(Puzzle puzzle, int expectedSum, bool verifyDiagonals)
        {
            int boxSize = Boxes.CalculateBoxSize(puzzle.Size);
            for (int boxIdx = 0; boxIdx < puzzle.Size; ++boxIdx)
            {
                var rowSums = new int[boxSize];
                var colSums = new int[boxSize];
                var startCoord = Boxes.GetStartingBoxCoordinate(boxIdx, boxSize);
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
            }
        }

        private static void _AssertPossibleValuesAtSquare(Coordinate coord, int[] possibleValues, ExactCoverMatrix matrix)
        {
            var square = matrix.GetSquare(coord);
            var foundValues = new List<PossibleSquareValue>();
            for (int valueIndex = 0; valueIndex < matrix.AllPossibleValues.Length; ++valueIndex)
            {
                var possibleValue = square.GetPossibleValue(valueIndex);
                int value = matrix.AllPossibleValues[valueIndex];
                if (possibleValue is not null && possibleValue.State == PossibilityState.UNKNOWN)
                {
                    foundValues.Add(possibleValue);
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
        }
    }
}
