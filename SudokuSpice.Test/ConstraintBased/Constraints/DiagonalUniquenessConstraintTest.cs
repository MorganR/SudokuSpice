﻿using SudokuSpice.ConstraintBased.Test;
using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    public class DiagonalUniquenessConstraintTest
    {
        [Fact]
        public void TryConstrain_WithNonUniqueValuesInDiagonals_Fails()
        {
            var puzzle = new Puzzle(new int?[,] {
                {    1, null, null, null },
                { null, null, null, null },
                { null,    3,    1, null },
                { null, null, null, null },
            });
            var matrix = ExactCoverGraph.Create(puzzle);
            var constraint = new DiagonalUniquenessConstraint();

            Assert.False(constraint.TryConstrain(puzzle, matrix));
        }

        [Fact]
        public void TryConstrain_WithUniqueValuesInDiagonals_Succeeds()
        {
            var puzzle = new Puzzle(new int?[,] {
                {    1, null, null, null },
                { null, null,    2, null },
                { null,    3,    3, null },
                {    4, null, null, null },
            });
            var matrix = ExactCoverGraph.Create(puzzle);
            var constraint = new DiagonalUniquenessConstraint();

            Assert.True(constraint.TryConstrain(puzzle, matrix));

            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(0, 1), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(0, 2), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(1, 0), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(1, 3), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(2, 0), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(2, 3), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(3, 1), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(3, 2), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(1, 1), new int[] { 2, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(3, 3), new int[] { 2, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(0, 3), new int[] { 1 }, matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(0, 0), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(2, 2), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(1, 2), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(2, 1), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(3, 0), matrix);
        }
    }
}
