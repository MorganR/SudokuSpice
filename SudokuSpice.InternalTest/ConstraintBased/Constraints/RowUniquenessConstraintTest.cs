﻿using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    public class RowUniquenessConstraintTest
    {
        [Fact]
        public void Constrain_ReturnsExpectedConstraints()
        {
            int size = 4;
            int[] possibleValues = new int[] { 1, 3, 5, 7 };
            var puzzle = new Puzzle(size);
            var matrix = ExactCoverGraph.Create(puzzle);
            var squareObjectives = new HashSet<Objective>(matrix.GetUnsatisfiedRequiredObjectivesWithConcretePossibilities());

            Assert.True(new RowUniquenessConstraint().TryConstrain(puzzle, matrix));

            Assert.Equal(
                size * possibleValues.Length + squareObjectives.Count,
                matrix.GetUnsatisfiedRequiredObjectivesWithConcretePossibilities().Count());
            Dictionary<int, HashSet<int>> rowsToValues = new();
            for (int i = 0; i < size; ++i)
            {
                rowsToValues[i] = new HashSet<int>();
            }
            var expectedColumns = new int[] { 0, 1, 2, 3 };
            Assert.All(matrix.GetUnsatisfiedRequiredObjectivesWithConcretePossibilities(),
                concreteObjective =>
                {
                    if (squareObjectives.Contains(concreteObjective))
                    {
                        return;
                    }
                    IObjective objective = concreteObjective;
                    var possibilities = objective.GetUnknownDirectPossibilities().Cast<Possibility>().ToArray();
                    int row = possibilities[0].Coordinate.Row;
                    int value = possibilities[0].Index;
                    Assert.DoesNotContain(value, rowsToValues[row]);
                    rowsToValues[row].Add(value);
                    var expectedCoordinates = expectedColumns.Select(column => new Coordinate(row, column)).ToArray();
                    Assert.Equal(expectedCoordinates.Length, possibilities.Length);
                    Assert.All(possibilities, p =>
                    {
                        Assert.Contains(p.Coordinate, expectedCoordinates);
                        Assert.Equal(value, p.Index);
                    });
                    Assert.All(expectedCoordinates, c => Assert.NotNull(possibilities.SingleOrDefault(p => p.Coordinate == c)));
                });
            Assert.All(
                rowsToValues.Values,
                values => Assert.Equal(new HashSet<int> { 0, 1, 2, 3 }, values));
        }
    }
}