﻿using SudokuSpice.Constraints;
using System.Linq;
using Xunit;

namespace SudokuSpice.Data.Test
{
    public class SquareLinkTest
    {
        [Fact]
        public void CreateConnectedLink_ConnectsCorrectly()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle, new int[] { 1, 2, 3, 4 });
            var square = new Square(new Coordinate(0, 0), 2);
            var possibleSquare = new PossibleSquareValue(square, 1);
            var constraintHeader = new ConstraintHeader(matrix);

            var link = SquareLink.CreateConnectedLink(possibleSquare, constraintHeader);

            Assert.Same(link, link.Up);
            Assert.Same(link, link.Down);
            Assert.Same(link, link.Right);
            Assert.Same(link, link.Left);
            Assert.Same(possibleSquare, link.PossibleSquare);
            Assert.Same(constraintHeader, link.Constraint);
            Assert.Equal(1, constraintHeader.Count);
            Assert.Same(link, constraintHeader.FirstLink);
            Assert.Same(link, possibleSquare.FirstLink);
        }

        [Fact]
        public void CreateConnectedLink_WithExistingLinks_ConnectsCorrectly()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle, new int[] { 1, 2, 3, 4 });
            var square = new Square(new Coordinate(0, 0), 2);
            var possibleSquare = new PossibleSquareValue(square, 1);
            var constraintHeader = new ConstraintHeader(matrix);

            var firstLink = SquareLink.CreateConnectedLink(possibleSquare, constraintHeader);
            var link = SquareLink.CreateConnectedLink(possibleSquare, constraintHeader);

            Assert.Same(firstLink, link.Up);
            Assert.Same(firstLink, link.Down);
            Assert.Same(firstLink, link.Right);
            Assert.Same(firstLink, link.Left);
            Assert.Same(link, firstLink.Up);
            Assert.Same(link, firstLink.Down);
            Assert.Same(link, firstLink.Right);
            Assert.Same(link, firstLink.Left);
            Assert.Same(possibleSquare, link.PossibleSquare);
            Assert.Same(constraintHeader, link.Constraint);
            Assert.Equal(2, constraintHeader.Count);
            Assert.Same(firstLink, constraintHeader.FirstLink);
            Assert.Same(firstLink, possibleSquare.FirstLink);
        }

        [Fact]
        public void TryRemoveFromConstraint_OnSuccess()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle, new int[] { 1, 2, 3, 4 });
            var constraintHeader = ConstraintHeader.CreateConnectedHeader(
                matrix,
                matrix.GetSquaresOnRow(0).ToArray().Select(s => s.AllPossibleValues[0]).ToArray());
            var firstLink = constraintHeader.FirstLink;
            var secondLink = firstLink.Down;
            var thirdLink = secondLink.Down;
            var fourthLink = thirdLink.Down;

            Assert.True(firstLink.TryRemoveFromConstraint());

            Assert.Same(fourthLink, firstLink.Up);
            Assert.Same(secondLink, firstLink.Down);
            Assert.Same(secondLink, constraintHeader.FirstLink);
            Assert.Same(fourthLink, secondLink.Up);
            Assert.Same(secondLink, fourthLink.Down);
            Assert.Equal(3, constraintHeader.Count);
        }

        [Fact]
        public void TryRemoveFromConstraint_WhenConstraintSatisfied_LeavesUnchanged()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle, new int[] { 1, 2, 3, 4 });
            var constraintHeader = ConstraintHeader.CreateConnectedHeader(
                matrix,
                matrix.GetSquaresOnRow(0).ToArray().Select(s => s.AllPossibleValues[0]).ToArray());
            var firstLink = constraintHeader.FirstLink;
            var secondLink = firstLink.Down;
            var thirdLink = secondLink.Down;
            var fourthLink = thirdLink.Down;
            var expectedConstraintCount = constraintHeader.Count;

            Assert.True(firstLink.TrySatisfyConstraint());
            Assert.True(secondLink.TryRemoveFromConstraint());

            Assert.Equal(expectedConstraintCount, constraintHeader.Count);
            Assert.Same(firstLink, secondLink.Up);
            Assert.Same(secondLink, firstLink.Down);
            Assert.Same(secondLink, thirdLink.Up);
            Assert.Same(thirdLink, secondLink.Down);
        }

        [Fact]
        public void ReturnToConstraint_Succeeds()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle, new int[] { 1, 2, 3, 4 });
            var constraintHeader = ConstraintHeader.CreateConnectedHeader(
                matrix,
                matrix.GetSquaresOnRow(0).ToArray().Select(s => s.AllPossibleValues[0]).ToArray());
            var firstLink = constraintHeader.FirstLink;
            var secondLink = firstLink.Down;
            var thirdLink = secondLink.Down;
            var fourthLink = thirdLink.Down;
            var expectedConstraintCount = constraintHeader.Count;

            Assert.True(firstLink.TryRemoveFromConstraint());
            firstLink.ReturnToConstraint();

            Assert.Equal(expectedConstraintCount, constraintHeader.Count);
            Assert.Same(firstLink, secondLink.Up);
            Assert.Same(secondLink, firstLink.Down);
            Assert.Same(firstLink, fourthLink.Down);
            Assert.Same(fourthLink, firstLink.Up);
        }

        [Fact]
        public void TrySatisfyConstraint_Succeeds()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle, new int[] { 1, 2, 3, 4 });
            new RowUniquenessConstraint().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint().Constrain(puzzle, matrix);

            var link = matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[0].FirstLink;
            var header = link.Constraint;
            Assert.True(link.TrySatisfyConstraint());

            Assert.True(header.IsSatisfied);
            Assert.True(link.Constraint.IsSatisfied);
            // Still connected horizontally.
            Assert.Same(link, link.Right.Left);
            Assert.Same(link, link.Left.Right);
            // Vertically connected links are dropped form possible values.
            Assert.Equal(PossibleSquareState.DROPPED, link.Up.PossibleSquare.State);
            Assert.Equal(3, link.Up.PossibleSquare.Square.NumPossibleValues);
            // Still connected vertically.
            Assert.Contains(link, link.Constraint.GetLinks());
        }

        [Fact]
        public void TrySatisfyConstraint_WithNoOtherChoicesOnConnectedPossible_Fails()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle, new int[] { 1, 2, 3, 4 });
            new RowUniquenessConstraint().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint().Constrain(puzzle, matrix);

            var square = matrix.GetSquare(new Coordinate(0, 0));
            var lastLink = square.AllPossibleValues[0].FirstLink;
            var header = lastLink.Constraint;
            for (int i = 1; i < square.AllPossibleValues.Length; i++)
            {
                Assert.True(square.AllPossibleValues[i].TryDrop());
            }
            var link = lastLink.Down;
            Assert.False(link.TrySatisfyConstraint());

            Assert.False(header.IsSatisfied);
            Assert.Equal(1, square.NumPossibleValues);
            Assert.Same(lastLink.PossibleSquare, square.GetStillPossibleValues().Single());
            Assert.Equal(PossibleSquareState.UNKNOWN, lastLink.PossibleSquare.State);
        }
    }
}
