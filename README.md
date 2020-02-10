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
|      SudokuSpice |   Easy |      11.959 us |     0.1190 us |     0.1114 us |   1.00 |    0.00 |
| SudokuSolverLite |   Easy |     150.213 us |     0.8853 us |     0.8281 us |  12.56 |    0.14 |
|      SudokuSharp |   Easy |       3.255 us |     0.0095 us |     0.0084 us |   0.27 |    0.00 |
|                  |        |                |               |               |        |         |
|      SudokuSpice | Medium |      81.065 us |     0.3643 us |     0.3407 us |   1.00 |    0.00 |
| SudokuSolverLite | Medium |   2,276.966 us |     8.9899 us |     7.9694 us |  28.09 |    0.15 |
|      SudokuSharp | Medium |     147.012 us |     0.8804 us |     0.7805 us |   1.81 |    0.01 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  HardA |     224.361 us |     0.4035 us |     0.3774 us |   1.00 |    0.00 |
| SudokuSolverLite |  HardA |   4,736.268 us |    29.2614 us |    27.3711 us |  21.11 |    0.12 |
|      SudokuSharp |  HardA |   1,629.822 us |     5.0176 us |     4.4480 us |   7.27 |    0.02 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  HardB |     116.610 us |     0.8534 us |     0.6663 us |   1.00 |    0.00 |
| SudokuSolverLite |  HardB |  23,974.044 us |   175.1255 us |   155.2442 us | 205.48 |    1.53 |
|      SudokuSharp |  HardB |   5,098.423 us |    28.7891 us |    25.5208 us |  43.73 |    0.36 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  EvilA |     763.383 us |     3.5235 us |     3.1235 us |   1.00 |    0.00 |
| SudokuSolverLite |  EvilA | 396,299.527 us | 3,315.2780 us | 3,101.1131 us | 519.20 |    5.35 |
|      SudokuSharp |  EvilA |  63,378.207 us |   213.3011 us |   199.5220 us |  83.04 |    0.47 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  EvilB |   1,944.641 us |     8.5928 us |     8.0377 us |   1.00 |    0.00 |
| SudokuSolverLite |  EvilB |  50,367.845 us |   163.7494 us |   136.7382 us |  25.90 |    0.14 |
|      SudokuSharp |  EvilB |   5,145.908 us |    11.3611 us |    10.6271 us |   2.65 |    0.01 |
