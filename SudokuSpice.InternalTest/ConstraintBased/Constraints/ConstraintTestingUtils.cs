using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    internal class ConstraintTestingUtils
    {
        public static void AssertPossibleValueIsOnConstraint(
            PossibleSquareValue possibleValue, ConstraintHeader constraint)
        {
            Link link = constraint.FirstLink;
            do
            {
                if (link.PossibleSquareValue == possibleValue)
                {
                    return;
                }
                link = link.Down;
            } while (link != constraint.FirstLink);
            Assert.True(false, "No matching possible square value found.");
        }
    }
}