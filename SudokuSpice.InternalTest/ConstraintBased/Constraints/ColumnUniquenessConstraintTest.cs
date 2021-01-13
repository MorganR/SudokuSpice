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
            var matrix = new ExactCoverMatrix(puzzle);

            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            Assert.Equal(size * possibleValues.Length, matrix.GetUnsatisfiedConstraintHeaders().Count());
            ConstraintHeader firstColumnConstraint = matrix.GetSquare(new Coordinate(0, 0)).GetPossibleValue(0).FirstLink.Constraint;
            ConstraintHeader secondColumnConstraint = matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(0).FirstLink.Constraint;
            ConstraintHeader thirdColumnConstraint = matrix.GetSquare(new Coordinate(0, 2)).GetPossibleValue(0).FirstLink.Constraint;
            ConstraintHeader fourthColumnConstraint = matrix.GetSquare(new Coordinate(0, 3)).GetPossibleValue(0).FirstLink.Constraint;
            Assert.NotSame(firstColumnConstraint, secondColumnConstraint);
            Assert.NotSame(firstColumnConstraint, thirdColumnConstraint);
            Assert.NotSame(firstColumnConstraint, fourthColumnConstraint);
            Assert.NotSame(secondColumnConstraint, thirdColumnConstraint);
            Assert.NotSame(secondColumnConstraint, fourthColumnConstraint);
            Assert.NotSame(thirdColumnConstraint, fourthColumnConstraint);
            Assert.Same(firstColumnConstraint, matrix.GetSquare(new Coordinate(1, 0)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(firstColumnConstraint, matrix.GetSquare(new Coordinate(2, 0)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(firstColumnConstraint, matrix.GetSquare(new Coordinate(3, 0)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(secondColumnConstraint, matrix.GetSquare(new Coordinate(1, 1)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(secondColumnConstraint, matrix.GetSquare(new Coordinate(2, 1)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(secondColumnConstraint, matrix.GetSquare(new Coordinate(3, 1)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(thirdColumnConstraint, matrix.GetSquare(new Coordinate(1, 2)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(thirdColumnConstraint, matrix.GetSquare(new Coordinate(2, 2)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(thirdColumnConstraint, matrix.GetSquare(new Coordinate(3, 2)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(fourthColumnConstraint, matrix.GetSquare(new Coordinate(1, 3)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(fourthColumnConstraint, matrix.GetSquare(new Coordinate(2, 3)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(fourthColumnConstraint, matrix.GetSquare(new Coordinate(3, 3)).GetPossibleValue(0).FirstLink.Constraint);
        }

        [Fact]
        public void Constrain_SetsUpSquareLinksForAllPossibleValues()
        {
            int size = 4;
            int[] possibleValues = new int[] { 1, 3, 5, 7 };
            var puzzle = new Puzzle(size);
            var matrix = new ExactCoverMatrix(puzzle);

            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {

                    for (int idx = 0; idx < possibleValues.Length; ++idx) {
                        PossibleSquareValue possibleValue = matrix.GetSquare(new Coordinate(row, col)).GetPossibleValue(idx);
                        Assert.NotNull(possibleValue.FirstLink);
                    }
                }
            }
        }
    }
}