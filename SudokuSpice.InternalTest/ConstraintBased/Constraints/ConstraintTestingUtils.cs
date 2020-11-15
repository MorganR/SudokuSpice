﻿using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    internal class ConstraintTestingUtils
    {
        public static void AssertPossibleSquareValueIsOnConstraint<TPuzzle>(
            PossibleSquareValue<TPuzzle> possibleValue, ConstraintHeader<TPuzzle> constraint)
            where TPuzzle : IReadOnlyPuzzle
        {
            SquareLink<TPuzzle> link = constraint.FirstLink;
            do
            {
                if (link.PossibleSquare == possibleValue)
                {
                    return;
                }
                link = link.Down;
            } while (link != constraint.FirstLink);
            Assert.True(false, "No matching possible square value found.");
        }
    }
}
