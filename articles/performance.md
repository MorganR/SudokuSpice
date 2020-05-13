# Performance

All benchmarks were run using [BenchmarkDotNet](https://benchmarkdotnet.org/articles/overview.html).

The performance of this library has been compared to other .NET sudoku libraries.

**Compared libraries:**

* [SudokuSharp](https://github.com/BenjaminChambers/SudokuSharp)
* [SudokuSolverLite](https://github.com/zhiliangxu/SudokuSolver)

## Puzzle solving performance

These were compared using a set of 1750 generated puzzles, with 24 - 30 preset squares, grouped
by the number of squares that needed to be guessed to solve the puzzle if using just the basic
Sudoku constraints and no heuristics. *SudokuSpice* demonstrates considerably more speed and
stability regardless of the number of guesses.

Note: SudokuSpiceConstraints is a different solver implementation that's still in beta.

|                 Method | sampleCollection |         Mean |        Error |       StdDev |  Ratio | RatioSD |
|----------------------- |----------------- |-------------:|-------------:|-------------:|-------:|--------:|
|            SudokuSpice |       Guesses: 0 |     25.27 us |     0.028 us |     0.025 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |       Guesses: 0 |     72.75 us |     0.171 us |     0.160 us |   2.88 |    0.01 |
|            SudokuSharp |       Guesses: 0 |    730.24 us |    24.979 us |    73.652 us |  29.56 |    3.65 |
|       SudokuSolverLite |       Guesses: 0 |  1,311.49 us |    26.373 us |    69.937 us |  51.37 |    2.68 |
|                        |                  |              |              |              |        |         |
|            SudokuSpice |       Guesses: 1 |     39.25 us |     0.140 us |     0.131 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |       Guesses: 1 |     77.74 us |     0.112 us |     0.105 us |   1.98 |    0.01 |
|            SudokuSharp |       Guesses: 1 |  1,535.82 us |    49.958 us |   144.936 us |  37.70 |    4.00 |
|       SudokuSolverLite |       Guesses: 1 |  4,320.99 us |   616.973 us | 1,819.158 us | 110.61 |   52.94 |
|                        |                  |              |              |              |        |         |
|            SudokuSpice |     Guesses: 2-3 |     52.02 us |     0.320 us |     0.299 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |     Guesses: 2-3 |     80.45 us |     0.121 us |     0.107 us |   1.55 |    0.01 |
|            SudokuSharp |     Guesses: 2-3 |  2,508.12 us |   176.608 us |   515.175 us |  50.63 |   11.58 |
|       SudokuSolverLite |     Guesses: 2-3 |  4,054.04 us |   218.452 us |   637.235 us |  76.83 |   11.31 |
|                        |                  |              |              |              |        |         |
|            SudokuSpice |     Guesses: 4-7 |     72.29 us |     0.318 us |     0.282 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |     Guesses: 4-7 |     85.50 us |     0.222 us |     0.207 us |   1.18 |    0.00 |
|            SudokuSharp |     Guesses: 4-7 |  3,509.47 us |   207.330 us |   598.194 us |  48.69 |    7.68 |
|       SudokuSolverLite |     Guesses: 4-7 | 10,709.14 us | 1,107.427 us | 3,247.891 us | 148.95 |   43.79 |
|                        |                  |              |              |              |        |         |
|            SudokuSpice |      Guesses: 8+ |     92.16 us |     0.874 us |     0.818 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |      Guesses: 8+ |     91.14 us |     0.423 us |     0.395 us |   0.99 |    0.01 |
|            SudokuSharp |      Guesses: 8+ |  5,608.38 us |   397.728 us | 1,153.882 us |  61.56 |   12.10 |
|       SudokuSolverLite |      Guesses: 8+ | 21,316.38 us | 2,473.149 us | 7,095.928 us | 248.16 |   95.30 |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to very
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|
|                SudokuSpice |   Easy |      11.160 us |     0.0227 us |     0.0202 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |      17.435 us |     0.0391 us |     0.0365 us |     1.56 |    0.00 |
| SudokuSpiceDynamicMultiple |   Easy |      22.431 us |     0.0449 us |     0.0398 us |     2.01 |    0.00 |
|     SudokuSpiceConstraints |   Easy |      28.521 us |     0.0418 us |     0.0370 us |     2.56 |    0.01 |
|                SudokuSharp |   Easy |       6.791 us |     0.0066 us |     0.0055 us |     0.61 |    0.00 |
|           SudokuSolverLite |   Easy |     158.927 us |     0.2036 us |     0.1700 us |    14.24 |    0.02 |
|                            |        |                |               |               |          |         |
|                SudokuSpice | Medium |      84.024 us |     0.3301 us |     0.3088 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |     114.455 us |     0.4465 us |     0.4176 us |     1.36 |    0.01 |
| SudokuSpiceDynamicMultiple | Medium |     140.599 us |     0.7713 us |     0.7214 us |     1.67 |    0.01 |
|     SudokuSpiceConstraints | Medium |      79.551 us |     0.1945 us |     0.1819 us |     0.95 |    0.00 |
|                SudokuSharp | Medium |   3,030.810 us |    20.9265 us |    19.5746 us |    36.07 |    0.22 |
|           SudokuSolverLite | Medium |   2,409.765 us |     4.7837 us |     4.4747 us |    28.68 |    0.12 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardA |      64.487 us |     0.1538 us |     0.1363 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |      90.592 us |     0.1677 us |     0.1487 us |     1.40 |    0.00 |
| SudokuSpiceDynamicMultiple |  HardA |     112.272 us |     0.2002 us |     0.1775 us |     1.74 |    0.00 |
|     SudokuSpiceConstraints |  HardA |      84.761 us |     0.0814 us |     0.0762 us |     1.31 |    0.00 |
|                SudokuSharp |  HardA |   3,239.970 us |    64.2654 us |    78.9237 us |    50.26 |    1.19 |
|           SudokuSolverLite |  HardA |  25,231.589 us |    32.9460 us |    30.8177 us |   391.28 |    0.87 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardB |     183.338 us |     0.8504 us |     0.7539 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     240.076 us |     0.7208 us |     0.6390 us |     1.31 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardB |     282.650 us |     1.7789 us |     1.6640 us |     1.54 |    0.01 |
|     SudokuSpiceConstraints |  HardB |      96.462 us |     0.1471 us |     0.1304 us |     0.53 |    0.00 |
|                SudokuSharp |  HardB |  23,623.887 us |   856.9693 us | 2,526.7932 us |   132.83 |   12.22 |
|           SudokuSolverLite |  HardB |   4,932.185 us |     8.1122 us |     7.5881 us |    26.90 |    0.11 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilA |     114.194 us |     0.1773 us |     0.1572 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     148.737 us |     0.3679 us |     0.3072 us |     1.30 |    0.00 |
| SudokuSpiceDynamicMultiple |  EvilA |     171.400 us |     1.5910 us |     1.4882 us |     1.50 |    0.01 |
|     SudokuSpiceConstraints |  EvilA |      94.374 us |     0.1791 us |     0.1675 us |     0.83 |    0.00 |
|                SudokuSharp |  EvilA |  42,915.898 us | 2,453.4413 us | 7,234.0270 us |   349.47 |   51.71 |
|           SudokuSolverLite |  EvilA | 406,520.717 us |   694.0898 us |   615.2926 us | 3,559.92 |    7.74 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilB |   1,175.542 us |    19.8557 us |    18.5730 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   1,641.804 us |    32.6006 us |    33.4784 us |     1.40 |    0.03 |
| SudokuSpiceDynamicMultiple |  EvilB |   1,976.243 us |    36.3720 us |    40.4274 us |     1.68 |    0.04 |
|     SudokuSpiceConstraints |  EvilB |     243.918 us |     0.6293 us |     0.5886 us |     0.21 |    0.00 |
|                SudokuSharp |  EvilB |  46,652.167 us |   856.3394 us |   801.0204 us |    39.69 |    0.82 |
|           SudokuSolverLite |  EvilB |  51,580.925 us |    97.4053 us |    91.1130 us |    43.89 |    0.68 |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|----------------------- |----------:|----------:|----------:|------:|--------:|
|            SudokuSpice |  1.458 ms | 0.0200 ms | 0.0187 ms |  1.00 |    0.00 |
| SudokuSpiceConstraints |  2.453 ms | 0.0309 ms | 0.0289 ms |  1.68 |    0.04 |
|     SudokuSharpSingles | 13.959 ms | 0.4725 ms | 1.3858 ms | 10.02 |    0.69 |
|       SudokuSharpMixed |  7.070 ms | 0.2192 ms | 0.6427 ms |  4.76 |    0.56 |

