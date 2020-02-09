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
|      SudokuSpice |  30.07 us |  0.059 us |  0.172 us |  30.06 us |  1.00 |    0.00 |
| SudokuSolverLite | 274.14 us | 26.004 us | 76.266 us | 299.52 us |  9.19 |    2.43 |
|      SudokuSharp |  13.84 us |  0.051 us |  0.149 us |  13.83 us |  0.46 |    0.01 |

They were also compared with a much smaller set of examples, most of which require more
advanced techniques. These demonstrate clearly that the slight overhead needed by SudokuSpice,
which dominates when solving easy puzzles, leads to effective performance enhancements for
more complicated examples.

|           Method | puzzle |           Mean |         Error |        StdDev |  Ratio | RatioSD |
|----------------- |------- |---------------:|--------------:|--------------:|-------:|--------:|
|      SudokuSpice |   Easy |      12.499 us |     0.0509 us |     0.0476 us |   1.00 |    0.00 |
| SudokuSolverLite |   Easy |     153.269 us |     1.2303 us |     1.1508 us |  12.26 |    0.07 |
|      SudokuSharp |   Easy |       3.262 us |     0.0124 us |     0.0116 us |   0.26 |    0.00 |
|                  |        |                |               |               |        |         |
|      SudokuSpice | Medium |      80.703 us |     0.2063 us |     0.1723 us |   1.00 |    0.00 |
| SudokuSolverLite | Medium |   2,299.988 us |     7.8900 us |     7.3803 us |  28.49 |    0.11 |
|      SudokuSharp | Medium |     149.317 us |     0.5409 us |     0.5059 us |   1.85 |    0.01 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  HardA |     227.919 us |     0.8015 us |     0.7498 us |   1.00 |    0.00 |
| SudokuSolverLite |  HardA |   4,703.643 us |    28.9588 us |    25.6712 us |  20.64 |    0.12 |
|      SudokuSharp |  HardA |   1,653.709 us |     3.0580 us |     2.5535 us |   7.25 |    0.03 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  HardB |     118.769 us |     0.4862 us |     0.4548 us |   1.00 |    0.00 |
| SudokuSolverLite |  HardB |  24,620.609 us |    72.9550 us |    64.6727 us | 207.39 |    0.98 |
|      SudokuSharp |  HardB |   5,119.287 us |    25.9460 us |    20.2569 us |  43.11 |    0.19 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  EvilA |     776.907 us |     4.7306 us |     4.4250 us |   1.00 |    0.00 |
| SudokuSolverLite |  EvilA | 388,311.027 us | 1,680.5714 us | 1,572.0076 us | 499.83 |    3.28 |
|      SudokuSharp |  EvilA |  63,717.150 us |   312.5381 us |   260.9835 us |  82.02 |    0.51 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  EvilB |   1,967.079 us |    13.0652 us |    12.2212 us |   1.00 |    0.00 |
| SudokuSolverLite |  EvilB |  49,377.684 us |   114.2272 us |   101.2594 us |  25.10 |    0.18 |
|      SudokuSharp |  EvilB |   5,155.765 us |    32.9996 us |    27.5562 us |   2.62 |    0.02 |
