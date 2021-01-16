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
            var possibleSquare = new Possibility(square, 1);
            var requirement = new Requirement(false, 1, matrix);

            var link = Link.CreateConnectedLink(possibleSquare, requirement);

            Assert.Same(link, link.Up);
            Assert.Same(link, link.Down);
            Assert.Same(link, link.Right);
            Assert.Same(link, link.Left);
            Assert.Same(possibleSquare, link.PossibleSquareValue);
            Assert.Same(requirement, link.Requirement);
            Assert.True(requirement.AreAllLinksRequired);
            Assert.False(requirement.AreAllLinksSelected);
            Assert.Same(link, requirement.FirstLink);
            Assert.Same(link, possibleSquare.FirstLink);
        }

        [Fact]
        public void CreateConnectedLink_WithExistingLinks_ConnectsCorrectly()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            var square = new Square(new Coordinate(0, 0), 2);
            var possibleSquare = new Possibility(square, 1);
            var requirement = new Requirement(false, 1, matrix);

            var firstLink = Link.CreateConnectedLink(possibleSquare, requirement);
            var link = Link.CreateConnectedLink(possibleSquare, requirement);

            Assert.Same(firstLink, link.Up);
            Assert.Same(firstLink, link.Down);
            Assert.Same(firstLink, link.Right);
            Assert.Same(firstLink, link.Left);
            Assert.Same(link, firstLink.Up);
            Assert.Same(link, firstLink.Down);
            Assert.Same(link, firstLink.Right);
            Assert.Same(link, firstLink.Left);
            Assert.Same(possibleSquare, link.PossibleSquareValue);
            Assert.Same(requirement, link.Requirement);
            Assert.False(requirement.AreAllLinksRequired);
            Assert.False(requirement.AreAllLinksSelected);
            Assert.Same(firstLink, requirement.FirstLink);
            Assert.Same(firstLink, possibleSquare.FirstLink);
        }

        [Fact]
        public void TryRemoveFromRequirement_OnSuccess()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            var requirement = Requirement.CreateFullyConnected(
                matrix,
                matrix.GetSquaresOnRow(0).ToArray().Select(s => s.GetPossibleValue(0)).ToArray());
            Link firstLink = requirement.FirstLink;
            Link secondLink = firstLink.Down;
            Link thirdLink = secondLink.Down;
            Link fourthLink = thirdLink.Down;

            Assert.True(firstLink.TryRemoveFromRequirement());

            Assert.Same(fourthLink, firstLink.Up);
            Assert.Same(secondLink, firstLink.Down);
            Assert.Same(secondLink, requirement.FirstLink);
            Assert.Same(fourthLink, secondLink.Up);
            Assert.Same(secondLink, fourthLink.Down);
            Assert.False(requirement.AreAllLinksRequired);
            Assert.False(requirement.AreAllLinksSelected);
        }

        [Fact]
        public void TryRemoveFromRequirement_WhenRequirementSatisfied_LeavesUnchanged()
        {
            var puzzle = new Puzzle(2);
            var matrix = new ExactCoverMatrix(puzzle);
            var requirement = Requirement.CreateFullyConnected(
                matrix,
                matrix.GetSquaresOnRow(0).ToArray().Select(s => s.GetPossibleValue(0)).ToArray());
            Link firstLink = requirement.FirstLink;
            Link secondLink = firstLink.Down;

            Assert.True(firstLink.TrySelectForRequirement());
            Assert.True(secondLink.TryRemoveFromRequirement());

            Assert.False(requirement.AreAllLinksRequired);
            Assert.True(requirement.AreAllLinksSelected);
            Assert.Same(firstLink, secondLink.Up);
            Assert.Same(secondLink, firstLink.Down);
            Assert.Same(secondLink, firstLink.Up);
            Assert.Same(firstLink, secondLink.Down);
        }

        [Fact]
        public void ReturnToRequirement_Succeeds()
        {
            var puzzle = new Puzzle(2);
            var matrix = new ExactCoverMatrix(puzzle);
            var requirement = Requirement.CreateFullyConnected(
                matrix,
                matrix.GetSquaresOnRow(0).ToArray().Select(s => s.GetPossibleValue(0)).ToArray());
            Link firstLink = requirement.FirstLink;
            Link secondLink = firstLink.Down;

            Assert.True(firstLink.TryRemoveFromRequirement());
            Assert.True(requirement.AreAllLinksRequired);
            firstLink.ReturnToRequirement();

            Assert.False(requirement.AreAllLinksRequired);
            Assert.Same(firstLink, secondLink.Up);
            Assert.Same(secondLink, firstLink.Down);
            Assert.Same(firstLink, secondLink.Down);
            Assert.Same(secondLink, firstLink.Up);
        }

        [Fact]
        public void TrySatisfyRequirement_Succeeds()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            Link link = matrix.GetSquare(new Coordinate(0, 0)).GetPossibleValue(0).FirstLink;
            Requirement header = link.Requirement;
            Assert.True(link.TrySelectForRequirement());

            Assert.True(header.AreAllLinksSelected);
            Assert.True(link.Requirement.AreAllLinksSelected);
            // Still connected horizontally.
            Assert.NotSame(link, link.Right);
            Assert.NotSame(link, link.Left);
            Assert.Same(link, link.Right.Left);
            Assert.Same(link, link.Left.Right);
            // Vertically connected links are dropped from possible values.
            Assert.Equal(PossibilityState.DROPPED, link.Up.PossibleSquareValue.State);
            Assert.Equal(3, link.Up.PossibleSquareValue.Square.NumPossibleValues);
            // Still connected vertically.
            Assert.Contains(link, link.Requirement.GetLinks());
        }

        [Fact]
        public void TrySelectForRequirement_WithNoOtherChoicesOnSquare_Fails()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            Square square = matrix.GetSquare(new Coordinate(0, 0));
            Link lastLink = square.GetPossibleValue(0).FirstLink;
            Requirement header = lastLink.Requirement;
            for (int i = 1; i < matrix.AllPossibleValues.Length; i++)
            {
                Assert.True(square.GetPossibleValue(i).TryDrop());
            }
            Link linkFromDifferentSquare = lastLink.Down;
            Assert.False(linkFromDifferentSquare.TrySelectForRequirement());

            Assert.False(header.AreAllLinksSelected);
            Assert.Equal(1, square.NumPossibleValues);
            Assert.Same(lastLink.PossibleSquareValue, square.GetStillPossibleValues().Single());
            Assert.Equal(PossibilityState.UNKNOWN, lastLink.PossibleSquareValue.State);
        }
    }
}