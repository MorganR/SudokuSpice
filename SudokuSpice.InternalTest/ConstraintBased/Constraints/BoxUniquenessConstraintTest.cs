﻿using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    public class BoxUniquenessConstraintTest
    {
        [Fact]
        public void Constrain_GroupsConstraintsAsExpected()
        {
            int size = 4;
            int[] possibleValues = new int[] { 1, 3, 5, 7 };
            var puzzle = new Puzzle(size);
            var matrix = new ExactCoverMatrix(puzzle);

            new BoxUniquenessConstraint().TryConstrain(puzzle, matrix);

            Assert.Equal(size * possibleValues.Length, matrix.GetUnsatisfiedConstraintHeaders().Count());
            ConstraintHeader firstBoxConstraint = matrix.GetSquare(new Coordinate(0, 0)).GetPossibleValue(0).FirstLink.Constraint;
            ConstraintHeader secondBoxConstraint = matrix.GetSquare(new Coordinate(0, 2)).GetPossibleValue(0).FirstLink.Constraint;
            ConstraintHeader thirdBoxConstraint = matrix.GetSquare(new Coordinate(2, 0)).GetPossibleValue(0).FirstLink.Constraint;
            ConstraintHeader fourthBoxConstraint = matrix.GetSquare(new Coordinate(2, 2)).GetPossibleValue(0).FirstLink.Constraint;
            Assert.NotSame(firstBoxConstraint, secondBoxConstraint);
            Assert.NotSame(firstBoxConstraint, thirdBoxConstraint);
            Assert.NotSame(firstBoxConstraint, fourthBoxConstraint);
            Assert.NotSame(secondBoxConstraint, thirdBoxConstraint);
            Assert.NotSame(secondBoxConstraint, fourthBoxConstraint);
            Assert.NotSame(thirdBoxConstraint, fourthBoxConstraint);
            Assert.Same(firstBoxConstraint, matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(firstBoxConstraint, matrix.GetSquare(new Coordinate(1, 0)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(firstBoxConstraint, matrix.GetSquare(new Coordinate(1, 1)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(secondBoxConstraint, matrix.GetSquare(new Coordinate(0, 3)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(secondBoxConstraint, matrix.GetSquare(new Coordinate(1, 2)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(secondBoxConstraint, matrix.GetSquare(new Coordinate(1, 3)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(thirdBoxConstraint, matrix.GetSquare(new Coordinate(2, 1)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(thirdBoxConstraint, matrix.GetSquare(new Coordinate(3, 0)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(thirdBoxConstraint, matrix.GetSquare(new Coordinate(3, 1)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(fourthBoxConstraint, matrix.GetSquare(new Coordinate(2, 3)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(fourthBoxConstraint, matrix.GetSquare(new Coordinate(3, 2)).GetPossibleValue(0).FirstLink.Constraint);
            Assert.Same(fourthBoxConstraint, matrix.GetSquare(new Coordinate(3, 3)).GetPossibleValue(0).FirstLink.Constraint);
        }

        [Fact]
        public void Constrain_SetsUpSquareLinksForAllPossibleValues()
        {
            int size = 4;
            int[] possibleValues = new int[] { 1, 3, 5, 7 };
            var puzzle = new Puzzle(size);
            var matrix = new ExactCoverMatrix(puzzle);

            new BoxUniquenessConstraint().TryConstrain(puzzle, matrix);

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