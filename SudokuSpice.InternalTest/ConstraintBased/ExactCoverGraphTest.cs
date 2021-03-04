using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.InternalTest
{
    public class ExactCoverGraphTest
    {
        [Fact]
        public void GetPossibilitiesOnRow_ReturnsExpectedSquares()
        {
            var puzzle = new Puzzle(4);
            var graph = ExactCoverGraph.Create(puzzle);

            int rowIndex = 1;
            var row = graph.GetPossibilitiesOnRow(rowIndex);
            Assert.Equal(4, row.Length);
            Assert.Equal(new Coordinate(rowIndex, 0), row[0]![0]!.Coordinate);
            Assert.Equal(new Coordinate(rowIndex, 1), row[1]![0]!.Coordinate);
            Assert.Equal(new Coordinate(rowIndex, 2), row[2]![0]!.Coordinate);
            Assert.Equal(new Coordinate(rowIndex, 3), row[3]![0]!.Coordinate);
        }

        [Fact]
        public void SelectSquareValue_DropsOtherPossibilitiesForSquareOnly()
        {
            var puzzle = new Puzzle(4);
            var graph = ExactCoverGraph.Create(puzzle);

            var initialObjectivesCount = graph.GetUnsatisfiedRequiredObjectives().Count();
            var seenCoordinates = new HashSet<Coordinate>();
            var possibilityIndices = new HashSet<int>() { 0, 1, 2, 3 };
            var coordToSet = new Coordinate(1, 1);
            int indexToSet = 1;
            var possibilitiesAtCoord = graph.GetAllPossibilitiesAt(coordToSet)!;

            Assert.True(possibilitiesAtCoord[indexToSet]!.TrySelect());

            Assert.Equal(initialObjectivesCount - 1, graph.GetUnsatisfiedRequiredObjectives().Count());
            for (int i = 0; i < puzzle.Size; ++i)
            {
                if (i == indexToSet)
                {
                    Assert.Equal(NodeState.SELECTED, possibilitiesAtCoord[i]!.State);
                } else
                {
                    Assert.Equal(NodeState.DROPPED, possibilitiesAtCoord[i]!.State);
                }
            }
            for (int row = 0; row < puzzle.Size; ++row)
            {
                for (int col = 0; col < puzzle.Size; ++col)
                {
                    var coord = new Coordinate(row, col);
                    if (coord == coordToSet)
                    {
                        continue;
                    }
                    var possibilities = graph.GetAllPossibilitiesAt(in coord);
                    Assert.All(possibilities, p => Assert.Equal(NodeState.UNKNOWN, p!.State));
                }
            }
        }

        [Fact]
        public void CopyUnknowns_KeepsUnknownsAndDropsOthers()
        {
            var puzzle = new Puzzle(4);
            var graph = ExactCoverGraph.Create(puzzle);

            int initialObjectivesCount = graph.GetUnsatisfiedRequiredObjectives().Count();
            var coordToSelect = new Coordinate(1, 1);
            int indexToDrop = 0;
            var possibilitiesToAlter = graph.GetAllPossibilitiesAt(in coordToSelect);
            Assert.True(possibilitiesToAlter![indexToDrop]!.TrySelect());
            Assert.All(possibilitiesToAlter, p => Assert.NotEqual(NodeState.UNKNOWN, p!.State));
            Assert.Equal(initialObjectivesCount - 1, graph.GetUnsatisfiedRequiredObjectives().Count());

            var copy = graph.CopyUnknowns();
            var possibilitiesToBeNull = copy.GetAllPossibilitiesAt(in coordToSelect);
            Assert.Null(possibilitiesToBeNull);
            for (int rowIndex = 0; rowIndex < puzzle.Size; ++rowIndex)
            {
                for (int colIndex = 0; colIndex < puzzle.Size; ++colIndex)
                {
                    var coord = new Coordinate(rowIndex, colIndex);
                    if (coord == coordToSelect)
                    {
                        continue;
                    }
                    var possibilities = copy.GetAllPossibilitiesAt(in coord);
                    Assert.NotNull(possibilities);
                    Assert.All(possibilities, p =>
                    {
                        Assert.NotNull(p);
                        Assert.Equal(coord, p!.Coordinate);
                        Assert.Equal(NodeState.UNKNOWN, p!.State);
                    });
                }
            }
            Assert.Equal(initialObjectivesCount - 1, copy.GetUnsatisfiedRequiredObjectives().Count());
        }
    }
}