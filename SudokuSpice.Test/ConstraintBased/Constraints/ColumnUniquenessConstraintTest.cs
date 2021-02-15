using SudokuSpice.ConstraintBased.Test;
using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    public class ColumnUniquenessConstraintTest
    {
        [Fact]
        public void TryConstrain_WithNonUniqueValuesInColumns_Fails()
        {
            var puzzle = new Puzzle(new int?[,] {
                { null, 1,    1,    1 },
                { null, 2, null, null },
                { null, 3,    1, null },
                { null, 4, null, null },
            });
            var matrix = ExactCoverGraph.Create(puzzle);
            var constraint = new ColumnUniquenessConstraint();

            Assert.False(constraint.TryConstrain(puzzle, matrix));
        }

        [Fact]
        public void TryConstrain_WithUniqueValuesInColumns_Succeeds()
        {
            var puzzle = new Puzzle(new int?[,] {
                { null, null, null,    1 },
                { null, null, null,    2 },
                { null, null,    3,    3 },
                { null,    4,    4,    4 },
            });
            var matrix = ExactCoverGraph.Create(puzzle);
            var constraint = new ColumnUniquenessConstraint();

            Assert.True(constraint.TryConstrain(puzzle, matrix));

            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(0, 0), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(1, 0), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(2, 0), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(3, 0), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(0, 1), new int[] { 1, 2, 3 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(1, 1), new int[] { 1, 2, 3 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(2, 1), new int[] { 1, 2, 3 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(0, 2), new int[] { 1, 2 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(1, 2), new int[] { 1, 2 }, matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(3, 1), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(2, 2), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(3, 2), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(0, 3), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(1, 3), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(2, 3), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(3, 3), matrix);
        }
    }
}
