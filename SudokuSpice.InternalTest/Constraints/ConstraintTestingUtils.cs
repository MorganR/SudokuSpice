using SudokuSpice.Data;
using Xunit;

namespace SudokuSpice.Constraints.InternalTest
{
    class ConstraintTestingUtils
    {
        public static void AssertPossibleSquareValueIsOnConstraint(PossibleSquareValue possibleValue, ConstraintHeader constraint)
        {
            var link = constraint.FirstLink;
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
