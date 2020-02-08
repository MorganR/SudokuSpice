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
|      SudokuSpice |  37.17 us |  0.041 us |  0.117 us |  37.16 us |  1.00 |    0.00 |
| SudokuSolverLite | 275.68 us | 26.204 us | 74.760 us | 299.70 us |  7.48 |    1.92 |
|      SudokuSharp |  14.39 us |  0.047 us |  0.137 us |  14.39 us |  0.39 |    0.00 |

They were also compared with a much smaller set of examples, most of which require more
advanced techniques. These demonstrate clearly that the slight overhead needed by SudokuSpice,
which dominates when solving easy puzzles, leads to effective performance enhancements for
more complicated examples.

|           Method | puzzle |           Mean |         Error |        StdDev |  Ratio | RatioSD |
|----------------- |------- |---------------:|--------------:|--------------:|-------:|--------:|
|      SudokuSpice |   Easy |      15.998 us |     0.0618 us |     0.0483 us |   1.00 |    0.00 |
| SudokuSolverLite |   Easy |     151.996 us |     0.5042 us |     0.4470 us |   9.50 |    0.03 |
|      SudokuSharp |   Easy |       3.312 us |     0.0111 us |     0.0104 us |   0.21 |    0.00 |
|                  |        |                |               |               |        |         |
|      SudokuSpice | Medium |      84.195 us |     0.3040 us |     0.2538 us |   1.00 |    0.00 |
| SudokuSolverLite | Medium |   2,334.912 us |    30.9263 us |    27.4154 us |  27.72 |    0.33 |
|      SudokuSharp | Medium |     149.305 us |     0.8223 us |     0.7692 us |   1.77 |    0.01 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  HardA |     255.460 us |     0.9964 us |     0.9320 us |   1.00 |    0.00 |
| SudokuSolverLite |  HardA |   4,763.972 us |    22.6090 us |    21.1485 us |  18.65 |    0.12 |
|      SudokuSharp |  HardA |   1,663.318 us |     5.4606 us |     5.1078 us |   6.51 |    0.03 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  HardB |     131.856 us |     0.7070 us |     0.6613 us |   1.00 |    0.00 |
| SudokuSolverLite |  HardB |  24,247.421 us |    95.3072 us |    89.1504 us | 183.90 |    0.97 |
|      SudokuSharp |  HardB |   5,119.482 us |    26.0262 us |    24.3449 us |  38.83 |    0.31 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  EvilA |     870.626 us |     1.7568 us |     1.5573 us |   1.00 |    0.00 |
| SudokuSolverLite |  EvilA | 394,885.157 us | 2,052.0411 us | 1,819.0810 us | 453.57 |    2.29 |
|      SudokuSharp |  EvilA |  64,103.959 us |   191.0829 us |   169.3900 us |  73.63 |    0.22 |
|                  |        |                |               |               |        |         |
|      SudokuSpice |  EvilB |   2,303.658 us |     4.9566 us |     4.3939 us |   1.00 |    0.00 |
| SudokuSolverLite |  EvilB |  51,707.770 us |   422.2456 us |   374.3098 us |  22.45 |    0.17 |
|      SudokuSharp |  EvilB |   5,200.431 us |    14.7042 us |    13.0349 us |   2.26 |    0.00 |
