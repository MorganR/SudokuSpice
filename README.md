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

|           Method |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |
|----------------- |----------:|----------:|----------:|----------:|------:|--------:|
|      SudokuSpice |  24.53 us |  0.051 us |  0.146 us |  24.51 us |  1.00 |    0.00 |
| SudokuSolverLite | 274.07 us | 24.247 us | 69.569 us | 294.83 us | 11.17 |    2.84 |
|      SudokuSharp |  13.80 us |  0.048 us |  0.139 us |  13.80 us |  0.56 |    0.01 |

They were also compared with a much smaller set of examples, most of which require more
advanced techniques. These demonstrate clearly that the slight overhead needed by SudokuSpice,
which dominates when solving easy puzzles, leads to effective performance enhancements for
more complicated examples.

|           Method | puzzle |           Mean |         Error |        StdDev |  Ratio | RatioSD |
|----------------- |------- |---------------:|--------------:|--------------:|-------:|--------:|
|      SudokuSpice |   Easy |       9.739 us |     0.0508 us |     0.0425 us |   1.00 |    0.00 |
| SudokuSolverLite |   Easy |     152.164 us |     0.8949 us |     0.7933 us |  15.62 |    0.11 |
|      SudokuSharp |   Easy |       3.250 us |     0.0128 us |     0.0120 us |   0.33 |    0.00 |
|                  |        |                |               |               |        |         |
|      SudokuSpice | Medium |      73.013 us |     0.2981 us |     0.2788 us |   1.00 |    0.00 |
| SudokuSolverLite | Medium |   2,309.330 us |     8.6750 us |     8.1146 us |  31.63 |    0.16 |
|      SudokuSharp | Medium |     147.588 us |     0.5743 us |     0.5372 us |   2.02 |    0.01 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  HardA |     198.216 us |     0.5472 us |     0.4851 us |   1.00 |    0.00 |
| SudokuSolverLite |  HardA |   4,664.937 us |    28.7687 us |    25.5027 us |  23.53 |    0.13 |
|      SudokuSharp |  HardA |   1,655.190 us |     7.5154 us |     6.2757 us |   8.35 |    0.03 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  HardB |      98.906 us |     0.2416 us |     0.2260 us |   1.00 |    0.00 |
| SudokuSolverLite |  HardB |  24,259.530 us |    97.0576 us |    86.0391 us | 245.27 |    1.13 |
|      SudokuSharp |  HardB |   5,136.522 us |    43.3617 us |    36.2089 us |  51.94 |    0.40 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  EvilA |     643.056 us |     4.1415 us |     3.6714 us |   1.00 |    0.00 |
| SudokuSolverLite |  EvilA | 391,539.869 us | 3,207.6810 us | 2,678.5589 us | 609.08 |    6.88 |
|      SudokuSharp |  EvilA |  63,056.368 us |   419.4118 us |   392.3180 us |  98.10 |    0.98 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  EvilB |   1,534.888 us |     5.2809 us |     4.4098 us |   1.00 |    0.00 |
| SudokuSolverLite |  EvilB |  49,001.955 us |   269.5799 us |   238.9755 us |  31.91 |    0.19 |
|      SudokuSharp |  EvilB |   5,183.240 us |   103.0969 us |    96.4369 us |   3.38 |    0.07 |

