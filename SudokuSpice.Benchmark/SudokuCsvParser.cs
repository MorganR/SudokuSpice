using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser;

namespace SudokuSpice.Benchmark
{
    internal static class SudokuCsvParser
    {
        private static readonly string _csvPath =
            Path.Combine(Directory.GetCurrentDirectory(), "data", "puzzles.csv");
        internal static IReadOnlyList<SudokuSample> ParseCsv()
        {
            var options = new CsvParserOptions(
                /* skipHeader = */true,
                ',',
                /* degreeOfParallelism = */12,
                /* keepOrder = */true);
            var mapping = new SudokuSampleMapping();
            var parser = new CsvParser<SudokuSample>(options, mapping);

            ParallelQuery<TinyCsvParser.Mapping.CsvMappingResult<SudokuSample>> results = parser.ReadFromFile(_csvPath, Encoding.ASCII);

            return results.Select(result => result.Result).ToList();
        }
    }
}