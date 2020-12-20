using SudokuSpice.RuleBased.Heuristics;

namespace SudokuSpice.RuleBased
{
    /// <summary>Provides utilities for interacting with standard Sudoku puzzles.</summary>
    public static class StandardPuzzles
    {
        /// <summary>Creates an efficient solver for solving standard Sudoku puzzles.</summary>
        public static PuzzleSolver CreateSolver()
        {
            var ruleKeeper = new StandardRuleKeeper();
            return new PuzzleSolver(
                ruleKeeper,
                new StandardHeuristic(
                    ruleKeeper,
                    ruleKeeper,
                    ruleKeeper));
        }
    }
}