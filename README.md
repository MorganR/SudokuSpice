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
|      SudokuSpice |  35.93 us |  0.137 us |  0.390 us |  35.83 us |  1.00 |    0.00 |
| SudokuSolverLite | 269.33 us | 29.258 us | 82.522 us | 299.45 us |  7.51 |    2.31 |
|      SudokuSharp |  14.77 us |  0.049 us |  0.144 us |  14.76 us |  0.41 |    0.01 |

They were also compared with a much smaller set of examples, most of which require more
advanced techniques. These demonstrate clearly that the slight overhead needed by SudokuSpice,
which dominates when solving easy puzzles, leads to effective performance enhancements for
more complicated examples.

|           Method | puzzle |           Mean |         Error |        StdDev |  Ratio | RatioSD |
|----------------- |------- |---------------:|--------------:|--------------:|-------:|--------:|
|      SudokuSpice |   Easy |      15.953 us |     0.0448 us |     0.0397 us |   1.00 |    0.00 |
| SudokuSolverLite |   Easy |     154.303 us |     0.6680 us |     0.6248 us |   9.67 |    0.04 |
|      SudokuSharp |   Easy |       3.322 us |     0.0158 us |     0.0140 us |   0.21 |    0.00 |
|                  |        |                |               |               |        |         |
|      SudokuSpice | Medium |     101.737 us |     2.0162 us |     2.6915 us |   1.00 |    0.00 |
| SudokuSolverLite | Medium |   2,467.917 us |    35.1903 us |    32.9170 us |  23.95 |    0.66 |
|      SudokuSharp | Medium |     155.063 us |     1.7833 us |     1.6681 us |   1.50 |    0.04 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  HardA |     274.335 us |     1.0385 us |     0.9714 us |   1.00 |    0.00 |
| SudokuSolverLite |  HardA |   4,751.982 us |    14.1592 us |    11.8236 us |  17.33 |    0.06 |
|      SudokuSharp |  HardA |   1,656.728 us |     7.2422 us |     6.7744 us |   6.04 |    0.03 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  HardB |     143.256 us |     0.4217 us |     0.3944 us |   1.00 |    0.00 |
| SudokuSolverLite |  HardB |  24,660.366 us |   122.7094 us |   102.4679 us | 172.22 |    0.70 |
|      SudokuSharp |  HardB |   5,519.469 us |   106.1315 us |   117.9649 us |  38.59 |    0.89 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  EvilA |     943.937 us |     0.8876 us |     0.7412 us |   1.00 |    0.00 |
| SudokuSolverLite |  EvilA | 389,259.847 us | 1,896.3955 us | 1,773.8895 us | 412.70 |    1.86 |
|      SudokuSharp |  EvilA |  64,378.248 us |   109.7437 us |    97.2850 us |  68.21 |    0.13 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  EvilB |   2,408.886 us |     4.1053 us |     3.4281 us |   1.00 |    0.00 |
| SudokuSolverLite |  EvilB |  49,832.456 us |   179.3283 us |   158.9699 us |  20.69 |    0.07 |
|      SudokuSharp |  EvilB |   5,241.682 us |    11.5811 us |    10.2663 us |   2.18 |    0.00 |