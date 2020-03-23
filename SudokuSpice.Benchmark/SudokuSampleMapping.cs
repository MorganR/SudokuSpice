using TinyCsvParser.Mapping;

namespace SudokuSpice.Benchmark
{
    class SudokuSampleMapping : CsvMapping<SudokuSample>
    {
        public SudokuSampleMapping() : base()
        {
            MapProperty(0, s => s.NumSet);
            MapProperty(1, s => s.NumSquaresToGuess);
            MapProperty(2, s => s.NumTotalGuesses);
            MapProperty(3, s => s.Puzzle);
        }
    }
}