using SudokuSpice.Data;
using Xunit;

namespace SudokuSpice.Constraints.Test
{
    class ConstraintTestingUtils
    {
        public static void AssertPossibleSquareValueIsOnConstraint(PossibleSquareValue possibleValue, ConstraintHeader constraint)
        {
            var link = constraint.FirstLink;
            do
            {
                if (ReferenceEquals(link.PossibleSquare, possibleValue))
                {
                    return;
                }
                link = link.Down;
            } while (link != constraint.FirstLink);
            Assert.True(false, "No matching possible square value found.");
        }
    }
}
