using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace SudokuSpice.Benchmark {
[SimpleJob(RunStrategy.Throughput, targetCount: 100, invocationCount: 10000)]
public class SudokuSpiceBenchmarker
{
    private int _idx = 0;
    private IReadOnlyList<MatrixSudokuSample> _samples;

    private MatrixSudokuSample _GetSample() {
        var sample = _samples[_idx++];
        if (_idx == _samples.Count) {
            _idx = 0;
        }
        return sample;
    }

    [GlobalSetup]
    public void PrepareCsvSamples() {
        _samples = SudokuCsvParser.ParseCsv().Select(sample => new MatrixSudokuSample(sample)).ToList();
    }

    [Benchmark]
    public bool SolveMillionSudokus()
    {
        var sample = _GetSample();
        var puzzle = new Puzzle(sample.Puzzle);
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
        bool isSolved = sample.MatchesSolution(puzzle);
        if (!isSolved) {
            throw new ApplicationException($"Failed to solve the puzzle! Idx: {_idx}");
        }
        return isSolved;
    }
}
}