namespace SudokuSpice.Benchmark
{
    class MatrixSudokuSample
    {
        internal PuzzleSample Puzzle { get; set; }
        internal PuzzleSample Solution { get; set; }

        internal MatrixSudokuSample(LegacySudokuSample sample)
        {
            Puzzle = new PuzzleSample("puzzle", CsvUtils.PuzzleStringToMatrix(sample.Puzzle));
            Solution = new PuzzleSample("solution", CsvUtils.PuzzleStringToMatrix(sample.Solution));
        }
    }
}