using SudokuSpice.ConstraintBased.Test;
using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    public class ColumnUniquenessConstraintTest
    {
        [Fact]
        public void TryConstrain_WithNonUniqueValuesInColumns_Fails()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] { null, 1,    1,    1 },
                new int?[] { null, 2, null, null },
                new int?[] { null, 3,    1, null },
                new int?[] { null, 4, null, null },
            });
            var graph = ExactCoverGraph.Create(puzzle);
            var constraint = new ColumnUniquenessConstraint();

            Assert.False(constraint.TryConstrain(puzzle, graph));
        }

        [Fact]
        public void TryConstrain_WithUniqueValuesInColumns_Succeeds()
        {
            var puzzle = new Puzzle(new int?[][] {
                new int?[] { null, null, null,    1 },
                new int?[] { null, null, null,    2 },
                new int?[] { null, null,    3,    3 },
                new int?[] { null,    4,    4,    4 },
            });
            var graph = ExactCoverGraph.Create(puzzle);
            var constraint = new ColumnUniquenessConstraint();

            Assert.True(constraint.TryConstrain(puzzle, graph));

            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(0, 0), new int[] { 1, 2, 3, 4 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(1, 0), new int[] { 1, 2, 3, 4 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(2, 0), new int[] { 1, 2, 3, 4 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(3, 0), new int[] { 1, 2, 3, 4 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(0, 1), new int[] { 1, 2, 3 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(1, 1), new int[] { 1, 2, 3 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(2, 1), new int[] { 1, 2, 3 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(0, 2), new int[] { 1, 2 }, graph);
            ExactCoverGraphs.AssertPossibleValuesAtSquare(new(1, 2), new int[] { 1, 2 }, graph);
            ExactCoverGraphs.AssertNoPossibleValuesAtSquare(new(3, 1), graph);
            ExactCoverGraphs.AssertNoPossibleValuesAtSquare(new(2, 2), graph);
            ExactCoverGraphs.AssertNoPossibleValuesAtSquare(new(3, 2), graph);
            ExactCoverGraphs.AssertNoPossibleValuesAtSquare(new(0, 3), graph);
            ExactCoverGraphs.AssertNoPossibleValuesAtSquare(new(1, 3), graph);
            ExactCoverGraphs.AssertNoPossibleValuesAtSquare(new(2, 3), graph);
            ExactCoverGraphs.AssertNoPossibleValuesAtSquare(new(3, 3), graph);
        }
    }
}