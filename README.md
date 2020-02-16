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
|                SudokuSpice |   Easy |       8.379 us |     0.0604 us |     0.0504 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |      14.005 us |     0.0597 us |     0.0559 us |     1.67 |    0.01 |
| SudokuSpiceDynamicMultiple |   Easy |      18.372 us |     0.0930 us |     0.0870 us |     2.19 |    0.02 |
|           SudokuSolverLite |   Easy |     155.532 us |     0.5705 us |     0.5337 us |    18.56 |    0.12 |
|                SudokuSharp |   Easy |       3.267 us |     0.0093 us |     0.0083 us |     0.39 |    0.00 |
|                            |        |                |               |               |          |         |
|                SudokuSpice | Medium |      49.278 us |     0.1218 us |     0.1080 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |      69.755 us |     0.1997 us |     0.1770 us |     1.42 |    0.01 |
| SudokuSpiceDynamicMultiple | Medium |      81.405 us |     0.3195 us |     0.2989 us |     1.65 |    0.01 |
|           SudokuSolverLite | Medium |   2,332.889 us |     8.1813 us |     7.6528 us |    47.35 |    0.21 |
|                SudokuSharp | Medium |     146.718 us |     0.4160 us |     0.3892 us |     2.98 |    0.01 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardA |     151.886 us |     0.3191 us |     0.2985 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |     193.974 us |     0.3975 us |     0.3524 us |     1.28 |    0.00 |
| SudokuSpiceDynamicMultiple |  HardA |     225.675 us |     0.6055 us |     0.5368 us |     1.49 |    0.00 |
|           SudokuSolverLite |  HardA |   4,675.654 us |    14.9685 us |    13.2692 us |    30.78 |    0.11 |
|                SudokuSharp |  HardA |   1,645.634 us |     3.5817 us |     3.3503 us |    10.83 |    0.03 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardB |      64.002 us |     0.3083 us |     0.2733 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |      92.523 us |     0.2734 us |     0.2557 us |     1.45 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardB |     114.863 us |     0.2938 us |     0.2748 us |     1.79 |    0.01 |
|           SudokuSolverLite |  HardB |  24,029.483 us |    61.5001 us |    57.5272 us |   375.49 |    2.06 |
|                SudokuSharp |  HardB |   5,097.594 us |    10.4438 us |     9.2581 us |    79.65 |    0.41 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilA |     120.282 us |     1.1640 us |     0.9088 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     155.382 us |     0.5641 us |     0.5001 us |     1.29 |    0.01 |
| SudokuSpiceDynamicMultiple |  EvilA |     177.334 us |     0.4215 us |     0.3737 us |     1.47 |    0.01 |
|           SudokuSolverLite |  EvilA | 392,263.633 us | 2,025.0581 us | 1,894.2406 us | 3,262.33 |   32.64 |
|                SudokuSharp |  EvilA |  63,347.042 us |   275.1784 us |   257.4020 us |   526.44 |    4.77 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilB |   1,117.152 us |     3.3189 us |     3.1045 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   1,529.678 us |     3.6753 us |     3.2581 us |     1.37 |    0.01 |
| SudokuSpiceDynamicMultiple |  EvilB |   1,816.599 us |     6.4820 us |     5.4128 us |     1.63 |    0.01 |
|           SudokuSolverLite |  EvilB |  49,276.113 us |   184.2061 us |   172.3065 us |    44.11 |    0.22 |
|                SudokuSharp |  EvilB |   5,141.655 us |    12.5069 us |    11.6990 us |     4.60 |    0.02 |
