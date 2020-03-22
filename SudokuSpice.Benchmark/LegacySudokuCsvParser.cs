using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser;

namespace SudokuSpice.Benchmark
{
    static class LegacySudokuCsvParser
    {
        private static readonly string _csvPath = 
            Path.Combine(Directory.GetCurrentDirectory(), "data", "legacy-sudoku.csv");
        internal static IReadOnlyList<LegacySudokuSample> ParseCsv()
        {
            var options = new CsvParserOptions(
                /* skipHeader = */true,
                ',',
                /* degreeOfParallelism = */12,
                /* keepOrder = */true);
            var mapping = new LegacySudokuSampleMapping();
            var parser = new CsvParser<LegacySudokuSample>(options, mapping);

            var results = parser.ReadFromFile(_csvPath, Encoding.ASCII);

            return results.Select(result => result.Result).ToList();
        }
    }
}