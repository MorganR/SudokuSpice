using SudokuSpice.RuleBased.Heuristics;
using SudokuSpice.RuleBased.Rules;

namespace SudokuSpice.RuleBased
{
    public static class StandardPuzzles
    {
        public static PuzzleSolver CreateSolver(int size)
        {
            var possibleValues = new PossibleValues(size);
            var ruleKeeper = new StandardRuleKeeper(possibleValues);
            return new PuzzleSolver(
                possibleValues,
                ruleKeeper,
                new StandardHeuristic(
                    possibleValues,
                    ruleKeeper,
                    ruleKeeper,
                    ruleKeeper));
        }
    }
}
