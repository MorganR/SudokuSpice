namespace SudokuSpice.ConstraintBased.InternalTest
{
    internal static class OptionalObjectives
    {
        internal static OptionalObjective[] CreateIndependentOptionalObjectives(int objectivesCount, int countToSatisfy)
        {
            var objectives = new OptionalObjective[objectivesCount];
            for (--objectivesCount; objectivesCount >= 0; --objectivesCount)
            {
                objectives[objectivesCount] = OptionalObjective.CreateWithPossibilities(
                   Possibilities.CreatePossibilities(new Coordinate(objectivesCount, 0), 3), countToSatisfy);
            }
            return objectives;
        }
    }
}