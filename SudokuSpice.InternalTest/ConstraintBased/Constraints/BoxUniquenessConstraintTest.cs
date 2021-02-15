using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    public class BoxUniquenessConstraintTest
    {
        [Fact]
        public void Constrain_GroupsConstraintsAsExpected()
        {
            int size = 4;
            int boxSize = 2;
            int[] possibleValues = new int[] { 1, 3, 5, 7 };
            var puzzle = new Puzzle(size);
            var matrix = ExactCoverGraph.Create(puzzle);
            var squareObjectives = new HashSet<Objective>(matrix.GetUnsatisfiedRequiredObjectives());

            Assert.True(new BoxUniquenessConstraint().TryConstrain(puzzle, matrix));

            Assert.Equal(
                size * possibleValues.Length + squareObjectives.Count,
                matrix.GetUnsatisfiedRequiredObjectives().Count());
            Dictionary<int, HashSet<int>> boxIndicesToValues = new();
            for (int i = 0; i < size; ++i)
            {
                boxIndicesToValues[i] = new HashSet<int>();
            }
            Assert.All(matrix.GetUnsatisfiedRequiredObjectives(),
                concreteObjective =>
                {
                    if (squareObjectives.Contains(concreteObjective))
                    {
                        return;
                    }
                    IObjective objective = concreteObjective;
                    var possibilities = objective.GetUnknownDirectPossibilities().Cast<Possibility>().ToArray();
                    int boxIndex = Boxes.CalculateBoxIndex(possibilities[0].Coordinate, boxSize);
                    int value = possibilities[0].Index;
                    Assert.DoesNotContain(value, boxIndicesToValues[boxIndex]);
                    boxIndicesToValues[boxIndex].Add(value);
                    var boxCoordinates = Boxes.YieldUnsetCoordsForBox(boxIndex, boxSize, puzzle).ToArray();
                    Assert.Equal(boxCoordinates.Length, possibilities.Length);
                    Assert.All(possibilities, p =>
                    {
                        Assert.Contains(p.Coordinate, boxCoordinates);
                        Assert.Equal(value, p.Index);
                    });
                    Assert.All(boxCoordinates, c => Assert.NotNull(possibilities.SingleOrDefault(p => p.Coordinate == c)));
                });
            Assert.All(
                boxIndicesToValues.Values,
                values => Assert.Equal(new HashSet<int> { 0, 1, 2, 3 }, values));
        }
    }
}