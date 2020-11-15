using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    internal class ConstraintTestingUtils
    {
        public static void AssertPossibleValueIsOnConstraint<TPuzzle>(
            PossibleValue<TPuzzle> possibleValue, ConstraintHeader<TPuzzle> constraint)
            where TPuzzle : IReadOnlyPuzzle
        {
            Link<TPuzzle> link = constraint.FirstLink;
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
