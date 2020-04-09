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

|           Method | sampleCollection |         Mean |        Error |        StdDev |  Ratio | RatioSD |
|----------------- |----------------- |-------------:|-------------:|--------------:|-------:|--------:|
|      SudokuSpice |       Guesses: 0 |     36.42 us |     0.617 us |      0.547 us |   1.00 |    0.00 |
|      SudokuSharp |       Guesses: 0 |    886.87 us |    27.842 us |     81.656 us |  24.00 |    2.73 |
| SudokuSolverLite |       Guesses: 0 |  1,639.37 us |    37.879 us |    111.094 us |  46.29 |    3.68 |
|                  |                  |              |              |               |        |         |
|      SudokuSpice |       Guesses: 1 |     55.92 us |     0.833 us |      0.779 us |   1.00 |    0.00 |
|      SudokuSharp |       Guesses: 1 |  1,880.08 us |    59.820 us |    175.442 us |  34.87 |    3.14 |
| SudokuSolverLite |       Guesses: 1 |  3,337.57 us |   420.602 us |  1,144.280 us |  66.84 |   17.08 |
|                  |                  |              |              |               |        |         |
|      SudokuSpice |     Guesses: 2-3 |     73.85 us |     1.456 us |      2.135 us |   1.00 |    0.00 |
|      SudokuSharp |     Guesses: 2-3 |  3,099.05 us |   299.541 us |    839.944 us |  41.46 |    9.79 |
| SudokuSolverLite |     Guesses: 2-3 |  5,003.59 us |   376.034 us |  1,054.439 us |  64.84 |   10.04 |
|                  |                  |              |              |               |        |         |
|      SudokuSpice |     Guesses: 4-7 |    101.10 us |     1.690 us |      1.498 us |   1.00 |    0.00 |
|      SudokuSharp |     Guesses: 4-7 |  4,010.63 us |   289.910 us |    854.807 us |  39.60 |    8.25 |
| SudokuSolverLite |     Guesses: 4-7 | 13,559.41 us | 1,942.848 us |  5,667.380 us | 122.49 |   51.50 |
|                  |                  |              |              |               |        |         |
|      SudokuSpice |      Guesses: 8+ |    126.99 us |     1.904 us |      1.590 us |   1.00 |    0.00 |
|      SudokuSharp |      Guesses: 8+ |  6,682.74 us |   425.897 us |  1,249.082 us |  50.66 |    6.97 |
| SudokuSolverLite |      Guesses: 8+ | 26,681.34 us | 4,438.471 us | 12,876.809 us | 213.87 |   86.28 |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to very
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |         StdDev |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|---------------:|---------:|--------:|
|                SudokuSpice |   Easy |      14.866 us |     0.2797 us |      0.3109 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |      23.225 us |     0.4597 us |      0.4721 us |     1.56 |    0.03 |
| SudokuSpiceDynamicMultiple |   Easy |      28.553 us |     0.4326 us |      0.4046 us |     1.91 |    0.05 |
|                SudokuSharp |   Easy |       9.525 us |     0.1694 us |      0.1585 us |     0.64 |    0.02 |
|           SudokuSolverLite |   Easy |     197.670 us |     2.0438 us |      1.9118 us |    13.25 |    0.36 |
|                            |        |                |               |                |          |         |
|                SudokuSpice | Medium |      91.466 us |     1.0343 us |      0.9675 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |     130.128 us |     1.9707 us |      1.8434 us |     1.42 |    0.03 |
| SudokuSpiceDynamicMultiple | Medium |     159.431 us |     3.0968 us |      3.1801 us |     1.74 |    0.04 |
|                SudokuSharp | Medium |   3,984.322 us |    78.6850 us |     87.4581 us |    43.68 |    1.08 |
|           SudokuSolverLite | Medium |   3,018.462 us |    23.9718 us |     21.2504 us |    33.02 |    0.41 |
|                            |        |                |               |                |          |         |
|                SudokuSpice |  HardA |      70.252 us |     0.8176 us |      0.7248 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |     104.200 us |     1.5609 us |      1.4601 us |     1.48 |    0.03 |
| SudokuSpiceDynamicMultiple |  HardA |     131.014 us |     2.5628 us |      2.7421 us |     1.87 |    0.04 |
|                SudokuSharp |  HardA |   4,249.822 us |    84.2540 us |    220.4779 us |    59.95 |    2.95 |
|           SudokuSolverLite |  HardA |  31,346.280 us |   305.0528 us |    285.3466 us |   446.11 |    4.92 |
|                            |        |                |               |                |          |         |
|                SudokuSpice |  HardB |     225.330 us |     3.5145 us |      2.9348 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     298.310 us |     5.5287 us |      5.1716 us |     1.32 |    0.03 |
| SudokuSpiceDynamicMultiple |  HardB |     348.722 us |     5.7943 us |      5.4200 us |     1.55 |    0.03 |
|                SudokuSharp |  HardB |  30,094.431 us | 1,625.2702 us |  4,792.1459 us |   135.49 |   21.50 |
|           SudokuSolverLite |  HardB |   6,189.921 us |   119.6616 us |    151.3336 us |    27.52 |    0.88 |
|                            |        |                |               |                |          |         |
|                SudokuSpice |  EvilA |     126.771 us |     2.5014 us |      2.5687 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     163.385 us |     2.3538 us |      2.2017 us |     1.29 |    0.03 |
| SudokuSpiceDynamicMultiple |  EvilA |     191.387 us |     2.4760 us |      2.3160 us |     1.51 |    0.04 |
|                SudokuSharp |  EvilA |  60,910.470 us | 4,630.3921 us | 13,652.8155 us |   468.74 |  104.16 |
|           SudokuSolverLite |  EvilA | 516,370.407 us | 7,259.9050 us |  6,790.9197 us | 4,080.65 |  117.27 |
|                            |        |                |               |                |          |         |
|                SudokuSpice |  EvilB |   1,551.908 us |    29.9524 us |     32.0487 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   2,130.226 us |    42.3154 us |     84.5084 us |     1.38 |    0.05 |
| SudokuSpiceDynamicMultiple |  EvilB |   2,506.239 us |    52.3401 us |    151.0131 us |     1.53 |    0.17 |
|                SudokuSharp |  EvilB |  62,147.376 us | 1,242.6020 us |  2,076.1088 us |    39.88 |    1.55 |
|           SudokuSolverLite |  EvilB |  66,000.701 us | 1,172.7187 us |  1,096.9618 us |    42.68 |    1.26 |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|             Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|------------------- |----------:|----------:|----------:|------:|--------:|
|        SudokuSpice |  1.676 ms | 0.0147 ms | 0.0137 ms |  1.00 |    0.00 |
| SudokuSharpSingles | 13.467 ms | 0.5075 ms | 1.4964 ms |  8.59 |    0.97 |
|   SudokuSharpMixed |  6.702 ms | 0.2146 ms | 0.6327 ms |  4.02 |    0.43 |
