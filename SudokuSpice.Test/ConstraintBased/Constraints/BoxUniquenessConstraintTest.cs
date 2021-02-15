using SudokuSpice.ConstraintBased.Test;
using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    public class BoxUniquenessConstraintTest
    {
        [Fact]
        public void TryConstrain_WithNonUniqueValuesInBoxes_Fails()
        {
            var puzzle = new Puzzle(new int?[,] {
                { null, null,    1,    1 },
                { null, null, null, null },
                { null,    3,    1,    2 },
                { null,    4,    3,    4 },
            });
            var matrix = ExactCoverGraph.Create(puzzle);
            var constraint = new BoxUniquenessConstraint();

            Assert.False(constraint.TryConstrain(puzzle, matrix));
        }

        [Fact]
        public void TryConstrain_WithUniqueValuesInBoxes_Succeeds()
        {
            var puzzle = new Puzzle(new int?[,] {
                { null, null,    1, null },
                { null, null, null, null },
                { null,    3,    1,    2 },
                { null,    4,    3,    4 },
            });
            var matrix = ExactCoverGraph.Create(puzzle);
            var constraint = new BoxUniquenessConstraint();

            Assert.True(constraint.TryConstrain(puzzle, matrix));

            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(0, 0), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(0, 1), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(1, 0), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(1, 1), new int[] { 1, 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(0, 3), new int[] { 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(1, 2), new int[] { 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(1, 3), new int[] { 2, 3, 4 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(2, 0), new int[] { 1, 2 }, matrix);
            ExactCoverMatrices.AssertPossibleValuesAtSquare(new(3, 0), new int[] { 1, 2 }, matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(0, 2), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(2, 1), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(2, 2), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(2, 3), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(3, 1), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(3, 2), matrix);
            ExactCoverMatrices.AssertNoPossibleValuesAtSquare(new(3, 3), matrix);
        }
    }
}
