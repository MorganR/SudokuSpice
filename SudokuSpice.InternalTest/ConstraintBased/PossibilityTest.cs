using SudokuSpice.ConstraintBased.Constraints;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    public class PossibilityTest
    {
        [Fact]
        public void TryDrop_DropsOnSuccess()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            int valueIndex = 1;
            Square square = matrix.GetSquare(new Coordinate(1, 0));
            Possibility possibility = square.GetPossibleValue(valueIndex);
            Link linkA = possibility.FirstLink;
            Link linkB = linkA.Right;
            Requirement constraintA = linkA.Requirement;
            Requirement constraintB = linkB.Requirement;

            Assert.True(possibility.TryDrop());
            Assert.Equal(3, square.NumPossibleValues);
            Assert.Equal(PossibilityState.DROPPED, possibility.State);
            Assert.DoesNotContain(linkA, constraintA.GetLinks());
            Assert.DoesNotContain(linkB, constraintB.GetLinks());
        }

        [Fact]
        public void TryDrop_LeavesUnchangedOnFailure()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            int valueIndex = 0;
            Assert.True(matrix.GetSquare(new Coordinate(0, 0)).GetPossibleValue(valueIndex).TryDrop());
            Assert.True(matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(valueIndex).TryDrop());
            Assert.True(matrix.GetSquare(new Coordinate(0, 2)).GetPossibleValue(valueIndex).TryDrop());
            Square square = matrix.GetSquare(new Coordinate(0, 3));
            Possibility possibility = square.GetPossibleValue(valueIndex);
            Link linkA = possibility.FirstLink;
            Link linkB = linkA.Right;
            Requirement constraintA = linkA.Requirement;
            Requirement constraintB = linkB.Requirement;
            Assert.False(possibility.TryDrop());

            Assert.Equal(4, square.NumPossibleValues);
            Assert.Equal(PossibilityState.UNKNOWN, possibility.State);
            Assert.Same(linkA, constraintA.FirstLink);
            Assert.Contains(linkB, constraintB.GetLinks());
        }

        [Fact]
        public void Return_UndoesDrop()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            int valueIndex = 0;
            Square square = matrix.GetSquare(new Coordinate(0, 0));
            Possibility possibility = square.GetPossibleValue(valueIndex);
            Link linkA = possibility.FirstLink;
            Link linkB = linkA.Right;
            Assert.True(possibility.TryDrop());
            possibility.Return();

            Assert.Equal(4, square.NumPossibleValues);
            Assert.Equal(PossibilityState.UNKNOWN, possibility.State);
            Requirement constraintA = linkA.Requirement;
            Requirement constraintB = linkB.Requirement;
            Assert.Contains(linkA, constraintA.GetLinks());
            Assert.Contains(linkB, constraintB.GetLinks());
        }

        [Fact]
        public void Return_WithSatisfiedConstraint_UndropsTheRowAsExpected()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            int valueIndex = 0;
            Square square = matrix.GetSquare(new Coordinate(0, 0));
            Possibility possibility = square.GetPossibleValue(valueIndex);
            Link linkA = possibility.FirstLink;
            Link linkB = linkA.Right;
            Requirement constraintA = linkA.Requirement;
            Requirement constraintB = linkB.Requirement;
            Assert.True(constraintB.TrySelect(possibility.FirstLink.Right.Up));
            possibility.Return();

            Assert.Equal(4, square.NumPossibleValues);
            Assert.Equal(PossibilityState.UNKNOWN, possibility.State);
            Assert.Contains(linkA, constraintA.GetLinks());
            Assert.Contains(linkB, constraintB.GetLinks());
        }

        [Fact]
        public void TrySelect_SelectsSquare()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            int valueIndex = 0;
            Square square = matrix.GetSquare(new Coordinate(0, 1));
            Possibility possibility = square.GetPossibleValue(valueIndex);
            Assert.True(possibility.TrySelect());

            Assert.Equal(PossibilityState.SELECTED, possibility.State);
        }

        [Fact]
        public void TrySelect_SatisfiesConstraints()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            int valueIndex = 0;
            Possibility possibility = matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(valueIndex);
            Assert.True(possibility.TrySelect());

            Requirement constraintA = possibility.FirstLink.Requirement;
            Requirement constraintB = possibility.FirstLink.Right.Requirement;
            Assert.True(constraintA.AreAllLinksSelected);
            Assert.True(constraintB.AreAllLinksSelected);
            Assert.DoesNotContain(constraintA, matrix.GetUnsatisfiedRequirements());
            Assert.DoesNotContain(constraintB, matrix.GetUnsatisfiedRequirements());
        }

        [Fact]
        public void TrySelect_DropsConstraintConnectedSquareValues()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            int valueIndex = 0;
            Possibility possibility = matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(valueIndex);
            Assert.True(possibility.TrySelect());

            Requirement constraintA = possibility.FirstLink.Requirement;
            Requirement constraintB = possibility.FirstLink.Right.Requirement;
            Assert.Equal(4, constraintA.GetLinks().Count());
            Assert.Equal(4, constraintB.GetLinks().Count());
            foreach (Link link in constraintA.GetLinks())
            {
                if (link.PossibleSquareValue == possibility)
                {
                    continue;
                }
                Assert.Equal(PossibilityState.DROPPED, link.PossibleSquareValue.State);
            }
            foreach (Link link in constraintB.GetLinks())
            {
                if (link.PossibleSquareValue == possibility)
                {
                    continue;
                }
                Assert.Equal(PossibilityState.DROPPED, link.PossibleSquareValue.State);
            }
        }

        [Fact]
        public void Deselect_WithSelectedValue_SetsStateAndSquareCountCorrectly()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            int valueIndex = 0;
            Square square = matrix.GetSquare(new Coordinate(0, 1));
            Possibility possibility = square.GetPossibleValue(valueIndex);
            Assert.True(possibility.TrySelect());
            possibility.Deselect();

            Assert.Equal(PossibilityState.UNKNOWN, possibility.State);
            Assert.Equal(4, square.NumPossibleValues);
        }

        [Fact]
        public void Deselect_WithSelectedValue_ReturnsDroppedRows()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            int valueIndex = 0;
            Square square = matrix.GetSquare(new Coordinate(0, 1));
            Possibility possibility = square.GetPossibleValue(valueIndex);
            Assert.True(possibility.TrySelect());
            possibility.Deselect();

            Link linkA = possibility.FirstLink;
            Link linkB = linkA.Right;
            Requirement constraintA = linkA.Requirement;
            Requirement constraintB = linkB.Requirement;
            Assert.False(constraintA.AreAllLinksSelected);
            Assert.False(constraintB.AreAllLinksSelected);
            foreach (Link link in constraintA.GetLinks())
            {
                if (link != linkA)
                {
                    Assert.Equal(PossibilityState.UNKNOWN, link.PossibleSquareValue.State);
                }
            }
            foreach (Link link in constraintB.GetLinks())
            {
                if (link != linkB)
                {
                    Assert.Equal(PossibilityState.UNKNOWN, link.PossibleSquareValue.State);
                }
            }
        }
    }
}