using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Models;

namespace SudokuSpice.Benchmark
{
    internal static class SudokuCsvParser
    {
        private static readonly string _csvPath =
            Path.Combine(Directory.GetCurrentDirectory(), "data", "puzzles.csv");
        internal static IReadOnlyList<SudokuSample> ParseCsv()
        {
            var options = new CsvOptions(
                Delimiter: ',',
                QuoteChar: '"',
                EscapeChar: '\\',
                Encoding: Encoding.ASCII,
                SkipHeader: true);
            var mapping = new SudokuSampleMapping();
            var parser = new CsvParser<SudokuSample>(options, mapping);

            IEnumerable<CsvMappingResult<SudokuSample>> results = parser.ReadFromFile(_csvPath);

            return results.Select(result => {
                if (!result.IsSuccess) {
                    throw new InvalidDataException($"Invalid result read from CSV line {result.LineNumber}. Error: {result.Error}");
                }
                return result.Result;
            }).ToList();
        }
    }
}