namespace SudokuSpice.ConstraintBased
{
    public interface IPossibility
    {
        public NodeState State { get; }
        internal void AppendObjective(Link toNewObjective);
        internal void NotifyReattachedToObjective(Link toReattach);
        internal bool TryNotifyDroppedFromObjective(Link toDetach);
    }
}
