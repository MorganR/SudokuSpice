# SudokuSpice

An efficient sudoku-solving library targeting .NET Core.

This code focuses on optimizing performance without sacrificing readability. Generally speaking, when faced with readability and flexibility improvements that have a slight performance cost, the version with improved readability has been implemented.

However that's not to say it performs poorly! See [the numbers](#Performance).

## Performance

All benchmarks were run using [BenchmarkDotNet](https://benchmarkdotnet.org/articles/overview.html).

Current performance benchmarks on my machine:

|               Method |         Mean |      Error |     StdDev |
|--------------------- |-------------:|-----------:|-----------:|
|     EasySudoku (9x9) |    15.250 us |  0.1700 us |  0.1590 us |
|   MediumSudoku (9x9) |    86.694 us |  0.2438 us |  0.2036 us |
|    HardSudokuA (9x9) |   912.694 us | 13.5620 us | 12.6859 us |
|    HardSudokuB (9x9) | 2,335.479 us | 37.0555 us | 34.6618 us |
|  MegaSudokuA (16x16) | 1,291.604 us | 15.5212 us | 14.5185 us |
|  MegaSudokuB (16x16) |   322.208 us |  1.8862 us |  1.7644 us |

### Comparisons
The performance of this library has been compared to other .NET sudoku libraries using a set of 1 million sudoku
puzzles ([source](https://www.kaggle.com/bryanpark/sudoku)).

Compared libraries:

* [SudokuSolverLite](https://github.com/zhiliangxu/SudokuSolver)

|          Library |      Mean |    Error |   StdDev |
|----------------- |----------:|---------:|---------:|
|      SudokuSpice |  37.14 us | 0.051 us | 0.147 us |
| SudokuSolverLite | 312.61 us | 0.869 us | 2.392 us |
