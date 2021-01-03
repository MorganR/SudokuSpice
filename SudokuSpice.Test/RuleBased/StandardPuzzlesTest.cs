using SudokuSpice.Test;
using Xunit;

namespace SudokuSpice.RuleBased.Test
{
    public class StandardPuzzlesTest
    {
        [Theory]
        [ClassData(typeof(ValidStandardPuzzles))]
        public void CreateSolver_CanSolveStandardPuzzles(int?[,] matrix)
        {
            var solver = StandardPuzzles.CreateSolver();

            var solved = solver.Solve(new PuzzleWithPossibleValues(matrix));

            PuzzleTestUtils.AssertStandardPuzzleSolved(solved);
        }
    }
}
