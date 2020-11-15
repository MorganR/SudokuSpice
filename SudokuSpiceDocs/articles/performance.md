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
|            SudokuSpice |       Guesses: 0 |     24.60 us |     0.125 us |     0.116 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |       Guesses: 0 |     69.62 us |     0.484 us |     0.453 us |   2.83 |    0.03 |
|            SudokuSharp |       Guesses: 0 |    643.25 us |    23.276 us |    68.629 us |  25.93 |    1.95 |
|       SudokuSolverLite |       Guesses: 0 |  1,104.78 us |    25.443 us |    74.218 us |  45.10 |    2.53 |
|                        |                  |              |              |              |        |         |
|            SudokuSpice |       Guesses: 1 |     39.00 us |     0.283 us |     0.251 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |       Guesses: 1 |     74.21 us |     0.834 us |     0.651 us |   1.90 |    0.02 |
|            SudokuSharp |       Guesses: 1 |  1,324.02 us |    48.841 us |   144.009 us |  33.27 |    3.35 |
|       SudokuSolverLite |       Guesses: 1 |  3,798.45 us |   324.780 us |   957.621 us |  88.37 |   15.47 |
|                        |                  |              |              |              |        |         |
|            SudokuSpice |     Guesses: 2-3 |     51.48 us |     0.266 us |     0.249 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |     Guesses: 2-3 |     76.57 us |     0.624 us |     0.583 us |   1.49 |    0.01 |
|            SudokuSharp |     Guesses: 2-3 |  2,078.17 us |   191.169 us |   560.666 us |  33.64 |    8.84 |
|       SudokuSolverLite |     Guesses: 2-3 |  3,490.32 us |   303.506 us |   880.526 us |  65.90 |   14.13 |
|                        |                  |              |              |              |        |         |
|            SudokuSpice |     Guesses: 4-7 |     69.57 us |     0.807 us |     0.755 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |     Guesses: 4-7 |     83.56 us |     0.511 us |     0.478 us |   1.20 |    0.01 |
|            SudokuSharp |     Guesses: 4-7 |  2,918.96 us |   201.496 us |   584.578 us |  42.45 |    8.41 |
|       SudokuSolverLite |     Guesses: 4-7 |  8,798.48 us | 1,180.112 us | 3,461.063 us | 127.75 |   62.11 |
|                        |                  |              |              |              |        |         |
|            SudokuSpice |      Guesses: 8+ |     91.45 us |     1.121 us |     1.049 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |      Guesses: 8+ |     89.55 us |     0.680 us |     0.636 us |   0.98 |    0.02 |
|            SudokuSharp |      Guesses: 8+ |  5,302.35 us |   412.978 us | 1,217.675 us |  63.55 |   14.29 |
|       SudokuSolverLite |      Guesses: 8+ | 18,266.05 us | 3,020.605 us | 8,715.137 us | 176.68 |  123.74 |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to very
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|
|                SudokuSpice |   Easy |      10.014 us |     0.0440 us |     0.0390 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |      15.971 us |     0.0906 us |     0.0848 us |     1.59 |    0.01 |
| SudokuSpiceDynamicMultiple |   Easy |      20.172 us |     0.1060 us |     0.0940 us |     2.01 |    0.01 |
|     SudokuSpiceConstraints |   Easy |      22.006 us |     0.0914 us |     0.0855 us |     2.20 |    0.01 |
|                SudokuSharp |   Easy |       6.424 us |     0.0202 us |     0.0189 us |     0.64 |    0.00 |
|           SudokuSolverLite |   Easy |     129.636 us |     0.5236 us |     0.4898 us |    12.95 |    0.06 |
|                            |        |                |               |               |          |         |
|                SudokuSpice | Medium |      62.648 us |     0.3223 us |     0.3015 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |      92.698 us |     0.3678 us |     0.3072 us |     1.48 |    0.01 |
| SudokuSpiceDynamicMultiple | Medium |     110.273 us |     0.3901 us |     0.3649 us |     1.76 |    0.01 |
|     SudokuSpiceConstraints | Medium |      66.144 us |     0.2064 us |     0.1830 us |     1.06 |    0.01 |
|                SudokuSharp | Medium |   2,578.794 us |    12.4429 us |    11.6391 us |    41.16 |    0.30 |
|           SudokuSolverLite | Medium |   1,993.533 us |     8.2805 us |     7.7456 us |    31.82 |    0.20 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardA |      49.172 us |     0.1452 us |     0.1287 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |      72.388 us |     0.3049 us |     0.2852 us |     1.47 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardA |      88.757 us |     0.3713 us |     0.3473 us |     1.81 |    0.01 |
|     SudokuSpiceConstraints |  HardA |      75.997 us |     0.4036 us |     0.3775 us |     1.55 |    0.01 |
|                SudokuSharp |  HardA |   2,829.260 us |    55.6242 us |    95.9490 us |    57.77 |    1.94 |
|           SudokuSolverLite |  HardA |  20,871.752 us |    65.1383 us |    57.7434 us |   424.47 |    1.81 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardB |     160.179 us |     1.3332 us |     1.1818 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     209.338 us |     0.6372 us |     0.5961 us |     1.31 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardB |     237.669 us |     1.2553 us |     1.0483 us |     1.48 |    0.01 |
|     SudokuSpiceConstraints |  HardB |      82.775 us |     0.3422 us |     0.3034 us |     0.52 |    0.00 |
|                SudokuSharp |  HardB |  20,629.192 us |   792.7721 us | 2,287.3287 us |   125.27 |   10.82 |
|           SudokuSolverLite |  HardB |   4,100.011 us |    29.1447 us |    25.8360 us |    25.60 |    0.23 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilA |      86.365 us |     0.1583 us |     0.1322 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     114.321 us |     0.5177 us |     0.4842 us |     1.32 |    0.01 |
| SudokuSpiceDynamicMultiple |  EvilA |     135.003 us |     0.8062 us |     0.6732 us |     1.56 |    0.01 |
|     SudokuSpiceConstraints |  EvilA |      80.216 us |     0.2698 us |     0.2524 us |     0.93 |    0.00 |
|                SudokuSharp |  EvilA |  38,439.546 us | 1,956.2232 us | 5,706.3948 us |   464.15 |   58.93 |
|           SudokuSolverLite |  EvilA | 338,857.520 us | 2,187.8911 us | 2,046.5547 us | 3,924.57 |   25.54 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilB |   1,056.842 us |    20.1479 us |    18.8464 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   1,474.816 us |    29.0199 us |    36.7009 us |     1.39 |    0.05 |
| SudokuSpiceDynamicMultiple |  EvilB |   1,797.543 us |    35.4025 us |    34.7700 us |     1.70 |    0.05 |
|     SudokuSpiceConstraints |  EvilB |     231.346 us |     0.5146 us |     0.4562 us |     0.22 |    0.00 |
|                SudokuSharp |  EvilB |  41,548.531 us |   724.3492 us |   677.5567 us |    39.33 |    1.16 |
|           SudokuSolverLite |  EvilB |  42,994.084 us |   182.1255 us |   170.3603 us |    40.69 |    0.81 |


## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|----------------------- |----------:|----------:|----------:|------:|--------:|
|            SudokuSpice |  1.324 ms | 0.0135 ms | 0.0126 ms |  1.00 |    0.00 |
| SudokuSpiceConstraints |  2.182 ms | 0.0425 ms | 0.0506 ms |  1.64 |    0.04 |
|     SudokuSharpSingles | 12.244 ms | 0.4420 ms | 1.2963 ms |  9.32 |    0.87 |
|       SudokuSharpMixed |  6.298 ms | 0.1744 ms | 0.5086 ms |  4.70 |    0.31 |
