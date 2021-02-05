namespace SudokuSpice.ConstraintBased
{
    public interface IOptionalObjective : IObjective, IPossibility
    {
        internal void RecordRequiredObjective(IObjective objective);
    }
}
