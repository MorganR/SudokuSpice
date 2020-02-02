using MorganRoff.Sudoku;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using TinyCsvParser;

namespace MorganRoff.Sudoku.Benchmark
{
    class Program
    {
        private static int sampleCount;
        private static int solvedCount;
        private static long maxTicks;
        private static long minTicks = long.MaxValue;

        static void Main(string[] args)
        {
            if (args[0] == "help") {
                Console.WriteLine(
                    "Solves a set of sudoku puzzles from a CSV file, formatted like:\n"
                    + "\tquizzes,solutions\n"
                    + "\t120450089...321,1234567...\n\n"
                    + "Pass the CSV filepath as the first argument:\n"
                    + "\tdotnet run -c RELEASE -- \"/path/to/file.csv\"");
                return;
            }
            Stopwatch mainWatch = new Stopwatch();
            mainWatch.Start();
            var options = new CsvParserOptions(
                /* skipHeader = */true,
                ',',
                /* degreeOfParallelism = */ 12,
                /* keepOrder = */false);
            var mapping = new SudokuSampleMapping();
            var parser = new CsvParser<SudokuSample>(options, mapping);

            var results = parser.ReadFromFile(args[0], Encoding.ASCII);

            var totalTicks = results.Select(result =>
            {
                Stopwatch watch = new Stopwatch();
                Interlocked.Increment(ref sampleCount);

                watch.Start();
                var solved = Solve(NumberStringToMatrix(result.Result.Puzzle));
                watch.Stop();
                if (SolutionsMatch(solved, NumberStringToMatrix(result.Result.Solution))) {
                    Interlocked.Increment(ref solvedCount);
                }
                return watch.ElapsedTicks;
            }).Aggregate(0L, (a, s) => {
                    UpdateMinAndMaxTicks(s);
                    return a + s;
                });
            mainWatch.Stop();

            Console.WriteLine($"Samples: {sampleCount}, Valid: {solvedCount}");
            Console.WriteLine($"Average step: {TicksToMicros((double)totalTicks / sampleCount)}us");
            Console.WriteLine($"Min step: {TicksToMicros(minTicks)}us");
            Console.WriteLine($"Max step: {TicksToMicros(maxTicks)}us");
            Console.WriteLine($"Main watch: {mainWatch.ElapsedMilliseconds}ms");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void UpdateMinAndMaxTicks(long ticks) {
            if (ticks > maxTicks) {
                maxTicks = ticks;
            }
            if (ticks < minTicks) {
                minTicks = ticks;
            }
        }

        private static double TicksToMicros(long ticks) {
            return ((double)ticks / TimeSpan.TicksPerMillisecond) * 1000;
        }

        private static double TicksToMicros(double ticks) {
            return (ticks / TimeSpan.TicksPerMillisecond) * 1000;
        }

        private static Puzzle Solve(int?[,] puzzleMatrix) {
            var puzzle = new Puzzle(puzzleMatrix);
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);
            var sudoku = new Solver(
                new SquareTracker(
                    puzzle,
                    restricts,
                    new List<IHeuristic>
                    {
                        new UniqueInRowHeuristic(puzzle, (RowRestrict) restricts[0]),
                        new UniqueInColumnHeuristic(puzzle, (ColumnRestrict) restricts[1]),
                        new UniqueInBoxHeuristic(puzzle, (BoxRestrict) restricts[2])
                    }));
            sudoku.Solve();
            return puzzle;
        }

        private static bool SolutionsMatch(Puzzle puzzle, int?[,] expected)
        {
            for (var row = 0; row < 9; row++)
            {
                for (var col = 0; col < 9; col++)
                {
                    if (puzzle[row, col] != expected[row, col])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static int?[,] NumberStringToMatrix(string nums) {
            var matrix = new int?[9,9];
            int idx = -1;
            for (var row = 0; row < 9; row++)
            {
                for (var col = 0; col < 9; col++)
                {
                    ++idx;
                    var c = nums[idx];
                    if (c == '0') {
                        continue;
                    }
                    matrix[row, col] = Int32.Parse(c.ToString());
                }
            }
            return matrix;
        }
    }
}
