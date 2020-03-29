# SudokuSpice

An efficient sudoku-solving library targeting .NET Core.

This code focuses on optimizing performance without sacrificing readability. Generally speaking,
when faced with readability and flexibility improvements that have a slight performance cost, the
version with improved readability has been implemented.

However that's not to say it performs poorly! See [the numbers](#Performance).

## Performance

All benchmarks were run using [BenchmarkDotNet](https://benchmarkdotnet.org/articles/overview.html).

### Comparison with other libraries

The performance of this library has been compared to other .NET sudoku libraries.

**Compared libraries:**

* [SudokuSharp](https://github.com/BenjaminChambers/SudokuSharp)
* [SudokuSolverLite](https://github.com/zhiliangxu/SudokuSolver)

#### Puzzle solving performance

These were compared using a set of 1750 generated puzzles, with 24 - 30 preset squares, grouped
by the number of squares that needed to be guessed to solve the puzzle if using just the basic
Sudoku constraints and no heuristics. *SudokuSpice* demonstrates considerably more speed and
stability regardless of the number of guesses, performing anywhere from 25 - 199 times faster
than the competitors.

|           Method | sampleCollection |         Mean |        Error |       StdDev |  Ratio | RatioSD |
|----------------- |----------------- |-------------:|-------------:|-------------:|-------:|--------:|
|      SudokuSpice |       Guesses: 0 |     28.68 us |     0.077 us |     0.068 us |   1.00 |    0.00 |
|      SudokuSharp |       Guesses: 0 |    713.50 us |    24.437 us |    71.670 us |  25.11 |    2.89 |
| SudokuSolverLite |       Guesses: 0 |  1,272.36 us |    29.306 us |    86.411 us |  43.72 |    2.02 |
|                  |                  |              |              |              |        |         |
|      SudokuSpice |       Guesses: 1 |     44.23 us |     0.228 us |     0.213 us |   1.00 |    0.00 |
|      SudokuSharp |       Guesses: 1 |  1,464.96 us |    58.644 us |   172.912 us |  33.10 |    4.06 |
| SudokuSolverLite |       Guesses: 1 |  3,912.33 us |   482.642 us | 1,400.231 us |  80.95 |   35.71 |
|                  |                  |              |              |              |        |         |
|      SudokuSpice |     Guesses: 2-3 |     57.30 us |     0.316 us |     0.295 us |   1.00 |    0.00 |
|      SudokuSharp |     Guesses: 2-3 |  2,319.26 us |   133.079 us |   388.198 us |  42.95 |    5.63 |
| SudokuSolverLite |     Guesses: 2-3 |  4,169.16 us |   273.020 us |   800.721 us |  73.56 |   13.02 |
|                  |                  |              |              |              |        |         |
|      SudokuSpice |     Guesses: 4-7 |     78.15 us |     0.637 us |     0.595 us |   1.00 |    0.00 |
|      SudokuSharp |     Guesses: 4-7 |  3,235.54 us |   209.796 us |   611.984 us |  40.67 |    6.09 |
| SudokuSolverLite |     Guesses: 4-7 |  9,811.26 us | 1,718.240 us | 4,984.924 us | 119.68 |   70.91 |
|                  |                  |              |              |              |        |         |
|      SudokuSpice |      Guesses: 8+ |    101.50 us |     0.849 us |     0.794 us |   1.00 |    0.00 |
|      SudokuSharp |      Guesses: 8+ |  5,475.19 us |   364.063 us | 1,073.447 us |  54.74 |    9.73 |
| SudokuSolverLite |      Guesses: 8+ | 21,083.10 us | 3,083.679 us | 8,946.311 us | 199.12 |   90.81 |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to very
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|
|                SudokuSpice |   Easy |      12.889 us |     0.0326 us |     0.0289 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |      18.704 us |     0.0563 us |     0.0527 us |     1.45 |    0.01 |
| SudokuSpiceDynamicMultiple |   Easy |      24.056 us |     0.0706 us |     0.0660 us |     1.87 |    0.01 |
|                SudokuSharp |   Easy |       6.450 us |     0.0315 us |     0.0279 us |     0.50 |    0.00 |
|           SudokuSolverLite |   Easy |     154.967 us |     0.3469 us |     0.3245 us |    12.02 |    0.03 |
|                            |        |                |               |               |          |         |
|                SudokuSpice | Medium |      89.434 us |     0.5854 us |     0.5189 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |     119.856 us |     0.5197 us |     0.4861 us |     1.34 |    0.01 |
| SudokuSpiceDynamicMultiple | Medium |     143.707 us |     2.8701 us |     3.0710 us |     1.61 |    0.04 |
|                SudokuSharp | Medium |   3,016.878 us |    19.4826 us |    18.2241 us |    33.74 |    0.33 |
|           SudokuSolverLite | Medium |   2,333.689 us |     8.2024 us |     7.6725 us |    26.10 |    0.21 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardA |      72.349 us |     0.2205 us |     0.1955 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |      96.390 us |     0.3747 us |     0.3505 us |     1.33 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardA |     114.961 us |     0.3478 us |     0.3253 us |     1.59 |    0.01 |
|                SudokuSharp |  HardA |   3,410.678 us |    66.5375 us |   116.5350 us |    46.76 |    1.85 |
|           SudokuSolverLite |  HardA |  24,448.585 us |   106.3509 us |    99.4807 us |   338.03 |    1.82 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardB |     204.731 us |     0.6133 us |     0.5737 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     253.740 us |     1.1590 us |     1.0841 us |     1.24 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardB |     291.110 us |     2.1591 us |     2.0196 us |     1.42 |    0.01 |
|                SudokuSharp |  HardB |  23,757.915 us |   782.5399 us | 2,307.3364 us |   114.72 |    9.63 |
|           SudokuSolverLite |  HardB |   4,764.380 us |    19.5984 us |    17.3735 us |    23.27 |    0.10 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilA |     126.147 us |     0.3506 us |     0.3108 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     155.518 us |     0.8120 us |     0.7595 us |     1.23 |    0.01 |
| SudokuSpiceDynamicMultiple |  EvilA |     173.981 us |     0.7002 us |     0.6550 us |     1.38 |    0.01 |
|                SudokuSharp |  EvilA |  43,164.285 us | 2,655.6507 us | 7,830.2459 us |   337.37 |   74.96 |
|           SudokuSolverLite |  EvilA | 384,679.567 us | 1,492.4348 us | 1,396.0245 us | 3,050.55 |   13.35 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilB |   1,243.641 us |    22.3984 us |    18.7036 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   1,731.877 us |    34.1659 us |    40.6721 us |     1.39 |    0.05 |
| SudokuSpiceDynamicMultiple |  EvilB |   1,969.617 us |    39.1858 us |    67.5936 us |     1.59 |    0.06 |
|                SudokuSharp |  EvilB |  47,411.128 us |   926.4433 us | 1,029.7392 us |    37.98 |    0.93 |
|           SudokuSolverLite |  EvilB |  49,495.384 us |   116.7605 us |   109.2179 us |    39.81 |    0.60 |

#### Puzzle generation performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|             Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|------------------- |----------:|----------:|----------:|------:|--------:|
|        SudokuSpice |  2.023 ms | 0.0385 ms | 0.0378 ms |  1.00 |    0.00 |
| SudokuSharpSingles | 13.241 ms | 0.5405 ms | 1.5681 ms |  6.58 |    0.40 |
|   SudokuSharpMixed |  6.991 ms | 0.2217 ms | 0.6432 ms |  3.60 |    0.34 |
