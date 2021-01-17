using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    public class RowUniquenessConstraintTest
    {
        [Fact]
        public void Constrain_ReturnsExpectedConstraints()
        {
            int size = 4;
            int[] possibleValues = new int[] { 1, 3, 5, 7 };
            var puzzle = new Puzzle(size);
            var matrix = new ExactCoverMatrix(puzzle);

            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);

            Assert.Equal(size * possibleValues.Length, matrix.GetUnsatisfiedRequirements().Count());
            Requirement firstRowRequirement = matrix.GetSquare(new Coordinate(0, 0)).GetPossibleValue(0).FirstLink.Objective;
            Requirement secondRowRequirement = matrix.GetSquare(new Coordinate(1, 0)).GetPossibleValue(0).FirstLink.Objective;
            Requirement thirdRowRequirement = matrix.GetSquare(new Coordinate(2, 0)).GetPossibleValue(0).FirstLink.Objective;
            Requirement fourthRowRequirement = matrix.GetSquare(new Coordinate(3, 0)).GetPossibleValue(0).FirstLink.Objective;
            Assert.NotSame(firstRowRequirement, secondRowRequirement);
            Assert.NotSame(firstRowRequirement, thirdRowRequirement);
            Assert.NotSame(firstRowRequirement, fourthRowRequirement);
            Assert.NotSame(secondRowRequirement, thirdRowRequirement);
            Assert.NotSame(secondRowRequirement, fourthRowRequirement);
            Assert.NotSame(thirdRowRequirement, fourthRowRequirement);
            Assert.Same(firstRowRequirement, matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(0).FirstLink.Objective);
            Assert.Same(firstRowRequirement, matrix.GetSquare(new Coordinate(0, 2)).GetPossibleValue(0).FirstLink.Objective);
            Assert.Same(firstRowRequirement, matrix.GetSquare(new Coordinate(0, 3)).GetPossibleValue(0).FirstLink.Objective);
            Assert.Same(secondRowRequirement, matrix.GetSquare(new Coordinate(1, 1)).GetPossibleValue(0).FirstLink.Objective);
            Assert.Same(secondRowRequirement, matrix.GetSquare(new Coordinate(1, 2)).GetPossibleValue(0).FirstLink.Objective);
            Assert.Same(secondRowRequirement, matrix.GetSquare(new Coordinate(1, 3)).GetPossibleValue(0).FirstLink.Objective);
            Assert.Same(thirdRowRequirement, matrix.GetSquare(new Coordinate(2, 1)).GetPossibleValue(0).FirstLink.Objective);
            Assert.Same(thirdRowRequirement, matrix.GetSquare(new Coordinate(2, 2)).GetPossibleValue(0).FirstLink.Objective);
            Assert.Same(thirdRowRequirement, matrix.GetSquare(new Coordinate(2, 3)).GetPossibleValue(0).FirstLink.Objective);
            Assert.Same(fourthRowRequirement, matrix.GetSquare(new Coordinate(3, 1)).GetPossibleValue(0).FirstLink.Objective);
            Assert.Same(fourthRowRequirement, matrix.GetSquare(new Coordinate(3, 2)).GetPossibleValue(0).FirstLink.Objective);
            Assert.Same(fourthRowRequirement, matrix.GetSquare(new Coordinate(3, 3)).GetPossibleValue(0).FirstLink.Objective);
        }

        [Fact]
        public void Constrain_SetsUpSquareLinksForAllPossibleValues()
        {
            int size = 4;
            int[] possibleValues = new int[] { 1, 3, 5, 7 };
            var puzzle = new Puzzle(size);
            var matrix = new ExactCoverMatrix(puzzle);

            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);

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