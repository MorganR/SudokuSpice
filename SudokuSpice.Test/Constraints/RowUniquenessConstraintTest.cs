using SudokuSpice.Data;
using Xunit;

namespace SudokuSpice.Constraints.Test
{
    public class RowUniquenessConstraintTest
    {
        [Fact]
        public void Constrain_ReturnsExpectedConstraints()
        {
            int size = 4;
            int[] possibleValues = new int[] { 1, 3, 5, 7 };
            var matrix = new ExactCoverMatrix(size, possibleValues);
            var puzzle = new Puzzle(size);

            new RowUniquenessConstraint().Constrain(puzzle, matrix);

            Assert.Equal(size * possibleValues.Length, matrix.ConstraintHeaders.Count);
            Assert.Same(matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[0], matrix.ConstraintHeaders[0].FirstLink.PossibleSquare);
            Assert.Same(matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[1], matrix.ConstraintHeaders[1].FirstLink.PossibleSquare);
            Assert.Same(matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[2], matrix.ConstraintHeaders[2].FirstLink.PossibleSquare);
            Assert.Same(matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[3], matrix.ConstraintHeaders[3].FirstLink.PossibleSquare);
            Assert.Same(matrix.GetSquare(new Coordinate(1, 0)).AllPossibleValues[0], matrix.ConstraintHeaders[4].FirstLink.PossibleSquare);
            Assert.Same(matrix.GetSquare(new Coordinate(2, 0)).AllPossibleValues[0], matrix.ConstraintHeaders[8].FirstLink.PossibleSquare);
            Assert.Same(matrix.GetSquare(new Coordinate(3, 0)).AllPossibleValues[0], matrix.ConstraintHeaders[12].FirstLink.PossibleSquare);

            int value = possibleValues[0];
            int row = 0;
            var headerAtRowValue = matrix.ConstraintHeaders[0];
            ConstraintTestingUtils.AssertPossibleSquareValueIsOnConstraint(matrix.GetSquare(new Coordinate(row, 0)).GetPossibleValue(value), headerAtRowValue);
            ConstraintTestingUtils.AssertPossibleSquareValueIsOnConstraint(matrix.GetSquare(new Coordinate(row, 1)).GetPossibleValue(value), headerAtRowValue);
            ConstraintTestingUtils.AssertPossibleSquareValueIsOnConstraint(matrix.GetSquare(new Coordinate(row, 2)).GetPossibleValue(value), headerAtRowValue);
            ConstraintTestingUtils.AssertPossibleSquareValueIsOnConstraint(matrix.GetSquare(new Coordinate(row, 3)).GetPossibleValue(value), headerAtRowValue);
        }

        [Fact]
        public void Constrain_SetsUpSquareLinksForAllPossibleValues()
        {
            int size = 4;
            int[] possibleValues = new int[] { 1, 3, 5, 7 };
            var matrix = new ExactCoverMatrix(size, possibleValues);
            var puzzle = new Puzzle(size);

            new RowUniquenessConstraint().Constrain(puzzle, matrix);

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    foreach (var possibleValue in matrix.GetSquare(new Coordinate(row, col)).AllPossibleValues)
                    {
                        Assert.NotNull(possibleValue.FirstLink);
                    }
                }
            }
        }
    }
}
