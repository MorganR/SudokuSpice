namespace SudokuSpice.ConstraintBased
{
    public interface IPossibility
    {
        public NodeState State { get; }
        internal void AppendObjective(Link toNewObjective);
        internal void ReturnFromObjective(Link toReattach);
        internal bool TryDropFromObjective(Link toDetach);
    }
}
