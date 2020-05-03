using SudokuSpice.Constraints;
using SudokuSpice.Data;
using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;

namespace SudokuSpice.Data.Test
{
    public class SquareLinkTest
    {
        [Fact]
        public void Constructor_AsFirstConnection_ConnectsCorrectly()
        {
            var square = new Square(new Coordinate(0, 0), 2);
            var possibleSquare = new PossibleSquareValue(square, 1);
            var constraintHeader = new ConstraintHeader();

            var link = new SquareLink(possibleSquare, constraintHeader);

            Assert.Same(link, link.Up);
            Assert.Same(link, link.Down);
            Assert.Same(link, link.Right);
            Assert.Same(link, link.Left);
            Assert.Same(link, possibleSquare.FirstLink);
            Assert.Same(link, constraintHeader.FirstLink);
            Assert.Same(possibleSquare, link.PossibleSquare);
            Assert.Same(constraintHeader, link.Constraint);
            Assert.Equal(1, constraintHeader.Count);
        }

        [Fact]
        public void Constructor_AsSecondConnection_ConnectsCorrectly()
        {
            var square = new Square(new Coordinate(0, 0), 2);
            var possibleSquare = new PossibleSquareValue(square, 1);
            var constraintHeader = new ConstraintHeader();
            var firstLink = new SquareLink(possibleSquare, constraintHeader);

            var link = new SquareLink(possibleSquare, constraintHeader);

            Assert.Same(firstLink, link.Right);
            Assert.Same(firstLink, link.Left);
            Assert.Same(firstLink, link.Up);
            Assert.Same(firstLink, link.Down);
            Assert.Same(link, firstLink.Right);
            Assert.Same(link, firstLink.Left);
            Assert.Same(link, firstLink.Up);
            Assert.Same(link, firstLink.Down);
            Assert.Same(link, possibleSquare.FirstLink.Right);
            Assert.Same(link, constraintHeader.FirstLink.Down);
            Assert.Same(possibleSquare, link.PossibleSquare);
            Assert.Same(constraintHeader, link.Constraint);
            Assert.Equal(2, constraintHeader.Count);
        }

        [Fact]
        public void TryRemoveFromConstraint_OnSuccess()
        {
            var square = new Square(new Coordinate(0, 0), 2);
            var possibleSquare = new PossibleSquareValue(square, 1);
            var constraintHeader = new ConstraintHeader();
            var firstLink = new SquareLink(possibleSquare, constraintHeader);
            var secondLink = new SquareLink(possibleSquare, constraintHeader);

            Assert.True(firstLink.TryRemoveFromConstraint());

            Assert.Same(secondLink, firstLink.Up);
            Assert.Same(secondLink, firstLink.Down);
            Assert.Same(secondLink, firstLink.Right);
            Assert.Same(secondLink, firstLink.Left);
            Assert.Same(secondLink, constraintHeader.FirstLink);
            Assert.Same(secondLink, secondLink.Up);
            Assert.Same(secondLink, secondLink.Down);
            Assert.Same(firstLink, secondLink.Right);
            Assert.Same(firstLink, secondLink.Left);
            Assert.Same(firstLink, possibleSquare.FirstLink);
            Assert.Equal(1, constraintHeader.Count);
        }

        [Fact]
        public void TryRemoveFromConstraint_OnFailure_LeavesUnchanged()
        {
            var square = new Square(new Coordinate(0, 0), 2);
            var possibleSquare = new PossibleSquareValue(square, 1);
            var constraintHeader = new ConstraintHeader();
            var link = new SquareLink(possibleSquare, constraintHeader);

            Assert.False(link.TryRemoveFromConstraint());

            Assert.Same(link, link.Up);
            Assert.Same(link, link.Down);
            Assert.Same(link, link.Right);
            Assert.Same(link, link.Left);
            Assert.Same(link, constraintHeader.FirstLink);
            Assert.Same(link, possibleSquare.FirstLink);
            Assert.Equal(1, constraintHeader.Count);
        }

        [Fact]
        public void ReturnToConstraint_Succeeds()
        {
            var square = new Square(new Coordinate(0, 0), 2);
            var possibleSquare = new PossibleSquareValue(square, 1);
            var constraintHeader = new ConstraintHeader();
            var firstLink = new SquareLink(possibleSquare, constraintHeader);
            var secondLink = new SquareLink(possibleSquare, constraintHeader);

            Assert.True(firstLink.TryRemoveFromConstraint());
            firstLink.ReturnToConstraint();

            Assert.Same(secondLink, firstLink.Up);
            Assert.Same(secondLink, firstLink.Down);
            Assert.Same(secondLink, firstLink.Right);
            Assert.Same(secondLink, firstLink.Left);
            Assert.Same(firstLink, secondLink.Up);
            Assert.Same(firstLink, secondLink.Down);
            Assert.Same(firstLink, secondLink.Right);
            Assert.Same(firstLink, secondLink.Left);
            Assert.Same(firstLink, possibleSquare.FirstLink);
            Assert.Equal(2, constraintHeader.Count);
        }

