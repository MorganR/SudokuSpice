using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.InternalTest
{
    public class ExactCoverMatrixTest
    {
        [Fact]
        public void GetPossibilitiesOnRow_ReturnsExpectedSquares()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverMatrix.Create(puzzle);

            int rowIndex = 1;
            var row = matrix.GetPossibilitiesOnRow(rowIndex);
            Assert.Equal(4, row.Length);
            Assert.Equal(new Coordinate(rowIndex, 0), row[0]![0]!.Coordinate);
            Assert.Equal(new Coordinate(rowIndex, 1), row[1]![0]!.Coordinate);
            Assert.Equal(new Coordinate(rowIndex, 2), row[2]![0]!.Coordinate);
            Assert.Equal(new Coordinate(rowIndex, 3), row[3]![0]!.Coordinate);
        }

        [Fact]
        // TODO: Move to public tests
        public void Create_ConfiguresSquareObjectives()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverMatrix.Create(puzzle);

            var objectives = matrix.GetUnsatisfiedRequiredObjectives();
            Assert.Equal(puzzle.Size * puzzle.Size, objectives.Count());
            var seenCoordinates = new HashSet<Coordinate>();
            var possibilityIndices = new HashSet<int>() { 0, 1, 2, 3 };
            Assert.All(objectives,
                concreteObjective =>
                {
                    IObjective objective = concreteObjective;
                    var possibilities = objective.GetUnknownDirectPossibilities().Cast<Possibility>().ToArray();
                    Assert.Equal(puzzle.Size, possibilities.Length);
                    Assert.Equal(possibilityIndices, new HashSet<int>(possibilities.Select(p => p.Index)));
                    var firstCoord = possibilities.First().Coordinate;
                    Assert.All(possibilities,
                        p =>
                        {
                            Assert.Equal(firstCoord, p.Coordinate);
                            Assert.Equal(NodeState.UNKNOWN, p.State);
                        });
                    Assert.DoesNotContain(firstCoord, seenCoordinates);
                    seenCoordinates.Add(firstCoord);
                });
        }

        [Fact]
        // TODO: Move to public tests
        public void SelectSquareValue_DropsOtherPossibilitiesForSquareOnly()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverMatrix.Create(puzzle);

            var initialObjectivesCount = matrix.GetUnsatisfiedRequiredObjectives().Count();
            var seenCoordinates = new HashSet<Coordinate>();
            var possibilityIndices = new HashSet<int>() { 0, 1, 2, 3 };
            var coordToSet = new Coordinate(1, 1);
            int indexToSet = 1;
            var possibilitiesAtCoord = matrix.GetAllPossibilitiesAt(coordToSet)!;

            Assert.True(possibilitiesAtCoord[indexToSet]!.TrySelect());

            Assert.Equal(initialObjectivesCount - 1, matrix.GetUnsatisfiedRequiredObjectives().Count());
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
                    var possibilities = matrix.GetAllPossibilitiesAt(in coord);
                    Assert.All(possibilities, p => Assert.Equal(NodeState.UNKNOWN, p!.State));
                }
            }
        }




        [Fact]
        public void CopyUnknowns_KeepsUnknownsAndDropsOthers()
        {
            var puzzle = new Puzzle(4);
            var matrix = ExactCoverMatrix.Create(puzzle);

            int initialObjectivesCount = matrix.GetUnsatisfiedRequiredObjectives().Count();
            var coordToSelect = new Coordinate(1, 1);
            int indexToDrop = 0;
            var possibilitiesToAlter = matrix.GetAllPossibilitiesAt(in coordToSelect);
            Assert.True(possibilitiesToAlter![indexToDrop]!.TrySelect());
            Assert.All(possibilitiesToAlter, p => Assert.NotEqual(NodeState.UNKNOWN, p!.State));
            Assert.Equal(initialObjectivesCount - 1, matrix.GetUnsatisfiedRequiredObjectives().Count());

            var copy = matrix.CopyUnknowns();
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