# Performance

All benchmarks were run using [BenchmarkDotNet](https://benchmarkdotnet.org/articles/overview.html).

The performance of this library has been compared to other .NET sudoku libraries.

**Compared libraries:**

* [SudokuSharp](https://github.com/BenjaminChambers/SudokuSharp)
* [SudokuSolverLite](https://github.com/zhiliangxu/SudokuSolver)

## Puzzle solving performance

These were compared using a set of 1750 generated puzzles, with 24 - 30 preset squares, grouped
by the number of squares that needed to be guessed to solve the puzzle if using just the basic
Sudoku rules and no heuristics. *SudokuSpice* demonstrates considerably more speed and
stability regardless of the number of guesses. These benchmarks use the Core CLR on 64 bit Window
or Linux. Values are roughly consistent for each OS.

|                 Method | sampleCollection |         Mean |        Error |       StdDev |       Median |  Ratio | RatioSD |
|----------------------- |----------------- |-------------:|-------------:|-------------:|-------------:|-------:|--------:|
|            SudokuSpice |       Guesses: 0 |     24.50 us |     0.084 us |     0.079 us |     24.51 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |       Guesses: 0 |     65.12 us |     0.476 us |     0.445 us |     65.14 us |   2.66 |    0.02 |
|            SudokuSharp |       Guesses: 0 |    649.59 us |    19.143 us |    55.841 us |    648.23 us |  27.80 |    1.58 |
|       SudokuSolverLite |       Guesses: 0 |  1,118.10 us |    23.346 us |    68.471 us |  1,117.15 us |  44.96 |    2.71 |
|                        |                  |              |              |              |              |        |         |
|            SudokuSpice |       Guesses: 1 |     38.49 us |     0.148 us |     0.139 us |     38.47 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |       Guesses: 1 |     69.49 us |     0.523 us |     0.490 us |     69.59 us |   1.81 |    0.01 |
|            SudokuSharp |       Guesses: 1 |  1,412.60 us |    50.334 us |   147.620 us |  1,436.72 us |  36.53 |    4.66 |
|       SudokuSolverLite |       Guesses: 1 |  3,250.69 us |   536.551 us | 1,548.073 us |  2,878.31 us |  87.55 |   42.56 |
|                        |                  |              |              |              |              |        |         |
|            SudokuSpice |     Guesses: 2-3 |     50.98 us |     0.545 us |     0.510 us |     51.04 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |     Guesses: 2-3 |     73.35 us |     0.924 us |     0.864 us |     73.47 us |   1.44 |    0.03 |
|            SudokuSharp |     Guesses: 2-3 |  2,188.83 us |   184.415 us |   543.752 us |  2,126.51 us |  39.84 |    9.79 |
|       SudokuSolverLite |     Guesses: 2-3 |  3,632.43 us |   337.702 us |   958.005 us |  3,407.57 us |  64.46 |   13.79 |
|                        |                  |              |              |              |              |        |         |
|            SudokuSpice |     Guesses: 4-7 |     70.55 us |     0.516 us |     0.483 us |     70.42 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |     Guesses: 4-7 |     79.73 us |     0.751 us |     0.702 us |     79.82 us |   1.13 |    0.01 |
|            SudokuSharp |     Guesses: 4-7 |  3,278.92 us |   217.052 us |   629.707 us |  3,218.28 us |  48.73 |   10.29 |
|       SudokuSolverLite |     Guesses: 4-7 |  7,799.89 us | 1,262.808 us | 3,602.863 us |  6,490.84 us | 105.81 |   64.68 |
|                        |                  |              |              |              |              |        |         |
|            SudokuSpice |      Guesses: 8+ |     90.41 us |     1.101 us |     1.030 us |     90.26 us |   1.00 |    0.00 |
| SudokuSpiceConstraints |      Guesses: 8+ |     86.19 us |     0.645 us |     0.603 us |     86.00 us |   0.95 |    0.02 |
|            SudokuSharp |      Guesses: 8+ |  5,503.43 us |   388.132 us | 1,132.200 us |  5,353.07 us |  58.23 |   11.68 |
|       SudokuSolverLite |      Guesses: 8+ | 19,137.99 us | 3,092.328 us | 9,069.263 us | 18,285.43 us | 232.26 |  103.63 |

Each library was also compared with a select set of examples, most of which require more advanced
techniques. These demonstrate that *SudokuSharp* can take the lead in some very simple cases, when
the slight overhead needed by *SudokuSpice* dominates, but that this overhead leads to 
effective performance enhancements for more complicated examples.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|
|                SudokuSpice |   Easy |       9.667 us |     0.0378 us |     0.0354 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |      15.690 us |     0.1058 us |     0.0826 us |     1.62 |    0.01 |
| SudokuSpiceDynamicMultiple |   Easy |      19.809 us |     0.0693 us |     0.0648 us |     2.05 |    0.01 |
|     SudokuSpiceConstraints |   Easy |      20.027 us |     0.1032 us |     0.0914 us |     2.07 |    0.01 |
|                SudokuSharp |   Easy |       6.529 us |     0.0222 us |     0.0197 us |     0.68 |    0.00 |
|           SudokuSolverLite |   Easy |     130.636 us |     0.4642 us |     0.3876 us |    13.51 |    0.05 |
|                            |        |                |               |               |          |         |
|                SudokuSpice | Medium |      62.741 us |     0.4592 us |     0.4071 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |      91.801 us |     0.4732 us |     0.4426 us |     1.46 |    0.01 |
| SudokuSpiceDynamicMultiple | Medium |     111.043 us |     1.0482 us |     0.9805 us |     1.77 |    0.01 |
|     SudokuSpiceConstraints | Medium |      62.202 us |     0.4050 us |     0.3382 us |     0.99 |    0.01 |
|                SudokuSharp | Medium |   2,764.116 us |    19.2357 us |    17.9931 us |    44.09 |    0.37 |
|           SudokuSolverLite | Medium |   2,024.932 us |    13.3828 us |    12.5183 us |    32.28 |    0.31 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardA |      48.406 us |     0.3009 us |     0.2815 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |      70.641 us |     0.3577 us |     0.3346 us |     1.46 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardA |      87.777 us |     0.3337 us |     0.3122 us |     1.81 |    0.01 |
|     SudokuSpiceConstraints |  HardA |      68.456 us |     0.5569 us |     0.4936 us |     1.42 |    0.01 |
|                SudokuSharp |  HardA |   2,876.129 us |    55.7731 us |    86.8319 us |    59.50 |    1.43 |
|           SudokuSolverLite |  HardA |  21,344.045 us |    97.6579 us |    86.5712 us |   441.21 |    2.25 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardB |     153.111 us |     0.7620 us |     0.6755 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     197.242 us |     0.7350 us |     0.6875 us |     1.29 |    0.01 |
| SudokuSpiceDynamicMultiple |  HardB |     236.189 us |     1.0204 us |     0.9544 us |     1.54 |    0.01 |
|     SudokuSpiceConstraints |  HardB |      77.891 us |     1.0864 us |     1.0162 us |     0.51 |    0.01 |
|                SudokuSharp |  HardB |  20,609.562 us |   745.6312 us | 2,163.2114 us |   139.73 |   10.63 |
|           SudokuSolverLite |  HardB |   4,155.032 us |    28.6461 us |    26.7956 us |    27.15 |    0.21 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilA |      85.396 us |     0.5135 us |     0.4552 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     111.505 us |     0.2771 us |     0.2457 us |     1.31 |    0.01 |
| SudokuSpiceDynamicMultiple |  EvilA |     133.045 us |     0.7118 us |     0.6659 us |     1.56 |    0.01 |
|     SudokuSpiceConstraints |  EvilA |      74.053 us |     0.3297 us |     0.3084 us |     0.87 |    0.01 |
|                SudokuSharp |  EvilA |  39,455.551 us | 2,425.7143 us | 7,114.2002 us |   451.03 |   97.33 |
|           SudokuSolverLite |  EvilA | 345,270.840 us | 2,220.8510 us | 2,077.3854 us | 4,043.42 |   27.93 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilB |   1,047.117 us |    20.2780 us |    21.6972 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |   1,450.411 us |    23.9683 us |    21.2472 us |     1.38 |    0.03 |
| SudokuSpiceDynamicMultiple |  EvilB |   1,734.834 us |    30.2263 us |    31.0401 us |     1.66 |    0.05 |
|     SudokuSpiceConstraints |  EvilB |     214.599 us |     0.6782 us |     0.6344 us |     0.20 |    0.00 |
|                SudokuSharp |  EvilB |  41,830.073 us |   827.1366 us | 1,015.7980 us |    40.00 |    1.07 |
|           SudokuSolverLite |  EvilB |  43,415.176 us |   394.0568 us |   349.3211 us |    41.27 |    0.85 |

### WASM

Performance is considerably different in a Mono WASM environment. The CSV benchmarks are not 
currently supported in this environment. Once again, `SudokuSharp` performs better on very simple
puzzles, but `SudokuSpice` is still considerably faster in more complicated scenarios. In WASM, the
constraint-based solver is *much* faster than the rule-based solver.

|                     Method | puzzle |           Mean |         Error |        StdDev |    Ratio | RatioSD |
|--------------------------- |------- |---------------:|--------------:|--------------:|---------:|--------:|
|                SudokuSpice |   Easy |       340.4 us |       5.66 us |       3.37 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |   Easy |       536.9 us |       3.61 us |       2.39 us |     1.58 |    0.02 |
| SudokuSpiceDynamicMultiple |   Easy |       598.9 us |       3.21 us |       2.13 us |     1.76 |    0.02 |
|     SudokuSpiceConstraints |   Easy |       457.6 us |       1.74 us |       1.04 us |     1.34 |    0.02 |
|                SudokuSharp |   Easy |       103.0 us |       0.35 us |       0.23 us |     0.30 |    0.00 |
|           SudokuSolverLite |   Easy |     3,737.0 us |      12.72 us |       7.57 us |    10.98 |    0.12 |
|                            |        |                |               |               |          |         |
|                SudokuSpice | Medium |     4,158.8 us |     122.18 us |      72.70 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle | Medium |     5,015.4 us |     172.59 us |     102.71 us |     1.21 |    0.03 |
| SudokuSpiceDynamicMultiple | Medium |     5,766.3 us |     199.48 us |     131.94 us |     1.39 |    0.03 |
|     SudokuSpiceConstraints | Medium |       946.8 us |       2.12 us |       1.11 us |     0.23 |    0.00 |
|                SudokuSharp | Medium |    49,088.2 us |   2,335.05 us |   1,544.49 us |    11.74 |    0.45 |
|           SudokuSolverLite | Medium |    58,481.2 us |     372.58 us |     221.72 us |    14.07 |    0.23 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardA |     3,451.6 us |      38.86 us |      23.13 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardA |     4,236.4 us |     100.99 us |      60.10 us |     1.23 |    0.02 |
| SudokuSpiceDynamicMultiple |  HardA |     4,486.6 us |      60.04 us |      31.40 us |     1.30 |    0.01 |
|     SudokuSpiceConstraints |  HardA |     1,124.6 us |       7.07 us |       4.67 us |     0.33 |    0.00 |
|                SudokuSharp |  HardA |    58,302.5 us |  14,566.55 us |   9,634.87 us |    16.75 |    2.92 |
|           SudokuSolverLite |  HardA |   607,741.0 us |   4,474.94 us |   2,662.96 us |   176.08 |    1.45 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  HardB |     7,229.4 us |     216.70 us |     113.34 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  HardB |     9,338.4 us |     426.45 us |     282.07 us |     1.29 |    0.04 |
| SudokuSpiceDynamicMultiple |  HardB |     9,471.4 us |     385.02 us |     254.66 us |     1.31 |    0.05 |
|     SudokuSpiceConstraints |  HardB |     1,287.4 us |       8.45 us |       5.59 us |     0.18 |    0.00 |
|                SudokuSharp |  HardB |   450,902.8 us | 221,649.35 us | 146,607.32 us |    60.30 |   17.68 |
|           SudokuSolverLite |  HardB |   118,479.1 us |   1,020.40 us |     674.93 us |    16.40 |    0.28 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilA |     5,080.8 us |      61.39 us |      40.60 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilA |     5,946.1 us |      79.40 us |      52.52 us |     1.17 |    0.01 |
| SudokuSpiceDynamicMultiple |  EvilA |     6,361.0 us |     149.54 us |      98.91 us |     1.25 |    0.02 |
|     SudokuSpiceConstraints |  EvilA |     1,152.5 us |       7.92 us |       5.24 us |     0.23 |    0.00 |
|                SudokuSharp |  EvilA |   854,732.1 us | 712,365.33 us | 471,185.56 us |   168.21 |   92.39 |
|           SudokuSolverLite |  EvilA | 9,755,034.5 us |  50,749.49 us |  26,542.97 us | 1,923.48 |   17.20 |
|                            |        |                |               |               |          |         |
|                SudokuSpice |  EvilB |    48,818.3 us |  11,700.69 us |   7,739.28 us |     1.00 |    0.00 |
|   SudokuSpiceDynamicSingle |  EvilB |    64,101.1 us |  19,887.11 us |  11,834.50 us |     1.35 |    0.28 |
| SudokuSpiceDynamicMultiple |  EvilB |    74,448.4 us |  21,047.40 us |  13,921.55 us |     1.57 |    0.44 |
|     SudokuSpiceConstraints |  EvilB |     3,738.6 us |      18.17 us |       9.50 us |     0.08 |    0.01 |
|                SudokuSharp |  EvilB |   885,933.6 us |  85,754.81 us |  56,721.50 us |    18.60 |    3.39 |
|           SudokuSolverLite |  EvilB | 1,248,090.4 us |  10,984.17 us |   5,744.93 us |    26.02 |    4.42 |

## Puzzle generating performance

To compare puzzle generation, *SudokuSpice* and *SudokuSharp* were used to generate puzzles with 30
preset squares. *SudokuSharp* shows two different numbers since it uses a different interface for
puzzle generation. The *SudokuSharpSingle* method is more similar to the *SudokuSpice* method,
where squares are cleared completely at random, whereas the *SudokuSharpMixed* method uses a mix of
single, double, and quadruple square clearings.

|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |
|----------------------- |----------:|----------:|----------:|------:|--------:|
|            SudokuSpice |  1.331 ms | 0.0178 ms | 0.0158 ms |  1.00 |    0.00 |
| SudokuSpiceConstraints |  2.148 ms | 0.0348 ms | 0.0325 ms |  1.61 |    0.03 |
|     SudokuSharpSingles | 12.926 ms | 0.7183 ms | 2.0953 ms |  9.51 |    1.47 |
|       SudokuSharpMixed |  6.617 ms | 0.1979 ms | 0.5773 ms |  4.84 |    0.39 |

### WASM

|                 Method |      Mean |      Error |    StdDev | Ratio | RatioSD |
|----------------------- |----------:|-----------:|----------:|------:|--------:|
|            SudokuSpice |  45.14 ms |   4.410 ms |  2.307 ms |  1.00 |    0.00 |
| SudokuSpiceConstraints |  32.67 ms |   1.250 ms |  0.744 ms |  0.72 |    0.03 |
|     SudokuSharpSingles | 218.70 ms | 111.379 ms | 66.280 ms |  5.08 |    1.52 |
|       SudokuSharpMixed | 133.19 ms |  83.359 ms | 55.136 ms |  3.04 |    1.31 |
