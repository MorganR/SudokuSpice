using TinyCsvParser.Mapping;

namespace SudokuSpice.Benchmark
{
    class LegacySudokuSampleMapping : CsvMapping<LegacySudokuSample>
    {
        public LegacySudokuSampleMapping() : base()
        {
            MapProperty(0, s => s.Puzzle);
            MapProperty(1, s => s.Solution);
        }
    }
}