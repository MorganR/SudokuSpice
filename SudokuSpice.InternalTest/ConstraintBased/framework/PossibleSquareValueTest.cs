﻿using SudokuSpice.ConstraintBased.Constraints;
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
            var matrix = new ExactCoverMatrix<Puzzle>(puzzle);
            new RowUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);

            int valueIndex = 1;
            Square<Puzzle> square = matrix.GetSquare(new Coordinate(1, 0));
            PossibleSquareValue<Puzzle> possibleValue = square.GetPossibleValue(valueIndex);
            SquareLink<Puzzle> linkA = possibleValue.FirstLink;
            SquareLink<Puzzle> linkB = linkA.Right;
            ConstraintHeader<Puzzle> constraintA = linkA.Constraint;
            ConstraintHeader<Puzzle> constraintB = linkB.Constraint;

            Assert.True(possibleValue.TryDrop());
            Assert.Equal(3, square.NumPossibleValues);
            Assert.Equal(PossibleSquareState.DROPPED, possibleValue.State);
            Assert.Equal(3, constraintA.Count);
            Assert.Equal(3, constraintB.Count);
            Assert.DoesNotContain(linkA, constraintA.GetLinks());
            Assert.DoesNotContain(linkB, constraintB.GetLinks());
        }

        [Fact]
        public void TryDrop_LeavesUnchangedOnFailure()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix<Puzzle>(puzzle);
            new RowUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);

            int valueIndex = 0;
            Assert.True(matrix.GetSquare(new Coordinate(0, 0)).GetPossibleValue(valueIndex).TryDrop());
            Assert.True(matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(valueIndex).TryDrop());
            Assert.True(matrix.GetSquare(new Coordinate(0, 2)).GetPossibleValue(valueIndex).TryDrop());
            Square<Puzzle> square = matrix.GetSquare(new Coordinate(0, 3));
            PossibleSquareValue<Puzzle> possibleValue = square.GetPossibleValue(valueIndex);
            SquareLink<Puzzle> linkA = possibleValue.FirstLink;
            SquareLink<Puzzle> linkB = linkA.Right;
            ConstraintHeader<Puzzle> constraintA = linkA.Constraint;
            ConstraintHeader<Puzzle> constraintB = linkB.Constraint;
            Assert.False(possibleValue.TryDrop());

            Assert.Equal(4, square.NumPossibleValues);
            Assert.Equal(PossibleSquareState.UNKNOWN, possibleValue.State);
            Assert.Equal(1, constraintA.Count);
            Assert.Equal(4, constraintB.Count);
            Assert.Same(linkA, constraintA.FirstLink);
            Assert.Contains(linkB, constraintB.GetLinks());
        }

        [Fact]
        public void Return_UndoesDrop()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix<Puzzle>(puzzle);
            new RowUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);

            int valueIndex = 0;
            Square<Puzzle> square = matrix.GetSquare(new Coordinate(0, 0));
            PossibleSquareValue<Puzzle> possibleValue = square.GetPossibleValue(valueIndex);
            SquareLink<Puzzle> linkA = possibleValue.FirstLink;
            SquareLink<Puzzle> linkB = linkA.Right;
            Assert.True(possibleValue.TryDrop());
            possibleValue.Return();

            Assert.Equal(4, square.NumPossibleValues);
            Assert.Equal(PossibleSquareState.UNKNOWN, possibleValue.State);
            ConstraintHeader<Puzzle> constraintA = linkA.Constraint;
            ConstraintHeader<Puzzle> constraintB = linkB.Constraint;
            Assert.Equal(4, constraintA.Count);
            Assert.Equal(4, constraintB.Count);
            Assert.Contains(linkA, constraintA.GetLinks());
            Assert.Contains(linkB, constraintB.GetLinks());
        }

        [Fact]
        public void Return_WithSatisfiedConstraint_UndropsTheRowAsExpected()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix<Puzzle>(puzzle);
            new RowUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);

            int valueIndex = 0;
            Square<Puzzle> square = matrix.GetSquare(new Coordinate(0, 0));
            PossibleSquareValue<Puzzle> possibleValue = square.GetPossibleValue(valueIndex);
            SquareLink<Puzzle> linkA = possibleValue.FirstLink;
            SquareLink<Puzzle> linkB = linkA.Right;
            ConstraintHeader<Puzzle> constraintA = linkA.Constraint;
            ConstraintHeader<Puzzle> constraintB = linkB.Constraint;
            Assert.True(constraintB.TrySatisfyFrom(possibleValue.FirstLink.Right.Up));
            possibleValue.Return();

            Assert.Equal(4, square.NumPossibleValues);
            Assert.Equal(PossibleSquareState.UNKNOWN, possibleValue.State);
            Assert.Equal(4, constraintA.Count);
            Assert.Equal(4, constraintB.Count);
            Assert.Contains(linkA, constraintA.GetLinks());
            Assert.Contains(linkB, constraintB.GetLinks());
        }

        [Fact]
        public void TrySelect_SelectsSquare()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix<Puzzle>(puzzle);
            new RowUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);

            int valueIndex = 0;
            Square<Puzzle> square = matrix.GetSquare(new Coordinate(0, 1));
            PossibleSquareValue<Puzzle> possibleValue = square.GetPossibleValue(valueIndex);
            Assert.True(possibleValue.TrySelect());

            Assert.Equal(PossibleSquareState.SELECTED, possibleValue.State);
        }

        [Fact]
        public void TrySelect_SatisfiesConstraints()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix<Puzzle>(puzzle);
            new RowUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);

            int valueIndex = 0;
            PossibleSquareValue<Puzzle> possibleValue = matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(valueIndex);
            Assert.True(possibleValue.TrySelect());

            ConstraintHeader<Puzzle> constraintA = possibleValue.FirstLink.Constraint;
            ConstraintHeader<Puzzle> constraintB = possibleValue.FirstLink.Right.Constraint;
            Assert.True(constraintA.IsSatisfied);
            Assert.True(constraintB.IsSatisfied);
            Assert.Equal(4, constraintA.Count);
            Assert.Equal(4, constraintB.Count);
            Assert.DoesNotContain(constraintA, matrix.GetUnsatisfiedConstraintHeaders());
            Assert.DoesNotContain(constraintB, matrix.GetUnsatisfiedConstraintHeaders());
        }

        [Fact]
        public void TrySelect_DropsConstraintConnectedSquareValues()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix<Puzzle>(puzzle);
            new RowUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);

            int valueIndex = 0;
            PossibleSquareValue<Puzzle> possibleValue = matrix.GetSquare(new Coordinate(0, 1)).GetPossibleValue(valueIndex);
            Assert.True(possibleValue.TrySelect());

            ConstraintHeader<Puzzle> constraintA = possibleValue.FirstLink.Constraint;
            ConstraintHeader<Puzzle> constraintB = possibleValue.FirstLink.Right.Constraint;
            Assert.Equal(4, constraintA.GetLinks().Count());
            Assert.Equal(4, constraintB.GetLinks().Count());
            foreach (SquareLink<Puzzle> link in constraintA.GetLinks())
            {
                if (link.PossibleSquare == possibleValue)
                {
                    continue;
                }
                Assert.Equal(PossibleSquareState.DROPPED, link.PossibleSquare.State);
            }
            foreach (SquareLink<Puzzle> link in constraintB.GetLinks())
            {
                if (link.PossibleSquare == possibleValue)
                {
                    continue;
                }
                Assert.Equal(PossibleSquareState.DROPPED, link.PossibleSquare.State);
            }
        }

        [Fact]
        public void Deselect_WithSelectedValue_SetsStateAndSquareCountCorrectly()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix<Puzzle>(puzzle);
            new RowUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);

            int valueIndex = 0;
            Square<Puzzle> square = matrix.GetSquare(new Coordinate(0, 1));
            PossibleSquareValue<Puzzle> possibleValue = square.GetPossibleValue(valueIndex);
            Assert.True(possibleValue.TrySelect());
            possibleValue.Deselect();

            Assert.Equal(PossibleSquareState.UNKNOWN, possibleValue.State);
            Assert.Equal(4, square.NumPossibleValues);
        }

        [Fact]
        public void Deselect_WithSelectedValue_ReturnsDroppedRows()
        {
            var puzzle = new Puzzle(4);
            var matrix = new ExactCoverMatrix<Puzzle>(puzzle);
            new RowUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);
            new ColumnUniquenessConstraint<Puzzle>().Constrain(puzzle, matrix);

            int valueIndex = 0;
            Square<Puzzle> square = matrix.GetSquare(new Coordinate(0, 1));
            PossibleSquareValue<Puzzle> possibleValue = square.GetPossibleValue(valueIndex);
            Assert.True(possibleValue.TrySelect());
            possibleValue.Deselect();

            SquareLink<Puzzle> linkA = possibleValue.FirstLink;
            SquareLink<Puzzle> linkB = linkA.Right;
            ConstraintHeader<Puzzle> constraintA = linkA.Constraint;
            ConstraintHeader<Puzzle> constraintB = linkB.Constraint;
            Assert.False(constraintA.IsSatisfied);
            Assert.False(constraintB.IsSatisfied);
            Assert.Equal(4, constraintA.Count);
            Assert.Equal(4, constraintB.Count);
            foreach (SquareLink<Puzzle> link in constraintA.GetLinks())
            {
                if (link != linkA)
                {
                    Assert.Equal(PossibleSquareState.UNKNOWN, link.PossibleSquare.State);
                }
            }
            foreach (SquareLink<Puzzle> link in constraintB.GetLinks())
            {
                if (link != linkB)
                {
                    Assert.Equal(PossibleSquareState.UNKNOWN, link.PossibleSquare.State);
                }
            }
        }
    }
}
