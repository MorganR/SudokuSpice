namespace SudokuSpice.Benchmark
{
    internal class CsvUtils
    {
        public static int?[,] PuzzleStringToMatrix(string puzzle)
        {
            int?[,] matrix = new int?[9, 9];
            int idx = -1;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    ++idx;
                    char c = puzzle[idx];
                    if (c == '0')
                    {
                        continue;
                    }
                    matrix[row, col] = int.Parse(c.ToString());
                }
            }
            return matrix;
        }
    }
}