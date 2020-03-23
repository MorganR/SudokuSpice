using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace SudokuSpice.Benchmark
{
    [SimpleJob(RunStrategy.Throughput, targetCount: 100, invocationCount: 10000)]
    public class LegacyCsvBenchmarker
    {
        private int _idx = 0;
        private IReadOnlyList<MatrixSudokuSample> _samples;

        private MatrixSudokuSample _GetSample()
        {
            var sample = _samples[_idx++];
            if (_idx == _samples.Count)
            {
                _idx = 0;
            }
            return sample;
        }

        [GlobalSetup]
        public void PrepareCsvSamples()
        {
            _samples = LegacySudokuCsvParser.ParseCsv().Select(sample => new MatrixSudokuSample(sample)).ToList();
        }

        [Benchmark(Baseline = true)]
        public bool SudokuSpice()
        {
            var sample = _GetSample();
            var puzzle = new Puzzle(sample.Puzzle.NullableMatrix);
            var solver = new Solver(puzzle);
            solver.Solve();
            return puzzle.NumEmptySquares == 0;
        }

        [Benchmark]
        public bool SudokuSharp()
        {
            var sample = _GetSample();
            var board = new SudokuSharp.Board();
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (sample.Puzzle.NullableMatrix[row, col].HasValue)
                    {
                        board.PutCell(new SudokuSharp.Location(col, row), sample.Puzzle.NullableMatrix[row, col].Value);
                    }
                }
            }

            board = board.Fill.Sequential();

            return board.IsSolved;
        }

        [Benchmark]
        public bool SudokuSolverLite()
        {
            var sample = _GetSample();
            // Must clone the input, since this method solves the puzzle in-place.
            return SudokuSolver.SudokuSolver.Solve((int[,])sample.Puzzle.Matrix.Clone());
        }
    }
}