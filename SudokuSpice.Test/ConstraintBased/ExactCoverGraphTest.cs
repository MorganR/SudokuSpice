using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    public class ExactCoverGraphTest
    {
        [Fact]
        public void Create_ConfiguresSquareObjectives()
        {
            var puzzle = new Puzzle(4);
            var graph = ExactCoverGraph.Create(puzzle);

            var objectives = graph.GetUnsatisfiedRequiredObjectives();
            Assert.Equal(puzzle.Size * puzzle.Size, objectives.Count());

            var seenCoordinates = new HashSet<Coordinate>();
            var possibilityIndices = new HashSet<int>() { 0, 1, 2, 3 };
            Assert.All(objectives,
                concreteObjective =>
                {
                    IObjective objective = concreteObjective;
                    var possibilities = objective.GetUnknownDirectPossibilities().Cast<Possibility>().ToArray();
                    // Assert that each square links every possibility at that coordinate.
                    Assert.Equal(puzzle.Size, possibilities.Length);
                    Assert.Equal(possibilityIndices, new HashSet<int>(possibilities.Select(p => p.Index)));
                    var firstCoord = possibilities.First().Coordinate;
                    Assert.All(possibilities,
                        p =>
                        {
                            Assert.Equal(firstCoord, p.Coordinate);
                            Assert.Equal(NodeState.UNKNOWN, p.State);
                        });
                    // Assert an objective is made for each square.
                    Assert.DoesNotContain(firstCoord, seenCoordinates);
                    seenCoordinates.Add(firstCoord);
                });
        }

        [Fact]
        public void GetAllPossibilitiesAt_ForUnsetCoordinate_ReturnsExpectedPossibilities()
        {
            var puzzle = new Puzzle(4);
            ExactCoverGraph graph = ExactCoverGraph.Create(puzzle);

            var possibilities = graph.GetAllPossibilitiesAt(new Coordinate());

            Assert.NotNull(possibilities);
            Assert.Equal(puzzle.Size, possibilities!.Length);
            for (int i = 0; i < puzzle.Size; ++i)
            {
                Assert.NotNull(possibilities[i]);
                Assert.Equal(i, possibilities[i]!.Index);
            }
        }

        [Fact]
        public void GetAllPossibilitiesAt_ForPresetCoordinate_ReturnsNull()
        {
            var puzzle = new Puzzle(4);
            var coord = new Coordinate();
            puzzle[in coord] = 1;
            ExactCoverGraph graph = ExactCoverGraph.Create(puzzle);

            Assert.Null(graph.GetAllPossibilitiesAt(in coord));
        }
    }
}