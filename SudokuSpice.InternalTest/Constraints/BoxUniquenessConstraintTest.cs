using SudokuSpice.Data;
using System.Linq;
using Xunit;

namespace SudokuSpice.Constraints.Test
{
    public class BoxUniquenessConstraintTest
    {
        [Fact]
        public void Constrain_GroupsConstraintsAsExpected()
        {
            int size = 4;
            int[] possibleValues = new int[] { 1, 3, 5, 7 };
            var puzzle = new Puzzle(size);
            var matrix = new ExactCoverMatrix<Puzzle>(puzzle);

            new BoxUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);

            Assert.Equal(size * possibleValues.Length, matrix.GetUnsatisfiedConstraintHeaders().Count());
            var firstBoxConstraint = matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[0].FirstLink.Constraint;
            var secondBoxConstraint = matrix.GetSquare(new Coordinate(0, 2)).AllPossibleValues[0].FirstLink.Constraint;
            var thirdBoxConstraint = matrix.GetSquare(new Coordinate(2, 0)).AllPossibleValues[0].FirstLink.Constraint;
            var fourthBoxConstraint = matrix.GetSquare(new Coordinate(2, 2)).AllPossibleValues[0].FirstLink.Constraint;
            Assert.NotSame(firstBoxConstraint, secondBoxConstraint);
            Assert.NotSame(firstBoxConstraint, thirdBoxConstraint);
            Assert.NotSame(firstBoxConstraint, fourthBoxConstraint);
            Assert.NotSame(secondBoxConstraint, thirdBoxConstraint);
            Assert.NotSame(secondBoxConstraint, fourthBoxConstraint);
            Assert.NotSame(thirdBoxConstraint, fourthBoxConstraint);
            Assert.Same(firstBoxConstraint, matrix.GetSquare(new Coordinate(0, 1)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(firstBoxConstraint, matrix.GetSquare(new Coordinate(1, 0)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(firstBoxConstraint, matrix.GetSquare(new Coordinate(1, 1)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(secondBoxConstraint, matrix.GetSquare(new Coordinate(0, 3)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(secondBoxConstraint, matrix.GetSquare(new Coordinate(1, 2)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(secondBoxConstraint, matrix.GetSquare(new Coordinate(1, 3)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(thirdBoxConstraint, matrix.GetSquare(new Coordinate(2, 1)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(thirdBoxConstraint, matrix.GetSquare(new Coordinate(3, 0)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(thirdBoxConstraint, matrix.GetSquare(new Coordinate(3, 1)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(fourthBoxConstraint, matrix.GetSquare(new Coordinate(2, 3)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(fourthBoxConstraint, matrix.GetSquare(new Coordinate(3, 2)).AllPossibleValues[0].FirstLink.Constraint);
            Assert.Same(fourthBoxConstraint, matrix.GetSquare(new Coordinate(3, 3)).AllPossibleValues[0].FirstLink.Constraint);
        }

        [Fact]
        public void Constrain_SetsUpSquareLinksForAllPossibleValues()
        {
            int size = 4;
            int[] possibleValues = new int[] { 1, 3, 5, 7 };
            var puzzle = new Puzzle(size);
            var matrix = new ExactCoverMatrix<Puzzle>(puzzle);

            new BoxUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);

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
