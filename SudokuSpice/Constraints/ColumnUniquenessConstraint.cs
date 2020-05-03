using SudokuSpice.Data;

namespace SudokuSpice.Constraints
{
    public class ColumnUniquenessConstraint : IConstraint
    {
        public void Constrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            for (int column = 0; column < puzzle.Size; column++)
            {
                var columnSquares = matrix.GetSquaresOnColumn(column);
                for (int i = 0; i < matrix.AllPossibleValues.Length; i++)
                {
                    var possibleSquares = new PossibleSquareValue[columnSquares.Count];
                    for (int row = 0; row < columnSquares.Count; row++)
                    {
                        possibleSquares[row] = columnSquares[row].AllPossibleValues[i];
                    }
                    matrix.AddConstraintHeader(ConstraintHeader.CreateConnectedHeader(possibleSquares));
                }
            }
        }
    }
}