        [Fact]
        public void TrySatisfyConstraint_Succeeds()
        {
            var matrix = new ExactCoverMatrix(2, new int[] {1, 2});
            var header = ConstraintHeader.CreateConnectedHeader(
                new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[0],
                    matrix.GetSquare(new Coordinate(1, 0)).AllPossibleValues[0],
                });
            ConstraintHeader.CreateConnectedHeader(
                new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[0],
                    matrix.GetSquare(new Coordinate(0, 1)).AllPossibleValues[0],
                });
            ConstraintHeader.CreateConnectedHeader(
               new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(1, 0)).AllPossibleValues[0],
                    matrix.GetSquare(new Coordinate(1, 1)).AllPossibleValues[0],
               });
            ConstraintHeader.CreateConnectedHeader(
               new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(0, 1)).AllPossibleValues[0],
                    matrix.GetSquare(new Coordinate(1, 1)).AllPossibleValues[0],
               });
            ConstraintHeader.CreateConnectedHeader(
                new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[1],
                    matrix.GetSquare(new Coordinate(1, 0)).AllPossibleValues[1],
                });
            ConstraintHeader.CreateConnectedHeader(
                new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[1],
                    matrix.GetSquare(new Coordinate(0, 1)).AllPossibleValues[1],
                });
            ConstraintHeader.CreateConnectedHeader(
               new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(1, 0)).AllPossibleValues[1],
                    matrix.GetSquare(new Coordinate(1, 1)).AllPossibleValues[1],
               });
            ConstraintHeader.CreateConnectedHeader(
               new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(0, 1)).AllPossibleValues[1],
                    matrix.GetSquare(new Coordinate(1, 1)).AllPossibleValues[1],
               });

            var link = matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[0].FirstLink;
            Assert.True(link.TrySatisfyConstraint());

            Assert.True(header.IsSatisfied);
            Assert.True(link.Constraint.IsSatisfied);
            Assert.NotSame(link.Right, link);
            Assert.Same(link.Right, link.Left);
            Assert.Same(link, link.Right.Left);
            Assert.Same(link, link.Right.Right);
            Assert.Contains(link, link.Constraint.GetLinks());
            Assert.Equal(PossibleSquareState.DROPPED, link.Up.PossibleSquare.State);
            Assert.Equal(1, link.Up.PossibleSquare.Square.NumPossibleValues);
            Assert.Same(link, link.Up.Down);
            Assert.Same(link, link.Up.Up);
            Assert.Same(link.Up.Right, link.Up.Left);
            Assert.NotSame(link.Up, link.Up.Right);
            Assert.Equal(1, link.Up.Right.Constraint.Count);
            Assert.DoesNotContain(link.Up.Right, link.Up.Right.Constraint.GetLinks());
        }

        [Fact]
        public void TrySatisfyConstraint_WithNoOtherChoicesOnConnectedPossible_Fails()
        {
            var matrix = new ExactCoverMatrix(2, new int[] {1, 2});
            var header = ConstraintHeader.CreateConnectedHeader(
                new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[0],
                    matrix.GetSquare(new Coordinate(1, 0)).AllPossibleValues[0],
                });
            ConstraintHeader.CreateConnectedHeader(
                new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[0],
                    matrix.GetSquare(new Coordinate(0, 1)).AllPossibleValues[0],
                });
            ConstraintHeader.CreateConnectedHeader(
               new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(1, 0)).AllPossibleValues[0],
                    matrix.GetSquare(new Coordinate(1, 1)).AllPossibleValues[0],
               });
            ConstraintHeader.CreateConnectedHeader(
               new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(0, 1)).AllPossibleValues[0],
                    matrix.GetSquare(new Coordinate(1, 1)).AllPossibleValues[0],
               });
            ConstraintHeader.CreateConnectedHeader(
                new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[1],
                    matrix.GetSquare(new Coordinate(1, 0)).AllPossibleValues[1],
                });
            ConstraintHeader.CreateConnectedHeader(
                new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[1],
                    matrix.GetSquare(new Coordinate(0, 1)).AllPossibleValues[1],
                });
            ConstraintHeader.CreateConnectedHeader(
               new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(1, 0)).AllPossibleValues[1],
                    matrix.GetSquare(new Coordinate(1, 1)).AllPossibleValues[1],
               });
            ConstraintHeader.CreateConnectedHeader(
               new List<PossibleSquareValue> {
                    matrix.GetSquare(new Coordinate(0, 1)).AllPossibleValues[1],
                    matrix.GetSquare(new Coordinate(1, 1)).AllPossibleValues[1],
               });
            Assert.True(matrix.GetSquare(new Coordinate(1, 0)).AllPossibleValues[1].TryDrop());

            var link = matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[0].FirstLink;
            Assert.False(link.TrySatisfyConstraint());

            Assert.False(header.IsSatisfied);
            Assert.NotSame(link.Right, link);
            Assert.Same(link.Right, link.Left);
            Assert.Same(link, link.Right.Left);
            Assert.Same(link, link.Right.Right);
            Assert.Contains(link, link.Constraint.GetLinks());
            Assert.Equal(PossibleSquareState.UNKNOWN, link.Up.PossibleSquare.State);
            Assert.Equal(1, link.Up.PossibleSquare.Square.NumPossibleValues);
            Assert.Same(link, link.Up.Down);
            Assert.Same(link, link.Up.Up);
            Assert.Same(link.Up.Right, link.Up.Left);
            Assert.NotSame(link.Up, link.Up.Right);
            Assert.Equal(2, link.Up.Right.Constraint.Count);
            Assert.Contains(link.Up.Right, link.Up.Right.Constraint.GetLinks());
        }
    }
}
