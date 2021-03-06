﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    internal static class ExactCoverGraphs
    {
        internal static void AssertNoPossibleValuesAtSquare(Coordinate coord, ExactCoverGraph graph)
        {
            var square = graph.GetAllPossibilitiesAt(coord);
            if (square is not null)
            {
                for (int valueIndex = 0; valueIndex < square.Length; ++valueIndex)
                {
                    var possibleValue = square[valueIndex];
                    if (possibleValue is not null)
                    {
                        Assert.Equal(NodeState.DROPPED, possibleValue.State);
                    }
                }
            }
        }

        internal static void AssertPossibleValuesAtSquare(Coordinate coord, int[] possibleValues, ExactCoverGraph graph)
        {
            var square = graph.GetAllPossibilitiesAt(coord);
            Assert.NotNull(square);
            var foundValues = new List<int>();
            for (int valueIndex = 0; valueIndex < square.Length; ++valueIndex)
            {
                var possibleValue = square[valueIndex];
                if (possibleValue is null)
                {
                    continue;
                }
                int value = graph.AllPossibleValues[valueIndex];
                if (possibleValue.State == NodeState.UNKNOWN)
                {
                    foundValues.Add(value);
                    Assert.True(
                        possibleValues.Contains(value),
                        $"Unexpected possible value {value} found at {coord}. Expected: {string.Join(',', possibleValues)}.");
                } else
                {
                    Assert.False(
                        possibleValues.Contains(value),
                        $"Missing possible value {value} at {coord}. Expected: {string.Join(',', possibleValues)}.");
                }
            }
            Assert.True(
                possibleValues.Length == foundValues.Count,
                $"Found {foundValues.Count} possible values when expected {possibleValues.Length} at {coord}. Expected {string.Join(',', possibleValues)}, found: {string.Join(',', foundValues)}.");
        }

    }
}