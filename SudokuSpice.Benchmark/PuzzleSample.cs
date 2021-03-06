namespace SudokuSpice.Benchmark
{
    public class PuzzleSample
    {
        public readonly string Name;
        public readonly int?[,] NullableMatrix;
        public readonly int?[][] NullableJaggedMatrix;
        public readonly int[,] Matrix;

        public PuzzleSample(string puzzleName, int?[,] data)
        {
            Name = puzzleName;
            NullableMatrix = (int?[,])data.Clone();
            int numRows = data.GetLength(0);
            int numCols = data.GetLength(1);
            NullableJaggedMatrix = new int?[numRows][];
            Matrix = new int[numRows, numCols];
            for (int row = 0; row < numRows; row++)
            {
                NullableJaggedMatrix[row] = new int?[numCols];
                for (int col = 0; col < numCols; col++)
                {
                    var value = NullableMatrix[row, col];
                    NullableJaggedMatrix[row][col] = value;
                    Matrix[row, col] = value ?? 0;
                }
            }
        }

        public override string ToString() => Name;
    }
}