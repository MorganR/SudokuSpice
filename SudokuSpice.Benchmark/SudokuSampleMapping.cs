using TinyCsvParser.Mapping;

namespace MorganRoff.Sudoku.Benchmark
{
class SudokuSampleMapping : CsvMapping<SudokuSample> {
    public SudokuSampleMapping() : base()
    {
        MapProperty(0, s => s.Puzzle);
        MapProperty(1, s => s.Solution);
    }
}
}