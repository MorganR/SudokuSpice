using SudokuSpice.Test;
using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    public class StandardPuzzlesTest
    {
        [Theory]
        [ClassData(typeof(ValidStandardPuzzles))]
        public void CreateSolver_CanSolveStandardPuzzles(int?[][] matrix)
        {
            var solver = StandardPuzzles.CreateSolver();

            var solved = solver.Solve(new Puzzle(matrix));

            PuzzleTestUtils.AssertStandardPuzzleSolved(solved);
        }
    }
}