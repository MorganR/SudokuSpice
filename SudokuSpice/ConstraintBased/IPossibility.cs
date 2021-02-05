namespace SudokuSpice.ConstraintBased
{
    public interface IPossibility
    {
        internal void AppendObjective(Link toNewObjective);
        internal void ReattachObjective(Link toReattach);
        internal bool TryDetachObjective(Link toDetach);
    }
}
