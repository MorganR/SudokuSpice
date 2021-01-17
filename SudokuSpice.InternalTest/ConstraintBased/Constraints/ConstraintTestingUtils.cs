using Xunit;

namespace SudokuSpice.ConstraintBased.Constraints.Test
{
    internal class ConstraintTestingUtils
    {
        public static void AssertPossibleValueIsOnRequirement(
            PossibleSquareValue possibleValue, Requirement constraint)
        {
            Link<PossibleSquareValue, Requirement> link = constraint.FirstLink;
            do
            {
                if (link.Possibility == possibleValue)
                {
                    return;
                }
                link = link.NextOnObjective;
            } while (link != constraint.FirstLink);
            Assert.True(false, "No matching possible square value found.");
        }
    }
}