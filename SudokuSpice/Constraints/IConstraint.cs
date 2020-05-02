using SudokuSpice.Data;

namespace SudokuSpice.Constraints
{
    public interface IConstraint
    {
        public void Constrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix);
    }
}
