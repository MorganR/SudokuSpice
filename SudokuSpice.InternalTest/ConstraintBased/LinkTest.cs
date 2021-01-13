using SudokuSpice.ConstraintBased.Constraints;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    public class LinkTest
    {
        [Fact]
        public void CreateConnectedLink_ConnectsCorrectly()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            var square = new Square(new Coordinate(0, 0), 2);
            var possibleSquare = new PossibleSquareValue(square, 1);
            var constraintHeader = new ConstraintHeader(1, matrix);

            var link = Link.CreateConnectedLink(possibleSquare, constraintHeader);

            Assert.Same(link, link.Up);
            Assert.Same(link, link.Down);
            Assert.Same(link, link.Right);
            Assert.Same(link, link.Left);
            Assert.Same(possibleSquare, link.PossibleSquareValue);
            Assert.Same(constraintHeader, link.Constraint);
            Assert.True(constraintHeader.AreAllLinksRequired);
            Assert.False(constraintHeader.AreAllLinksSelected);
            Assert.Same(link, constraintHeader.FirstLink);
            Assert.Same(link, possibleSquare.FirstLink);
        }

        [Fact]
        public void CreateConnectedLink_WithExistingLinks_ConnectsCorrectly()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            var square = new Square(new Coordinate(0, 0), 2);
            var possibleSquare = new PossibleSquareValue(square, 1);
            var constraintHeader = new ConstraintHeader(1, matrix);

            var firstLink = Link.CreateConnectedLink(possibleSquare, constraintHeader);
            var link = Link.CreateConnectedLink(possibleSquare, constraintHeader);

            Assert.Same(firstLink, link.Up);
            Assert.Same(firstLink, link.Down);
            Assert.Same(firstLink, link.Right);
            Assert.Same(firstLink, link.Left);
            Assert.Same(link, firstLink.Up);
            Assert.Same(link, firstLink.Down);
            Assert.Same(link, firstLink.Right);
            Assert.Same(link, firstLink.Left);
            Assert.Same(possibleSquare, link.PossibleSquareValue);
            Assert.Same(constraintHeader, link.Constraint);
            Assert.False(constraintHeader.AreAllLinksRequired);
            Assert.False(constraintHeader.AreAllLinksSelected);
            Assert.Same(firstLink, constraintHeader.FirstLink);
            Assert.Same(firstLink, possibleSquare.FirstLink);
        }

        [Fact]
        public void TryRemoveFromConstraint_OnSuccess()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            var constraintHeader = ConstraintHeader.CreateConnectedHeader(
                matrix,
                matrix.GetSquaresOnRow(0).ToArray().Select(s => s.AllPossibleValues[0]).ToArray());
            Link firstLink = constraintHeader.FirstLink;
            Link secondLink = firstLink.Down;
            Link thirdLink = secondLink.Down;
            Link fourthLink = thirdLink.Down;

            Assert.True(firstLink.TryRemoveFromConstraint());

            Assert.Same(fourthLink, firstLink.Up);
            Assert.Same(secondLink, firstLink.Down);
            Assert.Same(secondLink, constraintHeader.FirstLink);
            Assert.Same(fourthLink, secondLink.Up);
            Assert.Same(secondLink, fourthLink.Down);
            Assert.False(constraintHeader.AreAllLinksRequired);
            Assert.False(constraintHeader.AreAllLinksSelected);
        }

        [Fact]
        public void TryRemoveFromConstraint_WhenConstraintSatisfied_LeavesUnchanged()
        {
            var puzzle = new Puzzle(2);
            var matrix = new ExactCoverMatrix(puzzle);
            var constraintHeader = ConstraintHeader.CreateConnectedHeader(
                matrix,
                matrix.GetSquaresOnRow(0).ToArray().Select(s => s.AllPossibleValues[0]).ToArray());
            Link firstLink = constraintHeader.FirstLink;
            Link secondLink = firstLink.Down;

            Assert.True(firstLink.TrySelectForConstraint());
            Assert.True(secondLink.TryRemoveFromConstraint());

            Assert.False(constraintHeader.AreAllLinksRequired);
            Assert.True(constraintHeader.AreAllLinksSelected);
            Assert.Same(firstLink, secondLink.Up);
            Assert.Same(secondLink, firstLink.Down);
            Assert.Same(secondLink, firstLink.Up);
            Assert.Same(firstLink, secondLink.Down);
        }

        [Fact]
        public void ReturnToConstraint_Succeeds()
        {
            var puzzle = new Puzzle(2);
            var matrix = new ExactCoverMatrix(puzzle);
            var constraintHeader = ConstraintHeader.CreateConnectedHeader(
                matrix,
                matrix.GetSquaresOnRow(0).ToArray().Select(s => s.AllPossibleValues[0]).ToArray());
            Link firstLink = constraintHeader.FirstLink;
            Link secondLink = firstLink.Down;

            Assert.True(firstLink.TryRemoveFromConstraint());
            Assert.True(constraintHeader.AreAllLinksRequired);
            firstLink.ReturnToConstraint();

            Assert.False(constraintHeader.AreAllLinksRequired);
            Assert.Same(firstLink, secondLink.Up);
            Assert.Same(secondLink, firstLink.Down);
            Assert.Same(firstLink, secondLink.Down);
            Assert.Same(secondLink, firstLink.Up);
        }

        [Fact]
        public void TrySatisfyConstraint_Succeeds()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            Link link = matrix.GetSquare(new Coordinate(0, 0)).AllPossibleValues[0].FirstLink;
            ConstraintHeader header = link.Constraint;
            Assert.True(link.TrySelectForConstraint());

            Assert.True(header.AreAllLinksSelected);
            Assert.True(link.Constraint.AreAllLinksSelected);
            // Still connected horizontally.
            Assert.NotSame(link, link.Right);
            Assert.NotSame(link, link.Left);
            Assert.Same(link, link.Right.Left);
            Assert.Same(link, link.Left.Right);
            // Vertically connected links are dropped from possible values.
            Assert.Equal(PossibleValueState.DROPPED, link.Up.PossibleSquareValue.State);
            Assert.Equal(3, link.Up.PossibleSquareValue.Square.NumPossibleValues);
            // Still connected vertically.
            Assert.Contains(link, link.Constraint.GetLinks());
        }

        [Fact]
        public void TrySelectForConstraint_WithNoOtherChoicesOnSquare_Fails()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            Square square = matrix.GetSquare(new Coordinate(0, 0));
            Link lastLink = square.AllPossibleValues[0].FirstLink;
            ConstraintHeader header = lastLink.Constraint;
            for (int i = 1; i < square.AllPossibleValues.Length; i++)
            {
                Assert.True(square.AllPossibleValues[i].TryDrop());
            }
            Link linkFromDifferentSquare = lastLink.Down;
            Assert.False(linkFromDifferentSquare.TrySelectForConstraint());

            Assert.False(header.AreAllLinksSelected);
            Assert.Equal(1, square.NumPossibleValues);
            Assert.Same(lastLink.PossibleSquareValue, square.GetStillPossibleValues().Single());
            Assert.Equal(PossibleValueState.UNKNOWN, lastLink.PossibleSquareValue.State);
        }
    }
}