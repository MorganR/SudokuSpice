using SudokuSpice.ConstraintBased.Test;
using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    public class RowUniquenessConstraintTest
    {
        [Fact]
        public void TryConstrain_WithNonUniqueValuesInRows_Fails()
        {
            var puzzle = new Puzzle(new int?[,] {
                { null, 1, null, null },
                { null, 2, null,    2 },
                { null, 3, null, null },
                { null, 4, null, null },
            });
            var graph = ExactCoverGraph.Create(puzzle);
            var constraint = new RowUniquenessConstraint();

            Assert.False(constraint.TryConstrain(puzzle, graph));
        }

        [Fact]
        public void TryConstrain_WithUniqueValuesInRows_Succeeds()
        {
            var puzzle = new Puzzle(new int?[,] {
                { null, null, null, null },
                { null, null, null,    4 },
                { null, null,    3,    4 },
                {    1,    2,    3,    4 },
            });
            var graph = ExactCoverGraph.Create(puzzle);
            var constraint = new RowUniquenessConstraint();

            Assert.True(constraint.TryConstrain(puzzle, graph));

            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(0, 0), new int[] { 1, 2, 3, 4 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(0, 1), new int[] { 1, 2, 3, 4 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(0, 2), new int[] { 1, 2, 3, 4 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(0, 3), new int[] { 1, 2, 3, 4 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(1, 0), new int[] { 1, 2, 3 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(1, 1), new int[] { 1, 2, 3 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(1, 2), new int[] { 1, 2, 3 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(2, 0), new int[] { 1, 2 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(2, 1), new int[] { 1, 2 }, graph);
            ExactCoverGraphs.AssertNoPossibleValuesAtSquare(new(1, 3), graph);
            ExactCoverGraphs.AssertNoPossibleValuesAtSquare(new(2, 2), graph);
            ExactCoverGraphs.AssertNoPossibleValuesAtSquare(new(2, 3), graph);
            ExactCoverGraphs.AssertNoPossibleValuesAtSquare(new(3, 0), graph);
            ExactCoverGraphs.AssertNoPossibleValuesAtSquare(new(3, 1), graph);
            ExactCoverGraphs.AssertNoPossibleValuesAtSquare(new(3, 2), graph);
            ExactCoverGraphs.AssertNoPossibleValuesAtSquare(new(3, 3), graph);
        }
    }
}
