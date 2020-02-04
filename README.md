# SudokuSpice

An efficient sudoku-solving library targeting .NET Core.

This code focuses on optimizing performance without sacrificing readability. Generally speaking, when faced with readability and flexibility improvements that have a slight performance cost, the version with improved readability has been implemented.

However that's not to say it performs poorly! See [the numbers](#Performance).

## Performance

All benchmarks were run using [BenchmarkDotNet](https://benchmarkdotnet.org/articles/overview.html).

### Comparisons
The performance of this library has been compared to other .NET sudoku libraries.

**Compared libraries:**

* [SudokuSolverLite](https://github.com/zhiliangxu/SudokuSolver)
* [SudokuSharp](https://github.com/BenjaminChambers/SudokuSharp)

These were compared using a set of 1 million sudoku puzzles
([source](https://www.kaggle.com/bryanpark/sudoku)). These are fairly easy
puzzles without much (if any) need to guess or use advanced heuristics.

|          Library |      Mean |    Error |   StdDev |
|----------------- |----------:|---------:|---------:|
|      SudokuSpice |  37.14 us | 0.051 us | 0.147 us |
| SudokuSolverLite | 312.61 us | 0.869 us | 2.392 us |
|      SudokuSharp |  15.58 us | 0.216 us | 0.637 us |

They were also compared with a much smaller set of examples, most of which require more
advanced techniques. These demonstrate clearly that the slight overhead needed by SudokuSpice,
which dominates when solving easy puzzles, leads to effective performance enhancements for
more complicated examples.

|           Method | Puzzle |           Mean |         Error |        StdDev |
|----------------- |------- |---------------:|--------------:|--------------:|
|      SudokuSpice |   Easy |      16.931 us |     0.0726 us |     0.0644 us |
| SudokuSolverLite |   Easy |     169.072 us |     1.4528 us |     1.3590 us |
|      SudokuSharp |   Easy |       3.806 us |     0.0178 us |     0.0166 us |
|      SudokuSpice | Medium |      98.213 us |     0.3011 us |     0.2816 us |
| SudokuSolverLite | Medium |   2,504.763 us |     7.0187 us |     5.8609 us |
|      SudokuSharp | Medium |     167.423 us |     1.0731 us |     0.9513 us |
|      SudokuSpice |  HardA |     287.699 us |     0.8475 us |     0.7512 us |
| SudokuSolverLite |  HardA |   5,268.956 us |    25.7195 us |    22.7997 us |
|      SudokuSharp |  HardA |   1,809.859 us |     3.6908 us |     3.4524 us |
|      SudokuSpice |  HardB |     149.019 us |     0.4318 us |     0.3828 us |
| SudokuSolverLite |  HardB |  26,737.498 us |   269.0400 us |   224.6606 us |
|      SudokuSharp |  HardB |   5,689.521 us |    21.4347 us |    20.0500 us |
|      SudokuSpice |  EvilA |     999.594 us |     4.6958 us |     4.3924 us |
| SudokuSolverLite |  EvilA | 443,422.960 us | 1,842.3361 us | 1,538.4341 us |
|      SudokuSharp |  EvilA |  69,708.176 us |   369.1654 us |   345.3175 us |
|      SudokuSpice |  EvilB |   2,514.614 us |    11.8043 us |    10.4642 us |
| SudokuSolverLite |  EvilB |  56,683.954 us |   168.5001 us |   140.7052 us |
|      SudokuSharp |  EvilB |   5,602.801 us |    24.1145 us |    21.3769 us |
