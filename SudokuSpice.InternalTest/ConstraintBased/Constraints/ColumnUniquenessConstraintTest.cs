using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    public class ColumnUniquenessConstraintTest
    {
        [Fact]
        public void Constrain_ReturnsExpectedConstraints()
        {
            int size = 4;
            int[] possibleValues = new int[] { 1, 3, 5, 7 };
            var puzzle = new Puzzle(size);
            var matrix = ExactCoverMatrix.Create(puzzle);
            var squareObjectives = new HashSet<Objective>(matrix.GetUnsatisfiedRequiredObjectives());

            Assert.True(new ColumnUniquenessConstraint().TryConstrain(puzzle, matrix));

            Assert.Equal(
                size * possibleValues.Length + squareObjectives.Count,
                matrix.GetUnsatisfiedRequiredObjectives().Count());
            Dictionary<int, HashSet<int>> columnsToValues = new();
            for (int i = 0; i < size; ++i)
            {
                columnsToValues[i] = new HashSet<int>();
            }
            var expectedRows = new int[] { 0, 1, 2, 3 };
            Assert.All(matrix.GetUnsatisfiedRequiredObjectives(),
                concreteObjective =>
                {
                    if (squareObjectives.Contains(concreteObjective))
                    {
                        return;
                    }
                    IObjective objective = concreteObjective;
                    var possibilities = objective.GetUnknownDirectPossibilities().Cast<Possibility>().ToArray();
                    int column = possibilities[0].Coordinate.Column;
                    int value = possibilities[0].Index;
                    Assert.DoesNotContain(value, columnsToValues[column]);
                    columnsToValues[column].Add(value);
                    var expectedCoordinates = expectedRows.Select(row => new Coordinate(row, column)).ToArray();
                    Assert.Equal(expectedCoordinates.Length, possibilities.Length);
                    Assert.All(possibilities, p =>
                    {
                        Assert.Contains(p.Coordinate, expectedCoordinates);
                        Assert.Equal(value, p.Index);
                    });
                    Assert.All(expectedCoordinates, c => Assert.NotNull(possibilities.SingleOrDefault(p => p.Coordinate == c)));
                });
            Assert.All(
                columnsToValues.Values,
                values => Assert.Equal(new HashSet<int> { 0, 1, 2, 3 }, values));
        }
    }
}