using SudokuSpice.Data;

namespace SudokuSpice.Constraints
{
    public class RowUniquenessConstraint : IConstraint
    {
        public void Constrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            for (int row = 0; row < puzzle.Size; row++)
            {
                var rowSquares = matrix.GetSquaresOnRow(row);
                for (int i = 0; i < matrix.AllPossibleValues.Length; i++)
                {
                    var possibleSquares = new PossibleSquareValue[rowSquares.Length];
                    for (int col = 0; col < rowSquares.Length; col++)
                    {
                        possibleSquares[col] = rowSquares[col].AllPossibleValues[i];
                    }
                    ConstraintHeader.CreateConnectedHeader(matrix, possibleSquares);
                }
            }
        }
    }
}
