namespace SudokuSpice.Benchmark
{
    class CsvUtils
    {
        public static int?[,] PuzzleStringToMatrix(string puzzle)
        {
            var matrix = new int?[9, 9];
            int idx = -1;
            for (var row = 0; row < 9; row++)
            {
                for (var col = 0; col < 9; col++)
                {
                    ++idx;
                    var c = puzzle[idx];
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
