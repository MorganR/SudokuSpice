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
|      SudokuSpice |   Easy |      12.063 us |     0.0938 us |     0.0783 us |   1.00 |    0.00 |
| SudokuSolverLite |   Easy |     152.805 us |     0.8586 us |     0.7611 us |  12.67 |    0.10 |
|      SudokuSharp |   Easy |       3.267 us |     0.0078 us |     0.0073 us |   0.27 |    0.00 |
|                  |        |                |               |               |        |         |
|      SudokuSpice | Medium |      80.302 us |     1.2983 us |     1.2145 us |   1.00 |    0.00 |
| SudokuSolverLite | Medium |   2,509.095 us |    41.7669 us |    39.0688 us |  31.26 |    0.83 |
|      SudokuSharp | Medium |     150.979 us |     1.6312 us |     1.5258 us |   1.88 |    0.03 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  HardA |     223.722 us |     0.5162 us |     0.4576 us |   1.00 |    0.00 |
| SudokuSolverLite |  HardA |   4,664.293 us |    40.8597 us |    36.2211 us |  20.85 |    0.16 |
|      SudokuSharp |  HardA |   1,628.430 us |     5.4475 us |     5.0956 us |   7.28 |    0.02 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  HardB |     116.016 us |     0.7890 us |     0.6160 us |   1.00 |    0.00 |
| SudokuSolverLite |  HardB |  25,189.467 us |   341.3258 us |   319.2763 us | 217.23 |    3.31 |
|      SudokuSharp |  HardB |   5,166.321 us |    43.9029 us |    36.6609 us |  44.52 |    0.47 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  EvilA |     760.757 us |     4.6869 us |     4.1549 us |   1.00 |    0.00 |
| SudokuSolverLite |  EvilA | 385,255.200 us | 2,812.0879 us | 2,630.4287 us | 506.47 |    5.05 |
|      SudokuSharp |  EvilA |  62,937.397 us |   247.0954 us |   219.0436 us |  82.73 |    0.55 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  EvilB |   1,890.988 us |     8.9369 us |     8.3596 us |   1.00 |    0.00 |
| SudokuSolverLite |  EvilB |  49,118.798 us |   300.1732 us |   250.6582 us |  25.95 |    0.14 |
|      SudokuSharp |  EvilB |   5,094.670 us |    24.0295 us |    22.4772 us |   2.69 |    0.02 |
