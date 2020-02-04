namespace SudokuSpice.Benchmark
{
public class PuzzleSample
{
    public readonly string Name;
    public readonly int?[,] NullableMatrix;
    public readonly int[,] Matrix;

    public PuzzleSample(string puzzleName, int?[,] data) {
        Name = puzzleName;
        NullableMatrix = (int?[,]) data.Clone();
        int numRows = data.GetLength(0);
        int numCols = data.GetLength(1);
        Matrix = new int[numRows, numCols];
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                Matrix[row, col] = NullableMatrix[row, col] ?? 0;
            }
        }
    }

    public override string ToString() => Name;
}
}