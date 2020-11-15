using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    public class ColumnUniquenessConstraintTest
    {
        [Fact]
        public void Constrain_ReturnsExpectedConstraints()
        {
            int size = 4;
            int[] possibleValues = new int[] { 1, 3, 5, 7 };
            var puzzle = new Puzzle(size);
            var matrix = new ExactCoverMatrix<Puzzle>(puzzle);

            new ColumnUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);

            Assert.Equal(size * possibleValues.Length, matrix.GetUnsatisfiedConstraintHeaders().Count());
            ConstraintHeader<Puzzle> firstColumnConstraint = matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[0].FirstLink.Constraint;
            ConstraintHeader<Puzzle> secondColumnConstraint = matrix.GetSquare(new Coordinate(0, 1)).AllPossibleValues[0].FirstLink.Constraint;
            ConstraintHeader<Puzzle> thirdColumnConstraint = matrix.GetSquare(new Coordinate(0, 2)).AllPossibleValues[0].FirstLink.Constraint;
            ConstraintHeader<Puzzle> fourthColumnConstraint = matrix.GetSquare(new Coordinate(0, 3)).AllPossibleValues[0].FirstLink.Constraint;
            Assert.NotSame(firstColumnConstraint, secondColumnConstraint);
            Assert.NotSame(firstColumnConstraint, thirdColumnConstraint);
            Assert.NotSame(firstColumnConstraint, fourthColumnConstraint);
            Assert.NotSame(secondColumnConstraint, thirdColumnConstraint);
            Assert.NotSame(secondColumnConstraint, fourthColumnConstraint);
            Assert.NotSame(thirdColumnConstraint, fourthColumnConstraint);
            Assert.Same(firstColumnConstraint, matrix.GetSquare(new Coordinate(1, 0)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(firstColumnConstraint, matrix.GetSquare(new Coordinate(2, 0)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(firstColumnConstraint, matrix.GetSquare(new Coordinate(3, 0)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(secondColumnConstraint, matrix.GetSquare(new Coordinate(1, 1)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(secondColumnConstraint, matrix.GetSquare(new Coordinate(2, 1)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(secondColumnConstraint, matrix.GetSquare(new Coordinate(3, 1)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(thirdColumnConstraint, matrix.GetSquare(new Coordinate(1, 2)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(thirdColumnConstraint, matrix.GetSquare(new Coordinate(2, 2)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(thirdColumnConstraint, matrix.GetSquare(new Coordinate(3, 2)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(fourthColumnConstraint, matrix.GetSquare(new Coordinate(1, 3)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(fourthColumnConstraint, matrix.GetSquare(new Coordinate(2, 3)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(fourthColumnConstraint, matrix.GetSquare(new Coordinate(3, 3)).AllPossibleValues[0].FirstLink.Constraint);
        }

        [Fact]
        public void Constrain_SetsUpSquareLinksForAllPossibleValues()
        {
            int size = 4;
            int[] possibleValues = new int[] { 1, 3, 5, 7 };
            var puzzle = new Puzzle(size);
            var matrix = new ExactCoverMatrix<Puzzle>(puzzle);

            new ColumnUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    foreach (PossibleSquareValue<Puzzle> possibleValue in matrix.GetSquare(new Coordinate(row, col)).AllPossibleValues)
                    {
                        Assert.NotNull(possibleValue.FirstLink);
                    }
                }
            }
        }
    }
}
