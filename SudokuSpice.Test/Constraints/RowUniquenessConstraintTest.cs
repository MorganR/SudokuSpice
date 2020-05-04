using SudokuSpice.Data;
using System.Linq;
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

            Assert.Equal(size * possibleValues.Length, matrix.GetUnsatisfiedConstraintHeaders().Count());
            var firstRowConstraint = matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[0].FirstLink.Constraint;
            var secondRowConstraint = matrix.GetSquare(new Coordinate(1, 0)).AllPossibleValues[0].FirstLink.Constraint;
            var thirdRowConstraint = matrix.GetSquare(new Coordinate(2, 0)).AllPossibleValues[0].FirstLink.Constraint;
            var fourthRowConstraint = matrix.GetSquare(new Coordinate(3, 0)).AllPossibleValues[0].FirstLink.Constraint;
            Assert.NotSame(firstRowConstraint, secondRowConstraint);
            Assert.NotSame(firstRowConstraint, thirdRowConstraint);
            Assert.NotSame(firstRowConstraint, fourthRowConstraint);
            Assert.NotSame(secondRowConstraint, thirdRowConstraint);
            Assert.NotSame(secondRowConstraint, fourthRowConstraint);
            Assert.NotSame(thirdRowConstraint, fourthRowConstraint);
            Assert.Same(firstRowConstraint, matrix.GetSquare(new Coordinate(0, 1)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(firstRowConstraint, matrix.GetSquare(new Coordinate(0, 2)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(firstRowConstraint, matrix.GetSquare(new Coordinate(0, 3)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(secondRowConstraint, matrix.GetSquare(new Coordinate(1, 1)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(secondRowConstraint, matrix.GetSquare(new Coordinate(1, 2)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(secondRowConstraint, matrix.GetSquare(new Coordinate(1, 3)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(thirdRowConstraint, matrix.GetSquare(new Coordinate(2, 1)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(thirdRowConstraint, matrix.GetSquare(new Coordinate(2, 2)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(thirdRowConstraint, matrix.GetSquare(new Coordinate(2, 3)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(fourthRowConstraint, matrix.GetSquare(new Coordinate(3, 1)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(fourthRowConstraint, matrix.GetSquare(new Coordinate(3, 2)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(fourthRowConstraint, matrix.GetSquare(new Coordinate(3, 3)).AllPossibleValues[0].FirstLink.Constraint);
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
