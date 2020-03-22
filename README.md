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
|      SudokuSpice |  21.64 us |  0.030 us |  0.088 us |  21.64 us |  1.00 |    0.00 |
| SudokuSolverLite | 280.50 us | 23.878 us | 69.653 us | 301.02 us | 12.96 |    3.22 |
|      SudokuSharp |  14.22 us |  0.047 us |  0.138 us |  14.24 us |  0.66 |    0.01 |

They were also compared with a much smaller set of examples, most of which require more
advanced techniques. These demonstrate clearly that the slight overhead needed by SudokuSpice,
which dominates when solving easy puzzles, leads to effective performance enhancements for
more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|
|                SudokuSpice |   Easy |       9.201 us |     0.1785 us |     0.1910 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |      14.562 us |     0.0543 us |     0.0481 us |     1.58 |    0.04 |
| SudokuSpiceDynamicMultiple |   Easy |      18.845 us |     0.0426 us |     0.0398 us |     2.04 |    0.04 |
|           SudokuSolverLite |   Easy |     152.411 us |     0.5031 us |     0.4706 us |    16.53 |    0.35 |
|                SudokuSharp |   Easy |       3.303 us |     0.0103 us |     0.0096 us |     0.36 |    0.01 |
|                            |        |                |               |               |          |         |
|                SudokuSpice | Medium |      48.414 us |     0.1844 us |     0.1725 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |      69.151 us |     0.2265 us |     0.2008 us |     1.43 |    0.01 |
| SudokuSpiceDynamicMultiple | Medium |      82.616 us |     0.1808 us |     0.1510 us |     1.71 |    0.01 |
|           SudokuSolverLite | Medium |   2,347.889 us |     7.3404 us |     6.8662 us |    48.50 |    0.23 |
|                SudokuSharp | Medium |     148.094 us |     0.5610 us |     0.5247 us |     3.06 |    0.01 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardA |     146.366 us |     0.3345 us |     0.3129 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |     188.858 us |     0.6886 us |     0.6441 us |     1.29 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardA |     223.235 us |     0.9147 us |     0.8556 us |     1.53 |    0.01 |
|           SudokuSolverLite |  HardA |   4,801.402 us |    17.0098 us |    15.9110 us |    32.80 |    0.14 |
|                SudokuSharp |  HardA |   1,657.929 us |     3.5878 us |     3.3561 us |    11.33 |    0.04 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardB |      63.740 us |     0.1539 us |     0.1440 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |      91.178 us |     0.2370 us |     0.2217 us |     1.43 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardB |     113.013 us |     0.1076 us |     0.0954 us |     1.77 |    0.00 |
|           SudokuSolverLite |  HardB |  24,692.699 us |    72.3596 us |    67.6852 us |   387.40 |    1.34 |
|                SudokuSharp |  HardB |   5,176.687 us |    14.7057 us |    13.7557 us |    81.22 |    0.24 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilA |     115.913 us |     0.2660 us |     0.2489 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     149.531 us |     0.4635 us |     0.4108 us |     1.29 |    0.00 |
| SudokuSpiceDynamicMultiple |  EvilA |     173.255 us |     0.4829 us |     0.4517 us |     1.49 |    0.01 |
|           SudokuSolverLite |  EvilA | 397,273.707 us | 2,760.0986 us | 2,581.7979 us | 3,427.36 |   23.71 |
|                SudokuSharp |  EvilA |  64,968.675 us |   244.4226 us |   228.6330 us |   560.50 |    2.22 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilB |   1,076.107 us |     2.6981 us |     2.3918 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   1,488.289 us |     2.3136 us |     2.0510 us |     1.38 |    0.00 |
| SudokuSpiceDynamicMultiple |  EvilB |   1,800.304 us |     3.7162 us |     3.4761 us |     1.67 |    0.01 |
|           SudokuSolverLite |  EvilB |  50,251.081 us |   116.2257 us |   103.0310 us |    46.70 |    0.11 |
|                SudokuSharp |  EvilB |   5,226.226 us |    30.1161 us |    28.1706 us |     4.86 |    0.03 |
