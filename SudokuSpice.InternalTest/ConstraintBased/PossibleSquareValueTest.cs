using SudokuSpice.ConstraintBased.Constraints;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    public class PossibleSquareValueTest
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
            PossibleSquareValue possibility = square.GetPossibleValue(valueIndex);
            Link<PossibleSquareValue, Requirement> linkA = possibility.FirstLink;
            Link<PossibleSquareValue, Requirement> linkB = linkA.NextOnPossibility;
            Requirement constraintA = linkA.Objective;
            Requirement constraintB = linkB.Objective;

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
            PossibleSquareValue possibility = square.GetPossibleValue(valueIndex);
            Link<PossibleSquareValue, Requirement> linkA = possibility.FirstLink;
            Link<PossibleSquareValue, Requirement> linkB = linkA.NextOnPossibility;
            Requirement constraintA = linkA.Objective;
            Requirement constraintB = linkB.Objective;
            Assert.False(possibility.TryDrop());

            Assert.Equal(4, square.NumPossibleValues);
            Assert.Equal(PossibilityState.UNKNOWN, possibility.State);
            Assert.Same(linkA, constraintA.FirstPossibilityLink);
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
            PossibleSquareValue possibility = square.GetPossibleValue(valueIndex);
            Link<PossibleSquareValue, Requirement> linkA = possibility.FirstLink;
            Link<PossibleSquareValue, Requirement> linkB = linkA.NextOnPossibility;
            Assert.True(possibility.TryDrop());
            possibility.Return();

            Assert.Equal(4, square.NumPossibleValues);
            Assert.Equal(PossibilityState.UNKNOWN, possibility.State);
            Requirement constraintA = linkA.Objective;
            Requirement constraintB = linkB.Objective;
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
            PossibleSquareValue possibility = square.GetPossibleValue(valueIndex);
            Link<PossibleSquareValue, Requirement> linkA = possibility.FirstLink;
            Link<PossibleSquareValue, Requirement> linkB = linkA.NextOnPossibility;
            Requirement requirementA = linkA.Objective;
            Requirement requirementB = linkB.Objective;
            // This will drop the possibility via linkB.
            Assert.True(requirementB.TrySelectPossibility(linkB.PreviousOnObjective));

            possibility.Return();

            Assert.Equal(4, square.NumPossibleValues);
            Assert.Equal(PossibilityState.UNKNOWN, possibility.State);
            Assert.Contains(linkA, requirementA.GetLinks());
            Assert.Contains(linkB, requirementB.GetLinks());
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
            PossibleSquareValue possibility = square.GetPossibleValue(valueIndex);
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
            PossibleSquareValue possibility = matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(valueIndex);
            Assert.True(possibility.TrySelect());

            Requirement requirementA = possibility.FirstLink.Objective;
            Requirement requirementB = possibility.FirstLink.NextOnPossibility.Objective;
            Assert.True(requirementA.AreRequiredLinksSelected);
            Assert.True(requirementB.AreRequiredLinksSelected);
            Assert.DoesNotContain(requirementA, matrix.GetUnsatisfiedRequirements());
            Assert.DoesNotContain(requirementB, matrix.GetUnsatisfiedRequirements());
        }

        [Fact]
        public void TrySelect_DropsConstraintConnectedSquareValues()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            int valueIndex = 0;
            PossibleSquareValue possibility = matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(valueIndex);
            Assert.True(possibility.TrySelect());

            Requirement requirementA = possibility.FirstLink.Objective;
            Requirement requirementB = possibility.FirstLink.NextOnPossibility.Objective;
            Assert.Equal(4, requirementA.GetLinks().Count());
            Assert.Equal(4, requirementB.GetLinks().Count());
            foreach (Link<PossibleSquareValue, Requirement> link in requirementA.GetLinks())
            {
                if (link.Possibility == possibility)
                {
                    continue;
                }
                Assert.Equal(PossibilityState.DROPPED, link.Possibility.State);
            }
            foreach (Link<PossibleSquareValue, Requirement> link in requirementB.GetLinks())
            {
                if (link.Possibility == possibility)
                {
                    continue;
                }
                Assert.Equal(PossibilityState.DROPPED, link.Possibility.State);
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
            PossibleSquareValue possibility = square.GetPossibleValue(valueIndex);
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
            PossibleSquareValue possibility = square.GetPossibleValue(valueIndex);
            Assert.True(possibility.TrySelect());
            possibility.Deselect();

            Link<PossibleSquareValue, Requirement> linkA = possibility.FirstLink;
            Link<PossibleSquareValue, Requirement> linkB = linkA.NextOnPossibility;
            Requirement constraintA = linkA.Objective;
            Requirement constraintB = linkB.Objective;
            Assert.False(constraintA.AreRequiredLinksSelected);
            Assert.False(constraintB.AreRequiredLinksSelected);
            foreach (Link<PossibleSquareValue, Requirement> link in constraintA.GetLinks())
            {
                if (link != linkA)
                {
                    Assert.Equal(PossibilityState.UNKNOWN, link.Possibility.State);
                }
            }
            foreach (Link<PossibleSquareValue, Requirement> link in constraintB.GetLinks())
            {
                if (link != linkB)
                {
                    Assert.Equal(PossibilityState.UNKNOWN, link.Possibility.State);
                }
            }
        }

        [Fact]
        public void GetMinUnselectedCountFromRequirements()
        {
            var puzzle = new Puzzle(new int?[,] {
                {   1, null, null,    2},
                {null, null,    1, null},
                {null,    1, null, null},
                {   3, null,    4, null}
            });
            var matrix = new ExactCoverMatrix(puzzle);
            new RowUniquenessConstraint().TryConstrain(puzzle, matrix);
            new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix);

            Assert.Equal(2, matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(2).GetMinUnselectedCountFromRequirements());
            Assert.Equal(2, matrix.GetSquare(new Coordinate(2, 3)).GetPossibleValue(3).GetMinUnselectedCountFromRequirements());
        }
    }
}