using SudokuSpice.RuleBased.Heuristics;

namespace SudokuSpice.RuleBased
{
    public static class StandardPuzzles
    {
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