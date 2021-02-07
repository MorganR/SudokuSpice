using System.Linq;

namespace SudokuSpice.ConstraintBased
{
    internal static class Objectives
    {
        internal static bool LinksToUniqueRequiredObjective(Link toCheck, Link toIterate)
        {
            var requiredObjectives = toCheck.Objective.RequiredObjectives;
            return requiredObjectives.Any(
                required => !toIterate.GetLinksOnPossibility().Any(
                    otherObjective => 
                    otherObjective != toCheck &&
                    otherObjective.Objective.State != NodeState.DROPPED &&
                    otherObjective.Objective.RequiredObjectives.Contains(required)));
        }
    }
}
