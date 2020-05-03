using SudokuSpice.Constraints;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace SudokuSpice.Data.Test
{
    public class PossibleSquareValueTest
    {
        [Fact]
        public void TryDrop_DropsOnSuccess()
        {
            var matrix = new ExactCoverMatrix(4, new int[] {1, 3, 4, 5});
            var puzzle = new Puzzle(4);
            new RowUniquenessConstraint().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint().Constrain(puzzle, matrix);
            var rowConstraints = matrix.ConstraintHeaders.Take(4 * 4).ToList();
            var columnConstraints = matrix.ConstraintHeaders.Skip(4 * 4).Take(4 * 4).ToList();

            int valueIndex = 1;
            var square = matrix.GetSquare(new Coordinate(1, 0));
            var possibleValue = square.GetPossibleValue(valueIndex);

            Assert.True(possibleValue.TryDrop());
            Assert.Equal(3, square.NumPossibleValues);
            Assert.Equal(PossibleSquareState.DROPPED, possibleValue.State);
            var rowConstraint = rowConstraints[5];
            var columnConstraint = columnConstraints[1];
            Assert.Equal(3, rowConstraint.Count);
            Assert.Equal(3, columnConstraint.Count);
            var rowLink = possibleValue.FirstLink;
            var columnLink = rowLink.Right;
            Assert.DoesNotContain(rowLink, rowConstraint.GetLinks());
            Assert.DoesNotContain(columnLink, columnConstraint.GetLinks());
        }

        [Fact]
        public void TryDrop_LeavesUnchangedOnFailure()
        {
            var matrix = new ExactCoverMatrix(4, new int[] {1, 3, 4, 5});
            var puzzle = new Puzzle(4);
            new RowUniquenessConstraint().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint().Constrain(puzzle, matrix);
            var rowConstraints = matrix.ConstraintHeaders.Take(4 * 4).ToList();
            var columnConstraints = matrix.ConstraintHeaders.Skip(4 * 4).Take(4 * 4).ToList();

            var valueIndex = 0;
            Assert.True(matrix.GetSquare(new Coordinate(0, 0)).GetPossibleValue(valueIndex).TryDrop());
            Assert.True(matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(valueIndex).TryDrop());
            Assert.True(matrix.GetSquare(new Coordinate(0, 2)).GetPossibleValue(valueIndex).TryDrop());
            var square = matrix.GetSquare(new Coordinate(0, 3));
            var possibleValue = square.GetPossibleValue(valueIndex);
            Assert.False(possibleValue.TryDrop());

            Assert.Equal(4, square.NumPossibleValues);
            Assert.Equal(PossibleSquareState.UNKNOWN, possibleValue.State);
            var rowConstraint = rowConstraints[0];
            var columnConstraint = columnConstraints[12];
            Assert.Equal(1, rowConstraint.Count);
            Assert.Equal(4, columnConstraint.Count);
            Assert.Same(possibleValue.FirstLink, rowConstraint.FirstLink);
            var columnLink = possibleValue.FirstLink.Right;
            Assert.Contains(columnLink, columnConstraint.GetLinks());
        }

        [Fact]
        public void TryDrop_WithSatisfiedConstraint_LeavesSatisfiedConstraintUnchanged()
        {
            var matrix = new ExactCoverMatrix(4, new int[] {1, 3, 4, 5});
            var puzzle = new Puzzle(4);
            new RowUniquenessConstraint().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint().Constrain(puzzle, matrix);
            var rowConstraints = matrix.ConstraintHeaders.Take(4 * 4).ToList();
            var columnConstraints = matrix.ConstraintHeaders.Skip(4 * 4).Take(4 * 4).ToList();
            var rowConstraint = rowConstraints[5];
            var columnConstraint = columnConstraints[1];
            columnConstraint.IsSatisfied = true;

            var valueIndex = 1;
            var square = matrix.GetSquare(new Coordinate(1, 0));
            var possibleValue = square.GetPossibleValue(valueIndex);
            Assert.True(possibleValue.TryDrop());
          
            Assert.Equal(3, rowConstraint.Count);
            Assert.Equal(4, columnConstraint.Count);
            Assert.Equal(4, columnConstraint.GetLinks().Count);
            var rowLink = possibleValue.FirstLink;
            var columnLink = rowLink.Right;
            Assert.Contains(columnLink, columnConstraint.GetLinks());
        }

        [Fact]
        public void Return_UndoesDrop()
        {
            var matrix = new ExactCoverMatrix(4, new int[] {1, 3, 4, 5});
            var puzzle = new Puzzle(4);
            new RowUniquenessConstraint().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint().Constrain(puzzle, matrix);
            var rowConstraints = matrix.ConstraintHeaders.Take(4 * 4).ToList();
            var columnConstraints = matrix.ConstraintHeaders.Skip(4 * 4).Take(4 * 4).ToList();

            var valueIndex = 0;
            var square = matrix.GetSquare(new Coordinate(0, 0));
            var possibleValue = square.GetPossibleValue(valueIndex);
            Assert.True(possibleValue.TryDrop());
            possibleValue.Return();

            Assert.Equal(4, square.NumPossibleValues);
            Assert.Equal(PossibleSquareState.UNKNOWN, possibleValue.State);
            var rowConstraint = rowConstraints[0];
            var columnConstraint = columnConstraints[0];
            Assert.Equal(4, rowConstraint.Count);
            Assert.Equal(4, columnConstraint.Count);
            var rowLink = possibleValue.FirstLink;
            var columnLink = rowLink.Right;
            Assert.Contains(rowLink, rowConstraint.GetLinks());
            Assert.Contains(columnLink, columnConstraint.GetLinks());
        }

        [Fact]
        public void Return_WithSatisfiedConstraint_LeavesSatisfiedConstraintUnchanged()
        {
            var matrix = new ExactCoverMatrix(4, new int[] {1, 3, 4, 5});
            var puzzle = new Puzzle(4);
            new RowUniquenessConstraint().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint().Constrain(puzzle, matrix);
            var rowConstraints = matrix.ConstraintHeaders.Take(4 * 4).ToList();
            var columnConstraints = matrix.ConstraintHeaders.Skip(4 * 4).Take(4 * 4).ToList();

            var valueIndex = 0;
            var rowConstraint = rowConstraints[0];
            var columnConstraint = columnConstraints[0];
            var square = matrix.GetSquare(new Coordinate(0, 0));
            var possibleValue = square.GetPossibleValue(valueIndex);
            columnConstraint.IsSatisfied = true;
            Assert.True(possibleValue.TryDrop());
            possibleValue.Return();

            Assert.Equal(4, square.NumPossibleValues);
            Assert.Equal(PossibleSquareState.UNKNOWN, possibleValue.State);
            
            Assert.Equal(4, rowConstraint.Count);
            Assert.Equal(4, columnConstraint.Count);
            var rowLink = possibleValue.FirstLink;
            var columnLink = rowLink.Right;
            Assert.Contains(rowLink, rowConstraint.GetLinks());
            Assert.Contains(columnLink, columnConstraint.GetLinks());
        }

        [Fact]
        public void TrySelect_SelectsSquare()
        {
            var matrix = new ExactCoverMatrix(4, new int[] {1, 3, 4, 5});
            var puzzle = new Puzzle(4);
            new RowUniquenessConstraint().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint().Constrain(puzzle, matrix);

            var valueIndex = 0;
            var square = matrix.GetSquare(new Coordinate(0, 1));
            var possibleValue = square.GetPossibleValue(valueIndex);
            Assert.True(possibleValue.TrySelect());

            Assert.Equal(PossibleSquareState.SELECTED, possibleValue.State);
        }

        [Fact]
        public void TrySelect_SatisfiesConstraints()
        {
            var matrix = new ExactCoverMatrix(4, new int[] {1, 3, 4, 5});
            var puzzle = new Puzzle(4);
            new RowUniquenessConstraint().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint().Constrain(puzzle, matrix);
            var rowConstraints = matrix.ConstraintHeaders.Take(4 * 4).ToList();
            var columnConstraints = matrix.ConstraintHeaders.Skip(4 * 4).Take(4 * 4).ToList();

            var valueIndex = 0;
            var possibleValue = matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(valueIndex);
            Assert.True(possibleValue.TrySelect());

            var rowConstraint = rowConstraints[0];
            var columnConstraint = columnConstraints[4];
            Assert.True(rowConstraint.IsSatisfied);
            Assert.True(columnConstraint.IsSatisfied);
            Assert.Equal(4, rowConstraint.Count);
            Assert.Equal(4, columnConstraint.Count);
        }

        [Fact]
        public void TrySelect_DropsConstraintConnectedSquareValues()
        {
            var matrix = new ExactCoverMatrix(4, new int[] {1, 3, 4, 5});
            var puzzle = new Puzzle(4);
            new RowUniquenessConstraint().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint().Constrain(puzzle, matrix);
            var rowConstraints = matrix.ConstraintHeaders.Take(4 * 4).ToList();
            var columnConstraints = matrix.ConstraintHeaders.Skip(4 * 4).Take(4 * 4).ToList();

            var valueIndex = 0;
            var possibleValue = matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(valueIndex);
            Assert.True(possibleValue.TrySelect());

            var rowConstraint = rowConstraints[0];
            var columnConstraint = columnConstraints[4];
            Assert.Equal(4, rowConstraint.GetLinks().Count);
            Assert.Equal(4, columnConstraint.GetLinks().Count);
            foreach (var link in rowConstraint.GetLinks())
            {
                if (ReferenceEquals(link.PossibleSquare, possibleValue))
                {
                    continue;
                }
                Assert.Equal(PossibleSquareState.DROPPED, link.PossibleSquare.State);
            }
            foreach (var link in columnConstraint.GetLinks())
            {
                if (ReferenceEquals(link.PossibleSquare, possibleValue))
                {
                    continue;
                }
                Assert.Equal(PossibleSquareState.DROPPED, link.PossibleSquare.State);
            }
        }

        [Fact]
        public void Deselect_WithSelectedValue_SetsStateAndSquareCountCorrectly()
        {
            // TODO: Complete this
        }

        [Fact]
        public void Deselect_WithSelectedValue_ReturnsDroppedRows()
        {
            // TODO: Complete this
        }
    }
}
